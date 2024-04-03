using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
using Lotus.Core;
using Lotus.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Xml.Linq;

namespace Lotus.App.GeneralUtility
{
	/// <summary>
	/// 
	/// </summary>
	public class Coord
	{
		public XmlNode Parent { get; set; }

		public string Coords { get; set; }

		public XmlNode Child;
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<XmlElement> modifys = new List<XmlElement>();
		public MainWindow()
		{
			InitializeComponent();
		}

		private void buttonConvert_Click(object sender, RoutedEventArgs e)
		{
			if (textCoordSource.Text == string.Empty) return;

			var source = textCoordSource.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var coord = new List<(float x, float y)>();
			var stringBuilder = new StringBuilder();

			

			foreach (var item in source)
			{
				var pos = item.ToString().Replace(',', ' ');

				stringBuilder.AppendLine(XString.Depths[9] + "<gml:pos>" + pos + "</gml:pos>");
			}

			textCoordDest.Text = stringBuilder.ToString();
			textCoordSource.Text = "";
		}

		private void buttonConvertXml_Click(object sender, RoutedEventArgs e)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(textCoordSource.Text);
			var list = xmlDoc.DocumentElement.GetElementsByTagName("gml:coordinates");

			var parenst = new List<Coord>();

			foreach (XmlNode item in list) 
			{
				parenst.Add(new Coord()
				{
					Parent = item.ParentNode,
					Coords = item.InnerText,
					Child = item
				});

				//var text = item.InnerText;
				//var source = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				//foreach (var coord in source)
				//{
				//	var pos = coord.ToString().Replace(',', ' ');
				//	XmlElement element = xmlDoc.CreateElement("gml:pos");
				//	element.InnerText = pos.ToString();
				//	parent.AppendChild(element);
				//}

				//parent.RemoveChild(item);
			}

			foreach (var item in parenst)
			{
				var text = item.Coords;
				var source = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				foreach (var coord in source)
				{
					var pos = coord.ToString().Replace(',', ' ');
					XmlElement element = xmlDoc.CreateElement("gml", "pos", null);
					element.InnerText = pos.ToString();
					item.Parent.AppendChild(element);
				}
			}

			foreach (var item in parenst)
			{
				item.Parent.RemoveChild(item.Child);
			}

			textCoordDest.Text = xmlDoc.OuterXml;
		}

		private void buttonConvertCSharp_Click(object sender, RoutedEventArgs e)
		{
			List<string> outputs = new List<string>();
			string[] sourceCSharp = textCSharp.Text.Split(XChar.SeparatorNewLine, StringSplitOptions.RemoveEmptyEntries);


			for (int i = 0; i < sourceCSharp.Length; i++)
			{
				var currentStr = sourceCSharp[i];
				if(currentStr.Contains("/// <summary>"))
				{
					var convert = "/**";
					outputs.Add(convert);
				}
				else if (currentStr.Contains("/// </summary>"))
				{
					var convert = " */";
					outputs.Add(convert);
				}
				else if (currentStr.Contains("/// "))
				{
					var convert = currentStr.Replace("/// ", "* ").TrimStart().Insert(0, " ");
					outputs.Add(convert);
				}
				else
				{
					var convert = currentStr;
					outputs.Add(convert);
				}
			}

			textTypeScript.Text = outputs.Aggregate((a, b) => a + "\n" + b);
		}

		private void buttonConvertFiles_Click(object sender, RoutedEventArgs e)
		{

			var pathSource = @"D:\CODE\LotusPlatform\Lotus.DeNova\Lotus.DeNova.WebClient\public\images\Avatar";
			var filesInfo = Directory.EnumerateFiles(pathSource).ToArray().OrderBy(x => x);
			var index = 0;
			foreach (var file in filesInfo) 
			{
				var path = file.RemoveTo(@"\images");
				var name = path.RemoveToWith("Fatcow_user_").RemoveLastOccurrence("_32.png");
				var category = "Avatar";
				textConvertFiles.AppendText("\t{");
				textConvertFiles.AppendText("\t\n");
				textConvertFiles.AppendText($"\tid: {index},\n");
				textConvertFiles.AppendText($"\tname: '{name}',\n");
				textConvertFiles.AppendText($"\tcategory: '{category}',\n");
				textConvertFiles.AppendText($"\tsource: '{path}'\n");
				textConvertFiles.AppendText("\t},\n");
				index++;
			}
        }

