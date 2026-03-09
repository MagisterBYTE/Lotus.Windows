using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsCommon
	*@{*/
    /// <summary>
    /// Элемент меню с поддержкой иконки из связанной команды.
    /// </summary>
    public class LotusMenuItemIcon : MenuItem
    {
        #region Constants
        private const double _iconSize = 16;
        #endregion

        #region Constructors
        static LotusMenuItemIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LotusMenuItemIcon),
                new FrameworkPropertyMetadata(typeof(LotusMenuItemIcon)));
        }

        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusMenuItemIcon()
        {
            var image = new Image
            {
                Width = _iconSize,
                Height = _iconSize,
                Stretch = Stretch.Fill
            };
            image.SetBinding(Image.SourceProperty, new Binding($"{nameof(Command)}.{nameof(RoutedIconUICommand.MiddleIcon)}")
            {
                Source = this
            });
            Icon = image;
        }
        #endregion
    }
    /**@}*/
}
