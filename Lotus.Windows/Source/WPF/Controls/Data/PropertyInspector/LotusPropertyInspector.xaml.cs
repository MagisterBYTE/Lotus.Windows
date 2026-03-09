using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Lotus.Core;
using Lotus.Core.Inspector;
using Lotus.Maths;
using Lotus.UnitMeasurement;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Селектор шаблона данных для моделей свойств.
    /// </summary>
    public class PropertyModelDataSelector : DataTemplateSelector
    {
        #region Fields
        /// <summary>
        /// Шаблон для представления логического значения.
        /// </summary>
        public DataTemplate Boolean { get; set; }

        /// <summary>
        /// Шаблон для представления числовых значений.
        /// </summary>
        public DataTemplate Numeric { get; set; }

        /// <summary>
        /// Шаблон для представления значений единиц измерения.
        /// </summary>
        public DataTemplate Measurement { get; set; }

        /// <summary>
        /// Шаблон для представления перечисления.
        /// </summary>
        public DataTemplate Enum { get; set; }

        /// <summary>
        /// Шаблон для представления строкового значения.
        /// </summary>
        public DataTemplate String { get; set; }

        /// <summary>
        /// Шаблон для представления значения даты.
        /// </summary>
        public DataTemplate DateTime { get; set; }

        /// <summary>
        /// Шаблон для представления значения двухмерного вектора.
        /// </summary>
        public DataTemplate Vector2D { get; set; }

        /// <summary>
        /// Шаблон для представления недопустимого типа.
        /// </summary>
        public DataTemplate Invalid { get; set; }
        #endregion

        #region Main methods
        /// <summary>
        /// Выбор шаблона привязки данных.
        /// </summary>
        /// <param name="item">Объект.</param>
        /// <param name="container">Контейнер.</param>
        /// <returns>Нужный шаблон.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var template = Invalid;
            var model = item as PropertyModelBase;
            if (model is not null)
            {
                switch (model.PropertyType)
                {
                    case TPropertyType.Boolean:
                        template = Boolean;
                        break;
                    case TPropertyType.Numeric:
                        template = Numeric;
                        break;
                    case TPropertyType.Measurement:
                        template = Measurement;
                        break;
                    case TPropertyType.Enum:
                        template = Enum;
                        break;
                    case TPropertyType.String:
                        template = String;
                        break;
                    case TPropertyType.DateTime:
                        template = DateTime;
                        break;
                    case TPropertyType.Vector2D:
                        template = Vector2D;
                        break;
                    case TPropertyType.Object:
                        {
                            var modelObject = model as PropertyModelObject;

                            var dataTemplate = new DataTemplate
                            {
                                DataType = typeof(PropertyModelObject)
                            };

                            var element = new FrameworkElementFactory(modelObject!.EditorType);

                            var pdc = System.ComponentModel.TypeDescriptor.GetProperties(modelObject.EditorType);
                            var propertyDescriptionValue = pdc["Value"];

                            var dependencyProperty = DependencyPropertyDescriptor.FromProperty(propertyDescriptionValue);

                            var binding = new Binding("Value")
                            {
                                Mode = BindingMode.TwoWay,
                                Source = modelObject
                            };
                            element.SetBinding(dependencyProperty.DependencyProperty, binding);

                            dataTemplate.VisualTree = element;
                            template = dataTemplate;
                        }
                        break;
                    case TPropertyType.Unknow:
                        template = Invalid;
                        break;
                    default:
                        break;
                }
            }

            return template;
        }
        #endregion
    }

    /// <summary>
    /// Редактор свойств объекта.
    /// </summary>
    public partial class LotusPropertyInspector : UserControl, ILotusPropertyInspector, INotifyPropertyChanged
    {
        #region Static fields
        private static readonly PropertyChangedEventArgs PropertyArgsSelectedObject = new(nameof(SelectedObject));
        private static readonly PropertyChangedEventArgs PropertyArgsTypeName = new(nameof(TypeName));
        private static readonly PropertyChangedEventArgs PropertyArgsObjectName = new(nameof(ObjectName));
        private static readonly PropertyChangedEventArgs PropertyArgsIsGrouping = new(nameof(IsGrouping));
        private static readonly PropertyChangedEventArgs PropertyArgsIsFiltration = new(nameof(IsFiltration));
        private static readonly PropertyChangedEventArgs PropertyArgsFilterString = new(nameof(FilterString));
        private static readonly PropertyGroupDescription PropertyGroupDescriptionGroup = new(nameof(CPropertyDesc.Category));
        #endregion

        #region Fields
        protected internal object _selectedObject;
        protected internal string _typeName;
        protected internal string _objectName;
        protected internal bool _isGrouping;
        protected internal bool _isFiltration;
        protected internal string _filterString;
        protected internal CPropertyDesc[] _propertiesDesc;
        protected internal ListArray<PropertyModelBase> _properties;
        protected internal ListCollectionView _propertiesView;
        #endregion

        #region Properties
        /// <summary>
        /// Выбранный объект.
        /// </summary>
        public object SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    SetInstance();
                    NotifyPropertyChanged(PropertyArgsSelectedObject);
                }
            }
        }

        /// <summary>
        /// Имя типа.
        /// </summary>
        public string TypeName
        {
            get => _typeName;
            set
            {
                if (_typeName != value)
                {
                    _typeName = value;
                    NotifyPropertyChanged(PropertyArgsTypeName);
                }
            }
        }

        /// <summary>
        /// Имя объекта.
        /// </summary>
        public string ObjectName
        {
            get => _objectName;
            set
            {
                if (_objectName != value)
                {
                    _objectName = value;
                    NotifyPropertyChanged(PropertyArgsObjectName);
                }
            }
        }

        /// <summary>
        /// Статус основного группирования.
        /// </summary>
        [Browsable(false)]
        public bool IsGrouping
        {
            get => _isGrouping;
            set
            {
                if (_isGrouping != value)
                {
                    _isGrouping = value;

                    if (_isGrouping)
                        SetGroupings();
                    else
                        UnsetGroupings();

                    NotifyPropertyChanged(PropertyArgsIsGrouping);
                }
            }
        }

        /// <summary>
        /// Статус фильтрации данных.
        /// </summary>
        [Browsable(false)]
        public bool IsFiltration
        {
            get => _isFiltration;
            set
            {
                if (_isFiltration != value)
                {
                    _isFiltration = value;

                    if (_isFiltration)
                        _propertiesView.Filter += OnPropertyViewFilter;
                    else
                        _propertiesView.Filter -= OnPropertyViewFilter;

                    NotifyPropertyChanged(PropertyArgsIsFiltration);
                }
            }
        }

        /// <summary>
        /// Строка для фильтра.
        /// </summary>
        [Browsable(false)]
        public string FilterString
        {
            get => _filterString;
            set
            {
                _filterString = value;
                NotifyPropertyChanged(PropertyArgsFilterString);
                _propertiesView?.Refresh();
            }
        }

        /// <summary>
        /// Список свойств.
        /// </summary>
        public ListArray<PropertyModelBase> Properties => _properties;

        /// <summary>
        /// Список свойств для отображения.
        /// </summary>
        public ListCollectionView PropertiesView => _propertiesView;
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusPropertyInspector()
        {
            InitializeComponent();
            _properties = [];
            DataContext = this;
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Установка нового объекта для отображения свойств.
        /// </summary>
        private void SetInstance()
        {
            if (_selectedObject is not null)
            {
                _properties.Clear();

                if (_selectedObject is ILotusSupportViewInspector supportInspector)
                {
                    TypeName = supportInspector.InspectorTypeName;
                    ObjectName = supportInspector.InspectorObjectName;
                }

                if (_selectedObject is ILotusSupportEditInspector supportInspectorEx)
                {
                    _propertiesDesc = supportInspectorEx.GetPropertiesDesc();

                    for (var i = 0; i < _propertiesDesc.Length; i++)
                    {
                        if (_propertiesDesc[i].PropertyOrder == -1)
                        {
                            _propertiesDesc[i].PropertyOrder = i;
                        }
                    }
                }

                AddModelProperties();
                UpdateCategoryOrders();
                _properties.SortAscending();

                for (var i = 0; i < _properties.Count; i++)
                {
                    _properties[i].Instance = _selectedObject;
                }

                _propertiesView = new ListCollectionView(_properties);
                _propertiesView.Filter += OnPropertyViewFilter;
                dataProperties.ItemsSource = _propertiesView;

                if (toogleButtonGroup is not null && toogleButtonGroup.IsChecked.GetValueOrDefault())
                {
                    SetGroupings();
                }

                CheckIsValueFromList();
            }
            else
            {
                _properties.Clear();
                dataProperties.ItemsSource = null;
                TypeName = string.Empty;
                ObjectName = string.Empty;
                textDescription.Text = string.Empty;
            }
        }

        /// <summary>
        /// Добавление модели свойств.
        /// </summary>
        protected void AddModelProperties()
        {
            var props = _selectedObject.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(x => x.MetadataToken)
                .ToArray();

            for (var i = 0; i < props.Length; i++)
            {
                var propertyInfo = props[i];
                var type = propertyInfo.PropertyType;

                var browsableAttribute = propertyInfo.GetAttribute<BrowsableAttribute>();
                if (browsableAttribute is not null && browsableAttribute.Browsable == false)
                    continue;

                var propertyDesc = GetPropertyDesc(propertyInfo);

                if (propertyDesc is not null && propertyDesc.Any(item => item.IsHideInspector))
                    continue;

                if (type.Name == nameof(Boolean))
                {
                    _properties.Add(new PropertyModel<bool>(propertyInfo, propertyDesc!, TPropertyType.Boolean));
                    continue;
                }

                if (type.IsEnum)
                {
                    _properties.Add(new PropertyModelEnum(propertyInfo, propertyDesc!));
                    continue;
                }

                if (type.IsNumericType())
                {
                    AddModelPropertyNumeric(propertyInfo, propertyDesc!);
                    continue;
                }

                if (type.Name == nameof(TMeasurementValue))
                {
                    _properties.Add(new PropertyModelMeasurementValue(propertyInfo, propertyDesc!));
                    continue;
                }

                if (type.Name == nameof(DateTime))
                {
                    _properties.Add(new PropertyModel<DateTime>(propertyInfo, propertyDesc!, TPropertyType.DateTime));
                    continue;
                }

                if (type.Name == nameof(String))
                {
                    _properties.Add(new PropertyModel<string>(propertyInfo, propertyDesc!, TPropertyType.String));
                    continue;
                }

                if (type.Name == nameof(Point) ||
                    type.Name == nameof(Vector) ||
                    type.Name == nameof(Vector2Df) ||
                    type.Name == nameof(Vector2D))
                {
                    _properties.Add(new PropertyModelVector2D(propertyInfo, propertyDesc!));
                    continue;
                }

                if (propertyInfo.HasAttribute<LotusInspectorTypeEditor>())
                {
                    var attr = propertyInfo.GetAttribute<LotusInspectorTypeEditor>();
                    _properties.Add(new PropertyModelObject(propertyInfo, propertyDesc!, attr!.EditorType));
                    continue;
                }
            }
        }

        /// <summary>
        /// Добавление модели свойства для числовых типов.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства.</param>
        /// <param name="propertyDesc">Список описания свойства.</param>
        protected void AddModelPropertyNumeric(PropertyInfo propertyInfo, List<CPropertyDesc> propertyDesc)
        {
            switch (Type.GetTypeCode(propertyInfo.PropertyType))
            {
                case TypeCode.Char:
                    _properties.Add(new PropertyModelRange<char>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.SByte:
                    _properties.Add(new PropertyModelRange<sbyte>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.Byte:
                    _properties.Add(new PropertyModelRange<byte>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.Int16:
                    _properties.Add(new PropertyModelRange<short>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.UInt16:
                    _properties.Add(new PropertyModelRange<ushort>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.Int32:
                    _properties.Add(new PropertyModelRange<int>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.UInt32:
                    _properties.Add(new PropertyModelRange<uint>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.Int64:
                    _properties.Add(new PropertyModelRange<long>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.UInt64:
                    _properties.Add(new PropertyModelRange<ulong>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.Single:
                    _properties.Add(new PropertyModelRange<float>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.Double:
                    _properties.Add(new PropertyModelRange<double>(propertyInfo, propertyDesc));
                    break;
                case TypeCode.Decimal:
                    _properties.Add(new PropertyModelRange<decimal>(propertyInfo, propertyDesc));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Получение списка описания свойства по указанным метаданным свойства.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства.</param>
        /// <returns>Список описания свойств.</returns>
        protected List<CPropertyDesc>? GetPropertyDesc(PropertyInfo propertyInfo)
        {
            if (_propertiesDesc is null) return null;

            var result = new List<CPropertyDesc>();
            for (var i = 0; i < _propertiesDesc.Length; i++)
            {
                if (_propertiesDesc[i].PropertyName == propertyInfo.Name)
                {
                    result.Add(_propertiesDesc[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Обновление порядка отображения групп.
        /// </summary>
        protected void UpdateCategoryOrders()
        {
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
                    if (_properties[j].Category == group && _properties[j].CategoryOrder != -1)
                    {
                        order = _properties[j].CategoryOrder;
                        break;
                    }
                }

                if (order != -1)
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

        #region Filter methods
        /// <summary>
        /// Проверка на соответствие фильтру.
        /// </summary>
        /// <param name="item">Объект.</param>
        /// <returns>Статус проверки.</returns>
        protected virtual bool OnPropertyViewFilter(object item)
        {
            if (string.IsNullOrEmpty(FilterString)) return true;

            var propertyModel = item as PropertyModelBase;
            return propertyModel!.DisplayName.Contains(FilterString, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Изменение строки фильтра.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnTextFilterProperty_TextChanged(object sender, TextChangedEventArgs args)
        {
            FilterString = textFilterProperty.Text;
        }
        #endregion

        #region Grouping methods
        /// <summary>
        /// Установка группирования.
        /// </summary>
        protected virtual void SetGroupings()
        {
            if (_propertiesView is not null)
            {
                _propertiesView.GroupDescriptions.Clear();
                _propertiesView.GroupDescriptions.Add(PropertyGroupDescriptionGroup);
            }
        }

        /// <summary>
        /// Удаления группирования.
        /// </summary>
        protected virtual void UnsetGroupings()
        {
            _propertiesView?.GroupDescriptions.Remove(PropertyGroupDescriptionGroup);
        }

        /// <summary>
        /// Группирование свойств по категории.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioButtonGroup_Checked(object sender, RoutedEventArgs args)
        {
            toogleButtonAlphabetically.IsChecked = false;
            SetGroupings();
        }

        /// <summary>
        /// Группирование свойств по алфавиту.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioButtonAlphabetically_Checked(object sender, RoutedEventArgs args)
        {
            toogleButtonGroup.IsChecked = false;
            UnsetGroupings();
        }
        #endregion

        #region Work methods
        /// <summary>
        /// Проверка на значение из списка.
        /// </summary>
        protected void CheckIsValueFromList()
        {
            for (var i = 0; i < _properties.Count; i++)
            {
                _properties[i]?.CheckIsValueFromList();
            }
        }

        /// <summary>
        /// Открытие контекстного меню для списка значений строкового типа.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonStringContextMenu_Click(object sender, RoutedEventArgs args)
        {
            var element = (args.Source as FrameworkElement)!;
            var propertyModel = element.DataContext as PropertyModelBase;
            var contextMenu = element.ContextMenu;
            if (contextMenu is not null)
            {
                propertyModel?.AssingContenxMenuListValues(contextMenu);
                contextMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// Установка значения из списка для свойства строкового типа.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemSetValueFromListForString_Click(object sender, RoutedEventArgs args)
        {
            var menuItem = (args.OriginalSource as MenuItem)!;
            var propertyModel = menuItem.Tag as PropertyModelBase;
            if (propertyModel is not null)
            {
                propertyModel.SetValue(menuItem.Header.ToString()!);
                propertyModel.IsValueFromList = true;
            }
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Очистка фильтра.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonClearFilterProperty_Click(object sender, RoutedEventArgs args)
        {
            textFilterProperty.Text = string.Empty;
        }

        /// <summary>
        /// Изменение выбора свойства.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnDataProperties_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var propertyModel = _propertiesView?.CurrentItem as PropertyModelBase;
            if (propertyModel is not null)
            {
                textDescription.Text = propertyModel.Description;
            }
        }

        /// <summary>
        /// Потеря фокуса текстового поля значения строкового типа.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnTextBoxString_LostFocus(object sender, RoutedEventArgs args)
        {
            var propertyModel = _propertiesView?.CurrentItem as PropertyModelBase;
            propertyModel?.CheckIsValueFromList();
        }

        /// <summary>
        /// Вызов метода по атрибуту кнопки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonAttribute_Click(object sender, RoutedEventArgs args)
        {
            if (sender is Button button)
            {
                var methodName = button.Tag.ToString()!;
                XReflection.InvokeMethod(_selectedObject, methodName);
            }
        }
        #endregion

        #region Interface INotifyPropertyChanged
        /// <summary>
        /// Событие срабатывает ПОСЛЕ изменения свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вспомогательный метод для нотификации изменений свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        public void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Вспомогательный метод для нотификации изменений свойства.
        /// </summary>
        /// <param name="args">Аргументы события.</param>
        public void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
        #endregion
    }
    /**@}*/
}
