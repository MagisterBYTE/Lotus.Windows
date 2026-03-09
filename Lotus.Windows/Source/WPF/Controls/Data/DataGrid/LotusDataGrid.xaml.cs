using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Lotus.Windows
{
    /**
     * \defgroup WindowsWPFControlsData Элементы для работы с данными
     * \ingroup WindowsWPFControls
     * \brief Элементы для работы с данными.
     * @{
     */
    /// <summary>
    /// Тип фильтра столбца таблицы данных.
    /// </summary>
    public enum TColumnFilterType
    {
        /// <summary>
        /// Без фильтра.
        /// </summary>
        None,

        /// <summary>
        /// Строковый фильтр.
        /// </summary>
        String,

        /// <summary>
        /// Числовой фильтр.
        /// </summary>
        Number,

        /// <summary>
        /// Фильтр по дате и времени.
        /// </summary>
        DateTime,

        /// <summary>
        /// Фильтр по перечислению.
        /// </summary>
        Enum
    }

    /// <summary>
    /// Элемент управления отображением данных с расширенной функциональностью.
    /// </summary>
    public class LotusDataGrid : DataGrid
    {
        #region Static constructor
        static LotusDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LotusDataGrid),
                new FrameworkPropertyMetadata(typeof(LotusDataGrid)));
        }
        #endregion

        #region Attached DependencyProperty
        /// <summary>
        /// Тип фильтра столбца.
        /// </summary>
        public static readonly DependencyProperty ColumnFilterTypeProperty =
            DependencyProperty.RegisterAttached("ColumnFilterType", typeof(TColumnFilterType), typeof(LotusDataGrid),
                new PropertyMetadata(TColumnFilterType.None));

        /// <summary>
        /// Получить тип фильтра столбца.
        /// </summary>
        public static TColumnFilterType GetColumnFilterType(DataGridColumn column) =>
            (TColumnFilterType)column.GetValue(ColumnFilterTypeProperty);

        /// <summary>
        /// Установить тип фильтра столбца.
        /// </summary>
        public static void SetColumnFilterType(DataGridColumn column, TColumnFilterType value) =>
            column.SetValue(ColumnFilterTypeProperty, value);

        /// <summary>
        /// Элемент запроса, связанный со столбцом.
        /// </summary>
        public static readonly DependencyProperty ColumnQueryItemProperty =
            DependencyProperty.RegisterAttached("ColumnQueryItem", typeof(QueryItem), typeof(LotusDataGrid),
                new PropertyMetadata(null));

        /// <summary>
        /// Получить элемент запроса столбца.
        /// </summary>
        public static QueryItem? GetColumnQueryItem(DataGridColumn column) =>
            (QueryItem?)column.GetValue(ColumnQueryItemProperty);

        /// <summary>
        /// Установить элемент запроса столбца.
        /// </summary>
        public static void SetColumnQueryItem(DataGridColumn column, QueryItem? value) =>
            column.SetValue(ColumnQueryItemProperty, value);
        #endregion

        #region Declare DependencyProperty
        /// <summary>
        /// Статус отображения фильтров в заголовках столбцов.
        /// </summary>
        public static readonly DependencyProperty IsShowFilterColumnProperty =
            DependencyProperty.Register(nameof(IsShowFilterColumn), typeof(bool), typeof(LotusDataGrid),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Коллекция выбранных объектов, корректна при любой сортировке и фильтрации.
        /// </summary>
        public static readonly DependencyProperty SelectedObjectsProperty =
            DependencyProperty.Register(nameof(SelectedObjects), typeof(ObservableCollection<object>), typeof(LotusDataGrid),
                new FrameworkPropertyMetadata(null));
        #endregion

        #region Fields
        private readonly QueryBase _query = new();
        #endregion

        #region Properties
        /// <summary>
        /// Статус отображения фильтров в заголовках столбцов.
        /// </summary>
        [Browsable(false)]
        public bool IsShowFilterColumn
        {
            get => (bool)GetValue(IsShowFilterColumnProperty);
            set => SetValue(IsShowFilterColumnProperty, value);
        }

        /// <summary>
        /// Коллекция выбранных объектов, корректна при любой сортировке и фильтрации.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<object> SelectedObjects
        {
            get => (ObservableCollection<object>)GetValue(SelectedObjectsProperty);
            private set => SetValue(SelectedObjectsProperty, value);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusDataGrid()
        {
            SelectedObjects = [];
            SelectionChanged += OnSelectionChanged;
            Columns.CollectionChanged += OnColumnsChanged;
            _query.PropertyChanged += OnQueryChanged;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Генерирование столбца — определение типа фильтра по типу свойства.
        /// </summary>
        protected override void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            base.OnAutoGeneratingColumn(e);
            var filterType = DetectFilterType(e.PropertyType);
            SetColumnFilterType(e.Column, filterType);
            var queryItem = CreateQueryItem(filterType, e.PropertyName, e.PropertyType);
            if (queryItem != null)
                AttachQueryItem(e.Column, queryItem);
        }

        /// <summary>
        /// Смена источника данных — обновить источники enum-фильтров и перезапустить фильтрацию.
        /// </summary>
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue,
            System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            RefreshEnumSources();
            ApplyFilter();
        }

        /// <summary>
        /// Автогенерация столбцов завершена — теперь все QueryItemEnum созданы, заполняем SourceItems.
        /// </summary>
        protected override void OnAutoGeneratedColumns(EventArgs e)
        {
            base.OnAutoGeneratedColumns(e);
            RefreshEnumSources();
        }
        #endregion

        #region Column tracking
        private void OnColumnsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (DataGridColumn col in e.NewItems)
                    OnColumnAdded(col);

            if (e.OldItems != null)
                foreach (DataGridColumn col in e.OldItems)
                    OnColumnRemoved(col);
        }

        private void OnColumnAdded(DataGridColumn column)
        {
            // Уже подключён через OnAutoGeneratingColumn
            if (GetColumnQueryItem(column) != null) return;

            var filterType = GetColumnFilterType(column);
            var propertyName = column.SortMemberPath;
            if (string.IsNullOrEmpty(propertyName) || filterType == TColumnFilterType.None) return;

            var queryItem = CreateQueryItem(filterType, propertyName, null);
            if (queryItem != null)
                AttachQueryItem(column, queryItem);
        }

        private void OnColumnRemoved(DataGridColumn column)
        {
            var queryItem = GetColumnQueryItem(column);
            if (queryItem == null) return;
            _query._items.Remove(queryItem);
            queryItem._queryOwned = null!;
            SetColumnQueryItem(column, null);
            ApplyFilter();
        }

        private void AttachQueryItem(DataGridColumn column, QueryItem queryItem)
        {
            queryItem._queryOwned = _query;
            _query._items.Add(queryItem);
            SetColumnQueryItem(column, queryItem);
        }
        #endregion

        #region Filter methods
        private void OnQueryChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(QueryBase.SQLQuery))
                ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (ItemsSource == null) return;
            var view = CollectionViewSource.GetDefaultView(ItemsSource);
            if (view == null) return;

            var hasActive = _query._items.Any(qi => !qi.NotCalculation && IsQueryItemActive(qi));
            view.Filter = hasActive ? FilterItem : null;
        }

        private bool FilterItem(object item)
        {
            foreach (var qi in _query._items)
            {
                if (qi.NotCalculation || !IsQueryItemActive(qi)) continue;
                if (!qi.MatchesFilter(item)) return false;
            }
            return true;
        }

        private static bool IsQueryItemActive(QueryItem qi) => qi switch
        {
            QueryItemString q  => !string.IsNullOrEmpty(q.SearchValue),
            QueryItemNumber q  => q.ComparisonValueLeft != 0 || q.ComparisonValueRight != 0,
            QueryItemDateTime q => q.ComparisonValueLeft != default,
            QueryItemEnum q    => q.FilteredItems?.Count > 0,
            _                  => false
        };
        #endregion

        #region Enum sources
        private void RefreshEnumSources()
        {
            if (ItemsSource == null) return;
            var itemType = GetCollectionItemType(ItemsSource);
            if (itemType == null) return;

            foreach (var qi in _query._items.OfType<QueryItemEnum>())
            {
                var propInfo = itemType.GetProperty(qi.PropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo == null) continue;
                var enumType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                if (!enumType.IsEnum) continue;
                qi.SourceItems = [.. Enum.GetValues(enumType).Cast<object>()];
                qi.FilteredItems = [];
            }
        }
        #endregion

        #region Event handlers
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedObjects.Clear();
            foreach (var item in SelectedItems)
                SelectedObjects.Add(item);
        }
        #endregion

        #region Static helpers
        private static TColumnFilterType DetectFilterType(Type propertyType)
        {
            var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            if (type.IsEnum) return TColumnFilterType.Enum;
            if (type == typeof(string)) return TColumnFilterType.String;
            if (type == typeof(DateTime)) return TColumnFilterType.DateTime;
            if (type == typeof(int) || type == typeof(long) || type == typeof(float) ||
                type == typeof(double) || type == typeof(decimal) || type == typeof(short) ||
                type == typeof(byte) || type == typeof(uint) || type == typeof(ulong) ||
                type == typeof(ushort))
                return TColumnFilterType.Number;
            return TColumnFilterType.None;
        }

        private static QueryItem? CreateQueryItem(TColumnFilterType filterType, string propertyName, Type? propertyType)
        {
            QueryItem? qi = filterType switch
            {
                TColumnFilterType.String   => new QueryItemString(),
                TColumnFilterType.Number   => new QueryItemNumber(),
                TColumnFilterType.DateTime => new QueryItemDateTime(),
                TColumnFilterType.Enum     => new QueryItemEnum(),
                _                          => null
            };
            if (qi != null)
                qi._propertyName = propertyName;
            return qi;
        }

        private static Type? GetCollectionItemType(System.Collections.IEnumerable source)
        {
            var iface = source.GetType().GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return iface?.GetGenericArguments()[0];
        }
        #endregion
    }
    /**@}*/
}
