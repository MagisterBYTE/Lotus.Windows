using System.Windows.Controls;

namespace Lotus.Windows
{
    /**
     * \defgroup WindowsWPFControlsCommon Общие элементы управления
     * \ingroup WindowsWPFControls
     * \brief Общие элементы управления.
     * @{
     */
    /// <summary>
    /// Стандартная кнопка с поддержкой иконки из связанной команды.
    /// </summary>
    public partial class LotusButtonCommandIcon : Button
    {
        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusButtonCommandIcon()
        {
            InitializeComponent();
            SetResourceReference(StyleProperty, typeof(Button));
        }
        #endregion
    }
    /**@}*/
}