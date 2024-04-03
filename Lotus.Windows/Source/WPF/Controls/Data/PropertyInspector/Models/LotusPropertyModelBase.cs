using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;

using Lotus.Core;
using Lotus.Core.Inspector;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Допустимый тип свойства.
    /// </summary>
    public enum TPropertyType
    {
        /// <summary>
        /// Логический тип.
        /// </summary>
        Boolean,

        /// <summary>
        /// Числовой тип.
        /// </summary>
        Numeric,

        /// <summary>
        /// Тип единицы измерения.
        /// </summary>
        Measurement,

        /// <summary>
        /// Перечисление.
        /// </summary>
        Enum,

        /// <summary>
        /// Строковый тип.
        /// </summary>
        String,

        /// <summary>
        /// Тип даты-времени.
        /// </summary>
        DateTime,

        /// <summary>
        /// Двухмерный вектор.
        /// </summary>
        Vector2D,

        /// <summary>
        /// Базовый тип.
        /// </summary>
        Object,

        /// <summary>
        /// Неизвестный тип.
        /// </summary>
        Unknow
    }

    /// <summary>
    /// Базовая модель отображения свойства объекта.
    /// </summary>
    public class CPropertyModelBase : PropertyChangedBase, IComparable<CPropertyModelBase>, IDisposable
    {
        #region Static fields
        protected static readonly PropertyChangedEventArgs PropertyArgsIsValueFromList = new PropertyChangedEventArgs(nameof(IsValueFromList));
        #endregion

        #region Fields
        // Основные параметры
        protected internal PropertyInfo _info;
        protected internal TPropertyType _propertyType;
        protected internal object _instance;

        // Параметры описания
        protected internal string _displayName;
        protected internal string _description;
        protected internal int _propertyOrder = -1;
        protected internal string _category;
        protected internal int _categoryOrder = -1;

        // Параметры управления
        protected internal bool _isReadOnly;
        protected internal object _defaultValue;
        protected internal string _formatValue;

        // Список значений величины
        protected internal object _listValues;
        protected internal string _listValuesMemberName;
        protected internal TInspectorMemberType _listValuesMemberType;
        protected internal bool _isValueFromList;

        // Управление кнопкой
        protected internal string _buttonCaption;
        protected internal string _buttonMethodName;
        #endregion

        #region Properties
        //
        // ОСНОВНЫЕ ПАРАМЕТРЫ
        //
        /// <summary>
        /// Метаданные свойства объекта.
        /// </summary>
        public PropertyInfo Info
        {
            get { return _info; }
            set
            {
                _info = value;
            }
        }

        /// <summary>
        /// Допустимый тип свойства.
        /// </summary>
        public TPropertyType PropertyType
        {
            get { return _propertyType; }
            set
            {
                _propertyType = value;
            }
        }

        /// <summary>
        /// Экземпляр объекта.
        /// </summary>
        public object Instance
        {
            get { return _instance; }
            set
            {
                if (_instance != null)
                {
                    // Если объект поддерживает стандартную нотификацию
                    var property_changed_prev = _instance as INotifyPropertyChanged;
                    if (property_changed_prev != null)
                    {
                        property_changed_prev.PropertyChanged -= OnPropertyChangedFromInstance;
                    }
                }

                _instance = value;
                if (_instance != null)
                {
                    // Если объект поддерживает стандартную нотификацию
                    var property_changed = _instance as INotifyPropertyChanged;
                    if (property_changed != null)
                    {
                        property_changed.PropertyChanged += OnPropertyChangedFromInstance;
                    }

                    SetInstance();
                }
            }
        }

        //
        // ПАРАМЕТРЫ ОПИСАНИЯ
        //
        /// <summary>
        /// Отображаемое имя свойства.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    if (_info != null)
                    {
                        return _info.Name;
                    }
                    else
                    {
                        return "Noname";
                    }
                }
                else
                {
                    return _displayName;
                }
            }
            set
            {
                _displayName = value;
            }
        }

        /// <summary>
        /// Описание свойства.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Порядковый номер отображения свойства внутри категории.
        /// </summary>
        public int PropertyOrder
        {
            get { return _propertyOrder; }
            set
            {
                _propertyOrder = value;
            }
        }

        /// <summary>
        /// Категория свойства.
        /// </summary>
        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
            }
        }

        /// <summary>
        /// Порядковый номер отображения категории.
        /// </summary>
        public int CategoryOrder
        {
            get { return _categoryOrder; }
            set
            {
                _categoryOrder = value;
            }
        }

        //
        // ПАРАМЕТРЫ УПРАВЛЕНИЯ
        //
        /// <summary>
        /// Свойство только для чтения.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
            }
        }

        /// <summary>
        /// Значение свойства по умолчанию.
        /// </summary>
        public object DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
            }
        }

        /// <summary>
        /// Статус наличия значения по умолчанию.
        /// </summary>
        public bool IsDefaultValue
        {
            get { return _defaultValue != null; }
        }

        /// <summary>
        /// Формат отображения значения свойства.
        /// </summary>
        public string FormatValue
        {
            get { return _formatValue; }
            set
            {
                _formatValue = value;
            }
        }

        /// <summary>
        /// Статус наличия формата отображения значения свойства.
        /// </summary>
        public bool IsFormatValue
        {
            get { return string.IsNullOrEmpty(FormatValue) == false; }
        }

        //
        // СПИСОК ЗНАЧЕНИЙ ВЕЛИЧИНЫ
        //
        /// <summary>
        /// Список допустимых значений свойств.
        /// </summary>
        public object ListValues
        {
            get { return _listValues; }
            set
            {
                _listValues = value;
            }
        }

        /// <summary>
        /// Имя члена данных списка допустимых значений свойств.
        /// </summary>
        public string ListValuesMemberName
        {
            get { return _listValuesMemberName; }
            set
            {
                _listValuesMemberName = value;
            }
        }

        /// <summary>
        /// Тип члена данных списка допустимых значений свойств.
        /// </summary>
        public TInspectorMemberType ListValuesMemberType
        {
            get { return _listValuesMemberType; }
            set
            {
                _listValuesMemberType = value;
            }
        }

        /// <summary>
        /// Статус наличия списка значений.
        /// </summary>
        public bool IsListValues
        {
            get { return _listValues != null || _listValuesMemberName.IsExists(); }
        }

        /// <summary>
        /// Статус значения свойства из списка значений.
        /// </summary>
        public bool IsValueFromList
        {
            get { return _isValueFromList; }
            set
            {
                _isValueFromList = value;
                OnPropertyChanged(PropertyArgsIsValueFromList);
            }
        }

        //
        // УПРАВЛЕНИЕ КНОПКОЙ
        //
        /// <summary>
        /// Надпись над кнопкой.
        /// </summary>
        public string ButtonCaption
        {
            get { return _buttonCaption; }
        }

        /// <summary>
        /// Имя метода который вызывается при нажатии на кнопку.
        /// </summary>
        public string ButtonMethodName
        {
            get { return _buttonMethodName; }
        }

        /// <summary>
        /// Статус наличия атрибута для управления свойством через кнопку.
        /// </summary>
        public bool IsButtonMethod
        {
            get { return _buttonMethodName.IsExists(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public CPropertyModelBase()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        public CPropertyModelBase(PropertyInfo property_info)
            : this(property_info, TPropertyType.Unknow)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        /// <param name="property_type">Допустимый тип свойства.</param>
        public CPropertyModelBase(PropertyInfo property_info, TPropertyType property_type)
        {
            _info = property_info;
            _propertyType = property_type;
            ApplyInfoFromAttributes();
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        /// <param name="property_desc">Список описания свойства.</param>
        /// <param name="property_type">Допустимый тип свойства.</param>
        public CPropertyModelBase(PropertyInfo property_info, List<CPropertyDesc> property_desc,
            TPropertyType property_type)
        {
            _info = property_info;
            _propertyType = property_type;
            ApplyInfoFromDecs(property_desc);   // Имеет преимущество
            ApplyInfoFromAttributes();
        }
        #endregion

        #region System methods
        /// <summary>
        /// Сравнение объектов для упорядочивания.
        /// </summary>
        /// <param name="other">Сравниваемый объект.</param>
        /// <returns>Статус сравнения объектов.</returns>
        public int CompareTo(CPropertyModelBase? other)
        {
            if (other == null) return 0;

            var category_order = _categoryOrder.CompareTo(other.CategoryOrder);
            if (category_order == 0)
            {
                if (_category.IsExists())
                {
                    var category = _category.CompareTo(other.Category);
                    if (category == 0)
                    {
                        return _propertyOrder.CompareTo(other.PropertyOrder);
                    }
                    else
                    {
                        return category;
                    }
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return category_order;
            }
        }

        /// <summary>
        /// Преобразование к текстовому представлению.
        /// </summary>
        /// <returns>Краткое наименование финасового инструмента.</returns>
        public override string ToString()
        {
            return DisplayName;
        }
        #endregion

        #region IDisposable methods
        /// <summary>
        /// Освобождение управляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Освобождение управляемых ресурсов.
        /// </summary>
        /// <param name="disposing">Статус освобождения.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Освобождаем только управляемые ресурсы
            if (disposing)
            {
                Instance = null!;
            }

            // Освобождаем неуправляемые ресурсы
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Получение данных описание свойства с его атрибутов.
        /// </summary>
        protected void ApplyInfoFromAttributes()
        {
            if (_info != null)
            {
                var display_name = _info.GetAttribute<DisplayNameAttribute>();
                if (display_name != null && string.IsNullOrEmpty(_displayName))
                {
                    _displayName = display_name.DisplayName;
                }

                var description = _info.GetAttribute<DescriptionAttribute>();
                if (description != null && string.IsNullOrEmpty(_description))
                {
                    _description = description.Description;
                }

                var property_order = _info.GetAttribute<LotusPropertyOrderAttribute>();
                if (property_order != null)
                {
                    _propertyOrder = property_order.Order;
                }

                var auto_order = _info.GetAttribute<LotusAutoOrderAttribute>();
                if (auto_order != null)
                {
                    _propertyOrder = auto_order.Order;
                }

                var category = _info.GetAttribute<CategoryAttribute>();
                if (category != null && string.IsNullOrEmpty(_category))
                {
                    _category = category.Category;
                }

                var category_order = _info.GetAttribute<LotusCategoryOrderAttribute>();
                if (category_order != null)
                {
                    _categoryOrder = category_order.Order;
                }

                var read_only = _info.GetAttribute<ReadOnlyAttribute>();
                if (read_only != null)
                {
                    _isReadOnly = read_only.IsReadOnly;
                }
                if (_info.CanWrite == false)
                {
                    _isReadOnly = true;
                }

                var default_value = _info.GetAttribute<DefaultValueAttribute>();
                if (default_value != null)
                {
                    _defaultValue = default_value.Value!;
                }

                var list_values = _info.GetAttribute<LotusListValuesAttribute>();
                if (list_values != null)
                {
                    _listValues = list_values.ListValues;
                    _listValuesMemberName = list_values.MemberName;
                    _listValuesMemberType = list_values.MemberType;
                }

                var format_value = _info.GetAttribute<LotusNumberFormatAttribute>();
                if (format_value != null && string.IsNullOrEmpty(_formatValue))
                {
                    _formatValue = format_value.FormatValue;
                }

                var button_method = _info.GetAttribute<LotusButtonAttribute>();
                if (button_method != null && button_method.MethodName.IsExists())
                {
                    _buttonCaption = button_method.Label;
                    _buttonMethodName = button_method.MethodName;
                }
            }
        }

        /// <summary>
        /// Получение данных описание свойства с помощью внешнего описания свойства.
        /// </summary>
        /// <param name="descs">Список описания свойства.</param>
        protected void ApplyInfoFromDecs(List<CPropertyDesc> descs)
        {
            if (descs != null && descs.Count > 0)
            {
                for (var i = 0; i < descs.Count; i++)
                {
                    var desc = descs[i];
                    if (desc != null)
                    {
                        if (string.IsNullOrEmpty(desc.DisplayName) == false)
                        {
                            _displayName = desc.DisplayName;
                        }

                        if (string.IsNullOrEmpty(desc.Description) == false)
                        {
                            _description = desc.Description;
                        }

                        if (desc.PropertyOrder != -1)
                        {
                            _propertyOrder = desc.PropertyOrder;
                        }

                        if (string.IsNullOrEmpty(desc.Category) == false)
                        {
                            _category = desc.Category;
                        }

                        if (desc.CategoryOrder != -1)
                        {
                            _categoryOrder = desc.CategoryOrder;
                        }

                        if (desc.IsReadOnly)
                        {
                            _isReadOnly = true;
                        }

                        if (desc.DefaultValue != null)
                        {
                            _defaultValue = desc.DefaultValue;
                        }

                        if (desc.ListValues != null)
                        {
                            _listValues = desc.ListValues;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Установка значения напрямую.
        /// </summary>
        /// <remarks>
        /// В данном случае мы должны уведомить как инспектор свойств и сам объект.
        /// </remarks>
        /// <param name="value">Значение свойства.</param>
        public virtual void SetValue(object value)
        {
            // Устанавливаем значение свойства объекта
            if (_info != null)
            {
                _info.SetValue(_instance, value, null);
            }
        }

        /// <summary>
        /// Установка нового объекта.
        /// </summary>
        protected virtual void SetInstance()
        {

        }

        /// <summary>
        /// Заполнить контекстное меню списком допустимых значений.
        /// </summary>
        /// <param name="context_menu">Контекстное меню.</param>
        public virtual void AssingContenxMenuListValues(ContextMenu context_menu)
        {
            if (IsListValues)
            {
                var enumerable = CPropertyDesc.GetValue(_listValues, _listValuesMemberName,
                    _listValuesMemberType, _instance) as IEnumerable;
                if (context_menu != null && enumerable != null)
                {
                    context_menu.Items.Clear();
                    foreach (var item in enumerable)
                    {
                        context_menu.Items.Add(new MenuItem() { Header = item, Tag = this });
                    }
                }
            }
        }

        /// <summary>
        /// Проверка на значение что оно из списка значений.
        /// </summary>
        public virtual void CheckIsValueFromList()
        {
        }

        /// <summary>
        /// Обработчик события изменения свойства со стороны объекта.
        /// </summary>
        /// <remarks>
        /// В данном случае мы должны уведомить инспектор свойств.
        /// </remarks>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        protected virtual void OnPropertyChangedFromInstance(object? sender, PropertyChangedEventArgs args)
        {
        }
        #endregion
    }
    /**@}*/
}