using System.Windows;
using System.Windows.Controls;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsData
	*@{*/
    /// <summary>
    /// Селектор шаблона данных для элементов ViewModel иерархического дерева.
    /// </summary>
    /// <remarks>
    /// Выбирает <see cref="ViewModelHierarchyTemplateSelector"/> для объектов реализующих
    /// <see cref="ILotusViewModelHierarchy"/>, иначе делегирует базовой логике.
    /// </remarks>
    public class ViewModelHierarchyTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Шаблон для элементов реализующих <see cref="ILotusViewModelHierarchy"/>.
        /// </summary>
        public HierarchicalDataTemplate? HierarchyTemplate { get; set; }

        /// <inheritdoc/>
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
            => item is ILotusViewModelHierarchy ? HierarchyTemplate : base.SelectTemplate(item, container);
    }
    /**@}*/
}