		private void analysMap_Click(object sender, RoutedEventArgs e)
		{

			var folderTiles = "D:\\TelesNew";
			var filesTiles = Directory.GetFiles(folderTiles, "*.bmp")
				.OrderBy(x => x)
				.ToArray();

			var width = 25 * 32;
			var height = 25 * 32;

			var stride = width * PixelFormats.Bgr32.BitsPerPixel;
			var pixelData = new byte[stride * height];

			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);

			JsonWriter writer = new JsonTextWriter(sw);
			writer.Formatting = Newtonsoft.Json.Formatting.Indented;
			writer.WriteStartObject();
			writer.WritePropertyName("items");
			writer.WriteStartArray();

			var index = 0;
			var fullBitmap = new Bitmap(25 * 32, 25 * 32, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
			for (int iy = 0; iy < 25; iy++)
			{
				for (int ix = 0; ix < 25; ix++)
				{
					var fileName = filesTiles[index];
					var bitmap = new Bitmap(fileName);

					var pixels = bitmap.GetPixels();
					fullBitmap.PutPixels(pixels, ix * 32, iy * 32, 32);

					writer.WriteStartObject();

					writer.WritePropertyName("index");
					writer.WriteValue(index);

					writer.WritePropertyName("fileName");
					writer.WriteValue(System.IO.Path.GetFileName(fileName));

					writer.WritePropertyName("type");
					writer.WriteValue(Path.GetFileNameWithoutExtension(fileName).SubstringTo("_", false));

					writer.WritePropertyName("variant");
					writer.WriteValue(GetVariant(fileName));

					writer.WriteEndObject();

					index++;
				}
			}

			writer.WriteEndArray();
			writer.WriteEndObject();

			var filename = Path.Combine(folderTiles, "HMM3_Tile.bmp");
			fullBitmap.Save(filename);
			//using (FileStream streamFile = new FileStream(filename, FileMode.Create))
			//{
			//	PngBitmapEncoder encoder = new PngBitmapEncoder();
			//	encoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
			//	encoder.Save(streamFile);
			//}

			writer.Close();
			var filenameJson = Path.Combine(folderTiles, "HMM3_Tile.json");
			File.WriteAllText(filenameJson, sb.ToString());
		}

		public int GetVariant(string filename)
		{
			var result = filename.ExtractString("[","]");
			return XNumbers.ParseInt(result);
		}

		private void convertButton_Click(object sender, RoutedEventArgs e)
		{
			string path = "D:\\GMexMultimedia.axaml";
			string pathS = "D:\\CODE\\Lithosphere\\Source\\Lithosphere\\Lithosphere.Multimedia\\Assets\\GMexMultimedia.axaml";

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(path);

			modifys.Clear();

			// Преобразовываем геометрию
			VisitNode(xmlDoc.ChildNodes, CheckNodeGeometry);

			foreach (XmlElement elem in modifys) 
			{
				var valueAtrr = elem.GetAttribute("Geometry");
				elem.RemoveAttribute("Geometry");

				if(valueAtrr.Contains("RectangleGeometry"))
				{
					AddRectangleGeometry(xmlDoc, elem, valueAtrr);
				}
				else
				{
					if (valueAtrr.Contains("EllipseGeometry"))
					{
						AddEllipseGeometry(xmlDoc, elem, valueAtrr);
					}
					else
					{
						AddLineGeometry(xmlDoc, elem, valueAtrr);
					}
				}

			}


			// Преобразовываем ClipGeometry
			modifys.Clear();

			VisitNode(xmlDoc.ChildNodes, CheckNodeClipGeometry);

			foreach (XmlElement elem in modifys)
			{
				var valueAtrr = elem.GetAttribute("ClipGeometry");
				elem.RemoveAttribute("ClipGeometry");
				AddClipGeometry(xmlDoc, elem, valueAtrr);
			}

			// Удаляем TranslateTransform
			modifys.Clear();

			VisitNode(xmlDoc.ChildNodes, CheckNodeTranslateTransform);

			foreach (XmlElement elem in modifys)
			{
				elem.RemoveAttribute("Transform");
			}

			// Преобразовываем Pen
			modifys.Clear();

			VisitNode(xmlDoc.ChildNodes, CheckNodePen);

			foreach (XmlElement elem in modifys)
			{
				var valueAtrr = elem.GetAttribute("Pen");
				elem.RemoveAttribute("Pen");
				AddPen(xmlDoc, elem, valueAtrr);
			}

			// Удаляем MappingMode у LinearGradientBrush и RadialGradientBrush
			modifys.Clear();

			VisitNode(xmlDoc.ChildNodes, CheckNodeGradientBrush);

			foreach (XmlElement elem in modifys)
			{
				elem.RemoveAttribute("MappingMode");
			}

			// Удаляем RadiusX и RadiusY у RadialGradientBrush
			modifys.Clear();

			VisitNode(xmlDoc.ChildNodes, CheckNodeRadialGradientBrush);

			foreach (XmlElement elem in modifys)
			{
				var valueAtrr = elem.GetAttribute("RadiusY");
				elem.RemoveAttribute("RadiusX");
				elem.RemoveAttribute("RadiusY");
				
				var attrRadius = xmlDoc.CreateAttribute("Radius");
				attrRadius.Value = valueAtrr;
				elem.Attributes.Append(attrRadius);
			}

			xmlDoc.Save(pathS);
		}

