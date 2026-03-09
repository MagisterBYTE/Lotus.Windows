using System.Windows;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsSpecial
	*@{*/
    /// <summary>
    /// Окно отображения прогресса длительной операции.
    /// </summary>
    public partial class LotusWindowLongTaskInformer : Window
    {
        #region Declare DependencyProperty
        /// <summary>
        /// Значение прогресса (0..100).
        /// </summary>
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register(nameof(ProgressValue), typeof(double), typeof(LotusWindowLongTaskInformer),
                new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Текст информационного сообщения.
        /// </summary>
        public static readonly DependencyProperty InformerTextProperty =
            DependencyProperty.Register(nameof(InformerText), typeof(string), typeof(LotusWindowLongTaskInformer),
                new FrameworkPropertyMetadata(string.Empty));
        #endregion

        #region Properties
        /// <summary>
        /// Значение прогресса (0..100).
        /// </summary>
        public double ProgressValue
        {
            get => (double)GetValue(ProgressValueProperty);
            set => SetValue(ProgressValueProperty, value);
        }

        /// <summary>
        /// Текст информационного сообщения.
        /// </summary>
        public string InformerText
        {
            get => (string)GetValue(InformerTextProperty);
            set => SetValue(InformerTextProperty, value);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusWindowLongTaskInformer()
        {
            InitializeComponent();
        }
        #endregion
    }
    /**@}*/
}
