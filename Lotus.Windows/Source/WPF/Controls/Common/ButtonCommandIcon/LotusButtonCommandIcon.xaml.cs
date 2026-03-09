using System.Windows;
using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsCommon Общие элементы управления
     * \ingroup WindowsWPFControls
     * \brief Общие элементы управления.
     * @{
     */
    /// <summary>
    /// Стандартная кнопка с поддержкой иконки из связанной команды.
    /// </summary>
    public class LotusButtonCommandIcon : Button
    {
        #region Constructors
        static LotusButtonCommandIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LotusButtonCommandIcon),
                new FrameworkPropertyMetadata(typeof(LotusButtonCommandIcon)));
        }

        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusButtonCommandIcon()
        {
        }
        #endregion
    }
    /**@}*/
}
