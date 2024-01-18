//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Методы расширения
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsDependencyExtension.cs
*		Методы расширения для работы c DependencyObject.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFExtension
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Статический класс реализующий методы расширения для типа <see cref="DependencyObject"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XWindowsDependencyExtension
		{
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск визуального предка элемента
			/// </summary>
			/// <typeparam name="TElement">Требуемый тип элемента предка</typeparam>
			/// <param name="source_obj">Объект - источник поиска</param>
			/// <returns>Найденный элемент</returns>
			//---------------------------------------------------------------------------------------------------------
			public static TElement? FindVisualParent<TElement>(this DependencyObject source_obj) 
				where TElement : DependencyObject
			{
				do
				{
					if (source_obj is TElement)
					{
						return (TElement)source_obj;
					}
					source_obj = VisualTreeHelper.GetParent(source_obj);
				}
				while (source_obj != null);

				return null;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск логического предка элемента
			/// </summary>
			/// <typeparam name="TElement">Требуемый тип элемента предка</typeparam>
			/// <param name="source_obj">Объект - источник поиска</param>
			/// <returns>Найденный элемент</returns>
			//---------------------------------------------------------------------------------------------------------
			public static TElement? FindLogicalParent<TElement>(this DependencyObject source_obj)
				where TElement : DependencyObject
			{
				//get parent item
				DependencyObject parent_object = LogicalTreeHelper.GetParent(source_obj);

				//we've reached the end of the tree
				if (parent_object == null) return null;

				//check if the parent matches the type we're looking for
				var parent = parent_object as TElement;
				if (parent != null)
				{
					return parent;
				}
				else
				{
					return FindLogicalParent<TElement>(parent_object);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск визуального дочернего элемента
			/// </summary>
			/// <typeparam name="TElement">Требуемый тип дочернего элемента</typeparam>
			/// <param name="source_obj">Объект - источник поиска</param>
			/// <returns>Найденный элемент</returns>
			//---------------------------------------------------------------------------------------------------------
			public static TElement? FindVisualChild<TElement>(this DependencyObject source_obj)
				where TElement : DependencyObject
			{
				for (var i = 0; i < VisualTreeHelper.GetChildrenCount(source_obj); i++)
				{
					DependencyObject сhild = VisualTreeHelper.GetChild(source_obj, i);

					if (сhild is TElement element)
					{
						return element;
					}
					else
					{
						TElement? сhild_of_child = FindVisualChild<TElement>(сhild);

						if (сhild_of_child != null)
						{
							return сhild_of_child;
						}
					}
				}

				return null;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск логического дочернего элемента
			/// </summary>
			/// <typeparam name="TElement">Требуемый тип дочернего элемента</typeparam>
			/// <param name="source_obj">Объект - источник поиска</param>
			/// <returns>Найденный элемент</returns>
			//---------------------------------------------------------------------------------------------------------
			public static TElement? FindLogicalChild<TElement>(this DependencyObject source_obj)
				where TElement : DependencyObject
			{
				if (source_obj != null)
				{
					foreach (var child in LogicalTreeHelper.GetChildren(source_obj))
					{
						if (child is TElement element)
						{
							return element;
						}
						else
						{
							TElement? сhild_of_child = FindLogicalChild<TElement>((child as DependencyObject)!);

							if (сhild_of_child != null)
							{
								return сhild_of_child;
							}
						}
					}
				}

				return null;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Заполнение списка дочерними визуальными элементами
			/// </summary>
			/// <typeparam name="TElement">Требуемый тип дочернего элемента</typeparam>
			/// <param name="source_obj">Объект - источник поиска</param>
			/// <param name="elements">Список для заполнения</param>
			//---------------------------------------------------------------------------------------------------------
			public static void FillVisualChildList<TElement>(this DependencyObject source_obj, in List<TElement> elements) 
				where TElement : Visual
			{
				var count = VisualTreeHelper.GetChildrenCount(source_obj);
				for (var i = 0; i < count; i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(source_obj, i);
					if (child is TElement element)
					{
						elements.Add(element);
					}
					else if (child != null)
					{
						FillVisualChildList(child, elements);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перечисление дочерних визуальных объектов
			/// </summary>
			/// <typeparam name="TType">Требуемый тип</typeparam>
			/// <param name="source_obj">Объект - источник поиска</param>
			/// <returns>Перечислитель</returns>
			//---------------------------------------------------------------------------------------------------------
			public static IEnumerable<TType> EnumerateVisualChildren<TType>(this DependencyObject source_obj) where TType : DependencyObject
			{
				if (source_obj != null)
				{
					for (var i = 0; i < VisualTreeHelper.GetChildrenCount(source_obj); i++)
					{
						DependencyObject child = VisualTreeHelper.GetChild(source_obj, i);
						if (child is TType)
						{
							yield return (TType)child;
						}

						foreach (TType child_of_child in EnumerateVisualChildren<TType>(child))
						{
							yield return child_of_child;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перечисление дочерних логических объектов
			/// </summary>
			/// <typeparam name="TType">Требуемый тип</typeparam>
			/// <param name="source_obj">Объект - источник поиска</param>
			/// <returns>Перечислитель</returns>
			//---------------------------------------------------------------------------------------------------------
			public static IEnumerable<TType> EnumerateLogicalChildren<TType>(this DependencyObject source_obj) where TType : DependencyObject
			{
				if (source_obj != null)
				{
					foreach (var child in LogicalTreeHelper.GetChildren(source_obj))
					{
						if (child is TType)
						{
							yield return (TType)child;
						}

						foreach (TType child_of_child in EnumerateLogicalChildren<TType>((child as DependencyObject)!))
						{
							yield return child_of_child;
						}
					}
				}
			}
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================