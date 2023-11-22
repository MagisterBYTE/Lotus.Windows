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

namespace Lotus.App.GeneralUtility
{
	/// <summary>
	/// 
	/// </summary>
	public class Coord
	{
		public XmlNode Parent { get; set; }

		public String Coords { get; set; }

		public XmlNode Child;
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
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
					writer.WriteValue(Path.GetFileName(fileName));

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
    }
}
