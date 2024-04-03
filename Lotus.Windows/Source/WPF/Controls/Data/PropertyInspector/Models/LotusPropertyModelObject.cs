using System;
using System.Collections.Generic;
using System.Reflection;

using Lotus.Core.Inspector;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Модель отображения свойства объекта для универсального типа.
    /// </summary>
    public class CPropertyModelObject : PropertyModel<object>
    {
        #region Fields
        protected internal Type _editorType;
        #endregion

        #region Properties
        /// <summary>
        /// Тип редактора для свойства.
        /// </summary>
        public Type EditorType
        {
            get { return _editorType; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public CPropertyModelObject()
        {
            _propertyType = TPropertyType.Object;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        public CPropertyModelObject(PropertyInfo property_info)
            : base(property_info, TPropertyType.Object)
        {

        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="property_info">Метаданные свойства.</param>
        /// <param name="property_desc">Список описания свойства.</param>
        /// <param name="editor_type">Тип редактора для свойства.</param>
        public CPropertyModelObject(PropertyInfo property_info, List<CPropertyDesc> property_desc, Type editor_type)
            : base(property_info, property_desc, TPropertyType.Object)
        {
            _editorType = editor_type;
        }
        #endregion
    }
    /**@}*/
}