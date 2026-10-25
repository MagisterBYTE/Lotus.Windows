using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using Lotus.Core;
using Lotus.Core.Inspector;

namespace Lotus.Windows.App
{
    /// <summary>
    /// Главное окно тестового приложения.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RegisterDemoCommandBindings();
            PopulateLoggerDemo();
            PopulateDataGridDemo();
            PopulateTreeViewDemo();
            PopulateTreeGridDemo();
            propertyInspector.SelectedObject = new DemoInspectorObject1();
        }

        private void RegisterDemoCommandBindings()
        {
            CommandBindings.Add(new CommandBinding(XCommandManager.FileNew, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.FileOpen, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.FileSave, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.FileSaveAs, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.FileClose, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.EditCopy, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.EditCut, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.EditPaste, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.EditUndo, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.EditRedo, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.RecordAdd, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.RecordRemove, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.RecordMoveUp, OnDemoExecuted, OnDemoCanExecute));
            CommandBindings.Add(new CommandBinding(XCommandManager.RecordMoveDown, OnDemoExecuted, OnDemoCanExecute));
        }

        private void PopulateLoggerDemo()
        {
            loggerControl.Log("Приложение запущено.", Lotus.Core.TLogType.Info);
            loggerControl.Log("Обнаружено предупреждение в модуле.", Lotus.Core.TLogType.Warning);
            loggerControl.Log("Критическая ошибка инициализации.", Lotus.Core.TLogType.Error);
            loggerControl.Log("Операция выполнена успешно.", Lotus.Core.TLogType.Succeed);
            loggerControl.Log("Задача провалилась.", Lotus.Core.TLogType.Failed);
            loggerControl.LogModule("FileSystem", "Файл открыт.", Lotus.Core.TLogType.Info);
            loggerControl.LogModule("Database", "Соединение разорвано.", Lotus.Core.TLogType.Error);
        }

        private void OnDemoCanExecute(object sender, CanExecuteRoutedEventArgs args) => args.CanExecute = true;

        private void OnDemoExecuted(object sender, ExecutedRoutedEventArgs args) { }

        private LotusGrowlNotification? _growlWindow;

        private LotusGrowlNotification GrowlWindow =>
            _growlWindow ??= new LotusGrowlNotification();

        private void OnShowGrowlInfo_Click(object sender, RoutedEventArgs args) =>
            GrowlWindow.AddNotification(TNotificationType.Info, "Операция выполнена успешно.");

        private void OnShowGrowlWarning_Click(object sender, RoutedEventArgs args) =>
            GrowlWindow.AddNotification(TNotificationType.Warning, "Обнаружены устаревшие данные.");

        private void OnShowGrowlError_Click(object sender, RoutedEventArgs args) =>
            GrowlWindow.AddNotification(TNotificationType.Error, "Критическая ошибка инициализации.");

        private void OnShowLongTaskInformer_Click(object sender, RoutedEventArgs args)
        {
            var window = new LotusWindowLongTaskInformer
            {
                ProgressValue = 45,
                InformerText = "Обработка файлов..."
            };
            window.Show();
        }

        // ── TreeView demo ──────────────────────────────────────────────────────

        private sealed class DemoTreeNode
        {
            public string Name { get; set; } = "";
            public override string ToString() => Name;
        }

        private static ViewModelHierarchy<DemoTreeNode> MakeNode(string name, ViewModelHierarchy<DemoTreeNode>? parent)
            => new(new DemoTreeNode { Name = name }, parent);

        private void PopulateTreeViewDemo()
        {
            var documents = MakeNode("Документы", null);
            var projects  = MakeNode("Проекты", documents);
            documents.Add(projects);
            projects.Add(MakeNode("Lotus.Windows", projects));
            projects.Add(MakeNode("Lotus.Core", projects));
            projects.Add(MakeNode("Lotus.Math", projects));
            documents.Add(MakeNode("Настройки", documents));
            documents.Add(MakeNode("Отчёты", documents));

            var system = MakeNode("Система", null);
            system.Add(MakeNode("Пользователи", system));
            system.Add(MakeNode("Журнал событий", system));
            system.Add(MakeNode("Конфигурация", system));

            demoTreeView.ItemsSource = new[] { documents, system };
            documents.IsExpanded = true;
            projects.IsExpanded = true;
        }

        // ── DataGrid demo ──────────────────────────────────────────────────────

        private enum TDemoStatus { Active, Inactive, Pending }

        private sealed class DemoItem
        {
            public string Name { get; set; } = "";
            public int Age { get; set; }
            public double Score { get; set; }
            public DateTime BirthDate { get; set; }
            public TDemoStatus Status { get; set; }
        }

        private void PopulateDataGridDemo()
        {
            var items = new List<DemoItem>
            {
                new() { Name = "Алексей",   Age = 28, Score = 95.5,  BirthDate = new DateTime(1996, 3, 12), Status = TDemoStatus.Active   },
                new() { Name = "Мария",     Age = 34, Score = 78.0,  BirthDate = new DateTime(1990, 7, 24), Status = TDemoStatus.Inactive },
                new() { Name = "Иван",      Age = 22, Score = 88.3,  BirthDate = new DateTime(2002, 1,  5), Status = TDemoStatus.Pending  },
                new() { Name = "Ольга",     Age = 41, Score = 91.2,  BirthDate = new DateTime(1983, 11, 3), Status = TDemoStatus.Active   },
                new() { Name = "Дмитрий",   Age = 29, Score = 60.0,  BirthDate = new DateTime(1995, 6, 18), Status = TDemoStatus.Inactive },
                new() { Name = "Светлана",  Age = 37, Score = 74.7,  BirthDate = new DateTime(1987, 9, 30), Status = TDemoStatus.Active   },
                new() { Name = "Николай",   Age = 50, Score = 55.0,  BirthDate = new DateTime(1974, 4, 22), Status = TDemoStatus.Pending  },
                new() { Name = "Елена",     Age = 25, Score = 99.1,  BirthDate = new DateTime(1999, 2, 14), Status = TDemoStatus.Active   },
            };

            demoDataGrid.ItemsSource = items;
            demoDataGrid.SelectedObjects.CollectionChanged += (_, _) =>
            {
                selectedCountText.Text = demoDataGrid.SelectedObjects.Count.ToString();
                var names = string.Join(", ", demoDataGrid.SelectedObjects
                    .OfType<DemoItem>().Select(i => i.Name));
                selectedItemsText.Text = names.Length > 0 ? $"Выбраны: {names}" : "";
            };
        }

        private void OnToggleFilter_Click(object sender, RoutedEventArgs args)
        {
            demoDataGrid.IsShowFilterColumn = toggleFilter.IsChecked == true;
            toggleFilter.Content = demoDataGrid.IsShowFilterColumn ? "Скрыть фильтры" : "Показать фильтры";
        }

        // ── TreeGrid demo (сметная иерархия) ───────────────────────────────────

        private void PopulateTreeGridDemo()
        {
            var root = BuildDemoEstimateTree();
            demoTreeGrid.TreeRoots = new System.Collections.ObjectModel.ObservableCollection<ITreeGridNode> { root };
            root.IsExpanded = true;

            demoTreeGrid.SelectionChanged += (_, _) =>
            {
                treeGridSelectionText.Text = demoTreeGrid.SelectedTreeNode is DemoEstimateNode n
                    ? $"Выбрано: {n.DisplayName}"
                    : demoTreeGrid.SelectedTreeNode != null
                        ? $"Выбрано: {demoTreeGrid.SelectedTreeNode.DisplayName}"
                        : string.Empty;
            };
        }

        private static DemoEstimateNode BuildDemoEstimateTree()
        {
            var estimate = new DemoEstimateNode("Локальная смета", spansAllColumns: true);
            var section = new DemoEstimateNode("1. Подготовительные работы", spansAllColumns: true);
            // Подраздел с итогами по колонкам (не SpansAllColumns — иначе числовые ячейки скрыты стилем грида).
            var subsection = new DemoEstimateNode("1.1. Земляные работы", spansAllColumns: false);
            var pos1 = new DemoEstimateNode("Раскопка грунта вручную", spansAllColumns: false)
            {
                Unit = "м³",
                Quantity = 12.5,
                Price = 450
            };
            var pos2 = new DemoEstimateNode("Обратная засыпка пазух", spansAllColumns: false)
            {
                Unit = "м³",
                Quantity = 8,
                Price = 210
            };
            estimate.AddChild(section);
            section.AddChild(subsection);
            subsection.AddChild(pos1);
            subsection.AddChild(pos2);
            return estimate;
        }

        private void OnToggleTreeGridFilters_Click(object sender, RoutedEventArgs args)
        {
            demoTreeGrid.IsShowFilterColumn = toggleTreeGridFilters.IsChecked == true;
            toggleTreeGridFilters.Content = demoTreeGrid.IsShowFilterColumn ? "Скрыть фильтры" : "Показать фильтры";
        }

        /// <summary>
        /// Демо-узел сметы: группы только с наименованием (<see cref="ITreeGridNode.SpansAllColumns"/>), позиции с полями через <see cref="ITreeGridNode.GetValue"/>.
        /// </summary>
        private sealed class DemoEstimateNode : ITreeGridNode, ITreeGridContextMenuProvider
        {
            private readonly List<ITreeGridNode> _children = [];
            private string _displayName;
            private bool _isExpanded = true;
            private bool _isSelected;
            private string _unit = "";
            private double _quantity;
            private double _price;

            public DemoEstimateNode(string displayName, bool spansAllColumns)
            {
                _displayName = displayName;
                SpansAllColumns = spansAllColumns;
            }

            public bool SpansAllColumns { get; }

            public string DisplayName => _displayName;

            public bool IsExpanded
            {
                get => _isExpanded;
                set
                {
                    if (_isExpanded == value) return;
                    _isExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
                }
            }

            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    if (_isSelected == value) return;
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }

            public IReadOnlyList<ITreeGridNode> Children => _children;

            /// <summary>
            /// Строка-группа с видимыми колонками: итоги по всем листовым позициям ниже (как подраздел сметы).
            /// </summary>
            private bool IsTotalsGroup => !SpansAllColumns && _children.Count > 0;

            public string Unit
            {
                get => IsTotalsGroup ? AggregatedUnit() : _unit;
                set
                {
                    if (IsTotalsGroup) return;
                    if (_unit == value) return;
                    _unit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Unit)));
                }
            }

            public double Quantity
            {
                get => IsTotalsGroup ? AggregatedQuantity() : _quantity;
                set
                {
                    if (IsTotalsGroup) return;
                    if (Math.Abs(_quantity - value) < 0.0000001) return;
                    _quantity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
                }
            }

            public double Price
            {
                get => IsTotalsGroup ? AggregatedUnitPrice() : _price;
                set
                {
                    if (IsTotalsGroup) return;
                    if (Math.Abs(_price - value) < 0.0000001) return;
                    _price = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Price)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
                }
            }

            public double Amount => SpansAllColumns ? 0 : IsTotalsGroup ? AggregatedAmount() : _quantity * _price;

            public event PropertyChangedEventHandler? PropertyChanged;

            public void AddChild(DemoEstimateNode child)
            {
                _children.Add(child);
                child.PropertyChanged += OnChildPropertyChanged;
            }

            private void OnChildPropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                if (!IsTotalsGroup) return;
                if (e.PropertyName is null or nameof(Quantity) or nameof(Price) or nameof(Amount) or nameof(Unit))
                    RaiseAggregatedColumnsChanged();
            }

            private void RaiseAggregatedColumnsChanged()
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Unit)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Price)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
            }

            private IEnumerable<DemoEstimateNode> EnumerateLeafPositions()
            {
                foreach (var n in _children)
                {
                    if (n is not DemoEstimateNode c) continue;
                    if (c.IsTotalsGroup)
                    {
                        foreach (var leaf in c.EnumerateLeafPositions())
                            yield return leaf;
                    }
                    else if (c._children.Count == 0 && !c.SpansAllColumns)
                        yield return c;
                }
            }

            private string AggregatedUnit()
            {
                string? first = null;
                foreach (var leaf in EnumerateLeafPositions())
                {
                    var u = leaf._unit;
                    if (first == null)
                        first = u;
                    else if (first != u)
                        return "—";
                }

                return string.IsNullOrEmpty(first) ? "—" : first;
            }

            private double AggregatedQuantity()
            {
                var s = 0.0;
                foreach (var leaf in EnumerateLeafPositions())
                    s += leaf._quantity;
                return s;
            }

            private double AggregatedAmount()
            {
                var s = 0.0;
                foreach (var leaf in EnumerateLeafPositions())
                    s += leaf._quantity * leaf._price;
                return s;
            }

            private double AggregatedUnitPrice()
            {
                var q = AggregatedQuantity();
                return q > 0 ? AggregatedAmount() / q : 0;
            }

            public object? GetValue(string columnKey) => columnKey switch
            {
                nameof(DisplayName) => DisplayName,
                "Unit" => Unit,
                "Quantity" => Quantity,
                "Price" => Price,
                "Amount" => Amount,
                _ => null
            };

            public bool TrySetValue(string columnKey, object? value)
            {
                if (SpansAllColumns && columnKey != nameof(DisplayName))
                    return false;
                if (IsTotalsGroup && columnKey is "Unit" or "Quantity" or "Price" or "Amount")
                    return false;

                switch (columnKey)
                {
                    case nameof(DisplayName):
                        if (value is string s)
                        {
                            if (_displayName == s) return true;
                            _displayName = s;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
                            return true;
                        }

                        return false;
                    case "Unit":
                        Unit = value?.ToString() ?? string.Empty;
                        return true;
                    case "Quantity":
                        if (TryParseDouble(value, out var q))
                        {
                            Quantity = q;
                            return true;
                        }

                        return false;
                    case "Price":
                        if (TryParseDouble(value, out var p))
                        {
                            Price = p;
                            return true;
                        }

                        return false;
                    case "Amount":
                        return false;
                    default:
                        return false;
                }
            }

            private static bool TryParseDouble(object? value, out double d)
            {
                d = 0;
                switch (value)
                {
                    case double x:
                        d = x;
                        return true;
                    case float f:
                        d = f;
                        return true;
                    case decimal m:
                        d = (double)m;
                        return true;
                    case int i:
                        d = i;
                        return true;
                    case string text:
                        if (double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out var loc))
                        {
                            d = loc;
                            return true;
                        }

                        if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var inv))
                        {
                            d = inv;
                            return true;
                        }

                        return false;
                    default:
                        return false;
                }
            }

            public void BuildContextMenu(ContextMenu contextMenu)
            {
                var item = new MenuItem { Header = "Демо: свойства строки…" };
                item.Click += (_, _) =>
                    MessageBox.Show($"Узел «{DisplayName}»", "LotusTreeGrid");
                contextMenu.Items.Add(item);
            }
        }

        // ── PropertyInspector demo ─────────────────────────────────────────────

        private void OnInspectorSetObject1_Click(object sender, RoutedEventArgs args) =>
            propertyInspector.SelectedObject = new DemoInspectorObject1();

        private void OnInspectorSetObject2_Click(object sender, RoutedEventArgs args) =>
            propertyInspector.SelectedObject = new DemoInspectorObject2();

        private void OnInspectorClear_Click(object sender, RoutedEventArgs args) =>
            propertyInspector.SelectedObject = null;

        /// <summary>
        /// Демо-объект 1: разные типы свойств — строки, числа, флаги, перечисление, дата.
        /// </summary>
        private sealed class DemoInspectorObject1 : ILotusSupportViewInspector
        {
            [Category("Текст")]
            [DisplayName("Имя")]
            [Description("Строковое свойство — имя объекта.")]
            public string Name { get; set; } = "Демо-объект";

            [Category("Текст")]
            [DisplayName("Описание")]
            [Description("Описание объекта.")]
            public string Description { get; set; } = "Пример для тестирования инспектора";

            [Category("Числа")]
            [DisplayName("Целое число")]
            [Description("Целочисленное свойство в диапазоне 0..1000.")]
            [DefaultValue(42)]
            public int IntValue { get; set; } = 42;

            [Category("Числа")]
            [DisplayName("Дробное")]
            [Description("Вещественное свойство.")]
            [DefaultValue(3.14)]
            public double DoubleValue { get; set; } = 3.14;

            [Category("Числа")]
            [DisplayName("Только чтение")]
            [Description("Числовое свойство только для чтения.")]
            [ReadOnly(true)]
            public float ReadOnlyFloat { get; set; } = 99.9f;

            [Category("Флаги")]
            [DisplayName("Активен")]
            [Description("Логическое свойство — активность объекта.")]
            public bool IsActive { get; set; } = true;

            [Category("Флаги")]
            [DisplayName("Видимый")]
            [Description("Видимость объекта.")]
            public bool IsVisible { get; set; } = false;

            [Category("Дата")]
            [DisplayName("Дата создания")]
            [Description("Дата создания объекта.")]
            public DateTime CreatedAt { get; set; } = DateTime.Now;

            [Category("Перечисление")]
            [DisplayName("Статус")]
            [Description("Текущий статус объекта.")]
            public TDemoStatus Status { get; set; } = TDemoStatus.Active;

            public string InspectorTypeName => nameof(DemoInspectorObject1);
            public string InspectorObjectName => Name;
        }

        /// <summary>
        /// Демо-объект 2: акцент на числовые типы разных размерностей.
        /// </summary>
        private sealed class DemoInspectorObject2 : ILotusSupportViewInspector
        {
            [Category("Целые")]
            [DisplayName("Byte")]
            [Description("Беззнаковый байт (0..255).")]
            public byte ByteValue { get; set; } = 128;

            [Category("Целые")]
            [DisplayName("Int16")]
            [Description("Короткое целое.")]
            public short ShortValue { get; set; } = -1000;

            [Category("Целые")]
            [DisplayName("Int32")]
            [Description("Целое 32 бит.")]
            public int IntValue { get; set; } = 100000;

            [Category("Целые")]
            [DisplayName("Int64")]
            [Description("Целое 64 бит.")]
            public long LongValue { get; set; } = 9_000_000_000L;

            [Category("Дробные")]
            [DisplayName("Single")]
            [Description("Одинарная точность.")]
            public float FloatValue { get; set; } = 1.5f;

            [Category("Дробные")]
            [DisplayName("Double")]
            [Description("Двойная точность.")]
            public double DoubleValue { get; set; } = 2.718281828;

            [Category("Дробные")]
            [DisplayName("Decimal")]
            [Description("Высокая точность.")]
            public decimal DecimalValue { get; set; } = 1234.5678m;

            [Category("Флаги")]
            [DisplayName("Включён")]
            [Description("Простой флаг.")]
            public bool IsEnabled { get; set; } = true;

            public string InspectorTypeName => nameof(DemoInspectorObject2);
            public string InspectorObjectName => "Числовой объект";
        }
    }
}