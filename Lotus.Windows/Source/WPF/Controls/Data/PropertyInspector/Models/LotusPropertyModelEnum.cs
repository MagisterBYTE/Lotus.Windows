using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Lotus.Core.Inspector;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Модель отображения свойства объекта типа перечисления.
    /// </summary>
    public class CPropertyModelEnum : PropertyModel<Enum>
    {
        #region Fields
        protected internal ArrayList _enumValues;
        protected internal List<string> _enumNames;
        #endregion

        #region Properties
        /// <summary>
        /// Значения перечисления.
        /// </summary>
        public ArrayList EnumValues
        {
            get { return _enumValues; }
        }

        /// <summary>
        /// Имена перечисления.
        /// </summary>
        public List<string> EnumNames
        {
            get { return _enumNames; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public CPropertyModelEnum()
        {
            _propertyType = TPropertyType.Enum;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        public CPropertyModelEnum(PropertyInfo property_info)
            : base(property_info, TPropertyType.Enum)
        {
            _enumValues = new ArrayList(Enum.GetValues(property_info.PropertyType));
            _enumNames = new List<string>(Enum.GetNames(property_info.PropertyType));
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        /// <param name="property_desc">Список описания свойства.</param>
        public CPropertyModelEnum(PropertyInfo property_info, List<CPropertyDesc> property_desc)
            : base(property_info, property_desc, TPropertyType.Enum)
        {
            _enumValues = new ArrayList(Enum.GetValues(property_info.PropertyType));
            _enumNames = new List<string>(Enum.GetNames(property_info.PropertyType));
        }
        #endregion
    }
    /**@}*/
}