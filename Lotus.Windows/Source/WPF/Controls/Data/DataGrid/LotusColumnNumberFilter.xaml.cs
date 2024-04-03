using System.Windows;
using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Элемент служащий для формирования элемента запроса для строковых типов данных.
    /// </summary>
    public partial class LotusColumnNumberFilter : UserControl
    {
        #region Declare DependencyProperty 
        /// <summary>
        /// Элемент запроса для числовых данных.
        /// </summary>
        public static readonly DependencyProperty QueryItemProperty = DependencyProperty.Register(nameof(QueryItem),
            typeof(CQueryItemNumber), typeof(LotusColumnNumberFilter),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Properties
        /// <summary>
        /// Элемент запроса для числовых данных.
        /// </summary>
        public CQueryItemNumber QueryItem
        {
            get { return (CQueryItemNumber)GetValue(QueryItemProperty); }
            set { SetValue(QueryItemProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusColumnNumberFilter()
        {
            InitializeComponent();
            QueryItem = new CQueryItemNumber();
            QueryItem.BindingComboBoxToComparisonOperator(comboOperator);
        }
        #endregion

        #region Event handlers 
        /// <summary>
        /// Выбор оператора сравнения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnComboOperator_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            // Method intentionally left empty.
        }
        #endregion
    }
    /**@}*/
}