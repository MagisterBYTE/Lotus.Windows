using Lotus.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Lotus.App.GeneralUtility
{
	public class GeneratorCodeBase
	{
		public XmlElement Root {  get; set; }

		public string FolderName { get; set; }

		public string EntityName { get; set; }

		public GeneratorCodeBase() 
		{ 
		}

		public GeneratorCodeBase(XmlElement root)
		{
			Root = root;
		}

		public virtual void CreateEntityEnum(string? firstEnum = null, string? firstEnumCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Localilzation");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Перечисление однозначно определяющее ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);
			
			codeCSharp.Add($"public enum {GetEnumName()}");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			if (firstEnum != null) 
			{
				if(firstEnumCommnent != null) 
				{
					codeCSharp.AddCommentSummary(false, firstEnumCommnent, false);
				}
				codeCSharp.Add(firstEnum);
				codeCSharp.AddEmptyLine();
			}

			foreach (XmlNode node in Root.ChildNodes) 
			{
				if(node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					var textEs = GetNameForLang(element, "description", "es");
					var textZh = GetNameForLang(element, "description", "zh");

					codeCSharp.AddCommentSummary(false, textRu, false);
					
					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);

					codeCSharp.Add($"[LocalizeOnLang(\"ru\", \"{textRu}\")]");
					codeCSharp.Add($"[LocalizeOnLang(\"en\", \"{textEn}\")]");
					if(!string.IsNullOrEmpty(textZh)) 
					{
						codeCSharp.Add($"[LocalizeOnLang(\"zh\", \"{textZh}\")]");
					}
					if (!string.IsNullOrEmpty(textEs))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"es\", \"{textEs}\")]");
					}
					codeCSharp.Add(new CTextLine($"{enumName} = {serviceCode},"));

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if(!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetEnumName()}.cs");
			codeCSharp.Save(path);
		}

		public virtual void CreateEntityType(string baseClass)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Core.Common");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var nameType = GetNameForLang(Root, "description", "ru");
			codeCSharp.AddCommentSummary(false, nameType, false);

			codeCSharp.AddClassPublic($"{GetTypeName()} : {baseClass}<{GetEnumName()}>");
			codeCSharp.CurrentIndent++;

			codeCSharp.AddConstructor($"public {GetTypeName()}(string name, int serviceCode, {GetEnumName()} type)",
				"name, serviceCode");
			codeCSharp.CurrentIndent++;
			
			codeCSharp.Add("Type = type;");

			codeCSharp.CurrentIndent--;
			codeCSharp.AddConstructorClose();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypeName()}.cs");
			codeCSharp.Save(path);
		}

		public virtual void CreateEntityTypes(string? firstType = null, string? firstTypeCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Список однозначно определяющий доступные ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.AddClassStaticPublic($"{GetTypesName()}");
			codeCSharp.CurrentIndent++;

			if (firstType != null)
			{
				if (firstTypeCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstTypeCommnent, false);
				}
				codeCSharp.Add($"public static readonly {GetTypeName()} {firstType} = new (nameof({firstType}), -1, {GetEnumName()}.{firstType});");
				codeCSharp.AddEmptyLine();
			}

			List<string> types = new List<string>();
			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);
					var codeType = $"{GetEnumName()}.{enumName}";
					types.Add(enumName);

					codeCSharp.Add($"public static readonly {GetTypeName()} {enumName} = new (nameof({codeType}), {serviceCode}, {codeType});");
					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.AddCommentSummary(false, "Список " + CorrectNameForList(nameType), false);
			codeCSharp.Add($"public static readonly {GetTypeName()}[] All = new {GetTypeName()}[]");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			for (int i = 0; i < types.Count; i++)
			{
				var isLast = i == types.Count - 1;
				var text = isLast ? types[i] : types[i] + ",";
				codeCSharp.Add(text);
			}
			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock(";");

			//
			// GetByType
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByType = firstType == null ? "return All.First(x => x.Type == type);" :
				$"return All.FirstOrDefault(x => x.Type == type) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByType({GetEnumName()} type)", bodyGetByType);


			//
			// GetByServiceCode
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByServiceCode = firstType == null ? "return All.First(x => x.ServiceCode == serviceCode);" :
				$"return All.FirstOrDefault(x => x.ServiceCode == serviceCode) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByServiceCode(int serviceCode)", bodyGetByServiceCode);

			//
			// GetById
			//
			codeCSharp.AddEmptyLine();

			var bodyGetById = firstType == null ? "return All.First(x => x.Id == id);" :
				$"return All.FirstOrDefault(x => x.Id == id) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetById(int id)", bodyGetById);

			//
			// Translate
			//
			codeCSharp.AddEmptyLine();

			var bodyTranslate = "foreach (var type in All) { type.Translate(); }";
			codeCSharp.AddMethodWithBody($"public static void Translate()", bodyTranslate);

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypesName()}.cs");
			codeCSharp.Save(path);
		}

		public string GetNameForLang(XmlElement element, string elementName, string attributeValueLang)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if(item.NodeType == XmlNodeType.Element && item.Name == elementName)
				{
					var attrLang = item.Attributes["lang"];
					if(attrLang != null && attrLang.Value == attributeValueLang)
					{
						var attr = item.Attributes["name"];
						if (attr != null)
						{
							return attr.Value;
						}
					}
				}
			}

			return string.Empty;
		}

		public string GetServiceCode(XmlElement element)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrServiceCode = item.Attributes["serviceCode"];
					if (attrServiceCode != null)
					{
						return attrServiceCode.Value;
					}
				}
			}

			return string.Empty;
		}

		public string GetColor(XmlNode element)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrServiceCode = item.Attributes["color"];
					if (attrServiceCode != null)
					{
						return attrServiceCode.Value;
					}
				}
			}

			return "0";
		}

		public string GetShortName(XmlNode element)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrServiceCode = item.Attributes["shortname"];
					if (attrServiceCode != null)
					{
						return attrServiceCode.Value;
					}
				}
			}

			return string.Empty;
		}

		public string GetFieldName(XmlNode element)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrServiceCode = item.Attributes["fieldname"];
					if (attrServiceCode != null)
					{
						return attrServiceCode.Value;
					}
				}
			}

			return string.Empty;
		}

		public string GetImage(XmlNode element)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrServiceCode = item.Attributes["img"];
					if (attrServiceCode != null)
					{
						return attrServiceCode.Value;
					}
				}
			}

			return string.Empty;
		}

		public string CorrectNameForList(string nameType)
		{
			if(nameType.Contains("типы"))
			{
				return nameType.Replace("типы", "типов");
			}

			return nameType;
		}

		public string TransformToIdentificator(string name)
		{
			var value = name.Replace("-", " ")
				.Replace("(", " ")
				.Replace("+", " ")
				.Replace(")", " ")
				.Replace("'", "")
				.ToProperCase()
				.Replace(" ", "")
				.Replace(",", "")
				.Replace("/", "Divide")
				.Replace("₂", "2")
				.Trim();

			if(value.Contains("⁻¹"))
			{
				value = value.Replace("⁻¹", "");
				value = "Reciprocal" + value;
			}

			if (Char.IsAsciiDigit(value[0]))
			{
				char first = value[0];
				value = value.Substring(1);
				value = value + first;
			}

			return value;
		}

		public string GetEnumName()
		{
			if(EntityName.EndsWith("Category"))
			{
				return EntityName;
			}
			if (EntityName.EndsWith("Event"))
			{
				return EntityName;
			}

			return EntityName + "Enum";
		}

		public string GetTypeName()
		{
			if(EntityName.EndsWith("Type"))
			{
				return EntityName;
			}
			return EntityName + "Type";
		}

		public string GetTypesName()
		{
			if (EntityName.EndsWith("Type"))
			{
				return EntityName + "s";
			}
			return EntityName + "Types";
		}

		public string GetAliases(XmlElement element)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrServiceCode = item.Attributes["aliases"];
					if (attrServiceCode != null)
					{
						return attrServiceCode.Value;
					}
				}
			}

			return string.Empty;
		}

		public string GetShortNameForLang(XmlElement element, string attributeValueLang)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrLang = item.Attributes["lang"];
					if (attrLang != null && attrLang.Value == attributeValueLang)
					{
						var attr = item.Attributes["shortname"];
						if (attr != null)
						{
							return attr.Value;
						}
					}
				}
			}

			return string.Empty;
		}

		public double? GetA(XmlElement element)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrA = item.Attributes["a"];
					if (attrA != null)
					{
						return double.Parse(attrA.Value.Replace('.', ','));
					}
				}
			}

			return null;
		}

		public double? GetB(XmlElement element)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrB = item.Attributes["b"];
					if (attrB != null)
					{
						return double.Parse(attrB.Value.Replace('.', ','));
					}
				}
			}

			return null;
		}

		public string GetGroupTypeUnit(XmlNode root, XmlElement element)
		{
			string findGroupTypeUnit = "";

			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "rowSql")
				{
					var attrGroupTypeUnit = item.Attributes["groupTypeUnit"];
					if (attrGroupTypeUnit != null)
					{
						findGroupTypeUnit = attrGroupTypeUnit.Value;
					}
				}
			}

			if (string.IsNullOrEmpty(findGroupTypeUnit)) return findGroupTypeUnit;

			XmlNode? parentNode = null;
			foreach (XmlNode item in root.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "group")
				{
					foreach(XmlNode item2 in item.ChildNodes)
					{
						if(item2.NodeType == XmlNodeType.Element && item2.Name == "row")
						{
							var attrServiceCode = item2.Attributes["serviceCode"];
							if (attrServiceCode != null)
							{
								if(attrServiceCode.Value == "9")
								{
									parentNode = item;
									break;
								}
							}
						}
					}
				}
			}

			var find = "t.servicecode = ";
			var index = findGroupTypeUnit.IndexOf(find);
			var serviceCode = findGroupTypeUnit.Substring(index + find.Length).Trim();

			if (parentNode != null)
			{
				return GetDirectoyServiceEnName(parentNode, serviceCode);
			}

			return string.Empty;
		}

		public string GetBaseUnit(XmlNode root, XmlElement element)
		{
			string findBaseUnit = "";

			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "rowSql")
				{
					var attrBaseUnit = item.Attributes["baseUnits"];
					if (attrBaseUnit != null)
					{
						findBaseUnit = attrBaseUnit.Value;
					}
				}
			}

			if (string.IsNullOrEmpty(findBaseUnit)) return findBaseUnit;

			XmlNode? parentNode = null;
			foreach (XmlNode item in root.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "group")
				{
					foreach (XmlNode item2 in item.ChildNodes)
					{
						if (item2.NodeType == XmlNodeType.Element && item2.Name == "row")
						{
							var attrServiceCode = item2.Attributes["serviceCode"];
							if (attrServiceCode != null)
							{
								if (attrServiceCode.Value == "4")
								{
									parentNode = item;
									break;
								}
							}
						}
					}
				}
			}

			var find = "ds.servicecode = ";
			var index = findBaseUnit.IndexOf(find);
			var serviceCode = findBaseUnit.Substring(index + find.Length).Trim();

			if (parentNode != null)
			{
				return GetDirectoyServiceEnName(parentNode, serviceCode);
			}

			return string.Empty;
		}

		public string GetUnitGroup(XmlNode root, XmlElement element)
		{
			string findBaseUnit = "";

			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "rowSql")
				{
					var attrBaseUnit = item.Attributes["unit"];
					if (attrBaseUnit != null)
					{
						findBaseUnit = attrBaseUnit.Value;
					}
				}
			}

			if (string.IsNullOrEmpty(findBaseUnit)) return findBaseUnit;

			XmlNode? parentNode = null;
			foreach (XmlNode item in root.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "group")
				{
					foreach (XmlNode item2 in item.ChildNodes)
					{
						if (item2.NodeType == XmlNodeType.Element && item2.Name == "row")
						{
							var attrServiceCode = item2.Attributes["serviceCode"];
							if (attrServiceCode != null)
							{
								if (attrServiceCode.Value == "9")
								{
									parentNode = item;
									break;
								}
							}
						}
					}
				}
			}

			var serviceCode = findBaseUnit.ExtractString("ds.servicecode = ", "and g.servicecode").Trim();

			if (parentNode != null)
			{
				return GetDirectoyServiceEnName(parentNode, serviceCode);
			}

			return string.Empty;
		}

		public string GetSymbol(XmlNode root, XmlElement element)
		{
			string findSymbol = "";

			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "rowSql")
				{
					var attrSymbol = item.Attributes["symbol"];
					if (attrSymbol != null)
					{
						findSymbol = attrSymbol.Value;
					}
				}
			}

			if (string.IsNullOrEmpty(findSymbol)) return findSymbol;

			XmlNode? parentNode = null;
			foreach (XmlNode item in root.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "group")
				{
					foreach (XmlNode item2 in item.ChildNodes)
					{
						if (item2.NodeType == XmlNodeType.Element && item2.Name == "row")
						{
							var attrServiceCode = item2.Attributes["serviceCode"];
							if (attrServiceCode != null)
							{
								if (attrServiceCode.Value == "18")
								{
									parentNode = item;
									break;
								}
							}
						}
					}
				}
			}

			var serviceCode = findSymbol.ExtractString("ds.servicecode = ", "and g.servicecode").Trim();

			if (parentNode != null)
			{
				return GetDirectoyServiceEnName(parentNode, serviceCode);
			}

			return string.Empty;
		}

		public string GetGroupLoggingType(XmlNode root, XmlElement element)
		{
			string findGroupLoggingType = "";

			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "rowSql")
				{
					var attrGroupLoggingType = item.Attributes["groupTypeLogging"];
					if (attrGroupLoggingType != null)
					{
						findGroupLoggingType = attrGroupLoggingType.Value;
					}
				}
			}

			if (string.IsNullOrEmpty(findGroupLoggingType)) return findGroupLoggingType;

			XmlNode? parentNode = null;
			foreach (XmlNode item in root.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "group")
				{
					foreach (XmlNode item2 in item.ChildNodes)
					{
						if (item2.NodeType == XmlNodeType.Element && item2.Name == "row")
						{
							var attrServiceCode = item2.Attributes["serviceCode"];
							if (attrServiceCode != null)
							{
								if (attrServiceCode.Value == "8")
								{
									parentNode = item;
									break;
								}
							}
						}
					}
				}
			}

			var find = "t.servicecode = ";
			var index = findGroupLoggingType.IndexOf(find);
			var serviceCode = findGroupLoggingType.Substring(index + find.Length).Trim();

			if (parentNode != null)
			{
				return GetDirectoyServiceEnName(parentNode, serviceCode);
			}

			return string.Empty;
		}

		public string GetDirectoyServiceEnName(XmlNode group, string serviceCode)
		{
			foreach (XmlNode item in group.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "item")
				{
					foreach (XmlNode data in item.ChildNodes)
					{
						if (data.NodeType == XmlNodeType.Element && data.Name == "row")
						{
							var attrServiceCode = data.Attributes["serviceCode"];
							if (attrServiceCode != null)
							{
								if (attrServiceCode.Value == serviceCode)
								{
									return GetNameForLang(item as XmlElement, "description", "en");
								}
							}
						}
					}
				}
			}

			return string.Empty;
		}
	}

	public class GeneratorCodeCoreMeasurements : GeneratorCodeBase
	{
		public GeneratorCodeCoreMeasurements() { }

		public GeneratorCodeCoreMeasurements(XmlElement root)
			: base(root)
		{
		}

		public override void CreateEntityEnum(string? firstEnum = null, string? firstEnumCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Localilzation");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Перечисление однозначно определяющее ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.Add($"public enum {GetEnumName()}");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			if (firstEnum != null)
			{
				if (firstEnumCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstEnumCommnent, false);
				}
				codeCSharp.Add(firstEnum);
				codeCSharp.AddEmptyLine();
			}

			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					var textEs = GetNameForLang(element, "description", "es");
					var textZh = GetNameForLang(element, "description", "zh");

					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);

					codeCSharp.Add($"[LocalizeOnLang(\"ru\", \"{textRu}\")]");
					codeCSharp.Add($"[LocalizeOnLang(\"en\", \"{textEn}\")]");
					if (!string.IsNullOrEmpty(textZh))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"zh\", \"{textZh}\")]");
					}
					if (!string.IsNullOrEmpty(textEs))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"es\", \"{textEs}\")]");
					}
					codeCSharp.Add(new CTextLine($"{enumName} = {serviceCode},"));

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetEnumName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityType(string baseClass)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Core.Common");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var nameType = GetNameForLang(Root, "description", "ru");
			codeCSharp.AddCommentSummary(false, nameType, false);

			codeCSharp.AddClassPublic($"{GetTypeName()} : {baseClass}<{GetEnumName()}>");
			codeCSharp.CurrentIndent++;

			codeCSharp.Add("public string FieldName { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Тип группы единиц измерения.", false);
			codeCSharp.Add("public GroupUnitTypeEnum GroupUnitTypeEnum { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Группа единиц измерения.", false);
			codeCSharp.Add("public GroupUnitType GroupUnitType { get { return GroupUnitTypes.GetByType(GroupUnitTypeEnum); } }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Перечисление вида отметки.", false);
			codeCSharp.Add("public MarksTypeEnum SymbolEnum { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Тип вида отметки.", false);
			codeCSharp.Add("public MarksType SymbolType { get { return MarksTypes.GetByType(SymbolEnum); } }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddConstructor($"public {GetTypeName()}(string name, int serviceCode, {GetEnumName()} type, int color)",
				"name, serviceCode, color");
			codeCSharp.CurrentIndent++;

			codeCSharp.Add("Type = type;");

			codeCSharp.CurrentIndent--;
			codeCSharp.AddConstructorClose();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypeName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityTypes(string? firstType = null, string? firstTypeCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Список однозначно определяющий доступные ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.AddClassStaticPublic($"{GetTypesName()}");
			codeCSharp.CurrentIndent++;

			if (firstType != null)
			{
				if (firstTypeCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstTypeCommnent, false);
				}
				codeCSharp.Add($"public static readonly {GetTypeName()} {firstType} = new (nameof({firstType}), -1, {GetEnumName()}.{firstType}, 0);");
				codeCSharp.AddEmptyLine();
			}

			List<string> types = new List<string>();
			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);
					var codeType = $"{GetEnumName()}.{enumName}";
					var color = GetColor(element);
					var shortName = GetShortName(element);
					var fieldName = GetFieldName(element);

					var groupUnitTypeValue = GetUnitGroup(Root.ParentNode, node as XmlElement);
					var groupUnitTypeValueIdent = TransformToIdentificator(groupUnitTypeValue);
					var groupUnitType = $"GroupUnitTypeEnum.{groupUnitTypeValueIdent}";

					var symbolValue = GetSymbol(Root.ParentNode, node as XmlElement);
					var symbolValueIdent = TransformToIdentificator(symbolValue);
					var symbolType = $"MarksTypeEnum.{symbolValueIdent}";

					types.Add(enumName);

					codeCSharp.Add($"public static readonly {GetTypeName()} {enumName} = new (nameof({codeType}), {serviceCode}, {codeType}, {color})");

					codeCSharp.AddOpenBlock();
					codeCSharp.CurrentIndent++;

					codeCSharp.Add($"ShortName = \"{shortName}\",");
					codeCSharp.Add($"FieldName = \"{fieldName}\",");
					codeCSharp.Add($"GroupUnitTypeEnum = {groupUnitType},");
					codeCSharp.Add($"SymbolEnum = {symbolType}");

					codeCSharp.CurrentIndent--;
					codeCSharp.AddCloseBlock(";");


					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.AddCommentSummary(false, "Список " + CorrectNameForList(nameType), false);
			codeCSharp.Add($"public static readonly {GetTypeName()}[] All = new {GetTypeName()}[]");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			for (int i = 0; i < types.Count; i++)
			{
				var isLast = i == types.Count - 1;
				var text = isLast ? types[i] : types[i] + ",";
				codeCSharp.Add(text);
			}
			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock(";");

			//
			// GetByType
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByType = firstType == null ? "return All.First(x => x.Type == type);" :
				$"return All.FirstOrDefault(x => x.Type == type) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByType({GetEnumName()} type)", bodyGetByType);


			//
			// GetByServiceCode
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByServiceCode = firstType == null ? "return All.First(x => x.ServiceCode == serviceCode);" :
				$"return All.FirstOrDefault(x => x.ServiceCode == serviceCode) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByServiceCode(int serviceCode)", bodyGetByServiceCode);

			//
			// GetById
			//
			codeCSharp.AddEmptyLine();

			var bodyGetById = firstType == null ? "return All.First(x => x.Id == id);" :
				$"return All.FirstOrDefault(x => x.Id == id) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetById(int id)", bodyGetById);

			//
			// Translate
			//
			codeCSharp.AddEmptyLine();

			var bodyTranslate = "foreach (var type in All) { type.Translate(); }";
			codeCSharp.AddMethodWithBody($"public static void Translate()", bodyTranslate);

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypesName()}.cs");
			codeCSharp.Save(path);
		}
	}

	public class GeneratorCodeBaseColored : GeneratorCodeBase
	{
		public GeneratorCodeBaseColored() { }

		public GeneratorCodeBaseColored(XmlElement root)
			: base(root)
		{
		}

		public override void CreateEntityEnum(string? firstEnum = null, string? firstEnumCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Localilzation");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Перечисление однозначно определяющее ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.Add($"public enum {GetEnumName()}");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			if (firstEnum != null)
			{
				if (firstEnumCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstEnumCommnent, false);
				}
				codeCSharp.Add(firstEnum);
				codeCSharp.AddEmptyLine();
			}

			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					var textEs = GetNameForLang(element, "description", "es");
					var textZh = GetNameForLang(element, "description", "zh");

					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);

					codeCSharp.Add($"[LocalizeOnLang(\"ru\", \"{textRu}\")]");
					codeCSharp.Add($"[LocalizeOnLang(\"en\", \"{textEn}\")]");
					if (!string.IsNullOrEmpty(textZh))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"zh\", \"{textZh}\")]");
					}
					if (!string.IsNullOrEmpty(textEs))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"es\", \"{textEs}\")]");
					}
					codeCSharp.Add(new CTextLine($"{enumName} = {serviceCode},"));

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetEnumName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityType(string baseClass)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Core.Common");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var nameType = GetNameForLang(Root, "description", "ru");
			codeCSharp.AddCommentSummary(false, nameType, false);

			codeCSharp.AddClassPublic($"{GetTypeName()} : {baseClass}<{GetEnumName()}>");
			codeCSharp.CurrentIndent++;

			codeCSharp.AddConstructor($"public {GetTypeName()}(string name, int serviceCode, {GetEnumName()} type, int color)",
				"name, serviceCode, color");
			codeCSharp.CurrentIndent++;

			codeCSharp.Add("Type = type;");

			codeCSharp.CurrentIndent--;
			codeCSharp.AddConstructorClose();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypeName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityTypes(string? firstType = null, string? firstTypeCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Список однозначно определяющий доступные ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.AddClassStaticPublic($"{GetTypesName()}");
			codeCSharp.CurrentIndent++;

			if (firstType != null)
			{
				if (firstTypeCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstTypeCommnent, false);
				}
				codeCSharp.Add($"public static readonly {GetTypeName()} {firstType} = new(nameof({firstType}), -1, {GetEnumName()}.{firstType}, 0);");
				codeCSharp.AddEmptyLine();
			}

			List<string> types = new List<string>();
			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);
					var codeType = $"{GetEnumName()}.{enumName}";
					var color = GetColor(element);

					types.Add(enumName);

					codeCSharp.Add($"public static readonly {GetTypeName()} {enumName} = new(nameof({codeType}), {serviceCode}, {codeType}, {color});");
					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.AddCommentSummary(false, "Список " + CorrectNameForList(nameType), false);
			codeCSharp.Add($"public static readonly {GetTypeName()}[] All = new {GetTypeName()}[]");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			for (int i = 0; i < types.Count; i++)
			{
				var isLast = i == types.Count - 1;
				var text = isLast ? types[i] : types[i] + ",";
				codeCSharp.Add(text);
			}
			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock(";");

			//
			// GetByType
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByType = firstType == null ? "return All.First(x => x.Type == type);" :
				$"return All.FirstOrDefault(x => x.Type == type) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByType({GetEnumName()} type)", bodyGetByType);


			//
			// GetByServiceCode
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByServiceCode = firstType == null ? "return All.First(x => x.ServiceCode == serviceCode);" :
				$"return All.FirstOrDefault(x => x.ServiceCode == serviceCode) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByServiceCode(int serviceCode)", bodyGetByServiceCode);

			//
			// GetById
			//
			codeCSharp.AddEmptyLine();

			var bodyGetById = firstType == null ? "return All.First(x => x.Id == id);" :
				$"return All.FirstOrDefault(x => x.Id == id) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetById(int id)", bodyGetById);

			//
			// Translate
			//
			codeCSharp.AddEmptyLine();

			var bodyTranslate = "foreach (var type in All) { type.Translate(); }";
			codeCSharp.AddMethodWithBody($"public static void Translate()", bodyTranslate);

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypesName()}.cs");
			codeCSharp.Save(path);
		}

	}

	public class GeneratorCodeBaseColoredTexture : GeneratorCodeBaseColored
	{
		public GeneratorCodeBaseColoredTexture() { }

		public GeneratorCodeBaseColoredTexture(XmlElement root)
			: base(root)
		{
		}

		public override void CreateEntityEnum(string? firstEnum = null, string? firstEnumCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Localilzation");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Перечисление однозначно определяющее ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.Add($"public enum {GetEnumName()}");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			if (firstEnum != null)
			{
				if (firstEnumCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstEnumCommnent, false);
				}
				codeCSharp.Add(firstEnum);
				codeCSharp.AddEmptyLine();
			}

			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					var textEs = GetNameForLang(element, "description", "es");
					var textZh = GetNameForLang(element, "description", "zh");

					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);

					codeCSharp.Add($"[LocalizeOnLang(\"ru\", \"{textRu}\")]");
					codeCSharp.Add($"[LocalizeOnLang(\"en\", \"{textEn}\")]");
					if (!string.IsNullOrEmpty(textZh))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"zh\", \"{textZh}\")]");
					}
					if (!string.IsNullOrEmpty(textEs))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"es\", \"{textEs}\")]");
					}
					codeCSharp.Add(new CTextLine($"{enumName} = {serviceCode},"));

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetEnumName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityType(string baseClass)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Core.Common");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var nameType = GetNameForLang(Root, "description", "ru");
			codeCSharp.AddCommentSummary(false, nameType, false);

			codeCSharp.AddClassPublic($"{GetTypeName()} : {baseClass}<{GetEnumName()}>");
			codeCSharp.CurrentIndent++;

			codeCSharp.AddConstructor($"public {GetTypeName()}(string name, int serviceCode, {GetEnumName()} type, int color)",
				"name, serviceCode, color");
			codeCSharp.CurrentIndent++;

			codeCSharp.Add("Type = type;");

			codeCSharp.CurrentIndent--;
			codeCSharp.AddConstructorClose();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypeName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityTypes(string? firstType = null, string? firstTypeCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Список однозначно определяющий доступные ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.AddClassStaticPublic($"{GetTypesName()}");
			codeCSharp.CurrentIndent++;

			if (firstType != null)
			{
				if (firstTypeCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstTypeCommnent, false);
				}
				codeCSharp.Add($"public static readonly {GetTypeName()} {firstType} = new(nameof({firstType}), -1, {GetEnumName()}.{firstType}, 0);");
				codeCSharp.AddEmptyLine();
			}

			List<string> types = new List<string>();
			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);
					var codeType = $"{GetEnumName()}.{enumName}";
					var color = GetColor(element);
					var image = GetImage(element);

					types.Add(enumName);

					codeCSharp.Add($"public static readonly {GetTypeName()} {enumName} = new(nameof({codeType}), {serviceCode}, {codeType}, {color})");

					codeCSharp.AddOpenBlock();
					codeCSharp.CurrentIndent++;

					codeCSharp.AddComment($"Extension: bmp");
					codeCSharp.AddComment($"Resolution: 32×32");
					codeCSharp.AddComment($"Size: 190 B");
					codeCSharp.AddComment($"Bit depth: 1");

					codeCSharp.Add($"TextureData = Convert.FromBase64String(\"{image}\")");

					codeCSharp.CurrentIndent--;
					codeCSharp.AddCloseBlock(";");

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.AddCommentSummary(false, "Список " + CorrectNameForList(nameType), false);
			codeCSharp.Add($"public static readonly {GetTypeName()}[] All = new {GetTypeName()}[]");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			for (int i = 0; i < types.Count; i++)
			{
				var isLast = i == types.Count - 1;
				var text = isLast ? types[i] : types[i] + ",";
				codeCSharp.Add(text);
			}
			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock(";");

			//
			// GetByType
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByType = firstType == null ? "return All.First(x => x.Type == type);" :
				$"return All.FirstOrDefault(x => x.Type == type) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByType({GetEnumName()} type)", bodyGetByType);


			//
			// GetByServiceCode
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByServiceCode = firstType == null ? "return All.First(x => x.ServiceCode == serviceCode);" :
				$"return All.FirstOrDefault(x => x.ServiceCode == serviceCode) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByServiceCode(int serviceCode)", bodyGetByServiceCode);

			//
			// GetById
			//
			codeCSharp.AddEmptyLine();

			var bodyGetById = firstType == null ? "return All.First(x => x.Id == id);" :
				$"return All.FirstOrDefault(x => x.Id == id) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetById(int id)", bodyGetById);

			//
			// Translate
			//
			codeCSharp.AddEmptyLine();

			var bodyTranslate = "foreach (var type in All) { type.Translate(); }";
			codeCSharp.AddMethodWithBody($"public static void Translate()", bodyTranslate);

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypesName()}.cs");
			codeCSharp.Save(path);
		}

	}

	public class GeneratorCodeLoggingType : GeneratorCodeBase
	{
		public DictionaryList<string, string> Groups { get; set; } = new DictionaryList<string, string>();

		public GeneratorCodeLoggingType() { }

		public GeneratorCodeLoggingType(XmlElement root)
			:base(root)
		{
		}

		public string GetAliasname(XmlElement element)
		{
			foreach (XmlNode item in element.ChildNodes)
			{
				if (item.NodeType == XmlNodeType.Element && item.Name == "row")
				{
					var attrServiceCode = item.Attributes["aliasname"];
					if (attrServiceCode != null)
					{
						return attrServiceCode.Value;
					}
				}
			}

			return string.Empty;
		}

		public override void CreateEntityEnum(string? firstEnum = null, string? firstEnumCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Localilzation");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Перечисление однозначно определяющее ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.Add($"public enum {GetEnumName()}");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			if (firstEnum != null)
			{
				if (firstEnumCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstEnumCommnent, false);
				}
				codeCSharp.Add(firstEnum);
				codeCSharp.AddEmptyLine();
			}

			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					var textEs = GetNameForLang(element, "description", "es");
					var textZh = GetNameForLang(element, "description", "zh");

					codeCSharp.AddCommentSummary(false, textRu, false);


					codeCSharp.Add($"[LocalizeOnLang(\"ru\", \"{textRu}\")]");
					codeCSharp.Add($"[LocalizeOnLang(\"en\", \"{textEn}\")]");
					if (!string.IsNullOrEmpty(textZh))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"zh\", \"{textZh}\")]");
					}
					if (!string.IsNullOrEmpty(textEs))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"es\", \"{textEs}\")]");
					}

					var aliase = GetAliasname(element);
					var enumName = TransformToIdentificator(aliase);
					var serviceCode = GetServiceCode(element);

					codeCSharp.Add($"{enumName} = {serviceCode},");

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetEnumName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityType(string baseClass)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Core.Common");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var nameType = GetNameForLang(Root, "description", "ru");
			codeCSharp.AddCommentSummary(false, nameType, false);

			codeCSharp.AddClassPublic($"{GetTypeName()} : {baseClass}<{GetEnumName()}>");
			codeCSharp.CurrentIndent++;

			codeCSharp.AddCommentSummary(false, "Список псевдонимов кривой.", false);
			codeCSharp.Add("public string Aliases { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Тип группы единиц измерения, к которой принадлежит единица измерения данных этого типа кривой.", false);
			codeCSharp.Add("public GroupUnitTypeEnum GroupUnitTypeEnum { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Тип группы типов кривых, к которой принадлежит этот тип кривой.", false);
			codeCSharp.Add("public GroupLoggingTypeEnum GroupLoggingTypeEnum { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Группа единиц измерения, к которой принадлежит единица измерения данных этого типа кривой.", false);
			codeCSharp.Add("public GroupUnitType GroupUnitType { get { return GroupUnitTypes.GetByType(GroupUnitTypeEnum); } }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Группа типов кривых, к которой принадлежит этот тип кривой.", false);
			codeCSharp.Add("public GroupLoggingType GroupLoggingType { get { return GroupLoggingTypes.GetByType(GroupLoggingTypeEnum); } }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddConstructor($"public {GetTypeName()}(string name, int serviceCode, {GetEnumName()} type)",
				"name, serviceCode");
			codeCSharp.CurrentIndent++;

			codeCSharp.Add("Type = type;");

			codeCSharp.CurrentIndent--;
			codeCSharp.AddConstructorClose();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypeName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityTypes(string? firstType = null, string? firstTypeCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Список однозначно определяющий доступные ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.AddClassStaticPublic($"{GetTypesName()}");
			codeCSharp.CurrentIndent++;

			if (firstType != null)
			{
				if (firstTypeCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstTypeCommnent, false);
				}
				codeCSharp.Add($"public static readonly {GetTypeName()} {firstType} = new (nameof({firstType}), -1, {GetEnumName()}.{firstType});");
				codeCSharp.AddEmptyLine();
			}

			List<string> types = new List<string>();
			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					codeCSharp.AddCommentSummary(false, textRu, false);

					var aliaseName = GetAliasname(element);
					var aliases = GetAliases(element);
					var enumName = TransformToIdentificator(aliaseName);
					var serviceCode = GetServiceCode(element);
					var codeType = $"{GetEnumName()}.{enumName}";
					
					var groupUnitTypeValue = GetGroupTypeUnit(Root.ParentNode, node as XmlElement);
					var groupUnitTypeValueIdent = TransformToIdentificator(groupUnitTypeValue);
					var groupUnitType = $"GroupUnitTypeEnum.{groupUnitTypeValueIdent}";

					var groupGroupLogginType = GetGroupLoggingType(Root.ParentNode, node as XmlElement);
					var groupLogginTypeValueIdent = TransformToIdentificator(groupGroupLogginType);
					var groupLogginType = $"GroupLoggingTypeEnum.{groupLogginTypeValueIdent}";


					Groups.Add(groupLogginTypeValueIdent, enumName);

					types.Add(enumName);

					codeCSharp.Add($"public static readonly {GetTypeName()} {enumName} = new (nameof({codeType}), {serviceCode}, {codeType})");
					codeCSharp.AddOpenBlock();
					codeCSharp.CurrentIndent++;

					codeCSharp.Add($"ShortName = \"{aliaseName}\",");
					codeCSharp.Add($"Aliases = \"{aliases}\",");
					codeCSharp.Add($"GroupUnitTypeEnum = {groupUnitType},");
					codeCSharp.Add($"GroupLoggingTypeEnum = {groupLogginType},");

					codeCSharp.CurrentIndent--;
					codeCSharp.AddCloseBlock(";");

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.AddCommentSummary(false, "Список " + CorrectNameForList(nameType), false);
			codeCSharp.Add($"public static readonly {GetTypeName()}[] All = new {GetTypeName()}[]");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			for (int i = 0; i < types.Count; i++)
			{
				var isLast = i == types.Count - 1;
				var text = isLast ? types[i] : types[i] + ",";
				codeCSharp.Add(text);
			}
			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock(";");

			//
			// GetByType
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByType = firstType == null ? "return All.First(x => x.Type == type);" :
				$"return All.FirstOrDefault(x => x.Type == type) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByType({GetEnumName()} type)", bodyGetByType);


			//
			// GetByServiceCode
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByServiceCode = firstType == null ? "return All.First(x => x.ServiceCode == serviceCode);" :
				$"return All.FirstOrDefault(x => x.ServiceCode == serviceCode) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByServiceCode(int serviceCode)", bodyGetByServiceCode);

			//
			// GetById
			//
			codeCSharp.AddEmptyLine();

			var bodyGetById = firstType == null ? "return All.First(x => x.Id == id);" :
				$"return All.FirstOrDefault(x => x.Id == id) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetById(int id)", bodyGetById);

			//
			// Translate
			//
			codeCSharp.AddEmptyLine();

			var bodyTranslate = "foreach (var type in All) { type.Translate(); }";
			codeCSharp.AddMethodWithBody($"public static void Translate()", bodyTranslate);

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypesName()}.cs");
			codeCSharp.Save(path);
		}
	}

	public class GeneratorCodeUnitType : GeneratorCodeBase
	{
		public DictionaryList<string, string> Groups { get; set; } = new DictionaryList<string, string>();

		public GeneratorCodeUnitType() { }

		public GeneratorCodeUnitType(XmlElement root)
			: base(root)
		{
		}

		public override void CreateEntityEnum(string? firstEnum = null, string? firstEnumCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Localilzation");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Перечисление однозначно определяющее ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.Add($"public enum {GetEnumName()}");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			if (firstEnum != null)
			{
				if (firstEnumCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstEnumCommnent, false);
				}
				codeCSharp.Add(firstEnum);
				codeCSharp.AddEmptyLine();
			}

			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					var textEs = GetNameForLang(element, "description", "es");
					var textZh = GetNameForLang(element, "description", "zh");

					var shortNameRu = GetShortNameForLang(element, "ru");
					var shortNameEn = GetShortNameForLang(element, "en");
					var shortNameEs = GetShortNameForLang(element, "es");
					var shortNameZh = GetShortNameForLang(element, "zh");

					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);

					if(serviceCode == "75")
					{
						enumName += "Force";
					}

					if (serviceCode == "114")
					{
						enumName += "Other";
					}

					codeCSharp.Add($"[LocalizeOnLang(\"ru\", \"{textRu}\")]");
					codeCSharp.Add($"[LocalizeOnLang(\"en\", \"{textEn}\")]");
					if (!string.IsNullOrEmpty(textZh))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"zh\", \"{textZh}\")]");
					}
					if (!string.IsNullOrEmpty(textEs))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"es\", \"{textEs}\")]");
					}

					if (!string.IsNullOrEmpty(shortNameRu))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"ru\", \"{shortNameRu}\", \"ShortName\")]");
					}
					if (!string.IsNullOrEmpty(shortNameEn))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"en\", \"{shortNameEn}\", \"ShortName\")]");
					}
					if (!string.IsNullOrEmpty(shortNameEs))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"es\", \"{shortNameEs}\", \"ShortName\")]");
					}
					if (!string.IsNullOrEmpty(shortNameZh))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"zh\", \"{shortNameZh}\", \"ShortName\")]");
					}

					codeCSharp.Add(new CTextLine($"{enumName} = {serviceCode},"));

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetEnumName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityType(string baseClass)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Core.Common");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var nameType = GetNameForLang(Root, "description", "ru");
			codeCSharp.AddCommentSummary(false, nameType, false);

			codeCSharp.AddClassPublic($"{GetTypeName()} : {baseClass}<{GetEnumName()}>");
			codeCSharp.CurrentIndent++;


			codeCSharp.AddCommentSummary(false, "Статус базовой единицы измерения.", false);
			codeCSharp.Add("public bool IsBaseUnit { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Коэффициент А(множитель) единицы измерения.", false);
			codeCSharp.Add("public float A { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Коэффициент B(корректирующий) единицы измерения.", false);
			codeCSharp.Add("public float B { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Тип базовой единицы измерения для группы, к которой относится данная единица измерения.", false);
			codeCSharp.Add("public UnitTypeEnum BaseUnitTypeEnum { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Базовая единица измерения для группы, к которой относится данная единица измерения.", false);
			codeCSharp.Add("public UnitType BaseUnit { get { return UnitTypes.GetByType(BaseUnitTypeEnum); } }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Тип группы единиц измерения, к которой принадлежит данная единица измерения.", false);
			codeCSharp.Add("public GroupUnitTypeEnum GroupUnitTypeEnum { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddCommentSummary(false, "Группа единиц измерения, к которой принадлежит данная единица измерения.", false);
			codeCSharp.Add("public GroupUnitType GroupUnitType { get { return GroupUnitTypes.GetByType(GroupUnitTypeEnum); } }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddConstructor($"public {GetTypeName()}(string name, int serviceCode, {GetEnumName()} type)",
				"name, serviceCode");
			codeCSharp.CurrentIndent++;

			codeCSharp.Add("Type = type;");

			codeCSharp.CurrentIndent--;
			codeCSharp.AddConstructorClose();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypeName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityTypes(string? firstType = null, string? firstTypeCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Список однозначно определяющий доступные ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.AddClassStaticPublic($"{GetTypesName()}");
			codeCSharp.CurrentIndent++;

			if (firstType != null)
			{
				if (firstTypeCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstTypeCommnent, false);
				}
				codeCSharp.Add($"public static readonly {GetTypeName()} {firstType} = new (nameof({firstType}), -1, {GetEnumName()}.{firstType})");
				codeCSharp.AddOpenBlock();
				codeCSharp.CurrentIndent++;
				codeCSharp.Add($"A = 0.0f,");
				codeCSharp.Add($"B = 0.0f,");
				codeCSharp.Add($"IsBaseUnit = false,");
				codeCSharp.Add($"BaseUnitTypeEnum = UnitTypeEnum.Unspecified,");
				codeCSharp.Add($"GroupUnitTypeEnum = GroupUnitTypeEnum.Unspecified");
				codeCSharp.CurrentIndent--;
				codeCSharp.AddCloseBlock(";");
				codeCSharp.AddEmptyLine();
			}

			List<string> types = new List<string>();
			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);

					if (serviceCode == "75")
					{
						enumName += "Force";
					}

					if (serviceCode == "114")
					{
						enumName += "Other";
					}

					var codeType = $"{GetEnumName()}.{enumName}";
					var coefA = GetA(element);
					var coefB = GetB(element);
					var groupUnitTypeValue = GetGroupTypeUnit(Root.ParentNode, node as XmlElement);
					var groupUnitTypeValueIdent = TransformToIdentificator(groupUnitTypeValue);
					var groupUnitType = $"GroupUnitTypeEnum.{groupUnitTypeValueIdent}";

					Groups.Add(groupUnitTypeValueIdent, enumName);

					var baseUnitTypeValue = GetBaseUnit(Root.ParentNode, node as XmlElement);
					string baseUnitType = null;
					if (string.IsNullOrEmpty(baseUnitTypeValue) == false)
					{
						baseUnitType = $"UnitTypeEnum.{TransformToIdentificator(baseUnitTypeValue)}";
					}
					types.Add(enumName);

					codeCSharp.Add($"public static readonly {GetTypeName()} {enumName} = new (nameof({codeType}), {serviceCode}, {codeType})");
					codeCSharp.AddOpenBlock();
					codeCSharp.CurrentIndent++;
					if (coefA is not null && coefB is not null)
					{
						codeCSharp.Add($"A = {coefA.Value.ToString("G7").Replace(',', '.')}f,");
						codeCSharp.Add($"B = {coefB.Value.ToString("G7").Replace(',', '.')}f,");
						codeCSharp.Add($"IsBaseUnit = false,");
					}
					else
					{
						codeCSharp.Add($"A = 1.0f,");
						codeCSharp.Add($"B = 0.0f,");
						codeCSharp.Add($"IsBaseUnit = true,");
					}

					if (string.IsNullOrEmpty(baseUnitTypeValue) == false)
					{
						codeCSharp.Add($"BaseUnitTypeEnum = {baseUnitType},");
					}
					codeCSharp.Add($"GroupUnitTypeEnum = {groupUnitType}");

					codeCSharp.CurrentIndent--;
					codeCSharp.AddCloseBlock(";");
					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.AddCommentSummary(false, "Список " + CorrectNameForList(nameType), false);
			codeCSharp.Add($"public static readonly {GetTypeName()}[] All = new {GetTypeName()}[]");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			for (int i = 0; i < types.Count; i++)
			{
				var isLast = i == types.Count - 1;
				var text = isLast ? types[i] : types[i] + ",";
				codeCSharp.Add(text);
			}
			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock(";");

			//
			// GetByType
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByType = firstType == null ? "return All.First(x => x.Type == type);" :
				$"return All.FirstOrDefault(x => x.Type == type) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByType({GetEnumName()} type)", bodyGetByType);


			//
			// GetByServiceCode
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByServiceCode = firstType == null ? "return All.First(x => x.ServiceCode == serviceCode);" :
				$"return All.FirstOrDefault(x => x.ServiceCode == serviceCode) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByServiceCode(int serviceCode)", bodyGetByServiceCode);

			//
			// GetById
			//
			codeCSharp.AddEmptyLine();

			var bodyGetById = firstType == null ? "return All.First(x => x.Id == id);" :
				$"return All.FirstOrDefault(x => x.Id == id) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetById(int id)", bodyGetById);

			//
			// Translate
			//
			codeCSharp.AddEmptyLine();

			var bodyTranslate = "foreach (var type in All) { type.Translate(); }";
			codeCSharp.AddMethodWithBody($"public static void Translate()", bodyTranslate);

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypesName()}.cs");
			codeCSharp.Save(path);
		}
	}

	public class GeneratorCodeGroupUnitType : GeneratorCodeBase
	{
		public DictionaryList<string, string> Groups { get; set; }

		public GeneratorCodeGroupUnitType() { }

		public GeneratorCodeGroupUnitType(XmlElement root)
			: base(root)
		{
		}

		public override void CreateEntityEnum(string? firstEnum = null, string? firstEnumCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Localilzation");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Перечисление однозначно определяющее ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.Add($"public enum {GetEnumName()}");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			if (firstEnum != null)
			{
				if (firstEnumCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstEnumCommnent, false);
				}
				codeCSharp.Add(firstEnum);
				codeCSharp.AddEmptyLine();
			}

			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					var textEs = GetNameForLang(element, "description", "es");
					var textZh = GetNameForLang(element, "description", "zh");

					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);

					codeCSharp.Add($"[LocalizeOnLang(\"ru\", \"{textRu}\")]");
					codeCSharp.Add($"[LocalizeOnLang(\"en\", \"{textEn}\")]");
					if (!string.IsNullOrEmpty(textZh))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"zh\", \"{textZh}\")]");
					}
					if (!string.IsNullOrEmpty(textEs))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"es\", \"{textEs}\")]");
					}
					codeCSharp.Add(new CTextLine($"{enumName} = {serviceCode},"));

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetEnumName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityType(string baseClass)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Core.Common");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var nameType = GetNameForLang(Root, "description", "ru");
			codeCSharp.AddCommentSummary(false, nameType, false);

			codeCSharp.AddClassPublic($"{GetTypeName()} : {baseClass}<{GetEnumName()}>");
			codeCSharp.CurrentIndent++;

			codeCSharp.AddCommentSummary(false, "Список единиц измерения для данного вида.", false);
			codeCSharp.Add("public UnitType[] UnitTypes { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddConstructor($"public {GetTypeName()}(string name, int serviceCode, {GetEnumName()} type)",
				"name, serviceCode");
			codeCSharp.CurrentIndent++;

			codeCSharp.Add("Type = type;");

			codeCSharp.CurrentIndent--;
			codeCSharp.AddConstructorClose();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypeName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityTypes(string? firstType = null, string? firstTypeCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Список однозначно определяющий доступные ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.AddClassStaticPublic($"{GetTypesName()}");
			codeCSharp.CurrentIndent++;

			if (firstType != null)
			{
				if (firstTypeCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstTypeCommnent, false);
				}
				codeCSharp.Add($"public static readonly {GetTypeName()} {firstType} = new (nameof({firstType}), -1, {GetEnumName()}.{firstType});");
				codeCSharp.AddEmptyLine();
			}

			List<string> types = new List<string>();
			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);
					var codeType = $"{GetEnumName()}.{enumName}";
					types.Add(enumName);

					codeCSharp.Add($"public static readonly {GetTypeName()} {enumName} = new (nameof({codeType}), {serviceCode}, {codeType})");
					codeCSharp.AddOpenBlock();
					codeCSharp.CurrentIndent++;

					codeCSharp.Add("UnitTypes = new UnitType[]");
					codeCSharp.AddOpenBlock();
					codeCSharp.CurrentIndent++;

					var list = Groups[enumName];
					foreach (var unit in list)
					{
						codeCSharp.Add($"UnitTypes.{unit},");
					}

					codeCSharp.CurrentIndent--;
					codeCSharp.AddCloseBlock();

					codeCSharp.CurrentIndent--;
					codeCSharp.AddCloseBlock(";");

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.AddCommentSummary(false, "Список " + CorrectNameForList(nameType), false);
			codeCSharp.Add($"public static readonly {GetTypeName()}[] All = new {GetTypeName()}[]");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			for (int i = 0; i < types.Count; i++)
			{
				var isLast = i == types.Count - 1;
				var text = isLast ? types[i] : types[i] + ",";
				codeCSharp.Add(text);
			}
			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock(";");

			//
			// GetByType
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByType = firstType == null ? "return All.First(x => x.Type == type);" :
				$"return All.FirstOrDefault(x => x.Type == type) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByType({GetEnumName()} type)", bodyGetByType);


			//
			// GetByServiceCode
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByServiceCode = firstType == null ? "return All.First(x => x.ServiceCode == serviceCode);" :
				$"return All.FirstOrDefault(x => x.ServiceCode == serviceCode) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByServiceCode(int serviceCode)", bodyGetByServiceCode);

			//
			// GetById
			//
			codeCSharp.AddEmptyLine();

			var bodyGetById = firstType == null ? "return All.First(x => x.Id == id);" :
				$"return All.FirstOrDefault(x => x.Id == id) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetById(int id)", bodyGetById);

			//
			// Translate
			//
			codeCSharp.AddEmptyLine();

			var bodyTranslate = "foreach (var type in All) { type.Translate(); }";
			codeCSharp.AddMethodWithBody($"public static void Translate()", bodyTranslate);

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypesName()}.cs");
			codeCSharp.Save(path);
		}

	}

	public class GeneratorCodeGroupLogginType : GeneratorCodeBase
	{
		public DictionaryList<string, string> Groups { get; set; }

		public GeneratorCodeGroupLogginType() { }

		public GeneratorCodeGroupLogginType(XmlElement root)
			: base(root)
		{
		}

		public override void CreateEntityEnum(string? firstEnum = null, string? firstEnumCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Localilzation");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Перечисление однозначно определяющее ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.Add($"public enum {GetEnumName()}");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			if (firstEnum != null)
			{
				if (firstEnumCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstEnumCommnent, false);
				}
				codeCSharp.Add(firstEnum);
				codeCSharp.AddEmptyLine();
			}

			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					var textEs = GetNameForLang(element, "description", "es");
					var textZh = GetNameForLang(element, "description", "zh");

					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);

					codeCSharp.Add($"[LocalizeOnLang(\"ru\", \"{textRu}\")]");
					codeCSharp.Add($"[LocalizeOnLang(\"en\", \"{textEn}\")]");
					if (!string.IsNullOrEmpty(textZh))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"zh\", \"{textZh}\")]");
					}
					if (!string.IsNullOrEmpty(textEs))
					{
						codeCSharp.Add($"[LocalizeOnLang(\"es\", \"{textEs}\")]");
					}
					codeCSharp.Add(new CTextLine($"{enumName} = {serviceCode},"));

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetEnumName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityType(string baseClass)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceUsing("Lithosphere.Core.Common");
			codeCSharp.AddEmptyLine();
			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var nameType = GetNameForLang(Root, "description", "ru");
			codeCSharp.AddCommentSummary(false, nameType, false);

			codeCSharp.AddClassPublic($"{GetTypeName()} : {baseClass}<{GetEnumName()}>");
			codeCSharp.CurrentIndent++;

			codeCSharp.AddCommentSummary(false, "Список типов кривых для данной группы.", false);
			codeCSharp.Add("public LoggingType[] LoggingTypes { get; init; }");
			codeCSharp.AddEmptyLine();

			codeCSharp.AddConstructor($"public {GetTypeName()}(string name, int serviceCode, {GetEnumName()} type)",
				"name, serviceCode");
			codeCSharp.CurrentIndent++;

			codeCSharp.Add("Type = type;");

			codeCSharp.CurrentIndent--;
			codeCSharp.AddConstructorClose();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypeName()}.cs");
			codeCSharp.Save(path);
		}

		public override void CreateEntityTypes(string? firstType = null, string? firstTypeCommnent = null)
		{
			CTextGenerateCodeCSharp codeCSharp = new CTextGenerateCodeCSharp();

			codeCSharp.AddNamespaceOpen("Lithosphere.Core.Typed");
			codeCSharp.CurrentIndent++;

			var namePrefx = "Список однозначно определяющий доступные ";
			var nameType = GetNameForLang(Root, "description", "ru").ToLower();
			codeCSharp.AddCommentSummary(false, namePrefx + nameType, false);

			codeCSharp.AddClassStaticPublic($"{GetTypesName()}");
			codeCSharp.CurrentIndent++;

			if (firstType != null)
			{
				if (firstTypeCommnent != null)
				{
					codeCSharp.AddCommentSummary(false, firstTypeCommnent, false);
				}
				codeCSharp.Add($"public static readonly {GetTypeName()} {firstType} = new (nameof({firstType}), -1, {GetEnumName()}.{firstType});");
				codeCSharp.AddEmptyLine();
			}

			List<string> types = new List<string>();
			foreach (XmlNode node in Root.ChildNodes)
			{
				if (node is XmlElement element && element.Name == "item")
				{
					var textRu = GetNameForLang(element, "description", "ru");
					var textEn = GetNameForLang(element, "description", "en");
					codeCSharp.AddCommentSummary(false, textRu, false);

					var enumName = TransformToIdentificator(textEn);
					var serviceCode = GetServiceCode(element);
					var codeType = $"{GetEnumName()}.{enumName}";
					types.Add(enumName);

					codeCSharp.Add($"public static readonly {GetTypeName()} {enumName} = new (nameof({codeType}), {serviceCode}, {codeType})");

					codeCSharp.AddOpenBlock();
					codeCSharp.CurrentIndent++;

					codeCSharp.Add("LoggingTypes = new LoggingType[]");
					codeCSharp.AddOpenBlock();
					codeCSharp.CurrentIndent++;

					var list = Groups[enumName];
					foreach (var unit in list)
					{
						codeCSharp.Add($"LoggingTypes.{unit},");
					}

					codeCSharp.CurrentIndent--;
					codeCSharp.AddCloseBlock();

					codeCSharp.CurrentIndent--;
					codeCSharp.AddCloseBlock(";");

					codeCSharp.AddEmptyLine();
				}
			}

			codeCSharp.AddCommentSummary(false, "Список " + CorrectNameForList(nameType), false);
			codeCSharp.Add($"public static readonly {GetTypeName()}[] All = new {GetTypeName()}[]");
			codeCSharp.AddOpenBlock();
			codeCSharp.CurrentIndent++;

			for (int i = 0; i < types.Count; i++)
			{
				var isLast = i == types.Count - 1;
				var text = isLast ? types[i] : types[i] + ",";
				codeCSharp.Add(text);
			}
			codeCSharp.CurrentIndent--;
			codeCSharp.AddCloseBlock(";");

			//
			// GetByType
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByType = firstType == null ? "return All.First(x => x.Type == type);" :
				$"return All.FirstOrDefault(x => x.Type == type) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByType({GetEnumName()} type)", bodyGetByType);


			//
			// GetByServiceCode
			//
			codeCSharp.AddEmptyLine();

			var bodyGetByServiceCode = firstType == null ? "return All.First(x => x.ServiceCode == serviceCode);" :
				$"return All.FirstOrDefault(x => x.ServiceCode == serviceCode) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetByServiceCode(int serviceCode)", bodyGetByServiceCode);

			//
			// GetById
			//
			codeCSharp.AddEmptyLine();

			var bodyGetById = firstType == null ? "return All.First(x => x.Id == id);" :
				$"return All.FirstOrDefault(x => x.Id == id) ?? {firstType};";
			codeCSharp.AddMethodWithBody($"public static {GetTypeName()} GetById(int id)", bodyGetById);

			//
			// Translate
			//
			codeCSharp.AddEmptyLine();

			var bodyTranslate = "foreach (var type in All) { type.Translate(); }";
			codeCSharp.AddMethodWithBody($"public static void Translate()", bodyTranslate);

			codeCSharp.CurrentIndent--;
			codeCSharp.AddClassEndDeclaration();

			codeCSharp.CurrentIndent--;
			codeCSharp.AddNamespaceClose();

			var directory = Path.Combine(FolderName, EntityName);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{GetTypesName()}.cs");
			codeCSharp.Save(path);
		}
	}
}
