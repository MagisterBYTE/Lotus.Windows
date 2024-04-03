using System.Windows;
using System.Windows.Data;

namespace Lotus.Windows
{
    /**
     * \defgroup WindowsWPFControlsRibbon Элементы ленты
     * \ingroup WindowsWPFControls
     * \brief Элементы ленты.
     * @{
     */
    /// <summary>
    /// Кнопка ленты с поддержкой иконки из связанной команды.
    /// </summary>
    public partial class LotusRibbonButtonIcon : Fluent.Button
    {
        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusRibbonButtonIcon()
        {
            InitializeComponent();
            SetResourceReference(StyleProperty, typeof(Fluent.Button));
        }
        #endregion

        #region Event handlers 
        /// <summary>
        /// Загрузка кнопки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRibbonButton_Loaded(object sender, RoutedEventArgs args)
        {
            if (Command != null)
            {
                var command_ui = Command as RoutedIconUICommand;
                if (command_ui != null)
                {
                    var header_binding = new Binding(nameof(RoutedIconUICommand.Text));
                    header_binding.Source = command_ui;
                    var middle_icon_binding = new Binding(nameof(RoutedIconUICommand.MiddleIcon));
                    middle_icon_binding.Source = command_ui;
                    var large_icon_binding = new Binding(nameof(RoutedIconUICommand.LargeIcon));
                    large_icon_binding.Source = command_ui;
                    BindingOperations.SetBinding(this, Fluent.Button.HeaderProperty, header_binding);
                    BindingOperations.SetBinding(this, Fluent.Button.IconProperty, middle_icon_binding);
                    BindingOperations.SetBinding(this, Fluent.Button.LargeIconProperty, large_icon_binding);
                }
            }
        }
        #endregion
    }
    /**@}*/
}