using System.Windows;
using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsEditor
	*@{*/
    /// <summary>
    /// Текстовое поле с поддержкой текста-заполнителя и кнопки очистки.
    /// </summary>
    public class LotusTextBox : TextBox
    {
        #region Declare DependencyProperty
        /// <summary>
        /// Текст-заполнитель, отображаемый в случае отсутствия текста.
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText),
                typeof(string),
                typeof(LotusTextBox),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Статус отображения кнопки очистки.
        /// </summary>
        public static readonly DependencyProperty ShowButtonClearProperty =
            DependencyProperty.Register(nameof(ShowButtonClear),
                typeof(bool),
                typeof(LotusTextBox),
                new FrameworkPropertyMetadata(false));
        #endregion

        #region Properties
        /// <summary>
        /// Текст-заполнитель, отображаемый в случае отсутствия текста.
        /// </summary>
        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        /// <summary>
        /// Статус отображения кнопки очистки.
        /// </summary>
        public bool ShowButtonClear
        {
            get => (bool)GetValue(ShowButtonClearProperty);
            set => SetValue(ShowButtonClearProperty, value);
        }
        #endregion

        #region Constructors
        static LotusTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LotusTextBox),
                new FrameworkPropertyMetadata(typeof(LotusTextBox)));
        }

        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusTextBox()
        {
        }
        #endregion

        #region System methods
        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("buttonClear") is Button buttonClear)
            {
                buttonClear.Click += OnButtonClear_Click;
            }
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Очистка текстового поля.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonClear_Click(object sender, RoutedEventArgs args)
        {
            Text = string.Empty;
        }
        #endregion
    }
    /**@}*/
}
