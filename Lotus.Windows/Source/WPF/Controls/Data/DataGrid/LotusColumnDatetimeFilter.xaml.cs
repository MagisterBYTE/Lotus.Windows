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
    public partial class LotusColumnDatetimeFilter : UserControl
    {
        #region Declare DependencyProperty 
        /// <summary>
        /// Элемент запроса для данных дата-время.
        /// </summary>
        public static readonly DependencyProperty QueryItemProperty = DependencyProperty.Register(nameof(QueryItem),
            typeof(CQueryItemDateTime), typeof(LotusColumnDatetimeFilter),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Properties
        /// <summary>
        /// Элемент запроса для данных дата-время.
        /// </summary>
        [Browsable(false)]
        public CQueryItemDateTime QueryItem
        {
            get { return (CQueryItemDateTime)GetValue(QueryItemProperty); }
            set { SetValue(QueryItemProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusColumnDatetimeFilter()
        {
            InitializeComponent();
            QueryItem = new CQueryItemDateTime();
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