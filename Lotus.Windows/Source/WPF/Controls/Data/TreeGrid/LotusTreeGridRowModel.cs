using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Плоская модель строки TreeGrid: только иерархия, выбор, раскрытие и делегирование данных узлу.
    /// Конкретные поля сметы и пр. задаются в <see cref="ITreeGridNode"/> (GetValue/TrySetValue) или в шаблонах колонок.
    /// </summary>
    public class LotusTreeGridRowModel : INotifyPropertyChanged, IDisposable
    {
        private bool _isExpanded;
        private bool _isSelected;
        private int _level;
        private readonly bool _subscribeToNode;

        public ITreeGridNode Node { get; }

        public bool HasChildren => Node.Children.Count > 0;

        /// <summary>
        /// Групповая строка: одна объединённая ячейка на всю ширину (см. <see cref="ITreeGridNode.SpansAllColumns"/>).
        /// </summary>
        public bool SpansAllColumns => Node.SpansAllColumns;

        /// <summary>
        /// Текст для колонки дерева / заголовка строки (без привязки к доменным ключам).
        /// </summary>
        public string DisplayName => Node.DisplayName;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                Node.IsExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                Node.IsSelected = value;
                OnPropertyChanged();
            }
        }

        public int Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Indents));
            }
        }

        public IReadOnlyList<int> Indents => _level > 0 ? new int[_level] : [];

        /// <summary>
        /// Универсальный доступ к значениям колонок по ключу (биндинг вида <c>Path=[Ключ]</c>).
        /// </summary>
        public object? this[string columnKey]
        {
            get => Node.GetValue(columnKey);
            set
            {
                if (Node.TrySetValue(columnKey, value))
                    OnPropertyChanged($"Item[{columnKey}]");
            }
        }

        /// <param name="node">Узел дерева.</param>
        /// <param name="level">Уровень вложенности.</param>
        /// <param name="subscribeToNode">
        /// Если false, подписка на <see cref="INotifyPropertyChanged"/> узла не выполняется (временные модели, например для оценки фильтра).
        /// </param>
        public LotusTreeGridRowModel(ITreeGridNode node, int level, bool subscribeToNode = true)
        {
            Node = node;
            _level = level;
            _isExpanded = node.IsExpanded;
            _isSelected = node.IsSelected;
            _subscribeToNode = subscribeToNode;

            if (subscribeToNode)
                Node.PropertyChanged += OnNodePropertyChanged;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_subscribeToNode)
                Node.PropertyChanged -= OnNodePropertyChanged;
        }

        private void OnNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ITreeGridNode.IsExpanded))
            {
                _isExpanded = Node.IsExpanded;
                OnPropertyChanged(nameof(IsExpanded));
            }
            else if (e.PropertyName == nameof(ITreeGridNode.IsSelected))
            {
                _isSelected = Node.IsSelected;
                OnPropertyChanged(nameof(IsSelected));
            }
            else if (e.PropertyName == nameof(ITreeGridNode.SpansAllColumns))
            {
                OnPropertyChanged(nameof(SpansAllColumns));
            }
            else if (e.PropertyName == nameof(ITreeGridNode.DisplayName))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
            else
            {
                // Любые изменения данных узла — обновить привязки к индексатору и пр.
                OnPropertyChanged(null);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    /**@}*/
}
