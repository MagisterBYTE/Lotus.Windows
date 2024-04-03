using System.Windows;

using Fluent;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsRibbon
	*@{*/
    /// <summary>
    /// Кнопка ленты быстрого доступа с поддержкой иконки из связанной команды.
    /// </summary>
    public partial class LotusRibbonQuickAccessButton : QuickAccessMenuItem
    {
        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusRibbonQuickAccessButton()
        {
            InitializeComponent();
            SetResourceReference(StyleProperty, typeof(Fluent.QuickAccessMenuItem));
        }
        #endregion

        #region Event handlers 
        /// <summary>
        /// Загрузка кнопки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRibbonQuickAccessButton_Loaded(object sender, RoutedEventArgs args)
        {
            if (Command != null)
            {
                var command_ui = Command as RoutedIconUICommand;
                if (command_ui != null)
                {
                    IsChecked = true;
                }
            }
        }
        #endregion
    }
    /**@}*/
}