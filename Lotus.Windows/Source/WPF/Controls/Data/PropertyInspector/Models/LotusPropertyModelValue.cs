using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Lotus.Core;
using Lotus.Core.Inspector;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Модель отображения свойства объекта к конкретными типом значения свойства.
    /// </summary>
    /// <typeparam name="TValue">Тип значения свойства.</typeparam>
    public class PropertyModel<TValue> : PropertyModelBase, IComparable<PropertyModel<TValue>>
    {
        #region Static fields
        protected static readonly PropertyChangedEventArgs PropertyArgsValue = new(nameof(Value));
        #endregion

        #region Fields
        protected internal TValue _value;
        #endregion

        #region Properties
        /// <summary>
        /// Значение свойства.
        /// </summary>
        public virtual TValue Value
        {
            get { return _value; }
            set
            {
                // Произошло изменение свойства со стороны инспектора свойств
                _value = value;
                if (_info is not null && _info.CanWrite)
                {
                    // Обновляем значение свойства у объекта
                    _info.SetValue(_instance, _value, null);
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public PropertyModel()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства.</param>
        public PropertyModel(PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства.</param>
        /// <param name="propertyType">Допустимый тип свойства.</param>
        public PropertyModel(PropertyInfo propertyInfo, TPropertyType propertyType)
            : base(propertyInfo, propertyType)
        {

        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства.</param>
        /// <param name="propertyDesc">Список описания свойства.</param>
        /// <param name="propertyType">Допустимый тип свойства.</param>
        public PropertyModel(PropertyInfo propertyInfo, List<CPropertyDesc> propertyDesc, TPropertyType propertyType)
            : base(propertyInfo, propertyDesc, propertyType)
        {

        }
        #endregion

        #region System methods
        /// <summary>
        /// Сравнение объектов для упорядочивания.
        /// </summary>
        /// <param name="other">Сравниваемый объект.</param>
        /// <returns>Статус сравнения объектов.</returns>
        public int CompareTo(PropertyModel<TValue>? other)
        {
            return base.CompareTo(other);
        }

        /// <summary>
        /// Преобразование к текстовому представлению.
        /// </summary>
        /// <returns>Краткое наименование финансового инструмента.</returns>
        public override string ToString()
        {
            return DisplayName;
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Установка значения напрямую.
        /// </summary>
        /// <remarks>
        /// В данном случае мы должны уведомить как инспектор свойств и сам объект.
        /// </remarks>
        /// <param name="value">Значение свойства.</param>
        public override void SetValue(object value)
        {
            // Устанавливаем значение свойства объекта
            _info?.SetValue(_instance, value, null);

            // Уведомляем инспектор свойств
            _value = (TValue)value;
            OnPropertyChanged(PropertyArgsValue);
        }

        /// <summary>
        /// Установка нового объекта.
        /// </summary>
        /// <remarks>
        /// В данном случае мы должны уведомить инспектор свойств.
        /// </remarks>
        protected override void SetInstance()
        {
            if (_info is not null)
            {
                try
                {
                    // Получаем актуальное значение с объекта
                    _value = (TValue)_info.GetValue(_instance)!;
                }
                catch (InvalidCastException invalid_cast)
                {
                    XLogger.LogException(invalid_cast);
                }

                // Информируем
                OnPropertyChanged(PropertyArgsValue);
            }
        }

        /// <summary>
        /// Проверка на значение что оно из списка значений.
        /// </summary>
        public override void CheckIsValueFromList()
        {
            _isValueFromList = false;

            if (IsListValues)
            {
                var enumerable = CPropertyDesc.GetValue(_listValues, _listValuesMemberName,
                    _listValuesMemberType, _instance) as IEnumerable;
                if (enumerable is not null)
                {
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
                    foreach (var item in enumerable)
                    {
                        if (item.Equals(Value))
                        {
                            _isValueFromList = true;
                            break;
                        }
                    }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
                }
            }

            OnPropertyChanged(PropertyArgsIsValueFromList);
        }

        /// <summary>
        /// Обработчик события изменения свойства со стороны объекта.
        /// </summary>
        /// <remarks>
        /// В данном случае мы должны уведомить инспектор свойств.
        /// </remarks>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        protected override void OnPropertyChangedFromInstance(object? sender, PropertyChangedEventArgs args)
        {
            if (_info is not null && _info.Name == args.PropertyName)
            {
                // Получаем актуальное значение с объекта
                try
                {
                    _value = (TValue)_info.GetValue(_instance)!;
                }
                catch (InvalidCastException invalid_cast)
                {
                    XLogger.LogException(invalid_cast);
                }

                // Информируем
                OnPropertyChanged(PropertyArgsValue);
            }
        }
        #endregion
    }
    /**@}*/
}