		public void VisitNode(XmlNodeList nodeList, Func<XmlNode, bool> callback)
		{
			if(nodeList == null) return;

			foreach (XmlNode node in nodeList)
			{
				if (callback(node))
				{
					modifys.Add(node as XmlElement);
				}
				VisitNode(node.ChildNodes, callback);
			}
		}

		public bool CheckNodeGeometry(XmlNode node)
		{
			if(node is XmlElement element)
			{
				if(element.Name == "GeometryDrawing")
				{
					var status = CheckAttributeGeometry(element.Attributes);
					return status;
				}
			}

			return false;
		}

		public bool CheckAttributeGeometry(XmlAttributeCollection attributeCollection)
		{
			var attr = attributeCollection["Geometry"];
			if (attr != null)
			{
				if (attr.Value.Contains("RectangleGeometry") || 
					attr.Value.Contains("EllipseGeometry") ||
					attr.Value.Contains("LineGeometry"))
				{
					return true;
				}
			}

			return false;
		}

		public void AddRectangleGeometry(XmlDocument xmlDoc,  XmlElement elem, string valueAtrr)
		{
			XmlElement geomRect = xmlDoc.CreateElement("RectangleGeometry");
			var data = valueAtrr.ExtractString("Rect=", "}");
			var attr = xmlDoc.CreateAttribute("Rect");
			attr.Value = data;
			geomRect.Attributes.Append(attr);

			XmlElement geom = xmlDoc.CreateElement("GeometryDrawing.Geometry");
			geom.Attributes.RemoveAll();
			geom.AppendChild(geomRect);
			elem.AppendChild(geom);
		}

		public void AddEllipseGeometry(XmlDocument xmlDoc, XmlElement elem, string valueAtrr)
		{
			XmlElement geomEllipse = xmlDoc.CreateElement("EllipseGeometry");

			var dataX = valueAtrr.ExtractString("RadiusX=", ",");
			var attrX = xmlDoc.CreateAttribute("RadiusX");
			attrX.Value = dataX;
			geomEllipse.Attributes.Append(attrX);

			var dataY = valueAtrr.ExtractString("RadiusY=", ",");
			var attrY = xmlDoc.CreateAttribute("RadiusY");
			attrY.Value = dataY;
			geomEllipse.Attributes.Append(attrY);

			var dataCenter = valueAtrr.ExtractString("Center=", "}");
			var attrCenter = xmlDoc.CreateAttribute("Center");
			attrCenter.Value = dataCenter;
			geomEllipse.Attributes.Append(attrCenter);

			XmlElement geom = xmlDoc.CreateElement("GeometryDrawing.Geometry");
			geom.Attributes.RemoveAll();
			geom.AppendChild(geomEllipse);
			elem.AppendChild(geom);
		}

