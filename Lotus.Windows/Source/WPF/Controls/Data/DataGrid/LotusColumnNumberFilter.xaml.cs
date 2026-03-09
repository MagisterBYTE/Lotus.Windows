using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Элемент служащий для формирования элемента запроса для числовых типов данных.
    /// </summary>
    public partial class LotusColumnNumberFilter : UserControl
    {
        #region Declare DependencyProperty
        /// <summary>
        /// Элемент запроса для числовых данных.
        /// </summary>
        public static readonly DependencyProperty QueryItemProperty = DependencyProperty.Register(nameof(QueryItem),
            typeof(QueryItemNumber), typeof(LotusColumnNumberFilter),
            new FrameworkPropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Элемент запроса для числовых данных.
        /// </summary>
        [Browsable(false)]
        public QueryItemNumber? QueryItem
        {
            get => (QueryItemNumber?)GetValue(QueryItemProperty);
            set => SetValue(QueryItemProperty, value);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusColumnNumberFilter()
        {
            InitializeComponent();
        }
        #endregion
    }
    /**@}*/
}
