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
            typeof(CQueryItemString), typeof(LotusColumnStringFilter),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Properties
        /// <summary>
        /// Элемент запроса для строковых данных.
        /// </summary>
        [Browsable(false)]
        public CQueryItemString QueryItem
        {
            get { return (CQueryItemString)GetValue(QueryItemProperty); }
            set { SetValue(QueryItemProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusColumnStringFilter()
        {
            InitializeComponent();
            QueryItem = new CQueryItemString();
            QueryItem.BindingComboBoxToSearchOption(comboSearchOption);
        }
        #endregion
    }
    /**@}*/
}