		public void AddLineGeometry(XmlDocument xmlDoc, XmlElement elem, string valueAtrr)
		{
			XmlElement geomLine = xmlDoc.CreateElement("LineGeometry");

			var dataStart = valueAtrr.ExtractString("StartPoint=", ", ");
			var attrStart = xmlDoc.CreateAttribute("StartPoint");
			attrStart.Value = dataStart;
			geomLine.Attributes.Append(attrStart);

			var dataEnd = valueAtrr.ExtractString("EndPoint=", "}");
			var attrEnd = xmlDoc.CreateAttribute("EndPoint");
			attrEnd.Value = dataEnd;
			geomLine.Attributes.Append(attrEnd);


			XmlElement geom = xmlDoc.CreateElement("GeometryDrawing.Geometry");
			geom.Attributes.RemoveAll();
			geom.AppendChild(geomLine);
			elem.AppendChild(geom);
		}

		public bool CheckNodeClipGeometry(XmlNode node)
		{
			if (node is XmlElement element)
			{
				if (element.Name == "DrawingGroup")
				{
					var status = CheckAttributeClipGeometry(element.Attributes);
					return status;
				}
			}

			return false;
		}

		public bool CheckAttributeClipGeometry(XmlAttributeCollection attributeCollection)
		{
			var attr = attributeCollection["ClipGeometry"];
			if (attr != null)
			{
				if (attr.Value.Contains("PathGeometry"))
				{
					return true;
				}
			}

			return false;
		}

		public void AddClipGeometry(XmlDocument xmlDoc, XmlElement elem, string valueAtrr)
		{
			XmlElement geomLine = xmlDoc.CreateElement("PathGeometry");

			var dataFillRule = valueAtrr.ExtractString("FillRule=", ",");
			var attrFillRule = xmlDoc.CreateAttribute("FillRule");
			attrFillRule.Value = dataFillRule;
			geomLine.Attributes.Append(attrFillRule);

			var dataFigures = valueAtrr.ExtractString("Figures=", "}");
			var attrFigures = xmlDoc.CreateAttribute("Figures");
			attrFigures.Value = dataFigures;
			geomLine.Attributes.Append(attrFigures);


			XmlElement geom = xmlDoc.CreateElement("DrawingGroup.ClipGeometry");
			geom.AppendChild(geomLine);
			elem.AppendChild(geom);
		}

		public bool CheckNodeTranslateTransform(XmlNode node)
		{
			if (node is XmlElement element)
			{
				if (element.Name == "DrawingGroup")
				{
					var status = CheckAttributeTranslateTransform(element.Attributes);
					return status;
				}
			}

			return false;
		}

		public bool CheckAttributeTranslateTransform(XmlAttributeCollection attributeCollection)
		{
			var attr = attributeCollection["Transform"];
			if (attr != null)
			{
				if (attr.Value.Contains("TranslateTransform"))
				{
					return true;
				}
			}

			return false;
		}

		public bool CheckNodePen(XmlNode node)
		{
			if (node is XmlElement element)
			{
				if (element.Name == "GeometryDrawing")
				{
					var status = CheckAttributePen(element.Attributes);
					return status;
				}
			}

			return false;
		}

		public bool CheckAttributePen(XmlAttributeCollection attributeCollection)
		{
			var attr = attributeCollection["Pen"];
			if (attr != null)
			{
				if (attr.Value.Contains("Pen"))
				{
					return true;
				}
			}

			return false;
		}

