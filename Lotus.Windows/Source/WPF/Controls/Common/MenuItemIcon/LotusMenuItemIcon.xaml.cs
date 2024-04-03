using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsCommon
	*@{*/
    /// <summary>
    /// Элемент меню с поддержкой иконки из связанной команды.
    /// </summary>
    public partial class LotusMenuItemIcon : MenuItem
    {
        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusMenuItemIcon()
        {
            InitializeComponent();
            SetResourceReference(StyleProperty, typeof(MenuItem));
        }
        #endregion
    }
    /**@}*/
}