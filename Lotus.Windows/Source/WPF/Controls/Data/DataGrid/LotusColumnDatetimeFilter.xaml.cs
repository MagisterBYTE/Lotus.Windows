using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Элемент служащий для формирования элемента запроса для типов данных дата-время.
    /// </summary>
    public partial class LotusColumnDateTimeFilter : UserControl
    {
        #region Declare DependencyProperty
        /// <summary>
        /// Элемент запроса для данных дата-время.
        /// </summary>
        public static readonly DependencyProperty QueryItemProperty = DependencyProperty.Register(nameof(QueryItem),
            typeof(QueryItemDateTime), typeof(LotusColumnDateTimeFilter),
            new FrameworkPropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Элемент запроса для данных дата-время.
        /// </summary>
        [Browsable(false)]
        public QueryItemDateTime? QueryItem
        {
            get => (QueryItemDateTime?)GetValue(QueryItemProperty);
            set => SetValue(QueryItemProperty, value);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusColumnDateTimeFilter()
        {
            InitializeComponent();
        }
        #endregion
    }
    /**@}*/
}