		public void AddPen(XmlDocument xmlDoc, XmlElement elem, string valueAtrr)
		{
			XmlElement geomLine = xmlDoc.CreateElement("Pen");

			var dataBrush = valueAtrr.ExtractString("Brush=", ",");
			var attrBrush = xmlDoc.CreateAttribute("Brush");
			attrBrush.Value = dataBrush;
			geomLine.Attributes.Append(attrBrush);

			var dataThickness = valueAtrr.ExtractString("Thickness=", ",");
			var attrThickness = xmlDoc.CreateAttribute("Thickness");
			attrThickness.Value = dataThickness;
			geomLine.Attributes.Append(attrThickness);

			var dataStartLineCap = valueAtrr.ExtractString("StartLineCap=", ",");
			var attrStartLineCap = xmlDoc.CreateAttribute("LineCap");
			attrStartLineCap.Value = dataStartLineCap;
			geomLine.Attributes.Append(attrStartLineCap);

			//var dataEndLineCap = valueAtrr.ExtractString("EndLineCap=", ",");
			//var attrEndLineCap = xmlDoc.CreateAttribute("EndLineCap");
			//attrEndLineCap.Value = dataEndLineCap;
			//geomLine.Attributes.Append(attrEndLineCap);

			var dataLineJoin = valueAtrr.ExtractString("LineJoin=", "}");
			var attrLineJoin = xmlDoc.CreateAttribute("LineJoin");
			attrLineJoin.Value = dataLineJoin;
			geomLine.Attributes.Append(attrLineJoin);


			XmlElement geom = xmlDoc.CreateElement("GeometryDrawing.Pen");
			geom.AppendChild(geomLine);
			elem.AppendChild(geom);
		}

		public bool CheckNodeGradientBrush(XmlNode node)
		{
			if (node is XmlElement element)
			{
				if (element.Name == "LinearGradientBrush" || element.Name == "RadialGradientBrush")
				{
					var status = CheckAttributeGradientBrush(element.Attributes);
					return status;
				}
			}

			return false;
		}

		public bool CheckAttributeGradientBrush(XmlAttributeCollection attributeCollection)
		{
			var attr = attributeCollection["MappingMode"];
			if (attr != null)
			{
				return true;
			}

			return false;
		}

		public bool CheckNodeRadialGradientBrush(XmlNode node)
		{
			if (node is XmlElement element)
			{
				if (element.Name == "RadialGradientBrush")
				{
					var status = CheckAttributeRadialGradientBrush(element.Attributes);
					return status;
				}
			}

			return false;
		}

		public bool CheckAttributeRadialGradientBrush(XmlAttributeCollection attributeCollection)
		{
			var attr = attributeCollection["RadiusX"];
			if (attr != null)
			{
				return true;
			}

			return false;
		}

