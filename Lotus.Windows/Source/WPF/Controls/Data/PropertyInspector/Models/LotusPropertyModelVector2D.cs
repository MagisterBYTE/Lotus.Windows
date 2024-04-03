using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

using Lotus.Core.Inspector;
using Lotus.Maths;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Модель отображения свойства объекта c типом Vector2D.
    /// </summary>
    public class CPropertyModelVector2D : PropertyModel<Vector2D>
    {
        #region Properties
        /// <summary>
        /// Значение свойства.
        /// </summary>
        public override Vector2D Value
        {
            get { return _value; }
            set
            {
                // Произошло изменение свойства со стороны инспектора свойств
                _value = value;
                if (_info != null && _info.CanWrite)
                {


                    // Обновляем значение свойства у объекта
                    _info.SetValue(_instance, ConvertToRealType(), null);
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public CPropertyModelVector2D()
        {
            _propertyType = TPropertyType.Vector2D;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        public CPropertyModelVector2D(PropertyInfo property_info)
            : base(property_info, TPropertyType.Vector2D)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        /// <param name="property_desc">Список описания свойства.</param>
        public CPropertyModelVector2D(PropertyInfo property_info, List<CPropertyDesc> property_desc)
            : base(property_info, property_desc, TPropertyType.Vector2D)
        {
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Конвертирование значение Vector2D в реальный тип свойства.
        /// </summary>
        /// <returns></returns>
        public object ConvertToRealType()
        {
            if (_info.PropertyType == typeof(Vector2Df))
            {
                return new Vector2Df((float)_value.X, (float)_value.Y);
            }
            if (_info.PropertyType == typeof(Vector2Di))
            {
                return new Vector2Di((int)_value.X, (int)_value.Y);
            }
            if (_info.PropertyType == typeof(Point))
            {
                return new Point(_value.X, _value.Y);
            }
            if (_info.PropertyType == typeof(Vector))
            {
                return new Vector(_value.X, _value.Y);
            }

            return _value;
        }

        /// <summary>
        /// Конвертирование в значение Vector2D из реальный тип свойства.
        /// </summary>
        /// <returns></returns>
        public Vector2D ConvertFromRealType()
        {
            if (_info.PropertyType == typeof(Vector2Df))
            {
                var vector = (Vector2Df)_info.GetValue(_instance)!;
                return new Vector2D(vector.X, vector.Y);
            }
            if (_info.PropertyType == typeof(Vector2Di))
            {
                var vector = (Vector2Di)_info.GetValue(_instance)!;
                return new Vector2D(vector.X, vector.Y);
            }
            if (_info.PropertyType == typeof(Point))
            {
                var vector = (Point)_info.GetValue(_instance)!;
                return new Vector2D(vector.X, vector.Y);
            }
            if (_info.PropertyType == typeof(Vector))
            {
                var vector = (Vector)_info.GetValue(_instance)!;
                return new Vector2D(vector.X, vector.Y);
            }

            return (Vector2D)_info.GetValue(_instance)!;
        }

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
            if (_info != null)
            {
                _info.SetValue(_instance, value, null);
            }

            // Уведомляем инспектор свойств
            _value = (Vector2D)value;
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
            if (_info != null)
            {
                // Получаем актуальное значение с объекта
                _value = ConvertFromRealType();

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
                if (enumerable != null)
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
            if (_info != null && _info.Name == args.PropertyName)
            {
                // Получаем актуальное значение с объекта
                _value = ConvertFromRealType();

                // Информируем
                OnPropertyChanged(PropertyArgsValue);
            }
        }
        #endregion
    }
    /**@}*/
}