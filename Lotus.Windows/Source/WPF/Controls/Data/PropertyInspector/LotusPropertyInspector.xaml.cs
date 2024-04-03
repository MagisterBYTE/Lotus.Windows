//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для работы с данными
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusPropertyInspector.xaml.cs
*		Элемент - редактор свойств объекта.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Core.Inspector;
using Lotus.Maths;
using Lotus.UnitMeasurement;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFControlsData
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Селектор шаблона данных
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CPropertyModelDataSelector : DataTemplateSelector
		{
			#region ======================================= ДАННЫЕ ====================================================
			/// <summary>
			/// Шаблон для представления логического значения
			/// </summary>
			public DataTemplate Boolean { get; set; }

			/// <summary>
			/// Шаблон для представления числовых значений
			/// </summary>
			public DataTemplate Numeric { get; set; }

			/// <summary>
			/// Шаблон для представления значений единиц измерения
			/// </summary>
			public DataTemplate Measurement { get; set; }

			/// <summary>
			/// Шаблон для представления перечесления
			/// </summary>
			public DataTemplate Enum { get; set; }

			/// <summary>
			/// Шаблон для представления строкового значения
			/// </summary>
			public DataTemplate String { get; set; }

			/// <summary>
			/// Шаблон для представления значения даты
			/// </summary>
			public DataTemplate DateTime { get; set; }

			/// <summary>
			/// Шаблон для представления значения двухмерного вектора
			/// </summary>
			public DataTemplate Vector2D { get; set; }

			/// <summary>
			/// Шаблон для представления недопустимого типа
			/// </summary>
			public DataTemplate Invalid { get; set; }
			#endregion

			#region Main methods
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор шаблона привязки данных
			/// </summary>
			/// <param name="item">Объект</param>
			/// <param name="container">Контейнер</param>
			/// <returns>Нужный шаблон</returns>
			//---------------------------------------------------------------------------------------------------------
			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				DataTemplate template = Invalid;
				var model = item as CPropertyModelBase;
				if (model != null)
				{
					switch (model.PropertyType)
					{
						case TPropertyType.Boolean:
							{
								template = Boolean;
							}
							break;
						case TPropertyType.Numeric:
							{
								template = Numeric;
							}
							break;
						case TPropertyType.Measurement:
							{
								template = Measurement;
							}
							break;
						case TPropertyType.Enum:
							{
								template = Enum;
							}
							break;
						case TPropertyType.String:
							{
								template = String;
							}
							break;
						case TPropertyType.DateTime:
							{
								template = DateTime;
							}
							break;
						case TPropertyType.Vector2D:
							{
								template = Vector2D;
							}
							break;
						case TPropertyType.Object:
							{
								var model_object = model as CPropertyModelObject;

								var data_template = new DataTemplate();
								data_template.DataType = typeof(CPropertyModelObject);

								var element = new FrameworkElementFactory(model_object!.EditorType);

								System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(model_object.EditorType);
								var property_description_value = pdc["Value"];

								var dependency_property = DependencyPropertyDescriptor.FromProperty(property_description_value);

								var binding = new Binding("Value");
								binding.Mode = BindingMode.TwoWay;
								binding.Source = model_object;
								element.SetBinding(dependency_property.DependencyProperty, binding);

								data_template.VisualTree = element;

								template = data_template;
							}
							break;
						case TPropertyType.Unknow:
							{
								template = Invalid;
							}
							break;
						default:
							break;
					}
				}


				return template;
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Редактор свойств объекта
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusPropertyInspector : UserControl, ILotusPropertyInspector, INotifyPropertyChanged
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			private static readonly PropertyChangedEventArgs PropertyArgsSelectedObject = new PropertyChangedEventArgs(nameof(SelectedObject));
			private static readonly PropertyChangedEventArgs PropertyArgsTypeName = new PropertyChangedEventArgs(nameof(TypeName));
			private static readonly PropertyChangedEventArgs PropertyArgsObjectName = new PropertyChangedEventArgs(nameof(ObjectName));
			private static readonly PropertyChangedEventArgs PropertyArgsIsGrouping = new PropertyChangedEventArgs(nameof(IsGrouping));
			private static readonly PropertyChangedEventArgs PropertyArgsIsFiltration = new PropertyChangedEventArgs(nameof(IsFiltration));
			private static readonly PropertyChangedEventArgs PropertyArgsFilterString = new PropertyChangedEventArgs(nameof(FilterString));
			private static readonly PropertyGroupDescription PropertyGroupDescriptionGroup = new PropertyGroupDescription(nameof(CPropertyDesc.Category));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected internal object _selectedObject;
			protected internal string _typeName;
			protected internal string _objectName;
			protected internal bool _isGrouping;
			protected internal bool _isFiltration;
			protected internal string _filterString;
			protected internal CPropertyDesc[] _propertiesDesc;
			protected internal ListArray<CPropertyModelBase> _properties;
			protected internal ListCollectionView _propertiesView;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Выбранный объект
			/// </summary>
			public object SelectedObject
			{
				get { return _selectedObject; }
				set
				{
					if(_selectedObject != value)
					{
						_selectedObject = value;
						SetInstance();
						NotifyPropertyChanged(PropertyArgsSelectedObject);
					}
				}
			}

			/// <summary>
			/// Имя типа
			/// </summary>
			public string TypeName
			{
				get { return _typeName; }
				set
				{
					if (_typeName != value)
					{
						_typeName = value;
						textTypeName.Text = _typeName;
						NotifyPropertyChanged(PropertyArgsTypeName);
					}
				}
			}

			/// <summary>
			/// Имя объекта
			/// </summary>
			public string ObjectName
			{
				get { return _objectName; }
				set
				{
					if (_objectName != value)
					{
						_objectName = value;
						textObjectName.Text = _objectName;
						NotifyPropertyChanged(PropertyArgsObjectName);
					}
				}
			}

			/// <summary>
			/// Статус основного группирования
			/// </summary>
			[Browsable(false)]
			public bool IsGrouping
			{
				get { return _isGrouping; }
				set
				{
					if (_isGrouping != value)
					{
						_isGrouping = value;

						if (_isGrouping)
						{
							SetGroupings();
						}
						else
						{
							UnsetGroupings();
						}

						NotifyPropertyChanged(PropertyArgsIsGrouping);
					}
				}
			}

			/// <summary>
			/// Статус фильтрации данных
			/// </summary>
			[Browsable(false)]
			public bool IsFiltration
			{
				get { return _isFiltration; }
				set
				{
					if (_isFiltration != value)
					{
						_isFiltration = value;

						if (_isFiltration)
						{
							_propertiesView.Filter += OnPropertyViewFilter;
						}
						else
						{
							_propertiesView.Filter -= OnPropertyViewFilter;
						}

						NotifyPropertyChanged(PropertyArgsIsFiltration);
					}
				}
			}

			/// <summary>
			/// Строка для фильтра
			/// </summary>
			[Browsable(false)]
			public string FilterString
			{
				get { return _filterString; }
				set
				{
					_filterString = value;
					NotifyPropertyChanged(PropertyArgsFilterString);
					if (_propertiesView != null)
					{
						_propertiesView.Refresh();
					}
				}
			}

			/// <summary>
			/// Список свойств
			/// </summary>
			public ListArray<CPropertyModelBase> Properties
			{
				get { return _properties; }
			}

			/// <summary>
			/// Список свойств для отображения
			/// </summary>
			public ListCollectionView PropertiesView
			{
				get { return _propertiesView; }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusPropertyInspector()
			{
				InitializeComponent();
				_properties = new ListArray<CPropertyModelBase>();
			}
			#endregion

			#region Main methods
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка нового объекта для отображения свойств
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void SetInstance()
			{
				if (_selectedObject != null)
				{
					// Очищаем список свойств
					_properties.Clear();

					// Если есть общая поддержка инспектора свойств
					var support_inspector = _selectedObject as ILotusSupportViewInspector;
					if (support_inspector != null)
					{
						TypeName = support_inspector.InspectorTypeName;
						ObjectName = support_inspector.InspectorObjectName;
					}

					// Если есть расширенная поддержка инспектора свойств для получение описания свойств
					var support_inspector_ex = _selectedObject as ILotusSupportEditInspector;
					if (support_inspector_ex != null)
					{
						// Получаем список описания свойств
						_propertiesDesc = support_inspector_ex.GetPropertiesDesc();

						// Сформируем правильный порядок
						for (var i = 0; i < _propertiesDesc.Length; i++)
						{
							if(_propertiesDesc[i].PropertyOrder == -1)
							{
								_propertiesDesc[i].PropertyOrder = i;
							}
						}
					}

					// Добавляем свойства для отображения
					AddModelProperties();

					// Обновляем группы
					UpdateCategoryOrders();

					// Сортируем
					_properties.SortAscending();

					// Устанавливаем экземпляр объекта
					for (var i = 0; i < _properties.Count; i++)
					{
						_properties[i].Instance = _selectedObject;
					}

					// Создаем коллекцию для отображения
					_propertiesView = new ListCollectionView(_properties);
					_propertiesView.Filter += OnPropertyViewFilter;
					dataProperties.ItemsSource = _propertiesView;

					if (toogleButtonGroup != null && toogleButtonGroup.IsChecked.GetValueOrDefault())
					{
						SetGroupings();
					}

					// Информируем
					CheckIsValueFromList();
				}
				else
				{
					_properties.Clear();
					dataProperties.ItemsSource = null;
					textTypeName.Text = "";
					textObjectName.Text = "";
					textDescription.Text = "";
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление модели свойств
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void AddModelProperties()
			{
				// Получаем список свойств
				PropertyInfo[] props = _selectedObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(x => x.MetadataToken).ToArray();
				for (var i = 0; i < props.Length; i++)
				{
					PropertyInfo property_info = props[i];
					Type type = property_info.PropertyType;

					// Проверка на видимость свойства
					BrowsableAttribute? browsable_attribute = property_info.GetAttribute<BrowsableAttribute>();
					if(browsable_attribute != null && browsable_attribute.Browsable == false)
					{
						continue;
					}

					// Получаем список описаний свойства
					List<CPropertyDesc>? property_desc = GetPropertyDesc(property_info);

					//  Проверка на видимость
					if (property_desc != null && property_desc.Any(item => item.IsHideInspector))
					{
						continue;
					}

					// Логическое свойство
					if (type.Name == nameof(Boolean))
					{
						_properties.Add(new PropertyModel<bool>(property_info, property_desc!, TPropertyType.Boolean));
						continue;
					}

					// Перечисление
					if (type.IsEnum)
					{
						_properties.Add(new CPropertyModelEnum(property_info, property_desc!));
						continue;
					}

					// Числовое свойство
					if (type.IsNumericType())
					{
						AddModelPropertyNumeric(property_info, property_desc!);
						continue;
					}

					if (type.Name == nameof(TMeasurementValue))
					{
						_properties.Add(new PropertyModelMeasurementValue(property_info, property_desc!));
						continue;
					}

					if (type.Name == nameof(DateTime))
					{
						_properties.Add(new PropertyModel<DateTime>(property_info, property_desc!, TPropertyType.DateTime));
						continue;
					}

					if (type.Name == nameof(String))
					{
						_properties.Add(new PropertyModel<string>(property_info, property_desc!, TPropertyType.String));
						continue;
					}

					if (type.Name == nameof(Point) ||
						type.Name == nameof(Vector) ||
						type.Name == nameof(Vector2Df) ||
						type.Name == nameof(Vector2D))
					{
						_properties.Add(new CPropertyModelVector2D(property_info, property_desc!));
						continue;
					}

					// Если свойство имеет указанный атрибут значит применяется специальный редактор для свойства 
					if (property_info.HasAttribute<LotusInspectorTypeEditor>())
					{
						var attr = property_info.GetAttribute<LotusInspectorTypeEditor>();
						_properties.Add(new CPropertyModelObject(property_info, property_desc!, attr!.EditorType));
						continue;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление модели свойства для числовых типов
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			/// <param name="property_desc">Список описания свойства</param>
			//---------------------------------------------------------------------------------------------------------
			protected void AddModelPropertyNumeric(PropertyInfo property_info, List<CPropertyDesc> property_desc)
			{
				switch (Type.GetTypeCode(property_info.PropertyType))
				{
					case TypeCode.Empty:
						break;
					case TypeCode.Object:
						break;
					case TypeCode.DBNull:
						break;
					case TypeCode.Boolean:
						break;
					case TypeCode.Char:
						_properties.Add(new PropertyModelRange<char>(property_info, property_desc));
						break;
					case TypeCode.SByte:
						_properties.Add(new PropertyModelRange<sbyte>(property_info, property_desc));
						break;
					case TypeCode.Byte:
						_properties.Add(new PropertyModelRange<byte>(property_info, property_desc));
						break;
					case TypeCode.Int16:
						_properties.Add(new PropertyModelRange<short>(property_info, property_desc));
						break;
					case TypeCode.UInt16:
						_properties.Add(new PropertyModelRange<ushort>(property_info, property_desc));
						break;
					case TypeCode.Int32:
						_properties.Add(new PropertyModelRange<int>(property_info, property_desc));
						break;
					case TypeCode.UInt32:
						_properties.Add(new PropertyModelRange<uint>(property_info, property_desc));
						break;
					case TypeCode.Int64:
						_properties.Add(new PropertyModelRange<long>(property_info, property_desc));
						break;
					case TypeCode.UInt64:
						_properties.Add(new PropertyModelRange<ulong>(property_info, property_desc));
						break;
					case TypeCode.Single:
						_properties.Add(new PropertyModelRange<float>(property_info, property_desc));
						break;
					case TypeCode.Double:
						_properties.Add(new PropertyModelRange<double>(property_info, property_desc));
						break;
					case TypeCode.Decimal:
						_properties.Add(new PropertyModelRange<decimal>(property_info, property_desc));
						break;
					case TypeCode.DateTime:
						break;
					case TypeCode.String:
						break;
					default:
						break;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение списка описание свойства по указным метаданными свойства
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			/// <returns>Список описания свойств</returns>
			//---------------------------------------------------------------------------------------------------------
			protected List<CPropertyDesc>? GetPropertyDesc(PropertyInfo property_info)
			{
				var result = new List<CPropertyDesc>();

				if (_propertiesDesc == null) return null;

				for (var i = 0; i < _propertiesDesc.Length; i++)
				{
					if(_propertiesDesc[i].PropertyName == property_info.Name)
					{
						result.Add(_propertiesDesc[i]);
					}
				}

				return result;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление порядка отображения групп 
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void UpdateCategoryOrders()
			{
				// Собираем группы
				var groups = new List<string>();
				for (var i = 0; i < _properties.Count; i++)
				{
					groups.AddIfNotContains(_properties[i].Category);
				}

				for (var i = 0; i < groups.Count; i++)
				{
					var group = groups[i];
					var order = -1;
					for (var j = 0; j < _properties.Count; j++)
					{
						if(_properties[j].Category == group)
						{
							if(_properties[j].CategoryOrder != -1)
							{
								order = _properties[j].CategoryOrder;
								break;
							}
						}
					}

					if(order != -1)
					{
						for (var j = 0; j < _properties.Count; j++)
						{
							if (_properties[j].Category == group)
							{
								_properties[j].CategoryOrder = order;
							}
						}
					}
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ФИЛЬТРОВАНИЯ СВОЙСТВ ===============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на соответствие фильтру
			/// </summary>
			/// <param name="item">Объект</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			protected virtual bool OnPropertyViewFilter(object item)
			{
				if (string.IsNullOrEmpty(FilterString))
				{
					return true;
				}
				else
				{
					var property_model = item as CPropertyModelBase;
					return property_model!.DisplayName.Contains(FilterString, StringComparison.OrdinalIgnoreCase);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение строки фильтра
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTextFilterProperty_TextChanged(object sender, TextChangedEventArgs args)
			{
				FilterString = textFilterProperty.Text;
			}
			#endregion

			#region ======================================= МЕТОДЫ ГРУППИРОВАНИЯ СВОЙСТВ ==============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка группирования
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void SetGroupings()
			{
				if (_propertiesView != null)
				{
					_propertiesView.GroupDescriptions.Clear();
					_propertiesView.GroupDescriptions.Add(PropertyGroupDescriptionGroup);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления группирования
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void UnsetGroupings()
			{
				if (_propertiesView != null)
				{
					_propertiesView.GroupDescriptions.Remove(PropertyGroupDescriptionGroup);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Группирование свойств по категории
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioButtonGroup_Checked(object sender, RoutedEventArgs args)
			{
				toogleButtonAlphabetically.IsChecked = false;
				SetGroupings();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Группирование свойств по алфавиту
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioButtonAlphabetically_Checked(object sender, RoutedEventArgs args)
			{
				toogleButtonGroup.IsChecked = false;
				UnsetGroupings();
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ СО СПИСКОМ ЗНАЧЕНИЙ =========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на значение из списка
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void CheckIsValueFromList()
			{
				for (var i = 0; i < _properties.Count; i++)
				{
					CPropertyModelBase property_model = _properties[i];

					if (property_model != null)
					{
						property_model.CheckIsValueFromList();
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие контекстного меню для списка значений строкового типа
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonStringContextMenu_Click(object sender, RoutedEventArgs args)
			{
				var element = (args.Source as FrameworkElement)!;
				var property_model = element.DataContext as CPropertyModelBase;
				ContextMenu context_menu = element.ContextMenu;
				if (context_menu != null)
				{
					if (property_model != null)
					{
						property_model.AssingContenxMenuListValues(context_menu);
					}

					context_menu.IsOpen = true;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание контекстного меню для списка значений строкового типа
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonListValuesString_ContextMenuOpening(object sender, ContextMenuEventArgs args)
			{
				// Method intentionally left empty.
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка значения из списка для свойства строкового типа
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemSetValueFromListForString_Click(object sender, RoutedEventArgs args)
			{
				var menu_item = (args.OriginalSource as MenuItem)!;
				var property_model = menu_item.Tag as CPropertyModelBase;
				if (property_model != null)
				{
					property_model.SetValue(menu_item.Header.ToString()!);
					property_model.IsValueFromList = true;
				}
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка пользовательского элемента и готовность его к отображению
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnUserControl_Loaded(object sender, RoutedEventArgs args)
			{
				toogleButtonGroup.IsChecked = true;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Очистка фильтра
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonClearFilterProperty_Click(object sender, RoutedEventArgs args)
			{
				textFilterProperty.Text = "";
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение выбора свойства
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDataProperties_SelectionChanged(object sender, SelectionChangedEventArgs args)
			{
				var property_model = _propertiesView.CurrentItem as CPropertyModelBase;
				if (property_model != null)
				{
					textDescription.Text = property_model.Description;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Потеря фокуса текстового поля значения строкового типа
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTextBoxString_LostFocus(object sender, RoutedEventArgs args)
			{
				var property_model = _propertiesView.CurrentItem as CPropertyModelBase;
				if (property_model != null)
				{
					property_model.CheckIsValueFromList();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вызов метода
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonAttribute_Click(object sender, RoutedEventArgs args)
			{
				if(sender is Button button)
				{
					var method_name = (button.Tag.ToString())!;
					XReflection.InvokeMethod(_selectedObject, method_name);
				}
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler? PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(string property_name = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property_name));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(PropertyChangedEventArgs args)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, args);
				}
			}


			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================