		private void generateButtonButton_Click(object sender, RoutedEventArgs e)
		{
			string path = @"D:\CODE\Lithosphere\Source\GMex.Stable\GMex.Stable.Structure\Xml\ServiceDirectorys.xml";
			string destPath = @"D:\CODE\Lithosphere\Source\Lithosphere\Lithosphere.Core\Source\Typed";

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(path);

			modifys.Clear();

			GeneratorCodeBase generatorCodeLitho = new GeneratorCodeBase();

			//
			// Типы резервуаров - 1 (сделано)
			//
			//VisitNode(xmlDoc.ChildNodes, "Типы резервуаров", CheckNode_GROUP_DIRECTORY_SERVICE);

			//generatorCodeLitho = new GeneratorCodeBase(modifys[0]);
			//generatorCodeLitho.EntityName = "Reservoirs";
			//generatorCodeLitho.FolderName = Path.Combine(destPath);

			//generatorCodeLitho.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeLitho.CreateEntityType("TypedObject");
			//generatorCodeLitho.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Типы скважин - 2 (сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Типы скважин", CheckNode_GROUP_DIRECTORY_SERVICE);

			//generatorCodeLitho = new GeneratorCodeBase(modifys[0]);
			//generatorCodeLitho.EntityName = "WellType";
			//generatorCodeLitho.FolderName = Path.Combine(destPath);

			//generatorCodeLitho.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeLitho.CreateEntityType("TypedObject");
			//generatorCodeLitho.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Типы каротажа - 3 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Типы каротажа", CheckNode_GROUP_DIRECTORY_SERVICE);

			//var generatorCodeLoggingType = new GeneratorCodeLoggingType(modifys[0]);
			//generatorCodeLoggingType.EntityName = "LoggingType";
			//generatorCodeLoggingType.FolderName = Path.Combine(destPath);

			//generatorCodeLoggingType.CreateEntityEnum("NoType = 0,", "Значение не установлено");
			//generatorCodeLoggingType.CreateEntityType("TypedObject");
			//generatorCodeLoggingType.CreateEntityTypes("NoType", "Значение не установлено");

			//
			// Типы каротажа - 4 Единицы измерения каротажа (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Единицы измерения каротажа", CheckNode_GROUP_DIRECTORY_SERVICE);

			//var generatorCodeUnitType = new GeneratorCodeUnitType(modifys[0]);
			//generatorCodeUnitType.EntityName = "UnitType";
			//generatorCodeUnitType.FolderName = Path.Combine(destPath);

			//generatorCodeUnitType.CreateEntityEnum("Unspecified = 0,", "Неопределенная единицы измерения");
			//generatorCodeUnitType.CreateEntityType("TypedObject");
			//generatorCodeUnitType.CreateEntityTypes("Unspecified", "Неопределенная единицы измерения");

			//
			// Насыщение - 5 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Насыщение", CheckNode_GROUP_DIRECTORY_SERVICE);

			//var generatorCodeColored = new GeneratorCodeBaseColored(modifys[0]);
			//generatorCodeColored.EntityName = "SaturationType";
			//generatorCodeColored.FolderName = Path.Combine(destPath);

			//generatorCodeColored.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeColored.CreateEntityType("TypedColoredObject");
			//generatorCodeColored.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Литология - 6 (Почему NotSet = 38) (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Литология", CheckNode_GROUP_DIRECTORY_SERVICE);

			//var generatorCodeColoredTexture = new GeneratorCodeBaseColoredTexture(modifys[0]);
			//generatorCodeColoredTexture.EntityName = "LithologyType";
			//generatorCodeColoredTexture.FolderName = Path.Combine(destPath);

			//generatorCodeColoredTexture.CreateEntityEnum();
			//generatorCodeColoredTexture.CreateEntityType("TypedColoredTextureObject");
			//generatorCodeColoredTexture.CreateEntityTypes();

			//
			// Стратиграфические типы - 7
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Стратиграфические типы", CheckNode_GROUP_DIRECTORY_SERVICE);

			//generatorCodeLitho = new GeneratorCodeBase(modifys[0]);
			//generatorCodeLitho.EntityName = "StratigraphyType";
			//generatorCodeLitho.FolderName = Path.Combine(destPath);

			//generatorCodeLitho.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeLitho.CreateEntityType("TypedObject");
			//generatorCodeLitho.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Группы каротажных кривых - 8 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Группы каротажных кривых", CheckNode_GROUP_DIRECTORY_SERVICE);

			//var generatorGroupLogginType = new GeneratorCodeGroupLogginType(modifys[0]);
			//generatorGroupLogginType.Groups = generatorCodeLoggingType.Groups;
			//generatorGroupLogginType.EntityName = "GroupLoggingType";
			//generatorGroupLogginType.FolderName = Path.Combine(destPath);

			//generatorGroupLogginType.CreateEntityEnum("NoGroup = 0,", "Значение не установлено");
			//generatorGroupLogginType.CreateEntityType("TypedObject");
			//generatorGroupLogginType.CreateEntityTypes("NoGroup", "Значение не установлено");

			//
			// Виды единиц измерения - 9 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Виды единиц измерения", CheckNode_GROUP_DIRECTORY_SERVICE);

			//var generatorGroupUnitType = new GeneratorCodeGroupUnitType(modifys[0]);
			//generatorGroupUnitType.Groups = generatorCodeUnitType.Groups;
			//generatorGroupUnitType.EntityName = "GroupUnitType";
			//generatorGroupUnitType.FolderName = Path.Combine(destPath);

			//generatorGroupUnitType.CreateEntityEnum("Unspecified = 0,", "Неопределенная группа единицы измерения");
			//generatorGroupUnitType.CreateEntityType("TypedObject");
			//generatorGroupUnitType.CreateEntityTypes("Unspecified", "Неопределенная группа единицы измерения");

			//
			// Инклинометры - 10 
			//

			//
			// Варианты - 11 
			//

			//
			// Типы колон - 12 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Типы колон", CheckNode_GROUP_DIRECTORY_SERVICE);

			//generatorCodeLitho = new GeneratorCodeBase(modifys[0]);
			//generatorCodeLitho.EntityName = "ConstructionType";
			//generatorCodeLitho.FolderName = Path.Combine(destPath);

			//generatorCodeLitho.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeLitho.CreateEntityType("TypedObject");
			//generatorCodeLitho.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Типы осложнений - 13 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Типы осложнений", CheckNode_GROUP_DIRECTORY_SERVICE);

			//var generatorCodeBaseColored = new GeneratorCodeBaseColored(modifys[0]);
			//generatorCodeBaseColored.EntityName = "DrillingFail";
			//generatorCodeBaseColored.FolderName = Path.Combine(destPath);

			//generatorCodeBaseColored.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeBaseColored.CreateEntityType("TypedColoredObject");
			//generatorCodeBaseColored.CreateEntityTypes("NotSet", "Значение не установлено");


			//
			// Виды замеров керна - 14
			//
			modifys.Clear();
			VisitNode(xmlDoc.ChildNodes, "Виды замеров керна", CheckNode_GROUP_DIRECTORY_SERVICE);

			var generatorCodeCoreMeasurements = new GeneratorCodeCoreMeasurements(modifys[0]);
			generatorCodeCoreMeasurements.EntityName = "CoreMeasurements";
			generatorCodeCoreMeasurements.FolderName = Path.Combine(destPath);

			generatorCodeCoreMeasurements.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			generatorCodeCoreMeasurements.CreateEntityType("TypedColoredObject");
			generatorCodeCoreMeasurements.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Виды отметок - 18 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Виды отметок", CheckNode_GROUP_DIRECTORY_SERVICE);

			//generatorCodeLitho = new GeneratorCodeBase(modifys[0]);
			//generatorCodeLitho.EntityName = "MarksType";
			//generatorCodeLitho.FolderName = Path.Combine(destPath);

			//generatorCodeLitho.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeLitho.CreateEntityType("TypedObject");
			//generatorCodeLitho.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Виды данных связи - 19 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Виды данных связи", CheckNode_GROUP_DIRECTORY_SERVICE);

			//generatorCodeLitho = new GeneratorCodeBase(modifys[0]);
			//generatorCodeLitho.EntityName = "ConnectionDataType";
			//generatorCodeLitho.FolderName = Path.Combine(destPath);

			//generatorCodeLitho.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeLitho.CreateEntityType("TypedObject");
			//generatorCodeLitho.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Типы карт - 20 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Типы карт", CheckNode_GROUP_DIRECTORY_SERVICE);

			//generatorCodeLitho = new GeneratorCodeBase(modifys[0]);
			//generatorCodeLitho.EntityName = "MapType";
			//generatorCodeLitho.FolderName = Path.Combine(destPath);

			//generatorCodeLitho.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeLitho.CreateEntityType("TypedObject");
			//generatorCodeLitho.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Категории скважин - 21 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Категории скважин", CheckNode_GROUP_DIRECTORY_SERVICE);

			//generatorCodeLitho = new GeneratorCodeBase(modifys[0]);
			//generatorCodeLitho.EntityName = "WellCategory";
			//generatorCodeLitho.FolderName = Path.Combine(destPath);

			//generatorCodeLitho.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeLitho.CreateEntityType("TypedObject");
			//generatorCodeLitho.CreateEntityTypes("NotSet", "Значение не установлено");

			//
			// Типы мероприятий конструкции скважины - 22 (Сделано)
			//
			//modifys.Clear();
			//VisitNode(xmlDoc.ChildNodes, "Типы мероприятий конструкции скважины", CheckNode_GROUP_DIRECTORY_SERVICE);

			//generatorCodeLitho = new GeneratorCodeBase(modifys[0]);
			//generatorCodeLitho.EntityName = "ConstructionEvent";
			//generatorCodeLitho.FolderName = Path.Combine(destPath);

			//generatorCodeLitho.CreateEntityEnum("NotSet = 0,", "Значение не установлено");
			//generatorCodeLitho.CreateEntityType("TypedObject");
			//generatorCodeLitho.CreateEntityTypes("NotSet", "Значение не установлено");
		}

