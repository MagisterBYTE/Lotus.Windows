using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Контракт узла для компонента TreeGrid.
    /// </summary>
    public interface ITreeGridNode : INotifyPropertyChanged
    {
        /// <summary>
        /// Отображаемое имя узла.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Статус раскрытия дочерних узлов.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Статус выбора узла.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Дочерние узлы.
        /// </summary>
        IReadOnlyList<ITreeGridNode> Children { get; }

        /// <summary>
        /// Если true, строка представляет группу: сетка должна показать одну логическую ячейку на всю ширину
        /// (только имя/заголовок группы), без отдельных колонок данных.
        /// </summary>
        bool SpansAllColumns { get; }

        /// <summary>
        /// Получить значение поля для отображения/фильтрации.
        /// </summary>
        object? GetValue(string columnKey);

        /// <summary>
        /// Установить значение поля при редактировании.
        /// </summary>
        bool TrySetValue(string columnKey, object? value);
    }

    /// <summary>
    /// Дополнительный контракт поставщика контекстного меню узла.
    /// </summary>
    public interface ITreeGridContextMenuProvider
    {
        /// <summary>
        /// Построить контекстное меню для узла.
        /// </summary>
        void BuildContextMenu(ContextMenu contextMenu);
    }
    /**@}*/
}
