using System.Windows;
using System.Windows.Controls;

using HelixToolkit.SharpDX.Core.Model.Scene;

namespace Lotus.Windows
{
    /**
     * \defgroup WindowsViewerContent3DView Представление 3D контента
     * \ingroup WindowsViewerContent3D
     * \brief Элементы и конструкции для отображения 3D контента.
     * @{
     */
    /// <summary>
    /// Селектор шаблона данных для отображения иерархии элементов.
    /// </summary>
    public class CHelixToolkitDataSelector : DataTemplateSelector
    {
        #region Fields
        /// <summary>
        /// Шаблон для представления сцены.
        /// </summary>
        public DataTemplate Scene { get; set; }

        /// <summary>
        /// Шаблон для представления узла.
        /// </summary>
        public DataTemplate Node { get; set; }

        /// <summary>
        /// Шаблон для представления модели.
        /// </summary>
        public DataTemplate Model { get; set; }

        /// <summary>
        /// Шаблон для представления неизвестного узла.
        /// </summary>
        public DataTemplate Unknow { get; set; }
        #endregion

        #region Main methods
        /// <summary>
        /// Выбор шаблона привязки данных.
        /// </summary>
        /// <param name="item">Объект.</param>
        /// <param name="container">Контейнер.</param>
        /// <returns>Нужный шаблон.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var node = item as GroupNode;
            if (node != null)
            {
                return Node;
            }

            var model = item as MeshNode;
            if (model != null)
            {
                return Model;
            }

            var scene = item as SceneNode;
            if (scene != null)
            {
                return Scene;
            }

            return Unknow;
        }
        #endregion
    }
    /**@}*/
}