		public void VisitNode(XmlNodeList nodeList, string name, Func<XmlNode, string, bool> callback)
		{
			if (nodeList == null) return;

			foreach (XmlNode node in nodeList)
			{
				if (callback(node, name))
				{
					modifys.Add(node as XmlElement);
				}
				VisitNode(node.ChildNodes, name, callback);
			}
		}

		public bool CheckNode_GROUP_DIRECTORY_SERVICE(XmlNode node, string name)
		{
			if (node is XmlElement element && element.Name == "group")
			{
				foreach (XmlNode child in element.ChildNodes) 
				{
					if(child is XmlElement childXml && childXml.Name == "description")
					{
						var attrName = childXml.Attributes["name"];
						if(attrName.Value == name)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		private void transformButton_Click(object sender, RoutedEventArgs e)
		{
			var dir = "D:\\CODE\\LotusPlatform\\Lotus.Windows\\Lotus.Windows.ViewerText\\Source";

			//var files = Directory.GetFileSystemEntries(dir);

			foreach (string file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
			{
				TransformFile(file);
			}

			//for (int i = 0; i < files.Length; i++)
			//{
			//	TransformFile(files[i]);
			//}

		}

		private void TransformFile(string fileName)
		{
			const string removeDel = "//=====================================================================================================================";
			const string removePart = "//-------------------------------------------------------------------------------------------------------------";
			const string removePart2 = "//---------------------------------------------------------------------------------------------------------";
			const string removeNamespace = "namespace DeNova";

			const string replaceFields = "#region ======================================= ДАННЫЕ ====================================================";
			const string replaceProperties = "#region ======================================= СВОЙСТВА ==================================================";
			const string replaceConstructors = "#region ======================================= КОНСТРУКТОРЫ ==============================================";
			const string replaceInnerType = "#region ======================================= ВНУТРЕННИЕ ТИПЫ ===========================================";
			
			const string replaceIndex = "#region ======================================= ИНДЕКСАТОР ================================================";
			const string replaceConst = "#region ======================================= КОНСТАНТНЫЕ ДАННЫЕ ========================================";
			const string replaceStaticFielsd = "#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================";
			const string replaceStaticProperties = "#region ======================================= СТАТИЧЕСКИЕ СВОЙСТВА ========================================";
			const string replaceStaticMethods = "#region ======================================= СТАТИЧЕСКИЕ МЕТОДЫ ========================================";

			const string replaceMainMethods = "#region ======================================= ОБЩИЕ МЕТОДЫ ========================================";
			const string replaceSystemMethods = "#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================";


			var raw = File.ReadAllLines(fileName, Encoding.UTF8);
			CTextList fileList = new CTextList(raw);

			fileList.RemoveToLine("using");
			fileList.Lines.RemoveAll((x) => x!.RawString.Trim() == removeDel);
			fileList.Lines.RemoveAll((x) => x!.RawString.Trim() == removePart);
			fileList.Lines.RemoveAll((x) => x!.RawString.Trim() == removePart2);
			fileList.Lines.RemoveAll((x) => x!.RawString.Trim() == removeNamespace);

			fileList.ReplaceFirst("namespace Lotus", "namespace Lotus.Windows");
			fileList.ReplaceAll(replaceFields, "#region Fields");
			fileList.ReplaceAll(replaceProperties, "#region Properties");
			fileList.ReplaceAll(replaceConstructors, "#region Constructors");
			fileList.ReplaceAll(replaceInnerType, "#region Inner types");
			fileList.ReplaceAll(replaceIndex, "#region Indexer");
			fileList.ReplaceAll(replaceConst, "#region Const");
			fileList.ReplaceAll(replaceStaticFielsd, "#region Static fields");
			fileList.ReplaceAll(replaceStaticProperties, "#region Static properties");
			fileList.ReplaceAll(replaceStaticMethods, "#region Static methods");
			fileList.ReplaceAll(replaceMainMethods, "#region Main methods");
			fileList.ReplaceAll(replaceSystemMethods, "#region System methods");

			fileList.AddDotToComment();

			fileList.RemoveEmptyBraces("namespace Lotus.Windows");

			fileList.RemoveRegions();

			fileList.RemoveTabs();

			fileList.InsertEmptyLineBeforeNamespace("namespace Lotus.Windows");

			fileList.Save(fileName);
		}
	}
}
