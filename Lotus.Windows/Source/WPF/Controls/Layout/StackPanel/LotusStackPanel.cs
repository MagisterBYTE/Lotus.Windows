using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lotus.Windows;

/**
 * \defgroup WindowsWPFControlsLayout Элементы макетирования
 * \ingroup WindowsWPFControls
 * \brief Элементы макетирования.
 * @{
 */
/// <summary>
/// Тип размещения элементов.
/// </summary>
public enum TStackPlacement
{
    /// <summary>
    /// Последовательное размещение.
    /// </summary>
    Series,

    /// <summary>
    /// Распределенное по размеру родительской области.
    /// </summary>
    Distributed,

    /// <summary>
    /// Полностью размещенное по всей родительской области.
    /// </summary>
    Expanded
}

/// <summary>
/// Тип размещения элементов.
/// </summary>
public enum TStackPanelFill
{
    /// <summary>
    /// Размер элемента вычисляется самостоятельно.
    /// </summary>
    Auto,

    /// <summary>
    /// Элемент заполняет всю доступную область.
    /// </summary>
    Fill,

    /// <summary>
    /// Элемент игнорируется при размещении.
    /// </summary>
    Ignored
}

/// <summary>
/// Макетирующий элемент для последовательного размещения элементов.
/// </summary>
public class LotusStackPanel : Panel
{
    #region Declare DependencyProperty
    /// <summary>
    /// Свойство зависимости <see cref="StackPlacement"/>.
    /// </summary>
    public static readonly DependencyProperty StackPlacementProperty =
            DependencyProperty.Register(nameof(StackPlacement), typeof(TStackPlacement), typeof(LotusStackPanel),
                new FrameworkPropertyMetadata(TStackPlacement.Series,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Свойство зависимости <see cref="Orientation"/>.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(LotusStackPanel),
            new FrameworkPropertyMetadata(Orientation.Horizontal,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Свойство зависимости <see cref="MarginBetweenChildren"/>.
    /// </summary>
    public static readonly DependencyProperty MarginBetweenChildrenProperty =
        DependencyProperty.Register(nameof(MarginBetweenChildren), typeof(double), typeof(LotusStackPanel),
            new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Присоединённое свойство зависимости для типа размещения дочернего элемента.
    /// </summary>
    public static readonly DependencyProperty FillProperty = DependencyProperty.RegisterAttached("Fill",
        typeof(TStackPanelFill), typeof(LotusStackPanel),
        new FrameworkPropertyMetadata(TStackPanelFill.Auto,
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsMeasure |
            FrameworkPropertyMetadataOptions.AffectsParentArrange |
            FrameworkPropertyMetadataOptions.AffectsParentMeasure));
    #endregion

    #region DependencyProperty methods
    /// <summary>
    /// Установка типа размещения дочернего элемента.
    /// </summary>
    /// <param name="element">Элемент.</param>
    /// <param name="value">Тип размещения элемента.</param>
    public static void SetFill(DependencyObject element, TStackPanelFill value)
    {
        element.SetValue(FillProperty, value);
    }

    /// <summary>
    /// Получение типа размещения дочернего элемента.
    /// </summary>
    /// <param name="element">Элемент.</param>
    /// <returns>Тип размещения элемента.</returns>
    public static TStackPanelFill GetFill(DependencyObject element)
        => (TStackPanelFill)element.GetValue(FillProperty);

    private static double CalculateTotalMarginToAdd(UIElementCollection children, double marginBetweenChildren)
    {
        var visibleChildrenCount = children
            .OfType<UIElement>()
            .Count(x => x.Visibility != Visibility.Collapsed && GetFill(x) != TStackPanelFill.Ignored);
        return marginBetweenChildren * Math.Max(visibleChildrenCount - 1, 0);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Тип размещения элементов.
    /// </summary>
    public TStackPlacement StackPlacement
    {
        get => (TStackPlacement)GetValue(StackPlacementProperty);
        set => SetValue(StackPlacementProperty, value);
    }

    /// <summary>
    /// Ориентация элемента.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Расстояние между дочерними элементами.
    /// </summary>
    public double MarginBetweenChildren
    {
        get => (double)GetValue(MarginBetweenChildrenProperty);
        set => SetValue(MarginBetweenChildrenProperty, value);
    }
    #endregion

    #region Override methods
    /// <summary>
    /// Определить желаемые размеры.
    /// </summary>
    /// <param name="availableSize">Доступные размеры.</param>
    /// <returns>Желаемый размер элемента.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        var children = InternalChildren;

        double parentWidth = 0;
        double parentHeight = 0;
        double accumulatedWidth = 0;
        double accumulatedHeight = 0;

        var isHorizontal = Orientation == Orientation.Horizontal;
        var totalMarginToAdd = CalculateTotalMarginToAdd(children, MarginBetweenChildren);

        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null) { continue; }
            if (GetFill(child) != TStackPanelFill.Auto) { continue; }

            var childConstraint = new Size(Math.Max(0.0, availableSize.Width - accumulatedWidth),
                                           Math.Max(0.0, availableSize.Height - accumulatedHeight));
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

        if (isHorizontal)
            accumulatedWidth += totalMarginToAdd;
        else
            accumulatedHeight += totalMarginToAdd;

        var totalCountOfFillTypes = children
            .OfType<UIElement>()
            .Count(x => GetFill(x) == TStackPanelFill.Fill && x.Visibility != Visibility.Collapsed);

        var availableSpaceRemaining = isHorizontal
            ? Math.Max(0, availableSize.Width - accumulatedWidth)
            : Math.Max(0, availableSize.Height - accumulatedHeight);

        var eachFillTypeSize = totalCountOfFillTypes > 0
            ? availableSpaceRemaining / totalCountOfFillTypes
            : 0;

        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null) { continue; }
            if (GetFill(child) != TStackPanelFill.Fill) { continue; }

            var childConstraint = isHorizontal
                ? new Size(eachFillTypeSize, Math.Max(0.0, availableSize.Height - accumulatedHeight))
                : new Size(Math.Max(0.0, availableSize.Width - accumulatedWidth), eachFillTypeSize);

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

        parentWidth = Math.Max(parentWidth, accumulatedWidth);
        parentHeight = Math.Max(parentHeight, accumulatedHeight);
        return new Size(parentWidth, parentHeight);
    }

    /// <summary>
    /// Окончательно расположить дочерние элементы.
    /// </summary>
    /// <param name="finalSize">Итоговые размеры.</param>
    /// <returns>Использованный размер элемента.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = InternalChildren;
        var totalChildrenCount = children.Count;

        double accumulatedLeft = 0;
        double accumulatedTop = 0;

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
                allAutoSizedSum += isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
            }
        }

        var remainingForFillTypes = isHorizontal
            ? Math.Max(0, finalSize.Width - allAutoSizedSum - totalMarginToAdd)
            : Math.Max(0, finalSize.Height - allAutoSizedSum - totalMarginToAdd);
        var fillTypeSize = countOfFillTypes > 0 ? remainingForFillTypes / countOfFillTypes : 0;

        for (var i = 0; i < totalChildrenCount; ++i)
        {
            var child = children[i];
            if (child == null) { continue; }

            var childDesiredSize = child.DesiredSize;
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
}
/**@}*/
