using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Элемент служащий для формирования элемента запроса для строковых типов данных.
    /// </summary>
    public partial class LotusColumnStringFilter : UserControl
    {
        #region Declare DependencyProperty
        /// <summary>
        /// Элемент запроса для строковых данных.
        /// </summary>
        public static readonly DependencyProperty QueryItemProperty = DependencyProperty.Register(nameof(QueryItem),
            typeof(QueryItemString), typeof(LotusColumnStringFilter),
            new FrameworkPropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Элемент запроса для строковых данных.
        /// </summary>
        [Browsable(false)]
        public QueryItemString? QueryItem
        {
            get => (QueryItemString?)GetValue(QueryItemProperty);
            set => SetValue(QueryItemProperty, value);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusColumnStringFilter()
        {
            InitializeComponent();
        }
        #endregion
    }
    /**@}*/
}
