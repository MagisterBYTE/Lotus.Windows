using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Lotus.Maths;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsLayout
	*@{*/
    /// <summary>
    /// Основные операции мышью.
    /// </summary>
    public enum TViewHandling
    {
        /// <summary>
        /// Нет специального режима.
        /// </summary>
        None,

        /// <summary>
        /// Перемещение области видимости (средняя кнопка мыши).
        /// </summary>
        Panning,

        /// <summary>
        /// Увеличение/уменьшение (колесико мыши).
        /// </summary>
        Zooming,

        /// <summary>
        /// Увеличение региона (правый шифт + левая кнопка мыши).
        /// </summary>
        ZoomingRegion,

        /// <summary>
        /// Выбор региона.
        /// </summary>
        SelectingRegion,

        /// <summary>
        /// Выбор фигуры.
        /// </summary>
        SelectedShape,

        /// <summary>
        /// Операция пользователя.
        /// </summary>
        UserOperation
    }

    /// <summary>
    /// Основной элемент для управления масштабированием и перемещением контента в области просмотра.
    /// </summary>
    public class LotusContentViewer : ContentControl, IScrollInfo, INotifyPropertyChanged
    {
        #region Static fields
        protected static readonly PropertyChangedEventArgs PropertyArgsOperationDesc = new(nameof(OperationDesc));
        protected static readonly PropertyChangedEventArgs PropertyArgsCanVerticallyScroll = new(nameof(CanVerticallyScroll));
        protected static readonly PropertyChangedEventArgs PropertyArgsCanHorizontallyScroll = new(nameof(CanHorizontallyScroll));
        protected static readonly PropertyChangedEventArgs PropertyArgsExtentWidth = new(nameof(ExtentWidth));
        protected static readonly PropertyChangedEventArgs PropertyArgsExtentHeight = new(nameof(ExtentHeight));
        protected static readonly PropertyChangedEventArgs PropertyArgsViewportWidth = new(nameof(ViewportWidth));
        protected static readonly PropertyChangedEventArgs PropertyArgsViewportHeight = new(nameof(ViewportHeight));
        protected static readonly PropertyChangedEventArgs PropertyArgsHorizontalOffset = new(nameof(HorizontalOffset));
        protected static readonly PropertyChangedEventArgs PropertyArgsVerticalOffset = new(nameof(VerticalOffset));
        #endregion

        #region Declare DependencyProperty 
        //
        // Definitions for dependency properties.
        //
        public static readonly DependencyProperty ContentScaleProperty =
                DependencyProperty.Register("ContentScale", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(1.0, ContentScale_PropertyChanged, ContentScale_Coerce));

        public static readonly DependencyProperty MinContentScaleProperty =
                DependencyProperty.Register("MinContentScale", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.01, MinOrMaxContentScale_PropertyChanged));

        public static readonly DependencyProperty MaxContentScaleProperty =
                DependencyProperty.Register("MaxContentScale", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(10.0, MinOrMaxContentScale_PropertyChanged));

        public static readonly DependencyProperty ContentOffsetXProperty =
                DependencyProperty.Register("ContentOffsetX", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.0, ContentOffsetX_PropertyChanged, ContentOffsetX_Coerce));

        public static readonly DependencyProperty ContentOffsetYProperty =
                DependencyProperty.Register("ContentOffsetY", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.0, ContentOffsetY_PropertyChanged, ContentOffsetY_Coerce));

        public static readonly DependencyProperty AnimationDurationProperty =
                DependencyProperty.Register("AnimationDuration", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.4));

        public static readonly DependencyProperty ContentZoomFocusXProperty =
                DependencyProperty.Register("ContentZoomFocusX", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ContentZoomFocusYProperty =
                DependencyProperty.Register("ContentZoomFocusY", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ViewportZoomFocusXProperty =
                DependencyProperty.Register("ViewportZoomFocusX", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ViewportZoomFocusYProperty =
                DependencyProperty.Register("ViewportZoomFocusY", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ContentViewportWidthProperty =
                DependencyProperty.Register("ContentViewportWidth", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty ContentViewportHeightProperty =
                DependencyProperty.Register("ContentViewportHeight", typeof(double), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty =
                DependencyProperty.Register("IsMouseWheelScrollingEnabled", typeof(bool), typeof(LotusContentViewer),
                                            new FrameworkPropertyMetadata(false));
        #endregion

        #region DependencyProperty methods
        /// <summary>
        /// Статический конструктор.
        /// </summary>
        static LotusContentViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LotusContentViewer), new FrameworkPropertyMetadata(typeof(LotusContentViewer)));
        }

        /// <summary>
        /// Изменение масштаба.
        /// </summary>
        /// <param name="obj">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void ContentScale_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var content_viewer = (LotusContentViewer)obj;
            content_viewer.OnContentViewerContentScaleChanged();

            if (content_viewer._contentScaleTransform is not null)
            {
                //
                // Update the _content scale transform whenever 'ContentScale' changes.
                //
                content_viewer._contentScaleTransform.ScaleX = content_viewer.ContentScale;
                content_viewer._contentScaleTransform.ScaleY = content_viewer.ContentScale;
            }

            //
            // Update the size of the viewport in _content coordinates.
            //
            content_viewer.UpdateContentViewportSize();

            if (content_viewer._enableContentOffsetUpdateFromScale)
            {
                try
                {
                    // 
                    // Disable _content focus syncronization.  We are about to update _content offset whilst zooming
                    // to ensure that the viewport is focused on our desired _content focus point.  Setting this
                    // to 'true' stops the automatic update of the _content focus when _content offset changes.
                    //
                    content_viewer._disableContentFocusSync = true;

                    //
                    // Whilst zooming in or out keep the _content offset up-to-date so that the viewport is always
                    // focused on the _content focus point (and also so that the _content focus is locked to the 
                    // viewport focus point - this is how the google maps style zooming works).
                    //
                    var viewportOffsetX = content_viewer.ViewportZoomFocusX - (content_viewer.ViewportWidth / 2);
                    var viewportOffsetY = content_viewer.ViewportZoomFocusY - (content_viewer.ViewportHeight / 2);
                    var contentOffsetX = viewportOffsetX / content_viewer.ContentScale;
                    var contentOffsetY = viewportOffsetY / content_viewer.ContentScale;
                    content_viewer.ContentOffsetX = content_viewer.ContentZoomFocusX - (content_viewer.ContentViewportWidth / 2) - contentOffsetX;
                    content_viewer.ContentOffsetY = content_viewer.ContentZoomFocusY - (content_viewer.ContentViewportHeight / 2) - contentOffsetY;
                }
                finally
                {
                    content_viewer._disableContentFocusSync = false;
                }
            }

            if (content_viewer.ContentScaleChanged is not null)
            {
                content_viewer.ContentScaleChanged(content_viewer, EventArgs.Empty);
            }

            content_viewer._scrollOwner?.InvalidateScrollInfo();

            content_viewer.NotifyPropertyChanged(PropertyArgsExtentWidth);
            content_viewer.NotifyPropertyChanged(PropertyArgsExtentHeight);
            content_viewer.NotifyPropertyChanged(PropertyArgsHorizontalOffset);
            content_viewer.NotifyPropertyChanged(PropertyArgsVerticalOffset);
        }

        /// <summary>
        /// Корректировка масштаба.
        /// </summary>
        /// <param name="obj">Источник события.</param>
        /// <param name="base_value">Базовое значение.</param>
        /// <returns>Скорректированное значение.</returns>
        private static object ContentScale_Coerce(DependencyObject obj, object base_value)
        {
            var c = (LotusContentViewer)obj;
            var value = (double)base_value;
            value = Math.Min(Math.Max(value, c.MinContentScale), c.MaxContentScale);
            return value;
        }

        /// <summary>
        /// Изменение минимального/максимального масштаба.
        /// </summary>
        /// <param name="obj">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void MinOrMaxContentScale_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var c = (LotusContentViewer)obj;
            c.ContentScale = Math.Min(Math.Max(c.ContentScale, c.MinContentScale), c.MaxContentScale);
        }

        /// <summary>
        /// Изменение смещения контента по X.
        /// </summary>
        /// <param name="obj">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void ContentOffsetX_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var content_viewer = (LotusContentViewer)obj;
            content_viewer.OnContentViewerContentOffsetChanged();
            content_viewer.UpdateTranslationX();

            if (!content_viewer._disableContentFocusSync)
            {
                //
                // Normally want to automatically update _content focus when _content offset changes.
                // Although this is disabled using 'disableContentFocusSync' when _content offset changes due to in-progress zooming.
                //
                content_viewer.UpdateContentZoomFocusX();
            }

            if (content_viewer.ContentOffsetXChanged is not null)
            {
                //
                // Raise an event to let users of the control know that the _content offset has changed.
                //
                content_viewer.ContentOffsetXChanged(content_viewer, EventArgs.Empty);
            }

            if (!content_viewer._disableScrollOffsetSync && content_viewer._scrollOwner is not null)
            {
                //
                // Notify the owning ScrollViewer that the scrollbar offsets should be updated.
                //
                content_viewer._scrollOwner.InvalidateScrollInfo();
            }

            content_viewer.NotifyPropertyChanged(PropertyArgsHorizontalOffset);
        }

        /// <summary>
        /// Корректировка смещения контента по X.
        /// </summary>
        /// <param name="obj">Источник события.</param>
        /// <param name="base_value">Базовое значение.</param>
        /// <returns>Скорректированное значение.</returns>
        private static object ContentOffsetX_Coerce(DependencyObject obj, object base_value)
        {
            var c = (LotusContentViewer)obj;
            var value = (double)base_value;
            var min_offset_x = 0.0;
            var max_offset_x = Math.Max(0.0, c._unScaledExtent.Width - c._constrainedContentViewportWidth);
            value = Math.Min(Math.Max(value, min_offset_x), max_offset_x);
            return value;
        }

        /// <summary>
        /// Изменение смещения контента по Y.
        /// </summary>
        /// <param name="obj">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void ContentOffsetY_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var content_viewer = (LotusContentViewer)obj;
            content_viewer.OnContentViewerContentOffsetChanged();
            content_viewer.UpdateTranslationY();

            if (!content_viewer._disableContentFocusSync)
            {
                //
                // Normally want to automatically update _content focus when _content offset changes.
                // Although this is disabled using 'disableContentFocusSync' when _content offset changes due to in-progress zooming.
                //
                content_viewer.UpdateContentZoomFocusY();
            }

            if (content_viewer.ContentOffsetYChanged is not null)
            {
                //
                // Raise an event to let users of the control know that the _content offset has changed.
                //
                content_viewer.ContentOffsetYChanged(content_viewer, EventArgs.Empty);
            }

            if (!content_viewer._disableScrollOffsetSync && content_viewer._scrollOwner is not null)
            {
                //
                // Notify the owning ScrollViewer that the scrollbar offsets should be updated.
                //
                content_viewer._scrollOwner.InvalidateScrollInfo();
            }

            content_viewer.NotifyPropertyChanged(PropertyArgsVerticalOffset);
        }

        /// <summary>
        /// Корректировка смещения контента по Y.
        /// </summary>
        /// <param name="obj">Источник события.</param>
        /// <param name="base_value">Базовое значение.</param>
        /// <returns>Скорректированное значение.</returns>
        private static object ContentOffsetY_Coerce(DependencyObject obj, object base_value)
        {
            var c = (LotusContentViewer)obj;
            var value = (double)base_value;
            var min_offset_y = 0.0;
            var max_offset_y = Math.Max(0.0, c._unScaledExtent.Height - c._constrainedContentViewportHeight);
            value = Math.Min(Math.Max(value, min_offset_y), max_offset_y);
            return value;
        }
        #endregion

        #region Fields
        // Основное содержимое
        protected FrameworkElement? _content;

        // Перемещение и масштабирование
        protected ScaleTransform? _contentScaleTransform;
        protected TranslateTransform? _contentOffsetTransform;
        protected TransformGroup? _contentTotalTransform;
        protected bool _enableContentOffsetUpdateFromScale = false;
        protected bool _disableScrollOffsetSync = false;
        protected bool _disableContentFocusSync = false;
        protected double _constrainedContentViewportWidth = 0.0;
        protected double _constrainedContentViewportHeight = 0.0;

        // Поддержка скроллинга
        protected ScrollViewer? _scrollOwner;
        protected bool _canVerticallyScroll = false;
        protected bool _canHorizontallyScroll = false;
        protected Size _unScaledExtent = new(0, 0);
        protected Size _viewportScroll = new(0, 0);

        // Операции
        protected TViewHandling _operationCurrent;  // Текущая операция
        protected TViewHandling _operationPreview;  // Предыдущая операция
        protected string _operationDesc = string.Empty; // Описание операции

        // Прямоугольник увеличение области канвы
        protected bool _zoomingIsSupport = true;
        protected bool _zoomingStarting = false;
        protected Point _zoomingStartPoint;
        protected Vector _zoomingLeftUpPoint;
        protected Rect _zoomingRect;
        protected float _zoomingDragCorrect = 10;
        protected Rect _zoomingRectCorrect;

        // Выбор региона
        protected bool _selectingIsSupport = true;
        protected bool _selectingStarting = false;
        protected Point _selectingStartPoint;
        protected Vector _selectingLeftUpPoint;
        protected bool _selectingRightToLeft;
        protected Rect _selectingRect;
        protected float _selectingDragCorrect = 10;
        protected Rect _selectingRectCorrect;

        // Координаты курсора
        public Vector2Df MousePositionLeftDown { get; set; }
        public Vector2Df MousePositionRightDown { get; set; }
        public Vector2Df MousePositionMiddleDown { get; set; }
        public Vector2Df MousePositionCurrent { get; set; }
        public Vector2Df MouseDeltaCurrent { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Статус нахождения компонента в режиме разработки.
        /// </summary>
        public static bool IsDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                var is_design_mode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                return is_design_mode;
            }
        }

        //
        // ПЕРЕМЕЩЕНИЕ И МАСШТАБИРОВАНИЕ
        //
        /// <summary>
        /// Смещение контента по X.
        /// </summary>
        [Description("Смещение области просмотра по X в координатах контента")]
        public double ContentOffsetX
        {
            get { return (double)GetValue(ContentOffsetXProperty); }
            set { SetValue(ContentOffsetXProperty, value); }
        }

        /// <summary>
        /// События изменения смещения контента по X.
        /// </summary>
        public event EventHandler? ContentOffsetXChanged;

        /// <summary>
        /// Смещение контента по Y.
        /// </summary>
        [Description("Смещение области просмотра по Y в координатах контента")]
        public double ContentOffsetY
        {
            get { return (double)GetValue(ContentOffsetYProperty); }
            set { SetValue(ContentOffsetYProperty, value); }
        }

        /// <summary>
        /// События изменения смещения контента по Y.
        /// </summary>
        public event EventHandler? ContentOffsetYChanged;

        /// <summary>
        /// Масштаб контента.
        /// </summary>
        public double ContentScale
        {
            get { return (double)GetValue(ContentScaleProperty); }
            set { SetValue(ContentScaleProperty, value); }
        }

        /// <summary>
        /// События изменения масштаба.
        /// </summary>
        public event EventHandler? ContentScaleChanged;

        /// <summary>
        /// Минимальное значение масштаба контента.
        /// </summary>
        public double MinContentScale
        {
            get { return (double)GetValue(MinContentScaleProperty); }
            set { SetValue(MinContentScaleProperty, value); }
        }

        /// <summary>
        /// Максимальное значение масштаба.
        /// </summary>
        public double MaxContentScale
        {
            get { return (double)GetValue(MaxContentScaleProperty); }
            set { SetValue(MaxContentScaleProperty, value); }
        }

        /// <summary>
        /// Координата по X контента точки фокуса при масштабировании.
        /// </summary>
        public double ContentZoomFocusX
        {
            get { return (double)GetValue(ContentZoomFocusXProperty); }
            set { SetValue(ContentZoomFocusXProperty, value); }
        }

        /// <summary>
        /// Координата по Y контента точки фокуса при масштабировании.
        /// </summary>
        public double ContentZoomFocusY
        {
            get { return (double)GetValue(ContentZoomFocusYProperty); }
            set { SetValue(ContentZoomFocusYProperty, value); }
        }

        /// <summary>
        /// Координата по X области просмотра точки фокуса при масштабировании.
        /// </summary>
        public double ViewportZoomFocusX
        {
            get { return (double)GetValue(ViewportZoomFocusXProperty); }
            set { SetValue(ViewportZoomFocusXProperty, value); }
        }

        /// <summary>
        /// Координата по Y области просмотра точки фокуса при масштабировании.
        /// </summary>
        public double ViewportZoomFocusY
        {
            get { return (double)GetValue(ViewportZoomFocusYProperty); }
            set { SetValue(ViewportZoomFocusYProperty, value); }
        }

        /// <summary>
        /// Время анимации при эффектах масштабирования.
        /// </summary>
        public double AnimationDuration
        {
            get { return (double)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        /// <summary>
        /// Ширина области просмотра в координатах контента.
        /// </summary>
        public double ContentViewportWidth
        {
            get { return (double)GetValue(ContentViewportWidthProperty); }
            set { SetValue(ContentViewportWidthProperty, value); }
        }

        /// <summary>
        /// Высота области просмотра в координатах контента.
        /// </summary>
        public double ContentViewportHeight
        {
            get { return (double)GetValue(ContentViewportHeightProperty); }
            set { SetValue(ContentViewportHeightProperty, value); }
        }

        /// <summary>
        /// Возможность прокрутки области просмотра колесом мыши.
        /// </summary>
        public bool IsMouseWheelScrollingEnabled
        {
            get { return (bool)GetValue(IsMouseWheelScrollingEnabledProperty); }
            set { SetValue(IsMouseWheelScrollingEnabledProperty, value); }
        }

        //
        // ПОДДЕРЖКА СКРОЛЛИНГА ScrollViewer
        //
        /// <summary>
        /// Элемент ScrollViewer.
        /// </summary>
        public ScrollViewer ScrollOwner
        {
            get { return _scrollOwner; }
            set { _scrollOwner = value; }
        }

        /// <summary>
        /// Возможность вертикальной прокрутки.
        /// </summary>
        public bool CanVerticallyScroll
        {
            get { return _canVerticallyScroll; }
            set
            {
                _canVerticallyScroll = value;
                NotifyPropertyChanged(PropertyArgsCanVerticallyScroll);
            }
        }

        /// <summary>
        /// Возможность горизонтальной прокрутки.
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get { return _canHorizontallyScroll; }
            set
            {
                _canHorizontallyScroll = value;
                NotifyPropertyChanged(PropertyArgsCanHorizontallyScroll);
            }
        }

        /// <summary>
        /// Горизонтальный размер контента с учетом масштаба.
        /// </summary>
        public double ExtentWidth
        {
            get { return _unScaledExtent.Width * ContentScale; }
        }

        /// <summary>
        /// Вертикальный размер контента с учетом масштаба.
        /// </summary>
        public double ExtentHeight
        {
            get { return _unScaledExtent.Height * ContentScale; }
        }

        /// <summary>
        /// Горизонтальный размер окна просмотра для данного содержимого.
        /// </summary>
        public double ViewportWidth
        {
            get { return _viewportScroll.Width; }
        }

        /// <summary>
        /// Вертикальный размер окна просмотра для данного содержимого.
        /// </summary>
        public double ViewportHeight
        {
            get { return _viewportScroll.Height; }
        }

        /// <summary>
        /// Горизонтальное смещение прокручиваемого содержимого.
        /// </summary>
        public double HorizontalOffset
        {
            get { return ContentOffsetX * ContentScale; }
        }

        /// <summary>
        /// Вертикальное смещение прокручиваемого содержимого.
        /// </summary>
        public double VerticalOffset
        {
            get { return ContentOffsetY * ContentScale; }
        }

        //
        // ОПЕРАЦИИ
        //
        /// <summary>
        /// Текущая операция мышью.
        /// </summary>
        public TViewHandling OperationCurrent
        {
            get { return _operationCurrent; }
        }

        /// <summary>
        /// Предыдущая операция мышью.
        /// </summary>
        public TViewHandling OperationPreview
        {
            get { return _operationPreview; }
        }

        /// <summary>
        /// Описание текущей операции.
        /// </summary>
        public string OperationDesc
        {
            get { return _operationDesc; }
        }

        //
        // УВЕЛИЧЕНИЕ РЕГИОНА
        //
        /// <summary>
        /// Возможность увеличение прямоугольной области.
        /// </summary>
        [Description("Возможность увеличение прямоугольной области")]
        public bool ZoomingIsSupport
        {
            get { return _zoomingIsSupport; }
            set { _zoomingIsSupport = value; }
        }

        /// <summary>
        /// Начало увеличение прямоугольной области.
        /// </summary>
        public bool ZoomingStarting
        {
            get { return _zoomingStarting; }
        }

        /// <summary>
        /// Минимальное смещение для увеличения области.
        /// </summary>
        [Description("Минимальное смещение для увеличения области")]
        public float ZoomingDragCorrect
        {
            get { return _zoomingDragCorrect; }
            set { _zoomingDragCorrect = value; }
        }

        /// <summary>
        /// Текущий прямоугольник увеличение региона.
        /// </summary>
        public Rect ZoomingRect
        {
            get { return _zoomingRect; }
        }

        //
        // ВЫБОР РЕГИОНА
        //
        /// <summary>
        /// Возможность выбора прямоугольной области.
        /// </summary>
        [Description("Возможность выбора прямоугольной области")]
        public bool SelectingIsSupport
        {
            get { return _selectingIsSupport; }
            set { _selectingIsSupport = value; }
        }

        /// <summary>
        /// Минимальное смещение для выбора области.
        /// </summary>
        [Description("Минимальное смещение для выбора области")]
        public float SelectingDragCorrect
        {
            get { return _selectingDragCorrect; }
            set { _selectingDragCorrect = value; }
        }

        /// <summary>
        /// Выбора области справа налево.
        /// </summary>
        public bool SelectingRightToLeft
        {
            get { return _selectingRightToLeft; }
            set { _selectingRightToLeft = value; }
        }

        /// <summary>
        /// Текущий прямоугольник выбора региона.
        /// </summary>
        public Rect SelectingRect
        {
            get { return _selectingRect; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusContentViewer()
        {
            this.Loaded += OnContentViewerLoaded;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="content">Элемент контент.</param>
        public LotusContentViewer(FrameworkElement content)
        {
            Content = content;
            this.Loaded += OnContentViewerLoaded;
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Инициализация данных трансформации.
        /// </summary>
        public void InitContentTransformation()
        {
            //
            // Setup the transform on the _content so that we can scale it by 'ContentScale'.
            //
            this._contentScaleTransform = new ScaleTransform(this.ContentScale, this.ContentScale);

            //
            // Setup the transform on the _content so that we can translate it by 'ContentOffsetX' and 'ContentOffsetY'.
            //
            this._contentOffsetTransform = new TranslateTransform();
            UpdateTranslationX();
            UpdateTranslationY();

            //
            // Setup a transform group to contain the translation and scale transforms, and then
            // assign this to the _content's 'RenderTransform'.
            //
            _contentTotalTransform = new TransformGroup();
            _contentTotalTransform.Children.Add(this._contentOffsetTransform);
            _contentTotalTransform.Children.Add(this._contentScaleTransform);
            if (_content == null)
            {
                _content = Content as FrameworkElement;
                if (_content is not null)
                {
                    _content.RenderTransform = _contentTotalTransform;
                }
            }
            else
            {
                _content.RenderTransform = _contentTotalTransform;
            }
        }

        /// <summary>
        /// Увеличение в заданном масштабе и перемещение указанную точку фокуса до центра окна просмотра.
        /// </summary>
        /// <param name="new_сontent_scale">Новый масштаб.</param>
        /// <param name="content_zoom_focus">Точка масштабирования в координатах контента.</param>
        /// <param name="callback">Метод обратного вызова.</param>
        private void AnimatedZoomPointToViewportCenter(double new_сontent_scale, Point content_zoom_focus, EventHandler callback)
        {
            new_сontent_scale = Math.Min(Math.Max(new_сontent_scale, MinContentScale), MaxContentScale);

            XAnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
            XAnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
            XAnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
            XAnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

            ContentZoomFocusX = content_zoom_focus.X;
            ContentZoomFocusY = content_zoom_focus.Y;
            ViewportZoomFocusX = (ContentZoomFocusX - ContentOffsetX) * ContentScale;
            ViewportZoomFocusY = (ContentZoomFocusY - ContentOffsetY) * ContentScale;

            //
            // When zooming about a point make updates to ContentScale also update _content offset.
            //
            _enableContentOffsetUpdateFromScale = true;

            XAnimationHelper.StartAnimation(this, ContentScaleProperty, new_сontent_scale, AnimationDuration,
                delegate (object sender, EventArgs args)
                {
                    _enableContentOffsetUpdateFromScale = false;

                    if (callback is not null)
                    {
                        callback(this, EventArgs.Empty);
                    }
                });

            XAnimationHelper.StartAnimation(this, ViewportZoomFocusXProperty, ViewportWidth / 2, AnimationDuration);
            XAnimationHelper.StartAnimation(this, ViewportZoomFocusYProperty, ViewportHeight / 2, AnimationDuration);
        }

        /// <summary>
        /// Увеличение в заданном масштабе и перемещение указанную точку фокуса до центра окна просмотра.
        /// </summary>
        /// <param name="new_сontent_scale">Новый масштаб.</param>
        /// <param name="content_zoom_focus">Точка масштабирования в координатах контента.</param>
        private void ZoomPointToViewportCenter(double new_сontent_scale, Point content_zoom_focus)
        {
            new_сontent_scale = Math.Min(Math.Max(new_сontent_scale, MinContentScale), MaxContentScale);

            XAnimationHelper.CancelAnimation(this, ContentScaleProperty);
            XAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            XAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            ContentScale = new_сontent_scale;
            ContentOffsetX = content_zoom_focus.X - (ContentViewportWidth / 2);
            ContentOffsetY = content_zoom_focus.Y - (ContentViewportHeight / 2);
        }

        /// <summary>
        /// Сброс фокус видовой экрана в центре области просмотра.
        /// </summary>
        private void ResetViewportZoomFocus()
        {
            ViewportZoomFocusX = ViewportWidth / 2;
            ViewportZoomFocusY = ViewportHeight / 2;
        }

        /// <summary>
        /// Обновление размера области просмотра от заданного размера.
        /// </summary>
        /// <param name="new_size">Новый размер видового экрана.</param>
        private void UpdateViewportSize(Size new_size)
        {
            if (_viewportScroll == new_size)
            {
                //
                // The viewport is already the specified size.
                //
                return;
            }

            _viewportScroll = new_size;

            //
            // Update the viewport size in _content coordiates.
            //
            UpdateContentViewportSize();

            //
            // Initialise the _content zoom focus point.
            //
            UpdateContentZoomFocusX();
            UpdateContentZoomFocusY();

            //
            // Reset the viewport zoom focus to the center of the viewport.
            //
            ResetViewportZoomFocus();

            //
            // Update _content offset from itself when the size of the viewport changes.
            // This ensures that the _content offset remains properly clamped to its valid range.
            //
            // ContentOffsetX = ContentOffsetX;
            // ContentOffsetY = ContentOffsetY;

            //
            // Tell that owning ScrollViewer that scrollbar data has changed.
            //
            _scrollOwner?.InvalidateScrollInfo();
        }

        /// <summary>
        /// Обновление размера области просмотра вследствие изменения масштаба или размеров контента.
        /// </summary>
        private void UpdateContentViewportSize()
        {
            ContentViewportWidth = ViewportWidth / ContentScale;
            ContentViewportHeight = ViewportHeight / ContentScale;

            _constrainedContentViewportWidth = Math.Min(ContentViewportWidth, _unScaledExtent.Width);
            _constrainedContentViewportHeight = Math.Min(ContentViewportHeight, _unScaledExtent.Height);

            UpdateTranslationX();
            UpdateTranslationY();
        }

        /// <summary>
        /// Обновление координаты Х трансформации смещения контента.
        /// </summary>
        private void UpdateTranslationX()
        {
            var scaled_сontent_width = _unScaledExtent.Width * ContentScale;
            if (scaled_сontent_width < ViewportWidth)
            {
                //
                // Когда содержание может поместиться целиком внутри окна просмотра, то перемещаем в центр
                //
                _contentOffsetTransform!.X = (ContentViewportWidth - _unScaledExtent.Width) / 2;
            }
            else
            {
                _contentOffsetTransform!.X = -ContentOffsetX;
            }
        }

        /// <summary>
        ///  Обновление координаты Y трансформации смещения контента.
        /// </summary>
        private void UpdateTranslationY()
        {
            var scaled_content_height = _unScaledExtent.Height * ContentScale;
            if (scaled_content_height < ViewportHeight)
            {
                //
                // Когда содержание может поместиться целиком внутри окна просмотра, то перемещаем в центр
                //
                _contentOffsetTransform!.Y = (ContentViewportHeight - _unScaledExtent.Height) / 2;
            }
            else
            {
                _contentOffsetTransform!.Y = -ContentOffsetY;
            }
        }

        /// <summary>
        /// Обновление X координаты точки фокусировки области просмотра.
        /// </summary>
        private void UpdateContentZoomFocusX()
        {
            ContentZoomFocusX = ContentOffsetX + (_constrainedContentViewportWidth / 2);
        }

        /// <summary>
        /// Обновление Y координаты точки фокусировки области просмотра.
        /// </summary>
        private void UpdateContentZoomFocusY()
        {
            ContentZoomFocusY = ContentOffsetY + (_constrainedContentViewportHeight / 2);
        }
        #endregion

        #region IScrollInfo methods
        /// <summary>
        /// Величина горизонтальной прокрутки.
        /// </summary>
        /// <param name="offset">Величина, на которую содержимое смещается по горизонтали от окна просмотра.</param>
        public void SetHorizontalOffset(double offset)
        {
            if (_disableScrollOffsetSync)
            {
                return;
            }

            try
            {
                _disableScrollOffsetSync = true;

                ContentOffsetX = offset / ContentScale;
            }
            finally
            {
                _disableScrollOffsetSync = false;
            }
        }

        /// <summary>
        /// Величина вертикальной прокрутки.
        /// </summary>
        /// <param name="offset">Величина, на которую содержимое смещается по вертикали от окна просмотра.</param>
        public void SetVerticalOffset(double offset)
        {
            if (_disableScrollOffsetSync)
            {
                return;
            }

            try
            {
                _disableScrollOffsetSync = true;

                ContentOffsetY = offset / ContentScale;
            }
            finally
            {
                _disableScrollOffsetSync = false;
            }
        }

        /// <summary>
        /// Прокрутку вверх в содержимом на одну логическую единицу.
        /// </summary>
        public void LineUp()
        {
            ContentOffsetY -= ContentViewportHeight / 10;
        }

        /// <summary>
        /// Прокрутку вниз в содержимом на одну логическую единицу.
        /// </summary>
        public void LineDown()
        {
            ContentOffsetY += ContentViewportHeight / 10;
        }

        /// <summary>
        /// Прокрутка влево в содержимом на одну логическую единицу.
        /// </summary>
        public void LineLeft()
        {
            ContentOffsetX -= ContentViewportWidth / 10;
        }

        /// <summary>
        /// Прокрутка вправо в содержимом на одну логическую единицу.
        /// </summary>
        public void LineRight()
        {
            ContentOffsetX += ContentViewportWidth / 10;
        }

        /// <summary>
        /// Прокрутка вверх в содержимом на одну логическую страницу.
        /// </summary>
        public void PageUp()
        {
            ContentOffsetY -= ContentViewportHeight;
        }

        /// <summary>
        /// Прокрутка вниз в содержимом на одну логическую страницу.
        /// </summary>
        public void PageDown()
        {
            ContentOffsetY += ContentViewportHeight;
        }

        /// <summary>
        /// Прокрутка влево в содержимом на одну логическую страницу.
        /// </summary>
        public void PageLeft()
        {
            ContentOffsetX -= ContentViewportWidth;
        }

        /// <summary>
        /// Прокрутка вправо в содержимом на одну логическую страницу.
        /// </summary>
        public void PageRight()
        {
            ContentOffsetX += ContentViewportWidth;
        }

        /// <summary>
        /// Прокручивает содержимое вниз после нажатия пользователем колесика мыши.
        /// </summary>
        public void MouseWheelDown()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineDown();
            }
        }

        /// <summary>
        /// Прокручивает содержимое влево после нажатия пользователем колесика мыши.
        /// </summary>
        public void MouseWheelLeft()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineLeft();
            }
        }

        /// <summary>
        /// Прокручивает содержимое вправо после нажатия пользователем колесика мыши.
        /// </summary>
        public void MouseWheelRight()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineRight();
            }
        }

        /// <summary>
        /// Прокручивает содержимое вверх после нажатия пользователем колесика мыши.
        /// </summary>
        public void MouseWheelUp()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineUp();
            }
        }

        /// <summary>
        /// Принудительно прокручивание содержимое пока координатное пространство объекта Visual не станет видимым.
        /// </summary>
        /// <param name="visual">Объект который становится видимым.</param>
        /// <param name="rectangle">Ограничивающий прямоугольник, идентифицирующий пространство координат, которое необходимо сделать видимым.</param>
        /// <returns>Прямоугольник который является видимым.</returns>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (_content!.IsAncestorOf(visual))
            {
                var transformedRect = visual.TransformToAncestor(_content).TransformBounds(rectangle);
                var viewportRect = new Rect(ContentOffsetX, ContentOffsetY, ContentViewportWidth, ContentViewportHeight);
                if (!transformedRect.Contains(viewportRect))
                {
                    double horizOffset = 0;
                    double vertOffset = 0;

                    if (transformedRect.Left < viewportRect.Left)
                    {
                        //
                        // Want to move viewport left.
                        //
                        horizOffset = transformedRect.Left - viewportRect.Left;
                    }
                    else if (transformedRect.Right > viewportRect.Right)
                    {
                        //
                        // Want to move viewport right.
                        //
                        horizOffset = transformedRect.Right - viewportRect.Right;
                    }

                    if (transformedRect.Top < viewportRect.Top)
                    {
                        //
                        // Want to move viewport up.
                        //
                        vertOffset = transformedRect.Top - viewportRect.Top;
                    }
                    else if (transformedRect.Bottom > viewportRect.Bottom)
                    {
                        //
                        // Want to move viewport down.
                        //
                        vertOffset = transformedRect.Bottom - viewportRect.Bottom;
                    }

                    SnapContentOffsetTo(new Point(ContentOffsetX + horizOffset, ContentOffsetY + vertOffset));
                }
            }
            return rectangle;
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Выполняет построение визуального дерева текущего шаблона (если это необходимо) и возвращает значение,.
        /// указывающее, было ли визуальное дерево перестроено данным вызовом.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _content = this.Template.FindName("PART_Content", this) as FrameworkElement;
            if (_content is not null)
            {
                InitContentTransformation();
            }
        }

        /// <summary>
        /// Определить размеры.
        /// </summary>
        /// <remarks>
        /// Метод, который по заданному available_size определяет желаемые размеры и выставляет их в this.DesiredSize.
        /// В описании к методу написано, что результирующий DesiredSize может быть > availableSize, но для наследников FrameworkElement это не так.
        /// </remarks>
        /// <param name="constraint">Имеющиеся размеры.</param>
        /// <returns>Размер элемента.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (_content == null)
            {
                _content = this.Content as FrameworkElement;
            }

            if (this._contentScaleTransform == null)
            {
                InitContentTransformation();
            }

            var infinite_size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            var child_size = base.MeasureOverride(infinite_size);

            if (child_size != _unScaledExtent)
            {
                //
                // Use the size of the child as the un-scaled extent _content.
                //
                _unScaledExtent = child_size;

                _scrollOwner?.InvalidateScrollInfo();
            }

            //
            // Update the size of the viewport onto the _content based on the passed in 'constraint'.
            //
            UpdateViewportSize(constraint);

            var width = constraint.Width;
            var height = constraint.Height;

            if (double.IsInfinity(width))
            {
                //
                // Make sure we don't return infinity!
                //
                width = child_size.Width;
            }

            if (double.IsInfinity(height))
            {
                //
                // Make sure we don't return infinity!
                //
                height = child_size.Height;
            }

            UpdateTranslationX();
            UpdateTranslationY();

            NotifyPropertyChanged(PropertyArgsExtentWidth);
            NotifyPropertyChanged(PropertyArgsExtentHeight);
            NotifyPropertyChanged(PropertyArgsViewportHeight);
            NotifyPropertyChanged(PropertyArgsViewportWidth);

            return new Size(width, height);
        }

        /// <summary>
        /// Окончательно определить размеры.
        /// </summary>
        /// <param name="arrangeBounds">Требуемые размеры.</param>
        /// <returns>Размер элемента.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(this.DesiredSize);

            if (_content == null)
            {
                _content = this.Content as FrameworkElement;
            }

            if (_content!.DesiredSize != _unScaledExtent)
            {
                //
                // Use the size of the child as the un-scaled extent _content.
                //
                _unScaledExtent = _content.DesiredSize;

                _scrollOwner?.InvalidateScrollInfo();
            }

            //
            // Update the size of the viewport onto the _content based on the passed in 'arrangeBounds'.
            //
            UpdateViewportSize(arrangeBounds);

            NotifyPropertyChanged(PropertyArgsExtentWidth);
            NotifyPropertyChanged(PropertyArgsExtentHeight);
            NotifyPropertyChanged(PropertyArgsViewportHeight);
            NotifyPropertyChanged(PropertyArgsViewportWidth);

            return size;
        }
        #endregion

        #region Animate methods
        /// <summary>
        /// Анимация масштабирования указанной области контента.
        /// </summary>
        /// <param name="newScale">Масштаб.</param>
        /// <param name="contentRect">Прямоугольник области контента.</param>
        public void AnimatedZoomTo(double newScale, Rect contentRect)
        {
            AnimatedZoomPointToViewportCenter(newScale, new Point(contentRect.X + (contentRect.Width / 2), contentRect.Y + (contentRect.Height / 2)),
                delegate (object sender, EventArgs args)
                {
                    //
                    // At the end of the animation, ensure that we are snapped to the specified _content offset.
                    // Due to zooming in on the _content focus point and rounding errors, the _content offset may
                    // be slightly off what we want at the end of the animation and this bit of code corrects it.
                    //
                    ContentOffsetX = contentRect.X;
                    ContentOffsetY = contentRect.Y;
                });
        }

        /// <summary>
        /// Анимация масштабирования указанной области контента.
        /// </summary>
        /// <param name="contentRect">Прямоугольник области контента.</param>
        public void AnimatedZoomTo(Rect contentRect)
        {
            var scale_x = ContentViewportWidth / contentRect.Width;
            var scale_y = ContentViewportHeight / contentRect.Height;
            var new_scale = ContentScale * Math.Min(scale_x, scale_y);

            AnimatedZoomPointToViewportCenter(new_scale, new Point(contentRect.X + (contentRect.Width / 2), contentRect.Y + (contentRect.Height / 2)), null);
        }

        /// <summary>
        /// Масштабирование указанной области контента.
        /// </summary>
        /// <param name="contentRect">Прямоугольник области контента.</param>
        public void ZoomTo(Rect contentRect)
        {
            var scale_x = ContentViewportWidth / contentRect.Width;
            var scale_y = ContentViewportHeight / contentRect.Height;
            var new_scale = ContentScale * Math.Min(scale_x, scale_y);

            ZoomPointToViewportCenter(new_scale, new Point(contentRect.X + (contentRect.Width / 2), contentRect.Y + (contentRect.Height / 2)));
        }

        /// <summary>
        /// Мгновенное центрирование вида на указанной точке в координатах контента.
        /// </summary>
        /// <param name="contentOffset">Точка.</param>
        public void SnapContentOffsetTo(Point contentOffset)
        {
            XAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            XAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            ContentOffsetX = contentOffset.X;
            ContentOffsetY = contentOffset.Y;
        }

        /// <summary>
        /// Мгновенное центрирование вида на указанной точке в координатах контента.
        /// </summary>
        /// <param name="contentPoint">Точка.</param>
        public void SnapTo(Point contentPoint)
        {
            XAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            XAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            ContentOffsetX = contentPoint.X - (ContentViewportWidth / 2);
            ContentOffsetY = contentPoint.Y - (ContentViewportHeight / 2);
        }

        /// <summary>
        /// Анимация центрирования вида на указанной точке в координатах контента.
        /// </summary>
        /// <param name="contentPoint">Точка.</param>
        public void AnimatedSnapTo(Point contentPoint)
        {
            var newX = contentPoint.X - (ContentViewportWidth / 2);
            var newY = contentPoint.Y - (ContentViewportHeight / 2);

            XAnimationHelper.StartAnimation(this, ContentOffsetXProperty, newX, AnimationDuration);
            XAnimationHelper.StartAnimation(this, ContentOffsetYProperty, newY, AnimationDuration);
        }

        /// <summary>
        /// Анимация масштабирования с центром в указанной точке в координатах контента.
        /// </summary>
        /// <param name="newСontentScale">Новый масштаб.</param>
        /// <param name="contentZoomFocus">Точка масштабирования.</param>
        public void AnimatedZoomAboutPoint(double newСontentScale, Point contentZoomFocus)
        {
            newСontentScale = Math.Min(Math.Max(newСontentScale, MinContentScale), MaxContentScale);

            XAnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
            XAnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
            XAnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
            XAnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

            ContentZoomFocusX = contentZoomFocus.X;
            ContentZoomFocusY = contentZoomFocus.Y;
            ViewportZoomFocusX = (ContentZoomFocusX - ContentOffsetX) * ContentScale;
            ViewportZoomFocusY = (ContentZoomFocusY - ContentOffsetY) * ContentScale;

            //
            // When zooming about a point make updates to ContentScale also update _content offset.
            //
            _enableContentOffsetUpdateFromScale = true;

            XAnimationHelper.StartAnimation(this, ContentScaleProperty, newСontentScale, AnimationDuration,
                delegate (object sender, EventArgs args)
                {
                    _enableContentOffsetUpdateFromScale = false;

                    ResetViewportZoomFocus();
                });
        }

        /// <summary>
        /// Масштабирование с центром в указанной точке в координатах контента.
        /// </summary>
        /// <param name="newContentScale">Новый масштаб.</param>
        /// <param name="contentZoomFocus">Точка масштабирования.</param>
        public void ZoomAboutPoint(double newContentScale, System.Windows.Point contentZoomFocus)
        {
            newContentScale = Math.Min(Math.Max(newContentScale, MinContentScale), MaxContentScale);

            var screenSpaceZoomOffsetX = (contentZoomFocus.X - ContentOffsetX) * ContentScale;
            var screenSpaceZoomOffsetY = (contentZoomFocus.Y - ContentOffsetY) * ContentScale;
            var contentSpaceZoomOffsetX = screenSpaceZoomOffsetX / newContentScale;
            var contentSpaceZoomOffsetY = screenSpaceZoomOffsetY / newContentScale;
            var newContentOffsetX = contentZoomFocus.X - contentSpaceZoomOffsetX;
            var newContentOffsetY = contentZoomFocus.Y - contentSpaceZoomOffsetY;

            XAnimationHelper.CancelAnimation(this, ContentScaleProperty);
            XAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
            XAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

            ContentScale = newContentScale;
            ContentOffsetX = newContentOffsetX;
            ContentOffsetY = newContentOffsetY;
        }

        /// <summary>
        /// Анимация масштабирования по центру области просмотра.
        /// </summary>
        /// <param name="contentScale">Масштаб.</param>
        public void AnimatedZoomTo(double contentScale)
        {
            var zoom_center = new Point(ContentOffsetX + (ContentViewportWidth / 2), ContentOffsetY + (ContentViewportHeight / 2));
            AnimatedZoomAboutPoint(contentScale, zoom_center);
        }

        /// <summary>
        /// Масштабирование по центру области просмотра.
        /// </summary>
        /// <param name="contentScale">Масштаб.</param>
        public void ZoomTo(double contentScale)
        {
            var zoom_сenter = new Point(ContentOffsetX + (ContentViewportWidth / 2), ContentOffsetY + (ContentViewportHeight / 2));
            ZoomAboutPoint(contentScale, zoom_сenter);
        }

        /// <summary>
        /// Анимация масштабирования по размеру содержимого.
        /// </summary>
        public void AnimatedScaleToFit()
        {
            AnimatedZoomTo(new Rect(0, 0, _content!.ActualWidth, _content.ActualHeight));
        }

        /// <summary>
        /// Масштабирование по размеру содержимого.
        /// </summary>
        public void ScaleToFit()
        {
            if (_content == null)
            {
                throw new ApplicationException("PART_Content was not found in the LotusContentViewer visual template!");
            }

            ZoomTo(new Rect(0, 0, _content.ActualWidth, _content.ActualHeight));
        }
        #endregion

        #region Zoom methods
        /// <summary>
        /// Первичная инициализация данных для работы с увеличением региона.
        /// </summary>
        protected virtual void InitZoomingRegion()
        {
        }

        /// <summary>
        /// Начало операции увеличения региона.
        /// </summary>
        protected virtual void StartZoomingRegion()
        {
            if (_zoomingIsSupport)
            {
                _zoomingStarting = true;
                _zoomingStartPoint = new Point(MousePositionLeftDown.X, MousePositionLeftDown.Y);
                _zoomingRectCorrect.X = MousePositionLeftDown.X - _zoomingDragCorrect / 2;
                _zoomingRectCorrect.Y = MousePositionLeftDown.Y - _zoomingDragCorrect / 2;
                _zoomingRectCorrect.Width = _zoomingDragCorrect;
                _zoomingRectCorrect.Height = _zoomingDragCorrect;
            }
        }

        /// <summary>
        /// Операции увеличения региона (вызывается в MouseMove).
        /// </summary>
        protected virtual void ProcessZoomingRegion()
        {
            if (_zoomingIsSupport)
            {
                // Если есть выход за пределы корректировочного прямоугольника
                if (!_zoomingRectCorrect.Contains(MousePositionCurrent))
                {
                    if (_operationCurrent != TViewHandling.ZoomingRegion)
                    {
                        _operationCurrent = TViewHandling.ZoomingRegion;
                        _operationDesc = "УВЕЛИЧЕНИЕ РЕГИОНА";
                        NotifyPropertyChanged(PropertyArgsOperationDesc);
                    }

                    if (_zoomingStartPoint.X < MousePositionCurrent.X)
                    {
                        _zoomingLeftUpPoint.X = _zoomingStartPoint.X;
                    }
                    else
                    {
                        _zoomingLeftUpPoint.X = MousePositionCurrent.X;
                    }

                    if (_zoomingStartPoint.Y < MousePositionCurrent.Y)
                    {
                        _zoomingLeftUpPoint.Y = _zoomingStartPoint.Y;
                    }
                    else
                    {
                        _zoomingLeftUpPoint.Y = MousePositionCurrent.Y;
                    }

                    _zoomingRect.X = _zoomingLeftUpPoint.X;
                    _zoomingRect.Y = _zoomingLeftUpPoint.Y;
                    _zoomingRect.Width = Math.Abs(_zoomingStartPoint.X - MousePositionCurrent.X);
                    _zoomingRect.Height = Math.Abs(_zoomingStartPoint.Y - MousePositionCurrent.Y);
                }
            }
        }

        /// <summary>
        /// Окончание операции увеличения региона.
        /// </summary>
        protected virtual void EndZoomingRegion()
        {
            if (_zoomingIsSupport)
            {
                this.AnimatedZoomTo(_zoomingRect);
            }

            _zoomingStarting = false;
            _operationCurrent = TViewHandling.None;
            _operationDesc = "";
            NotifyPropertyChanged(PropertyArgsOperationDesc);
        }
        #endregion

        #region Selecting methods
        /// <summary>
        /// Первичная инициализация данных для работы с выделением региона.
        /// </summary>
        protected virtual void InitSelectingRegion()
        {
        }

        /// <summary>
        /// Начало операции выделения региона.
        /// </summary>
        protected virtual void StartSelectingRegion()
        {
            if (_selectingIsSupport)
            {
                _selectingStarting = true;
                _selectingStartPoint = new Point(MousePositionLeftDown.X, MousePositionLeftDown.Y);
                _selectingRectCorrect.X = MousePositionLeftDown.X - _selectingDragCorrect / 2;
                _selectingRectCorrect.Y = MousePositionLeftDown.Y - _selectingDragCorrect / 2;
                _selectingRectCorrect.Width = _selectingDragCorrect;
                _selectingRectCorrect.Height = _selectingDragCorrect;
            }
        }

        /// <summary>
        /// Операция выделения региона (вызывается в MouseMove).
        /// </summary>
        protected virtual void ProcessSelectingRegion()
        {
            if (_selectingIsSupport)
            {
                // Если есть выход за пределы корректировочного прямоугольника
                if (!_selectingRectCorrect.Contains(MousePositionCurrent))
                {
                    if (_operationCurrent != TViewHandling.SelectingRegion)
                    {
                        _operationCurrent = TViewHandling.SelectingRegion;
                        _operationDesc = "ВЫДЕЛЕНИЕ РЕГИОНА" + _selectingStartPoint.ToString();
                        NotifyPropertyChanged(PropertyArgsOperationDesc);
                    }

                    if (_selectingStartPoint.X < MousePositionCurrent.X)
                    {
                        _selectingLeftUpPoint.X = _selectingStartPoint.X;
                        _selectingRightToLeft = false;
                    }
                    else
                    {
                        _selectingLeftUpPoint.X = MousePositionCurrent.X;
                        _selectingRightToLeft = true;
                    }

                    if (_selectingStartPoint.Y < MousePositionCurrent.Y)
                    {
                        _selectingLeftUpPoint.Y = _selectingStartPoint.Y;
                    }
                    else
                    {
                        _selectingLeftUpPoint.Y = MousePositionCurrent.Y;
                    }

                    _selectingRect.X = (float)_selectingLeftUpPoint.X;
                    _selectingRect.Y = (float)_selectingLeftUpPoint.Y;
                    _selectingRect.Width = (float)Math.Abs(_selectingStartPoint.X - MousePositionCurrent.X);
                    _selectingRect.Height = (float)Math.Abs(_selectingStartPoint.Y - MousePositionCurrent.Y);
                }
            }
        }

        /// <summary>
        /// Окончание операции выделения региона.
        /// </summary>
        protected virtual void EndSelectingRegion()
        {
            _selectingStarting = false;
            _operationCurrent = TViewHandling.None;
            _operationDesc = "";
            NotifyPropertyChanged(PropertyArgsOperationDesc);
        }
        #endregion

        #region Pan methods
        /// <summary>
        /// Начало операции перемещения области просмотра.
        /// </summary>
        protected virtual void StartPanning()
        {
            // Смещаем смотровое окно
            this.Cursor = Cursors.SizeAll;
            _operationCurrent = TViewHandling.Panning;
            _operationDesc = "СМЕЩЕНИЕ ОБЛАСТИ";
            NotifyPropertyChanged(PropertyArgsOperationDesc);
        }

        /// <summary>
        /// Операция перемещения области просмотра (вызывается в MouseMove).
        /// </summary>
        protected virtual void ProcessPanning()
        {
            // Смещаем смотровое окно
            var drag_offset = new Vector((MousePositionCurrent - MousePositionMiddleDown).X,
                (MousePositionCurrent - MousePositionMiddleDown).Y);

            this.ContentOffsetX -= drag_offset.X;
            this.ContentOffsetY -= drag_offset.Y;
        }

        /// <summary>
        /// Окончание операции перемещения области просмотра.
        /// </summary>
        protected virtual void EndPanning()
        {
            this.Cursor = Cursors.Arrow;
            _operationCurrent = TViewHandling.None;
            _operationDesc = "";
            NotifyPropertyChanged(PropertyArgsOperationDesc);
        }
        #endregion

        #region Event handlers - Content 
        /// <summary>
        /// Смещение окна просмотра.
        /// </summary>
        protected virtual void OnContentViewerContentOffsetChanged()
        {
        }

        /// <summary>
        /// Изменение размеров окна просмотра.
        /// </summary>
        protected virtual void OnContentViewerContentSizeChanged()
        {
        }

        /// <summary>
        /// Изменение масштаба контента.
        /// </summary>
        protected virtual void OnContentViewerContentScaleChanged()
        {
        }
        #endregion

        #region Event handlers - Action 
        /// <summary>
        /// Элемент загружен.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        protected virtual void OnContentViewerLoaded(object sender, RoutedEventArgs args)
        {
            if (_content == null)
            {
                _content = Content as FrameworkElement;
            }

            InitContentTransformation();
        }

        /// <summary>
        /// Нажатия кнопки мыши.
        /// </summary>
        /// <param name="e">Аргументы события.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            _content!.Focus();
            Keyboard.Focus(_content);

            // 1) Получаем позиции курсора в координатах канвы
            MousePositionCurrent = _contentTotalTransform!.Inverse!.Transform(e.GetPosition(this)).ToVector2Df();

            // 2) Сохраняем текущую операцию
            _operationPreview = _operationCurrent;

            // 3) Нажата левая кнопка мыши
            if (e.ChangedButton == MouseButton.Left)
            {
                MousePositionLeftDown = MousePositionCurrent;

                if (Keyboard.IsKeyDown(Key.Z))
                {
                    // Увеличиваем регион
                    StartZoomingRegion();
                }
                else
                {
                    // Начало выделения региона
                    StartSelectingRegion();
                }
            }

            // Правая кнопка мыши - Открывание контекстного меню
            if (e.ChangedButton == MouseButton.Right)
            {
                if (ContextMenu is not null)
                {
                    ContextMenu.IsOpen = true;
                }
                MousePositionRightDown = MousePositionCurrent;
            }

            // Перемещение
            if (e.ChangedButton == MouseButton.Middle)
            {
                MousePositionMiddleDown = MousePositionCurrent;
                StartPanning();
            }

            // Захватываем мышь
            if (_operationCurrent != TViewHandling.None)
            {
                CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Перемещение курсора мыши.
        /// </summary>
        /// <param name="e">Аргументы события.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Получаем текущие координаты
            var current_content = _contentTotalTransform!.Inverse!.Transform(e.GetPosition(this)).ToVector2Df();

            // Смотрим смещение
            MouseDeltaCurrent = current_content - MousePositionCurrent;

            // Обновляем координаты
            MousePositionCurrent = current_content;

            // Если зажата левая кнопка мыши
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Увеличение региона
                if (_zoomingStarting)
                {
                    ProcessZoomingRegion();
                }
                else
                {
                    if (_selectingStarting)
                    {
                        // Выделение региона
                        ProcessSelectingRegion();
                    }
                }
            }
            else
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    // Перемещение
                    if (_operationCurrent == TViewHandling.Panning)
                    {
                        ProcessPanning();
                    }

                    e.Handled = true;
                }
                else
                {
                }
            }
        }

        /// <summary>
        /// Отпускание кнопки мыши.
        /// </summary>
        /// <param name="e">Аргументы события.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (_operationCurrent == TViewHandling.ZoomingRegion)
                {
                    EndZoomingRegion();
                }
                else
                {
                    if (_operationCurrent == TViewHandling.SelectingRegion)
                    {
                        EndSelectingRegion();
                    }
                }
            }
            else
            {
                if (e.ChangedButton == MouseButton.Middle)
                {
                    EndPanning();
                }
                else
                {

                }
            }

            ReleaseMouseCapture();
            e.Handled = true;
        }

        /// <summary>
        /// Вращение колеса мыши.
        /// </summary>
        /// <param name="e">Аргументы события.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            e.Handled = true;

            if (e.Delta > 0)
            {
                var curContentMousePoint = e.GetPosition(_content!);
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    this.ZoomAboutPoint(this.ContentScale + 0.1, curContentMousePoint);
                }
                else
                {
                    this.ZoomAboutPoint(this.ContentScale + 0.01, curContentMousePoint);
                }
            }
            else if (e.Delta < 0)
            {
                var curContentMousePoint = e.GetPosition(_content!);
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    this.ZoomAboutPoint(this.ContentScale - 0.1, curContentMousePoint);
                }
                else
                {
                    this.ZoomAboutPoint(this.ContentScale - 0.01, curContentMousePoint);
                }
            }
        }
        #endregion

        #region Interface INotifyPropertyChanged 
        /// <summary>
        /// Событие срабатывает ПОСЛЕ изменения свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вспомогательный метод для нотификации изменений свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        public void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged is not null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Вспомогательный метод для нотификации изменений свойства.
        /// </summary>
        /// <param name="args">Аргументы события.</param>
        public void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged is not null)
            {
                PropertyChanged(this, args);
            }
        }
        #endregion
    }
    /**@}*/
}