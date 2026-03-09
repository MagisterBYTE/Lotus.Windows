using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFExtension
	*@{*/
    /// <summary>
    /// Статический класс реализующий методы расширения для типа <see cref="DependencyObject"/>.
    /// </summary>
    public static class XWindowsDependencyExtension
    {
        /// <summary>
        /// Поиск визуального предка элемента.
        /// </summary>
        /// <typeparam name="TElement">Требуемый тип элемента предка.</typeparam>
        /// <param name="sourceObj">Объект - источник поиска.</param>
        /// <returns>Найденный элемент.</returns>
        public static TElement? FindVisualParent<TElement>(this DependencyObject sourceObj)
            where TElement : DependencyObject
        {
            do
            {
                if (sourceObj is TElement element)
                {
                    return element;
                }
                sourceObj = VisualTreeHelper.GetParent(sourceObj);
            }
            while (sourceObj is not null);

            return null;
        }

        /// <summary>
        /// Поиск логического предка элемента.
        /// </summary>
        /// <typeparam name="TElement">Требуемый тип элемента предка.</typeparam>
        /// <param name="sourceObj">Объект - источник поиска.</param>
        /// <returns>Найденный элемент.</returns>
        public static TElement? FindLogicalParent<TElement>(this DependencyObject sourceObj)
            where TElement : DependencyObject
        {
            //get parent item
            var parent_object = LogicalTreeHelper.GetParent(sourceObj);

            //we've reached the end of the tree
            if (parent_object == null) return null;

            //check if the parent matches the type we're looking for
            var parent = parent_object as TElement;
            if (parent is not null)
            {
                return parent;
            }
            else
            {
                return FindLogicalParent<TElement>(parent_object);
            }
        }

        /// <summary>
        /// Поиск визуального дочернего элемента.
        /// </summary>
        /// <typeparam name="TElement">Требуемый тип дочернего элемента.</typeparam>
        /// <param name="sourceObj">Объект - источник поиска.</param>
        /// <returns>Найденный элемент.</returns>
        public static TElement? FindVisualChild<TElement>(this DependencyObject sourceObj)
            where TElement : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(sourceObj); i++)
            {
                var сhild = VisualTreeHelper.GetChild(sourceObj, i);

                if (сhild is TElement element)
                {
                    return element;
                }
                else
                {
                    var сhildOfChild = FindVisualChild<TElement>(сhild);

                    if (сhildOfChild is not null)
                    {
                        return сhildOfChild;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Поиск логического дочернего элемента.
        /// </summary>
        /// <typeparam name="TElement">Требуемый тип дочернего элемента.</typeparam>
        /// <param name="sourceObj">Объект - источник поиска.</param>
        /// <returns>Найденный элемент.</returns>
        public static TElement? FindLogicalChild<TElement>(this DependencyObject sourceObj)
            where TElement : DependencyObject
        {
            if (sourceObj is not null)
            {
                foreach (var child in LogicalTreeHelper.GetChildren(sourceObj))
                {
                    if (child is TElement element)
                    {
                        return element;
                    }
                    else
                    {
                        var сhildOfChild = FindLogicalChild<TElement>((child as DependencyObject)!);

                        if (сhildOfChild is not null)
                        {
                            return сhildOfChild;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Заполнение списка дочерними визуальными элементами.
        /// </summary>
        /// <typeparam name="TElement">Требуемый тип дочернего элемента.</typeparam>
        /// <param name="sourceObj">Объект - источник поиска.</param>
        /// <param name="elements">Список для заполнения.</param>
        public static void FillVisualChildList<TElement>(this DependencyObject sourceObj, in List<TElement> elements)
            where TElement : Visual
        {
            var count = VisualTreeHelper.GetChildrenCount(sourceObj);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(sourceObj, i);
                if (child is TElement element)
                {
                    elements.Add(element);
                }
                else if (child is not null)
                {
                    FillVisualChildList(child, elements);
                }
            }
        }

        /// <summary>
        /// Перечисление дочерних визуальных объектов.
        /// </summary>
        /// <typeparam name="TType">Требуемый тип.</typeparam>
        /// <param name="sourceObj">Объект - источник поиска.</param>
        /// <returns>Перечислитель.</returns>
        public static IEnumerable<TType> EnumerateVisualChildren<TType>(this DependencyObject sourceObj) where TType : DependencyObject
        {
            if (sourceObj is not null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(sourceObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(sourceObj, i);
                    if (child is TType type)
                    {
                        yield return type;
                    }

                    foreach (var сhildOfChild in EnumerateVisualChildren<TType>(child))
                    {
                        yield return сhildOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Перечисление дочерних логических объектов.
        /// </summary>
        /// <typeparam name="TType">Требуемый тип.</typeparam>
        /// <param name="sourceObj">Объект - источник поиска.</param>
        /// <returns>Перечислитель.</returns>
        public static IEnumerable<TType> EnumerateLogicalChildren<TType>(this DependencyObject sourceObj) where TType : DependencyObject
        {
            if (sourceObj is not null)
            {
                foreach (var child in LogicalTreeHelper.GetChildren(sourceObj))
                {
                    if (child is TType type)
                    {
                        yield return type;
                    }

                    foreach (var сhildOfChild in EnumerateLogicalChildren<TType>((child as DependencyObject)!))
                    {
                        yield return сhildOfChild;
                    }
                }
            }
        }
    }
    /**@}*/
}