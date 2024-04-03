using System.Windows;
using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsEditor
	*@{*/
    /// <summary>
    /// Текстовое поле с дополнительною функциональностью.
    /// </summary>
    public partial class LotusTextBox : TextBox
    {
        #region Declare DependencyProperty 
        /// <summary>
        /// Заполнитель текста в случае отсутствие текста.
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(nameof(PlaceholderText),
            typeof(string),
            typeof(LotusTextBox),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Статус показа кнопки отчистки.
        /// </summary>
        public static readonly DependencyProperty ShowButtonClearProperty = DependencyProperty.Register(nameof(ShowButtonClear),
            typeof(bool),
            typeof(LotusTextBox),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region Properties
        /// <summary>
        /// Заполнитель текста в случае отсутствие текста.
        /// </summary>
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        /// <summary>
        /// Статус показа кнопки отчистки.
        /// </summary>
        public bool ShowButtonClear
        {
            get { return (bool)GetValue(ShowButtonClearProperty); }
            set { SetValue(ShowButtonClearProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusTextBox()
        {
            InitializeComponent();
            Style = this.Resources["PlaceHolderTextBoxStyleKey"] as Style;
            BorderBrush = System.Windows.Media.Brushes.DarkGray;
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
            this.Text = "";
        }
        #endregion
    }
    /**@}*/
}