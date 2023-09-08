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
using System.Windows.Shapes;
using System.Xml;
using Lotus.Core;

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
    }
}
