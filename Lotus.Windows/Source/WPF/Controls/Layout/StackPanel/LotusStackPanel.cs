//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы макетирования
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusStackPanel.cs
*		Макетирующий элемент для последовательного размещения элементов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/**
         * \defgroup WindowsWPFControlsLayout Элементы макетирования
         * \ingroup WindowsWPFControls
         * \brief Элементы макетирования.
         * @{
         */
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Тип размещения элементов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TStackPlacement
		{
			/// <summary>
			/// Последовательное размещение
			/// </summary>
			Series,

			/// <summary>
			/// Распределенное по размеру родительской области
			/// </summary>
			Distributed,

			/// <summary>
			/// Полностью размещенное по всей родительской области
			/// </summary>
			Expanded
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Тип размещения элементов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TStackPanelFill
		{
			/// <summary>
			/// Размер элемента вычисляется самостоятельно
			/// </summary>
			Auto,

			/// <summary>
			/// Элемент заполняет всю доступную область
			/// </summary>
			Fill,

			/// <summary>
			/// Элемент игнорируется при размещении
			/// </summary>
			Ignored
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Макетирующий элемент для последовательного размещения элементов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class LotusStackPanel : Panel, INotifyPropertyChanged
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			//
			// Definitions for dependency properties.
			//
			public static readonly DependencyProperty StackPlacementProperty =
					DependencyProperty.Register(nameof(StackPlacement), typeof(TStackPlacement), typeof(LotusStackPanel),
						new FrameworkPropertyMetadata(TStackPlacement.Series, StackPlacement_PropertyChanged));

			public static readonly DependencyProperty OrientationProperty = 
				DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(LotusStackPanel),
					new FrameworkPropertyMetadata(
					Orientation.Horizontal,
					FrameworkPropertyMetadataOptions.AffectsArrange |
					FrameworkPropertyMetadataOptions.AffectsMeasure));

			public static readonly DependencyProperty MarginBetweenChildrenProperty = 
				DependencyProperty.Register(nameof(MarginBetweenChildren), typeof(Double), typeof(LotusStackPanel),
					new FrameworkPropertyMetadata(0.0,
						FrameworkPropertyMetadataOptions.AffectsArrange |
						FrameworkPropertyMetadataOptions.AffectsMeasure));

			public static readonly DependencyProperty FillProperty = DependencyProperty.RegisterAttached("Fill", 
				typeof(TStackPanelFill), typeof(LotusStackPanel),
				new FrameworkPropertyMetadata(
					TStackPanelFill.Auto,
					FrameworkPropertyMetadataOptions.AffectsArrange |
					FrameworkPropertyMetadataOptions.AffectsMeasure |
					FrameworkPropertyMetadataOptions.AffectsParentArrange |
					FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение тип размещения элементов
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void StackPlacement_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				var stack_panel = (LotusStackPanel)obj;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установки типа размещения элементов
			/// </summary>
			/// <param name="element">Элемент</param>
			/// <param name="value">Тип размещения элемента</param>
			//---------------------------------------------------------------------------------------------------------
			public static void SetFill(DependencyObject element, TStackPanelFill value)
			{
				element.SetValue(FillProperty, value);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение типа размещения элементов
			/// </summary>
			/// <param name="element">Элемент</param>
			/// <returns>Тип размещения элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			public static TStackPanelFill GetFill(DependencyObject element)
			{
				return (TStackPanelFill)element.GetValue(FillProperty);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вычисление общего расстояния между элементами
			/// </summary>
			/// <param name="children">Коллекция дочерних элементов</param>
			/// <param name="margin_between_children">Расстояние между элементами</param>
			/// <returns>Общее расстояния между элементами</returns>
			//---------------------------------------------------------------------------------------------------------
			static Double CalculateTotalMarginToAdd(UIElementCollection children, Double margin_between_children)
			{
				var visibleChildrenCount = children
					.OfType<UIElement>()
					.Count(x => x.Visibility != Visibility.Collapsed && GetFill(x) != TStackPanelFill.Ignored);
				var marginMultiplier = Math.Max(visibleChildrenCount - 1, 0);
				var totalMarginToAdd = margin_between_children * marginMultiplier;
				return totalMarginToAdd;
			}
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Тип размещения элементов
			/// </summary>
			public TStackPlacement StackPlacement
			{
				get { return (TStackPlacement)GetValue(StackPlacementProperty); }
				set { SetValue(StackPlacementProperty, value); }
			}

			/// <summary>
			/// Ориентация элемента
			/// </summary>
			public Orientation Orientation
			{
				get { return (Orientation)GetValue(OrientationProperty); }
				set { SetValue(OrientationProperty, value); }
			}

			/// <summary>
			/// Расстояние между дочерними элементами
			/// </summary>
			public Double MarginBetweenChildren
			{
				get { return (Double)GetValue(MarginBetweenChildrenProperty); }
				set { SetValue(MarginBetweenChildrenProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusStackPanel()
			{

			}
			#endregion

			#region ======================================= ПЕРЕГРУЖЕННЫЕ МЕТОДЫ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Опредилить размеры
			/// </summary>
			/// <remarks>
			/// Метод, который по заданному available_size определяет желаемые размеры и выставляет их в this.DesiredSize.
			/// В описании к методу написано, что результирующий DesiredSize может быть > availableSize, но для наследников FrameworkElement это не так.
			/// </remarks>
			/// <param name="availableSize">Имеющиеся размеры</param>
			/// <returns>Размер элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			protected override Size MeasureOverride(Size availableSize)
			{
				UIElementCollection children = InternalChildren;

				Double parentWidth = 0;
				Double parentHeight = 0;
				Double accumulatedWidth = 0;
				Double accumulatedHeight = 0;

				var isHorizontal = Orientation == Orientation.Horizontal;
				var totalMarginToAdd = CalculateTotalMarginToAdd(children, MarginBetweenChildren);

				for (var i = 0; i < children.Count; i++)
				{
					UIElement child = children[i];

					if (child == null) { continue; }

					// Handle only the Auto's first to calculate remaining space for Fill's
					if (GetFill(child) != TStackPanelFill.Auto) { continue; }

					// Child constraint is the remaining size; this is total size minus size consumed by previous children.
					var childConstraint = new Size(Math.Max(0.0, availableSize.Width - accumulatedWidth),
												   Math.Max(0.0, availableSize.Height - accumulatedHeight));

					// Measure child.
					child.Measure(childConstraint);
					var childDesiredSize = child.DesiredSize;

					if (isHorizontal)
					{
						accumulatedWidth += childDesiredSize.Width;
						parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
					}
					else
					{
						parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
						accumulatedHeight += childDesiredSize.Height;
					}
				}

				// Add all margin to accumulated size before calculating remaining space for
				// Fill elements.
				if (isHorizontal)
				{
					accumulatedWidth += totalMarginToAdd;
				}
				else
				{
					accumulatedHeight += totalMarginToAdd;
				}

				var totalCountOfFillTypes = children
					.OfType<UIElement>()
					.Count(x => GetFill(x) == TStackPanelFill.Fill
							 && x.Visibility != Visibility.Collapsed);

				var availableSpaceRemaining = isHorizontal
					? Math.Max(0, availableSize.Width - accumulatedWidth)
					: Math.Max(0, availableSize.Height - accumulatedHeight);

				var eachFillTypeSize = totalCountOfFillTypes > 0
					? availableSpaceRemaining / totalCountOfFillTypes
					: 0;

				for (var i = 0; i < children.Count; i++)
				{
					UIElement child = children[i];

					if (child == null) { continue; }

					// Handle all the Fill's giving them a portion of the remaining space
					if (GetFill(child) != TStackPanelFill.Fill) { continue; }

					// Child constraint is the remaining size; this is total size minus size consumed by previous children.
					var childConstraint = isHorizontal
						? new Size(eachFillTypeSize,
								   Math.Max(0.0, availableSize.Height - accumulatedHeight))
						: new Size(Math.Max(0.0, availableSize.Width - accumulatedWidth),
								   eachFillTypeSize);

					// Measure child.
					child.Measure(childConstraint);
					var childDesiredSize = child.DesiredSize;

					if (isHorizontal)
					{
						accumulatedWidth += childDesiredSize.Width;
						parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
					}
					else
					{
						parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
						accumulatedHeight += childDesiredSize.Height;
					}
				}

				// Make sure the final accumulated size is reflected in parentSize. 
				parentWidth = Math.Max(parentWidth, accumulatedWidth);
				parentHeight = Math.Max(parentHeight, accumulatedHeight);
				var parent = new Size(parentWidth, parentHeight);

				return parent;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончательно определить размеры
			/// </summary>
			/// <param name="finalSize">Требуемые размеры</param>
			/// <returns>Размер элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			protected override Size ArrangeOverride(Size finalSize)
			{
				UIElementCollection children = InternalChildren;
				var totalChildrenCount = children.Count;

				Double accumulatedLeft = 0;
				Double accumulatedTop = 0;

				var isHorizontal = Orientation == Orientation.Horizontal;
				var marginBetweenChildren = MarginBetweenChildren;

				var totalMarginToAdd = CalculateTotalMarginToAdd(children, marginBetweenChildren);

				var allAutoSizedSum = 0.0;
				var countOfFillTypes = 0;
				foreach (var child in children.OfType<UIElement>())
				{
					var fillType = GetFill(child);
					if (fillType != TStackPanelFill.Auto)
					{
						if (child.Visibility != Visibility.Collapsed && fillType != TStackPanelFill.Ignored)
							countOfFillTypes += 1;
					}
					else
					{
						var desiredSize = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
						allAutoSizedSum += desiredSize;
					}
				}

				var remainingForFillTypes = isHorizontal
					? Math.Max(0, finalSize.Width - allAutoSizedSum - totalMarginToAdd)
					: Math.Max(0, finalSize.Height - allAutoSizedSum - totalMarginToAdd);
				var fillTypeSize = remainingForFillTypes / countOfFillTypes;

				for (var i = 0; i < totalChildrenCount; ++i)
				{
					UIElement child = children[i];
					if (child == null) { continue; }
					Size childDesiredSize = child.DesiredSize;
					var fillType = GetFill(child);
					var isCollapsed = child.Visibility == Visibility.Collapsed || fillType == TStackPanelFill.Ignored;
					var isLastChild = i == totalChildrenCount - 1;
					var marginToAdd = isLastChild || isCollapsed ? 0 : marginBetweenChildren;

					var rcChild = new Rect(
						accumulatedLeft,
						accumulatedTop,
						Math.Max(0.0, finalSize.Width - accumulatedLeft),
						Math.Max(0.0, finalSize.Height - accumulatedTop));

					if (isHorizontal)
					{
						rcChild.Width = fillType == TStackPanelFill.Auto || isCollapsed ? childDesiredSize.Width : fillTypeSize;
						rcChild.Height = finalSize.Height;
						accumulatedLeft += rcChild.Width + marginToAdd;
					}
					else
					{
						rcChild.Width = finalSize.Width;
						rcChild.Height = fillType == TStackPanelFill.Auto || isCollapsed ? childDesiredSize.Height : fillTypeSize;
						accumulatedTop += rcChild.Height + marginToAdd;
					}

					child.Arrange(rcChild);
				}

				return finalSize;
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства.
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(String property_name = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property_name));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства.
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(PropertyChangedEventArgs args)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, args);
				}
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================