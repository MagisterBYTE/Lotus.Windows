using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Таблица с иерархией строк (дерево + колонки), фильтрами заголовков и одиночным выбором.
    /// Источник дерева задаётся через <see cref="TreeRoots"/>; плоский список строк — <see cref="LotusTreeGridRowModel"/> (внутренний ItemsSource).
    /// </summary>
    public class LotusTreeGrid : LotusDataGrid
    {
        #region Static constructor
        static LotusTreeGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LotusTreeGrid),
                new FrameworkPropertyMetadata(typeof(LotusTreeGrid)));
        }
        #endregion

        #region DependencyProperty
        /// <summary>
        /// Корни дерева (наблюдаемая коллекция или перечисление узлов <see cref="ITreeGridNode"/>).
        /// </summary>
        public static readonly DependencyProperty TreeRootsProperty =
            DependencyProperty.Register(nameof(TreeRoots), typeof(IEnumerable), typeof(LotusTreeGrid),
                new FrameworkPropertyMetadata(null, OnTreeRootsPropertyChanged));

        /// <summary>
        /// Выбранный узел дерева (синхронизируется с выбором строки).
        /// </summary>
        public static readonly DependencyProperty SelectedTreeNodeProperty =
            DependencyProperty.Register(nameof(SelectedTreeNode), typeof(ITreeGridNode), typeof(LotusTreeGrid),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private static void OnTreeRootsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LotusTreeGrid)d).OnTreeRootsChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable);
        }
        #endregion

        #region Fields
        private readonly ObservableCollection<LotusTreeGridRowModel> _flatRows = [];
        private readonly List<ITreeGridNode> _subscribedNodes = [];
        private bool _suppressExternalItemsSource;
        #endregion

        #region Properties
        /// <summary>
        /// Корни дерева.
        /// </summary>
        public IEnumerable? TreeRoots
        {
            get => (IEnumerable?)GetValue(TreeRootsProperty);
            set => SetValue(TreeRootsProperty, value);
        }

        /// <summary>
        /// Узел, соответствующий текущей выбранной строке.
        /// </summary>
        public ITreeGridNode? SelectedTreeNode
        {
            get => (ITreeGridNode?)GetValue(SelectedTreeNodeProperty);
            set => SetValue(SelectedTreeNodeProperty, value);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusTreeGrid()
        {
            AutoGenerateColumns = false;
            SelectionMode = DataGridSelectionMode.Single;
            CanUserAddRows = false;
            SelectionChanged += OnTreeGridSelectionChanged;
            AddHandler(ContextMenuOpeningEvent, new ContextMenuEventHandler(OnRowContextMenuOpening), true);
            _suppressExternalItemsSource = true;
            ItemsSource = _flatRows;
            _suppressExternalItemsSource = false;
        }
        #endregion

        #region Overrides
        /// <inheritdoc />
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (_suppressExternalItemsSource || ReferenceEquals(newValue, _flatRows))
            {
                base.OnItemsSourceChanged(oldValue, newValue);
                return;
            }

            if (newValue is not null && !ReferenceEquals(newValue, _flatRows))
            {
                _suppressExternalItemsSource = true;
                ItemsSource = _flatRows;
                _suppressExternalItemsSource = false;
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Если активны фильтры столбцов, строка проходит, если выполняются условия по самой строке
        /// или по любому из потомков (чтобы оставались видимы предки найденных позиций).
        /// </remarks>
        protected override bool PassesColumnFilters(object item)
        {
            if (item is not LotusTreeGridRowModel row)
                return base.PassesColumnFilters(item);

            if (!HasActiveColumnFilters())
                return true;

            if (base.PassesColumnFilters(row))
                return true;

            return AnyDescendantMatchesColumnFilters(row.Node);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Фиксируем текст в источник до снятия шаблона редактирования, иначе TwoWay из ячейки может не сохраниться.
        /// </remarks>
        protected override void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
        {
            if (!e.Cancel && e.EditAction == DataGridEditAction.Commit && e.EditingElement is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            base.OnCellEditEnding(e);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Добавить текстовую колонку с привязкой к <see cref="ITreeGridNode.GetValue"/> по ключу и опциональным строковым фильтром.
        /// </summary>
        public DataGridTextColumn AddTextColumn(string header, string columnKey, TColumnFilterType filterType = TColumnFilterType.None)
        {
            var col = new DataGridTextColumn
            {
                Header = header,
                Binding = new Binding($"[{columnKey}]") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.LostFocus },
                SortMemberPath = columnKey
            };
            Columns.Add(col);
            AttachTreeColumnFilter(col, columnKey, filterType);
            ApplySpansCollapsedCellStyles(col);
            return col;
        }

        /// <summary>
        /// Добавить числовую колонку с фильтром по числу.
        /// </summary>
        /// <param name="header">Заголовок столбца.</param>
        /// <param name="columnKey">Ключ значения в <see cref="ITreeGridNode.GetValue"/>.</param>
        /// <param name="stringFormat">
        /// Формат отображения для привязки (как в <c>string.Format</c>), например <c>{0:N2}</c> или <c>{0:F3}</c>.
        /// В XAML для литеральной фигурной скобки пишут <c>{}{0:N2}</c>; в C# задавайте обычную строку <c>{0:N2}</c>.
        /// </param>
        public DataGridTextColumn AddNumericTextColumn(string header, string columnKey,
            string stringFormat = "{0:N2}")
        {
            var col = new DataGridTextColumn
            {
                Header = header,
                Binding = new Binding($"[{columnKey}]") { Mode = BindingMode.TwoWay, StringFormat = stringFormat, UpdateSourceTrigger = UpdateSourceTrigger.LostFocus },
                SortMemberPath = columnKey
            };
            Columns.Add(col);
            AttachTreeColumnFilter(col, columnKey, TColumnFilterType.Number);
            ApplySpansCollapsedCellStyles(col);
            return col;
        }

        /// <summary>
        /// Колонка наименования с отступами уровня и переключателем раскрытия.
        /// </summary>
        /// <param name="header">Заголовок столбца.</param>
        /// <param name="withStringFilter">Подключить строковый фильтр по ключу <c>DisplayName</c> узла.</param>
        public DataGridTemplateColumn AddTreeNameColumn(string header = "Наименование", bool withStringFilter = true)
        {
            var col = new DataGridTemplateColumn
            {
                Header = header,
                Width = new DataGridLength(2, DataGridLengthUnitType.Star),
                SortMemberPath = "DisplayName"
            };
            Columns.Add(col);
            void OnLoaded(object _, RoutedEventArgs __)
            {
                Loaded -= OnLoaded;
                if (col.CellTemplate is null && TryFindResource("LotusTreeGridNameCellTemplateKey") is DataTemplate dt)
                    col.CellTemplate = dt;
            }

            Loaded += OnLoaded;
            if (TryFindResource("LotusTreeGridNameCellTemplateKey") is DataTemplate immediate)
                col.CellTemplate = immediate;

            if (withStringFilter)
            {
                SetColumnFilterType(col, TColumnFilterType.String);
                var qi = new QueryItemTreeGridString();
                qi.PropertyName = "DisplayName";
                AttachQueryItem(col, qi);
            }

            return col;
        }

        private static void ApplySpansCollapsedCellStyles(DataGridTextColumn col)
        {
            var elementStyle = new Style(typeof(TextBlock));
            var hideWhenGroup = new DataTrigger
            {
                Binding = new Binding(nameof(LotusTreeGridRowModel.SpansAllColumns)),
                Value = true
            };
            hideWhenGroup.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
            elementStyle.Triggers.Add(hideWhenGroup);
            col.ElementStyle = elementStyle;

            var editingStyle = new Style(typeof(TextBox));
            var readOnlyGroup = new DataTrigger
            {
                Binding = new Binding(nameof(LotusTreeGridRowModel.SpansAllColumns)),
                Value = true
            };
            readOnlyGroup.Setters.Add(new Setter(UIElement.IsEnabledProperty, false));
            editingStyle.Triggers.Add(readOnlyGroup);
            col.EditingElementStyle = editingStyle;
        }
        #endregion

        #region Tree / flat list
        private void OnTreeRootsChanged(IEnumerable? oldRoots, IEnumerable? newRoots)
        {
            if (oldRoots is INotifyCollectionChanged oldNcc)
                oldNcc.CollectionChanged -= OnRootsCollectionChanged;
            if (newRoots is INotifyCollectionChanged newNcc)
                newNcc.CollectionChanged += OnRootsCollectionChanged;

            RebuildFlatRows();
        }

        private void OnRootsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => RebuildFlatRows();

        private void RebuildFlatRows()
        {
            UnsubscribeAllNodes();
            foreach (var row in _flatRows)
                row.Dispose();
            _flatRows.Clear();

            foreach (var root in EnumerateRoots())
            {
                SubscribeTree(root);
                AppendVisibleRows(root, 0);
            }

            CollectionViewSource.GetDefaultView(_flatRows)?.Refresh();
        }

        private IEnumerable<ITreeGridNode> EnumerateRoots()
        {
            if (TreeRoots == null) yield break;
            foreach (var item in TreeRoots)
            {
                if (item is ITreeGridNode node)
                    yield return node;
            }
        }

        private void AppendVisibleRows(ITreeGridNode node, int level)
        {
            _flatRows.Add(new LotusTreeGridRowModel(node, level));
            if (!node.IsExpanded) return;
            foreach (var child in node.Children)
                AppendVisibleRows(child, level + 1);
        }

        private void SubscribeTree(ITreeGridNode node)
        {
            if (_subscribedNodes.Contains(node)) return;
            node.PropertyChanged += OnSubscribedNodePropertyChanged;
            _subscribedNodes.Add(node);
            foreach (var c in node.Children)
                SubscribeTree(c);
        }

        private void UnsubscribeAllNodes()
        {
            foreach (var n in _subscribedNodes)
                n.PropertyChanged -= OnSubscribedNodePropertyChanged;
            _subscribedNodes.Clear();
        }

        private void OnSubscribedNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not ITreeGridNode) return;
            if (e.PropertyName is null or nameof(ITreeGridNode.IsExpanded))
                Dispatcher.BeginInvoke(new Action(RebuildFlatRows));
        }

        private bool HasActiveColumnFilters()
        {
            foreach (var qi in ColumnFilterQuery.Items)
            {
                if (!qi.NotCalculation && IsQueryItemActive(qi))
                    return true;
            }
            return false;
        }

        private bool AnyDescendantMatchesColumnFilters(ITreeGridNode node)
        {
            foreach (var c in node.Children)
            {
                var childRow = new LotusTreeGridRowModel(c, 0, subscribeToNode: false);
                if (base.PassesColumnFilters(childRow))
                    return true;
                if (AnyDescendantMatchesColumnFilters(c))
                    return true;
            }
            return false;
        }
        #endregion

        #region Selection / context menu
        private void OnTreeGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem is LotusTreeGridRowModel row)
            {
                ClearSelectionFlagsExcept(row.Node);
                row.IsSelected = true;
                SetCurrentValue(SelectedTreeNodeProperty, row.Node);
            }
            else
                SetCurrentValue(SelectedTreeNodeProperty, null);
        }

        private void ClearSelectionFlagsExcept(ITreeGridNode? keep)
        {
            foreach (var root in EnumerateRoots())
                ClearSelectionRecursive(root, keep);
        }

        private static void ClearSelectionRecursive(ITreeGridNode node, ITreeGridNode? keep)
        {
            if (!ReferenceEquals(node, keep))
                node.IsSelected = false;
            foreach (var c in node.Children)
                ClearSelectionRecursive(c, keep);
        }

        private void OnRowContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (e.OriginalSource is not DependencyObject source)
                return;

            var row = source.FindVisualParent<DataGridRow>();
            if (row?.Item is not LotusTreeGridRowModel model)
                return;

            if (model.Node is not ITreeGridContextMenuProvider provider)
                return;

            e.Handled = true;
            var menu = row.ContextMenu ?? new ContextMenu();
            row.ContextMenu = menu;
            menu.Items.Clear();
            provider.BuildContextMenu(menu);
            menu.PlacementTarget = row;
            menu.IsOpen = true;
        }
        #endregion

        #region Column filters
        private void AttachTreeColumnFilter(DataGridColumn column, string columnKey, TColumnFilterType filterType)
        {
            if (filterType == TColumnFilterType.None)
                return;

            SetColumnFilterType(column, filterType);
            var queryItem = CreateTreeQueryItem(filterType, columnKey);
            if (queryItem != null)
                AttachQueryItem(column, queryItem);
        }

        private static QueryItem? CreateTreeQueryItem(TColumnFilterType filterType, string columnKey)
        {
            QueryItem? qi = filterType switch
            {
                TColumnFilterType.String => new QueryItemTreeGridString(),
                TColumnFilterType.Number => new QueryItemTreeGridNumber(),
                _ => null
            };
            if (qi != null)
                qi.PropertyName = columnKey;
            return qi;
        }
        #endregion
    }
    /**@}*/
}
