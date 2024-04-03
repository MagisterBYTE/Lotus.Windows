using System.Collections.Generic;
using System.Reflection;

using Lotus.Core;
using Lotus.Core.Inspector;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Модель отображения свойства объекта который имеет диапазон изменений.
    /// </summary>
    /// <typeparam name="TNumeric">Тип значения свойства.</typeparam>
    public class PropertyModelRange<TNumeric> : PropertyModel<TNumeric>
    {
        #region Fields
        protected internal TNumeric _minValue;
        protected internal TNumeric _maxValue;
        #endregion

        #region Properties
        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public TNumeric MinValue
        {
            get { return _minValue; }
        }

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        public TNumeric MaxValue
        {
            get { return _maxValue; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public PropertyModelRange()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        public PropertyModelRange(PropertyInfo property_info)
            : base(property_info, TPropertyType.Numeric)
        {
            GetInfoFromAttributesRange();
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        /// <param name="property_desc">Список описания свойства.</param>
        public PropertyModelRange(PropertyInfo property_info, List<CPropertyDesc> property_desc)
            : base(property_info, property_desc, TPropertyType.Numeric)
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
                    _minValue = (TNumeric)min_value.MinValue;
                }
                else
                {
                    var field_info = typeof(TNumeric).GetField(nameof(MinValue), BindingFlags.Static | BindingFlags.Public);
                    if (field_info != null)
                    {
                        _minValue = (TNumeric)field_info.GetValue(null)!;
                    }
                }

                var max_value = _info.GetAttribute<LotusMaxValueAttribute>();
                if (max_value != null)
                {
                    _maxValue = (TNumeric)max_value.MaxValue;
                }
                else
                {
                    var field_info = typeof(TNumeric).GetField(nameof(MaxValue), BindingFlags.Static | BindingFlags.Public);
                    if (field_info != null)
                    {
                        _maxValue = (TNumeric)field_info.GetValue(null)!;
                    }
                }
            }
        }
        #endregion
    }
    /**@}*/
}