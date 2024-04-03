using System;
using System.Collections.Generic;
using System.Reflection;

using Lotus.Core;
using Lotus.Core.Inspector;
using Lotus.UnitMeasurement;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Модель отображения свойства объекта типа <see cref="TMeasurementValue"/>.
    /// </summary>
    public class PropertyModelMeasurementValue : PropertyModel<TMeasurementValue>
    {
        #region Fields
        protected internal double _minValue;
        protected internal double _maxValue;
        #endregion

        #region Properties
        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public double MinValue
        {
            get { return _minValue; }
        }

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        public double MaxValue
        {
            get { return _maxValue; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public PropertyModelMeasurementValue()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        public PropertyModelMeasurementValue(PropertyInfo property_info)
            : base(property_info, TPropertyType.Measurement)
        {
            GetInfoFromAttributesRange();
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        /// <param name="property_desc">Список описания свойства.</param>
        public PropertyModelMeasurementValue(PropertyInfo property_info, List<CPropertyDesc> property_desc)
            : base(property_info, property_desc, TPropertyType.Measurement)
        {
            GetInfoFromAttributesRange();
        }

        #endregion

        #region Main methods
        /// <summary>
        /// Получение данных описание свойства с его атрибутов.
        /// </summary>
        protected void GetInfoFromAttributesRange()
        {
            if (_info != null)
            {
                var min_value = _info.GetAttribute<LotusMinValueAttribute>();
                if (min_value != null)
                {
                    _minValue = Convert.ToDouble(min_value.MinValue);
                }
                else
                {
                    _minValue = double.MinValue;
                }

                var max_value = _info.GetAttribute<LotusMaxValueAttribute>();
                if (max_value != null)
                {
                    _maxValue = Convert.ToDouble(max_value.MaxValue);
                }
                else
                {
                    _maxValue = double.MaxValue;
                }
            }
        }
        #endregion
    }
    /**@}*/
}