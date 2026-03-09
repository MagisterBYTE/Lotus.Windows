using System.Windows;
using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Элемент служащий для формирования элемента запроса для перечисляемых типов данных.
    /// </summary>
    public partial class LotusColumnEnumFilter : UserControl
    {
        #region Declare DependencyProperty
        /// <summary>
        /// Элемент запроса для перечисляемых данных.
        /// </summary>
        public static readonly DependencyProperty QueryItemProperty = DependencyProperty.Register(nameof(QueryItem),
            typeof(QueryItemEnum), typeof(LotusColumnEnumFilter),
            new FrameworkPropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Элемент запроса для перечисляемых данных.
        /// </summary>
        public QueryItemEnum? QueryItem
        {
            get => (QueryItemEnum?)GetValue(QueryItemProperty);
            set => SetValue(QueryItemProperty, value);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusColumnEnumFilter()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Отметка чекбокса — добавить значение в фильтр.
        /// </summary>
        private void OnCheckBox_Checked(object sender, RoutedEventArgs args)
        {
            if (sender is not CheckBox checkBox || QueryItem == null) return;
            if (!QueryItem.FilteredItems.Contains(checkBox.Content))
            {
                QueryItem.FilteredItems.Add(checkBox.Content);
                QueryItem.NotifyFilteredItemsChanged();
                comboBoxSourceItems.Text = QueryItem.JoinFilteredItems();
            }
        }

        /// <summary>
        /// Снятие чекбокса — убрать значение из фильтра.
        /// </summary>
        private void OnCheckBox_Unchecked(object sender, RoutedEventArgs args)
        {
            if (sender is not CheckBox checkBox || QueryItem == null) return;
            var index = QueryItem.FilteredItems.IndexOf(checkBox.Content);
            if (index > -1)
            {
                QueryItem.FilteredItems.RemoveAt(index);
                QueryItem.NotifyFilteredItemsChanged();
                comboBoxSourceItems.Text = QueryItem.JoinFilteredItems();
            }
        }

        /// <summary>
        /// Выбор в ComboBox — обновить отображаемый текст.
        /// </summary>
        private void OnComboBoxSourceItems_Selected(object sender, SelectionChangedEventArgs args)
        {
            if (QueryItem != null)
                comboBoxSourceItems.Text = QueryItem.JoinFilteredItems();
        }
        #endregion
    }
    /**@}*/
}
