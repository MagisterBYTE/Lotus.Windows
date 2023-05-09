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
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
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

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор шаблона привязки данных
			/// </summary>
			/// <param name="item">Объект</param>
			/// <param name="container">Контейнер</param>
			/// <returns>Нужный шаблон</returns>
			//---------------------------------------------------------------------------------------------------------
			public override DataTemplate SelectTemplate(Object item, DependencyObject container)
			{
				DataTemplate template = Invalid;
				CPropertyModelBase model = item as CPropertyModelBase;
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
								//if (model.IsReadOnly)
								//{
									template = String;
								//}
								//else
								//{
									template = Enum;
								//}
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
								CPropertyModelObject model_object = model as CPropertyModelObject;

								DataTemplate data_template = new DataTemplate();
								data_template.DataType = typeof(CPropertyModelObject);

								FrameworkElementFactory element = new FrameworkElementFactory(model_object.EditorType);

								System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(model_object.EditorType);
								var property_description_value = pdc["Value"];

								var dependency_property = DependencyPropertyDescriptor.FromProperty(property_description_value);

								Binding binding = new Binding("Value");
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


				return (template);
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
			internal Object mSelectedObject;
			internal String mTypeName;
			internal String mObjectName;
			internal Boolean mIsGrouping;
			internal Boolean mIsFiltration;
			internal String mFilterString;
			internal CPropertyDesc[] mPropertiesDesc;
			internal ListArray<CPropertyModelBase> mProperties;
			internal ListCollectionView mPropertiesView;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Выбранный объект
			/// </summary>
			public Object SelectedObject
			{
				get { return (mSelectedObject); }
				set
				{
					if(mSelectedObject != value)
					{
						mSelectedObject = value;
						SetInstance();
						NotifyPropertyChanged(PropertyArgsSelectedObject);
					}
				}
			}

			/// <summary>
			/// Имя типа
			/// </summary>
			public String TypeName
			{
				get { return (mTypeName); }
				set
				{
					if (mTypeName != value)
					{
						mTypeName = value;
						textTypeName.Text = mTypeName;
						NotifyPropertyChanged(PropertyArgsTypeName);
					}
				}
			}

			/// <summary>
			/// Имя объекта
			/// </summary>
			public String ObjectName
			{
				get { return (mObjectName); }
				set
				{
					if (mObjectName != value)
					{
						mObjectName = value;
						textObjectName.Text = mObjectName;
						NotifyPropertyChanged(PropertyArgsObjectName);
					}
				}
			}

			/// <summary>
			/// Статус основного группирования
			/// </summary>
			[Browsable(false)]
			public Boolean IsGrouping
			{
				get { return (mIsGrouping); }
				set
				{
					if (mIsGrouping != value)
					{
						mIsGrouping = value;

						if (mIsGrouping)
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
			public Boolean IsFiltration
			{
				get { return (mIsFiltration); }
				set
				{
					if (mIsFiltration != value)
					{
						mIsFiltration = value;

						if (mIsFiltration)
						{
							mPropertiesView.Filter += OnPropertyViewFilter;
						}
						else
						{
							mPropertiesView.Filter -= OnPropertyViewFilter;
						}

						NotifyPropertyChanged(PropertyArgsIsFiltration);
					}
				}
			}

			/// <summary>
			/// Строка для фильтра
			/// </summary>
			[Browsable(false)]
			public String FilterString
			{
				get { return (mFilterString); }
				set
				{
					mFilterString = value;
					NotifyPropertyChanged(PropertyArgsFilterString);
					if (mPropertiesView != null)
					{
						mPropertiesView.Refresh();
					}
				}
			}

			/// <summary>
			/// Список свойств
			/// </summary>
			public ListArray<CPropertyModelBase> Properties
			{
				get { return (mProperties); }
			}

			/// <summary>
			/// Список свойств для отображения
			/// </summary>
			public ListCollectionView PropertiesView
			{
				get { return (mPropertiesView); }
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
				mProperties = new ListArray<CPropertyModelBase>();
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка нового объекта для отображения свойств
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void SetInstance()
			{
				if (mSelectedObject != null)
				{
					// Очищаем список свойств
					mProperties.Clear();

					// Если есть общая поддержка инспектора свойств
					ILotusSupportViewInspector support_inspector = mSelectedObject as ILotusSupportViewInspector;
					if (support_inspector != null)
					{
						TypeName = support_inspector.InspectorTypeName;
						ObjectName = support_inspector.InspectorObjectName;
					}

					// Если есть расширенная поддержка инспектора свойств для получение описания свойств
					ILotusSupportEditInspector support_inspector_ex = mSelectedObject as ILotusSupportEditInspector;
					if (support_inspector_ex != null)
					{
						// Получаем список описания свойств
						mPropertiesDesc = support_inspector_ex.GetPropertiesDesc();

						// Сформируем правильный порядок
						for (Int32 i = 0; i < mPropertiesDesc.Length; i++)
						{
							if(mPropertiesDesc[i].PropertyOrder == -1)
							{
								mPropertiesDesc[i].PropertyOrder = i;
							}
						}
					}

					// Добавляем свойства для отображения
					AddModelProperties();

					// Обновляем группы
					UpdateCategoryOrders();

					// Сортируем
					mProperties.SortAscending();

					// Устанавливаем экземпляр объекта
					for (Int32 i = 0; i < mProperties.Count; i++)
					{
						mProperties[i].Instance = mSelectedObject;
					}

					// Создаем коллекцию для отображения
					mPropertiesView = new ListCollectionView(mProperties);
					mPropertiesView.Filter += OnPropertyViewFilter;
					dataProperties.ItemsSource = mPropertiesView;

					if (toogleButtonGroup.IsChecked.Value)
					{
						SetGroupings();
					}

					// Информируем
					CheckIsValueFromList();
				}
				else
				{
					mProperties.Clear();
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
				PropertyInfo[] props = mSelectedObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(x => x.MetadataToken).ToArray();
				for (Int32 i = 0; i < props.Length; i++)
				{
					PropertyInfo property_info = props[i];
					Type type = property_info.PropertyType;

					// Проверка на видимость свойства
					BrowsableAttribute browsable_attribute = property_info.GetAttribute<BrowsableAttribute>();
					if(browsable_attribute != null && browsable_attribute.Browsable == false)
					{
						continue;
					}

					// Получаем список описаний свойства
					List<CPropertyDesc> property_desc = GetPropertyDesc(property_info);

					//  Проверка на видимость
					if (property_desc != null && property_desc.Any(item => item.IsHideInspector))
					{
						continue;
					}

					// Логическое свойство
					if (type.Name == nameof(Boolean))
					{
						mProperties.Add(new PropertyModel<Boolean>(property_info, property_desc, TPropertyType.Boolean));
						continue;
					}

					// Перечисление
					if (type.IsEnum)
					{
						mProperties.Add(new CPropertyModelEnum(property_info, property_desc));
						continue;
					}

					// Числовое свойство
					if (type.IsNumericType())
					{
						AddModelPropertyNumeric(property_info, property_desc);
						continue;
					}

					if (type.Name == nameof(TMeasurementValue))
					{
						mProperties.Add(new PropertyModelMeasurementValue(property_info, property_desc));
						continue;
					}

					if (type.Name == nameof(DateTime))
					{
						mProperties.Add(new PropertyModel<DateTime>(property_info, property_desc, TPropertyType.DateTime));
						continue;
					}

					if (type.Name == nameof(String))
					{
						mProperties.Add(new PropertyModel<String>(property_info, property_desc, TPropertyType.String));
						continue;
					}

					if (type.Name == nameof(Point) ||
						type.Name == nameof(Vector) ||
						type.Name == nameof(Vector2Df) ||
						type.Name == nameof(Vector2D))
					{
						mProperties.Add(new CPropertyModelVector2D(property_info, property_desc));
						continue;
					}

					// Если свойство имеет указанный атрибут значит применяется специальный редактор для свойства 
					if (property_info.HasAttribute<LotusInspectorTypeEditor>())
					{
						var attr = property_info.GetAttribute<LotusInspectorTypeEditor>();
						mProperties.Add(new CPropertyModelObject(property_info, property_desc, attr.EditorType));
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
						mProperties.Add(new PropertyModelRange<Char>(property_info, property_desc));
						break;
					case TypeCode.SByte:
						mProperties.Add(new PropertyModelRange<SByte>(property_info, property_desc));
						break;
					case TypeCode.Byte:
						mProperties.Add(new PropertyModelRange<Byte>(property_info, property_desc));
						break;
					case TypeCode.Int16:
						mProperties.Add(new PropertyModelRange<Int16>(property_info, property_desc));
						break;
					case TypeCode.UInt16:
						mProperties.Add(new PropertyModelRange<UInt16>(property_info, property_desc));
						break;
					case TypeCode.Int32:
						mProperties.Add(new PropertyModelRange<Int32>(property_info, property_desc));
						break;
					case TypeCode.UInt32:
						mProperties.Add(new PropertyModelRange<UInt32>(property_info, property_desc));
						break;
					case TypeCode.Int64:
						mProperties.Add(new PropertyModelRange<Int64>(property_info, property_desc));
						break;
					case TypeCode.UInt64:
						mProperties.Add(new PropertyModelRange<UInt64>(property_info, property_desc));
						break;
					case TypeCode.Single:
						mProperties.Add(new PropertyModelRange<Single>(property_info, property_desc));
						break;
					case TypeCode.Double:
						mProperties.Add(new PropertyModelRange<Double>(property_info, property_desc));
						break;
					case TypeCode.Decimal:
						mProperties.Add(new PropertyModelRange<Decimal>(property_info, property_desc));
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
			protected List<CPropertyDesc> GetPropertyDesc(PropertyInfo property_info)
			{
				List<CPropertyDesc> result = new List<CPropertyDesc>();

				if (mPropertiesDesc == null) return (null);

				for (Int32 i = 0; i < mPropertiesDesc.Length; i++)
				{
					if(mPropertiesDesc[i].PropertyName == property_info.Name)
					{
						result.Add(mPropertiesDesc[i]);
					}
				}

				return (result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление порядка отображения групп 
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void UpdateCategoryOrders()
			{
				// Собираем группы
				List<String> groups = new List<String>();
				for (Int32 i = 0; i < mProperties.Count; i++)
				{
					groups.AddIfNotContains(mProperties[i].Category);
				}

				for (Int32 i = 0; i < groups.Count; i++)
				{
					String group = groups[i];
					Int32 order = -1;
					for (Int32 j = 0; j < mProperties.Count; j++)
					{
						if(mProperties[j].Category == group)
						{
							if(mProperties[j].CategoryOrder != -1)
							{
								order = mProperties[j].CategoryOrder;
								break;
							}
						}
					}

					if(order != -1)
					{
						for (Int32 j = 0; j < mProperties.Count; j++)
						{
							if (mProperties[j].Category == group)
							{
								mProperties[j].CategoryOrder = order;
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
			protected virtual Boolean OnPropertyViewFilter(Object item)
			{
				if (String.IsNullOrEmpty(FilterString))
				{
					return (true);
				}
				else
				{
					CPropertyModelBase property_model = item as CPropertyModelBase;
					return (property_model.DisplayName.Contains(FilterString, StringComparison.OrdinalIgnoreCase));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение строки фильтра
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTextFilterProperty_TextChanged(Object sender, TextChangedEventArgs args)
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
				if (mPropertiesView != null)
				{
					mPropertiesView.GroupDescriptions.Clear();
					mPropertiesView.GroupDescriptions.Add(PropertyGroupDescriptionGroup);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления группирования
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void UnsetGroupings()
			{
				if (mPropertiesView != null)
				{
					mPropertiesView.GroupDescriptions.Remove(PropertyGroupDescriptionGroup);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Группирование свойств по категории
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioButtonGroup_Checked(Object sender, RoutedEventArgs args)
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
			private void OnRadioButtonAlphabetically_Checked(Object sender, RoutedEventArgs args)
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
				for (Int32 i = 0; i < mProperties.Count; i++)
				{
					CPropertyModelBase property_model = mProperties[i];

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
			private void OnButtonStringContextMenu_Click(Object sender, RoutedEventArgs args)
			{
				FrameworkElement element = args.Source as FrameworkElement;
				CPropertyModelBase property_model = element.DataContext as CPropertyModelBase;
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
			private void OnButtonListValuesString_ContextMenuOpening(Object sender, ContextMenuEventArgs args)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка значения из списка для свойства строкового типа
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemSetValueFromListForString_Click(Object sender, RoutedEventArgs args)
			{
				MenuItem menu_item = args.OriginalSource as MenuItem;
				CPropertyModelBase property_model = menu_item.Tag as CPropertyModelBase;
				if (property_model != null)
				{
					property_model.SetValue(menu_item.Header.ToString());
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
			private void OnUserControl_Loaded(Object sender, RoutedEventArgs args)
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
			private void OnButtonClearFilterProperty_Click(Object sender, RoutedEventArgs args)
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
			private void OnDataProperties_SelectionChanged(Object sender, SelectionChangedEventArgs args)
			{
				CPropertyModelBase property_model = mPropertiesView.CurrentItem as CPropertyModelBase;
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
			private void OnTextBoxString_LostFocus(Object sender, RoutedEventArgs args)
			{
				CPropertyModelBase property_model = mPropertiesView.CurrentItem as CPropertyModelBase;
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
			private void OnButtonAttribute_Click(Object sender, RoutedEventArgs args)
			{
				if(sender is Button button)
				{
					String method_name = button.Tag.ToString();
					XReflection.InvokeMethod(mSelectedObject, method_name);
				}
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(String property_name = "")
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