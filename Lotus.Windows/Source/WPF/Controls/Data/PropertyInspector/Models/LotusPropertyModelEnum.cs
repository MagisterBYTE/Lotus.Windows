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
    public class PropertyModelEnum : PropertyModel<Enum>
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
        public PropertyModelEnum()
        {
            _propertyType = TPropertyType.Enum;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства.</param>
        public PropertyModelEnum(PropertyInfo propertyInfo)
            : base(propertyInfo, TPropertyType.Enum)
        {
            _enumValues = new ArrayList(Enum.GetValues(propertyInfo.PropertyType));
            _enumNames = new List<string>(Enum.GetNames(propertyInfo.PropertyType));
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства.</param>
        /// <param name="propertyDesc">Список описания свойства.</param>
        public PropertyModelEnum(PropertyInfo propertyInfo, List<CPropertyDesc> propertyDesc)
            : base(propertyInfo, propertyDesc, TPropertyType.Enum)
        {
            _enumValues = new ArrayList(Enum.GetValues(propertyInfo.PropertyType));
            _enumNames = new List<string>(Enum.GetNames(propertyInfo.PropertyType));
        }
        #endregion
    }
    /**@}*/
}