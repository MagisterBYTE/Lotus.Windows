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
        /// <param name="propertyInfo">Метаданные свойства.</param>
        public CPropertyModelObject(PropertyInfo propertyInfo)
            : base(propertyInfo, TPropertyType.Object)
        {

        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="propertyInfo">Метаданные свойства.</param>
        /// <param name="propertyDesc">Список описания свойства.</param>
        /// <param name="editorType">Тип редактора для свойства.</param>
        public CPropertyModelObject(PropertyInfo propertyInfo, List<CPropertyDesc> propertyDesc, Type editorType)
            : base(propertyInfo, propertyDesc, TPropertyType.Object)
        {
            _editorType = editorType;
        }
        #endregion
    }
    /**@}*/
}