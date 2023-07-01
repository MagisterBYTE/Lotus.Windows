//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы макетирования
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusContentViewer.cs
*		Основной элемент для управления масштабированием и перемещением контента в области просмотра.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.ComponentModel;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Maths;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFControlsLayout
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Основные операции мышью
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TViewHandling
		{
			/// <summary>
			/// Нет специального режима
			/// </summary>
			None,

			/// <summary>
			/// Перемещение области видимости (средняя кнопка мыши)
			/// </summary>
			Panning,

			/// <summary>
			/// Увеличение/уменьшение (колесико мыши)
			/// </summary>
			Zooming,

			/// <summary>
			/// Увеличение региона (правый шифт + левая кнопка мыши)
			/// </summary>
			ZoomingRegion,

			/// <summary>
			/// Выбор региона
			/// </summary>
			SelectingRegion,

			/// <summary>
			/// Выбор фигуры
			/// </summary>
			SelectedShape,

			/// <summary>
			/// Операция пользователя
			/// </summary>
			UserOperation
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Основной элемент для управления масштабированием и перемещением контента в области просмотра
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class LotusContentViewer : ContentControl, IScrollInfo, INotifyPropertyChanged
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static readonly PropertyChangedEventArgs PropertyArgsOperationDesc = new PropertyChangedEventArgs(nameof(OperationDesc));
			protected static readonly PropertyChangedEventArgs PropertyArgsCanVerticallyScroll = new PropertyChangedEventArgs(nameof(CanVerticallyScroll));
			protected static readonly PropertyChangedEventArgs PropertyArgsCanHorizontallyScroll = new PropertyChangedEventArgs(nameof(CanHorizontallyScroll));
			protected static readonly PropertyChangedEventArgs PropertyArgsExtentWidth = new PropertyChangedEventArgs(nameof(ExtentWidth));
			protected static readonly PropertyChangedEventArgs PropertyArgsExtentHeight = new PropertyChangedEventArgs(nameof(ExtentHeight));
			protected static readonly PropertyChangedEventArgs PropertyArgsViewportWidth = new PropertyChangedEventArgs(nameof(ViewportWidth));
			protected static readonly PropertyChangedEventArgs PropertyArgsViewportHeight = new PropertyChangedEventArgs(nameof(ViewportHeight));
			protected static readonly PropertyChangedEventArgs PropertyArgsHorizontalOffset = new PropertyChangedEventArgs(nameof(HorizontalOffset));
			protected static readonly PropertyChangedEventArgs PropertyArgsVerticalOffset = new PropertyChangedEventArgs(nameof(VerticalOffset));
			#endregion

			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			//
			// Definitions for dependency properties.
			//
			public static readonly DependencyProperty ContentScaleProperty =
					DependencyProperty.Register("ContentScale", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(1.0, ContentScale_PropertyChanged, ContentScale_Coerce));

			public static readonly DependencyProperty MinContentScaleProperty =
					DependencyProperty.Register("MinContentScale", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.01, MinOrMaxContentScale_PropertyChanged));

			public static readonly DependencyProperty MaxContentScaleProperty =
					DependencyProperty.Register("MaxContentScale", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(10.0, MinOrMaxContentScale_PropertyChanged));

			public static readonly DependencyProperty ContentOffsetXProperty =
					DependencyProperty.Register("ContentOffsetX", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.0, ContentOffsetX_PropertyChanged, ContentOffsetX_Coerce));

			public static readonly DependencyProperty ContentOffsetYProperty =
					DependencyProperty.Register("ContentOffsetY", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.0, ContentOffsetY_PropertyChanged, ContentOffsetY_Coerce));

			public static readonly DependencyProperty AnimationDurationProperty =
					DependencyProperty.Register("AnimationDuration", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.4));

			public static readonly DependencyProperty ContentZoomFocusXProperty =
					DependencyProperty.Register("ContentZoomFocusX", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.0));

			public static readonly DependencyProperty ContentZoomFocusYProperty =
					DependencyProperty.Register("ContentZoomFocusY", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.0));

			public static readonly DependencyProperty ViewportZoomFocusXProperty =
					DependencyProperty.Register("ViewportZoomFocusX", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.0));

			public static readonly DependencyProperty ViewportZoomFocusYProperty =
					DependencyProperty.Register("ViewportZoomFocusY", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.0));

			public static readonly DependencyProperty ContentViewportWidthProperty =
					DependencyProperty.Register("ContentViewportWidth", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.0));

			public static readonly DependencyProperty ContentViewportHeightProperty =
					DependencyProperty.Register("ContentViewportHeight", typeof(Double), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(0.0));

			public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty =
					DependencyProperty.Register("IsMouseWheelScrollingEnabled", typeof(Boolean), typeof(LotusContentViewer),
												new FrameworkPropertyMetadata(false));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Статический конструктор
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			static LotusContentViewer()
			{
				DefaultStyleKeyProperty.OverrideMetadata(typeof(LotusContentViewer), new FrameworkPropertyMetadata(typeof(LotusContentViewer)));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение масштаба
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void ContentScale_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				var content_viewer = (LotusContentViewer)obj;
				content_viewer.OnContentViewerContentScaleChanged();

				if (content_viewer.mContentScaleTransform != null)
				{
					//
					// Update the mContent scale transform whenever 'ContentScale' changes.
					//
					content_viewer.mContentScaleTransform.ScaleX = content_viewer.ContentScale;
					content_viewer.mContentScaleTransform.ScaleY = content_viewer.ContentScale;
				}

				//
				// Update the size of the viewport in mContent coordinates.
				//
				content_viewer.UpdateContentViewportSize();

				if (content_viewer.mEnableContentOffsetUpdateFromScale)
				{
					try
					{
						// 
						// Disable mContent focus syncronization.  We are about to update mContent offset whilst zooming
						// to ensure that the viewport is focused on our desired mContent focus point.  Setting this
						// to 'true' stops the automatic update of the mContent focus when mContent offset changes.
						//
						content_viewer.mDisableContentFocusSync = true;

						//
						// Whilst zooming in or out keep the mContent offset up-to-date so that the viewport is always
						// focused on the mContent focus point (and also so that the mContent focus is locked to the 
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
						content_viewer.mDisableContentFocusSync = false;
					}
				}

				if (content_viewer.ContentScaleChanged != null)
				{
					content_viewer.ContentScaleChanged(content_viewer, EventArgs.Empty);
				}

				if (content_viewer.mScrollOwner != null)
				{
					content_viewer.mScrollOwner.InvalidateScrollInfo();
				}

				content_viewer.NotifyPropertyChanged(PropertyArgsExtentWidth);
				content_viewer.NotifyPropertyChanged(PropertyArgsExtentHeight);
				content_viewer.NotifyPropertyChanged(PropertyArgsHorizontalOffset);
				content_viewer.NotifyPropertyChanged(PropertyArgsVerticalOffset);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Корректировка масштаба
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="base_value">Базовое значение</param>
			/// <returns>Скорректированное значение</returns>
			//---------------------------------------------------------------------------------------------------------
			private static Object ContentScale_Coerce(DependencyObject obj, Object base_value)
			{
				var c = (LotusContentViewer)obj;
				var value = (Double)base_value;
				value = Math.Min(Math.Max(value, c.MinContentScale), c.MaxContentScale);
				return value;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение минимального/максимального масштаба
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void MinOrMaxContentScale_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				var c = (LotusContentViewer)obj;
				c.ContentScale = Math.Min(Math.Max(c.ContentScale, c.MinContentScale), c.MaxContentScale);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение смещения контента по X
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void ContentOffsetX_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				var content_viewer = (LotusContentViewer)obj;
				content_viewer.OnContentViewerContentOffsetChanged();
				content_viewer.UpdateTranslationX();

				if (!content_viewer.mDisableContentFocusSync)
				{
					//
					// Normally want to automatically update mContent focus when mContent offset changes.
					// Although this is disabled using 'disableContentFocusSync' when mContent offset changes due to in-progress zooming.
					//
					content_viewer.UpdateContentZoomFocusX();
				}

				if (content_viewer.ContentOffsetXChanged != null)
				{
					//
					// Raise an event to let users of the control know that the mContent offset has changed.
					//
					content_viewer.ContentOffsetXChanged(content_viewer, EventArgs.Empty);
				}

				if (!content_viewer.mDisableScrollOffsetSync && content_viewer.mScrollOwner != null)
				{
					//
					// Notify the owning ScrollViewer that the scrollbar offsets should be updated.
					//
					content_viewer.mScrollOwner.InvalidateScrollInfo();
				}

				content_viewer.NotifyPropertyChanged(PropertyArgsHorizontalOffset);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Корректировка смещения контента по X
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="base_value">Базовое значение</param>
			/// <returns>Скорректированное значение</returns>
			//---------------------------------------------------------------------------------------------------------
			private static Object ContentOffsetX_Coerce(DependencyObject obj, Object base_value)
			{
				var c = (LotusContentViewer)obj;
				var value = (Double)base_value;
				var min_offset_x = 0.0;
				var max_offset_x = Math.Max(0.0, c.mUnScaledExtent.Width - c.mConstrainedContentViewportWidth);
				value = Math.Min(Math.Max(value, min_offset_x), max_offset_x);
				return value;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение смещения контента по Y
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void ContentOffsetY_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				var content_viewer = (LotusContentViewer)obj;
				content_viewer.OnContentViewerContentOffsetChanged();
				content_viewer.UpdateTranslationY();

				if (!content_viewer.mDisableContentFocusSync)
				{
					//
					// Normally want to automatically update mContent focus when mContent offset changes.
					// Although this is disabled using 'disableContentFocusSync' when mContent offset changes due to in-progress zooming.
					//
					content_viewer.UpdateContentZoomFocusY();
				}

				if (content_viewer.ContentOffsetYChanged != null)
				{
					//
					// Raise an event to let users of the control know that the mContent offset has changed.
					//
					content_viewer.ContentOffsetYChanged(content_viewer, EventArgs.Empty);
				}

				if (!content_viewer.mDisableScrollOffsetSync && content_viewer.mScrollOwner != null)
				{
					//
					// Notify the owning ScrollViewer that the scrollbar offsets should be updated.
					//
					content_viewer.mScrollOwner.InvalidateScrollInfo();
				}

				content_viewer.NotifyPropertyChanged(PropertyArgsVerticalOffset);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Корректировка смещения контента по Y
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="base_value">Базовое значение</param>
			/// <returns>Скорректированное значение</returns>
			//---------------------------------------------------------------------------------------------------------
			private static Object ContentOffsetY_Coerce(DependencyObject obj, Object base_value)
			{
				var c = (LotusContentViewer)obj;
				var value = (Double)base_value;
				var min_offset_y = 0.0;
				var max_offset_y = Math.Max(0.0, c.mUnScaledExtent.Height - c.mConstrainedContentViewportHeight);
				value = Math.Min(Math.Max(value, min_offset_y), max_offset_y);
				return value;
			}
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основное содержимое
			protected internal FrameworkElement mContent = null;

			// Перемещение и масштабирование
			protected internal ScaleTransform mContentScaleTransform = null;
			protected internal TranslateTransform mContentOffsetTransform = null;
			protected internal TransformGroup mContentTotalTransform = null;
			protected internal Boolean mEnableContentOffsetUpdateFromScale = false;
			protected internal Boolean mDisableScrollOffsetSync = false;
			protected internal Boolean mDisableContentFocusSync = false;
			protected internal Double mConstrainedContentViewportWidth = 0.0;
			protected internal Double mConstrainedContentViewportHeight = 0.0;

			// Поддержка скроллинга
			protected internal ScrollViewer mScrollOwner = null;
			protected internal Boolean mCanVerticallyScroll = false;
			protected internal Boolean mCanHorizontallyScroll = false;
			protected internal Size mUnScaledExtent = new Size(0, 0);
			protected internal Size mViewportScroll = new Size(0, 0);

			// Операции
			protected internal TViewHandling mOperationCurrent;  // Текущая операция
			protected internal TViewHandling mOperationPreview;  // Предыдущая операция
			protected internal String mOperationDesc; // Описание операции

			// Прямоугольник увеличение области канвы
			protected internal Boolean mZoomingIsSupport = true;
			protected internal Boolean mZoomingStarting = false;
			protected internal Point mZoomingStartPoint;
			protected internal Vector mZoomingLeftUpPoint;
			protected internal Rect mZoomingRect;
			protected internal Single mZoomingDragCorrect = 10;
			protected internal Rect mZoomingRectCorrect;

			// Выбор региона
			protected internal Boolean mSelectingIsSupport = true;
			protected internal Boolean mSelectingStarting = false;
			protected internal Point mSelectingStartPoint;
			protected internal Vector mSelectingLeftUpPoint;
			protected internal Boolean mSelectingRightToLeft;
			protected internal Rect mSelectingRect;
			protected internal Single mSelectingDragCorrect = 10;
			protected internal Rect mSelectingRectCorrect;

			// Координаты курсора
			public Vector2Df MousePositionLeftDown;
			public Vector2Df MousePositionRightDown;
			public Vector2Df MousePositionMiddleDown;
			public Vector2Df MousePositionCurrent;
			public Vector2Df MouseDeltaCurrent;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Статус нахождения компонента в режиме разработки
			/// </summary>
			public static Boolean IsDesignMode
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
			/// Смещение контента по X
			/// </summary>
			[Description("Смещение области просмотра по X в координтах контета")]
			public Double ContentOffsetX
			{
				get { return (Double)GetValue(ContentOffsetXProperty); }
				set { SetValue(ContentOffsetXProperty, value); }
			}

			/// <summary>
			/// События изменения смещения контента по X
			/// </summary>
			public event EventHandler ContentOffsetXChanged;

			/// <summary>
			/// Смещение контента по Y
			/// </summary>
			[Description("Смещение области просмотра по Y в координтах контета")]
			public Double ContentOffsetY
			{
				get { return (Double)GetValue(ContentOffsetYProperty); }
				set { SetValue(ContentOffsetYProperty, value); }
			}

			/// <summary>
			/// События изменения смещения контента по Y
			/// </summary>
			public event EventHandler ContentOffsetYChanged;

			/// <summary>
			/// Масштаб контента
			/// </summary>
			public Double ContentScale
			{
				get { return (Double)GetValue(ContentScaleProperty); }
				set { SetValue(ContentScaleProperty, value); }
			}

			/// <summary>
			/// События изменения масштаба
			/// </summary>
			public event EventHandler ContentScaleChanged;

			/// <summary>
			/// Минимальное значение масштаба контента
			/// </summary>
			public Double MinContentScale
			{
				get { return (Double)GetValue(MinContentScaleProperty); }
				set { SetValue(MinContentScaleProperty, value); }
			}

			/// <summary>
			/// Максимальное значение масштаба
			/// </summary>
			public Double MaxContentScale
			{
				get { return (Double)GetValue(MaxContentScaleProperty); }
				set { SetValue(MaxContentScaleProperty, value); }
			}

			/// <summary>
			/// Координата по X контента точки фокуса при масштабировании
			/// </summary>
			public Double ContentZoomFocusX
			{
				get { return (Double)GetValue(ContentZoomFocusXProperty); }
				set { SetValue(ContentZoomFocusXProperty, value); }
			}

			/// <summary>
			/// Координата по Y контента точки фокуса при масштабировании
			/// </summary>
			public Double ContentZoomFocusY
			{
				get { return (Double)GetValue(ContentZoomFocusYProperty); }
				set { SetValue(ContentZoomFocusYProperty, value); }
			}

			/// <summary>
			/// Координата по X области просмотра точки фокуса при масштабировании
			/// </summary>
			public Double ViewportZoomFocusX
			{
				get { return (Double)GetValue(ViewportZoomFocusXProperty); }
				set { SetValue(ViewportZoomFocusXProperty, value); }
			}

			/// <summary>
			/// Координата по Y области просмотра точки фокуса при масштабировании
			/// </summary>
			public Double ViewportZoomFocusY
			{
				get { return (Double)GetValue(ViewportZoomFocusYProperty); }
				set { SetValue(ViewportZoomFocusYProperty, value); }
			}

			/// <summary>
			/// Время анимации при эффектах масштабирования
			/// </summary>
			public Double AnimationDuration
			{
				get { return (Double)GetValue(AnimationDurationProperty); }
				set { SetValue(AnimationDurationProperty, value); }
			}

			/// <summary>
			/// Ширина области просмотра в координтах контента
			/// </summary>
			public Double ContentViewportWidth
			{
				get { return (Double)GetValue(ContentViewportWidthProperty); }
				set { SetValue(ContentViewportWidthProperty, value); }
			}

			/// <summary>
			/// Высота области просмотра в координтах контента
			/// </summary>
			public Double ContentViewportHeight
			{
				get { return (Double)GetValue(ContentViewportHeightProperty); }
				set { SetValue(ContentViewportHeightProperty, value); }
			}

			/// <summary>
			/// Возможность прокрутки области просмотра колесом мыши
			/// </summary>
			public Boolean IsMouseWheelScrollingEnabled
			{
				get { return (Boolean)GetValue(IsMouseWheelScrollingEnabledProperty); }
				set { SetValue(IsMouseWheelScrollingEnabledProperty, value); }
			}

			//
			// ПОДДЕРЖКА СКРОЛЛИНГА ScrollViewer
			//
			/// <summary>
			/// Элемент ScrollViewer
			/// </summary>
			public ScrollViewer ScrollOwner
			{
				get { return mScrollOwner; }
				set { mScrollOwner = value; }
			}

			/// <summary>
			/// Возможность вертикальной прокрутки
			/// </summary>
			public Boolean CanVerticallyScroll
			{
				get { return mCanVerticallyScroll; }
				set
				{
					mCanVerticallyScroll = value;
					NotifyPropertyChanged(PropertyArgsCanVerticallyScroll);
				}
			}

			/// <summary>
			/// Возможность горизонтальной прокрутки
			/// </summary>
			public Boolean CanHorizontallyScroll
			{
				get { return mCanHorizontallyScroll; }
				set
				{
					mCanHorizontallyScroll = value;
					NotifyPropertyChanged(PropertyArgsCanHorizontallyScroll);
				}
			}

			/// <summary>
			/// Горизонтальный размер контента с учетом масштаба
			/// </summary>
			public Double ExtentWidth
			{
				get { return mUnScaledExtent.Width * ContentScale; }
			}

			/// <summary>
			/// Вертикальный размер контента с учетом масштаба
			/// </summary>
			public Double ExtentHeight
			{
				get { return mUnScaledExtent.Height * ContentScale; }
			}

			/// <summary>
			/// Горизонтальный размер окна просмотра для данного содержимого
			/// </summary>
			public Double ViewportWidth
			{
				get { return mViewportScroll.Width; }
			}

			/// <summary>
			/// Вертикальный размер окна просмотра для данного содержимого
			/// </summary>
			public Double ViewportHeight
			{
				get { return mViewportScroll.Height; }
			}

			/// <summary>
			/// Горизонтальное смещение прокручиваемого содержимого
			/// </summary>
			public Double HorizontalOffset
			{
				get { return ContentOffsetX * ContentScale; }
			}

			/// <summary>
			/// Вертикальное смещение прокручиваемого содержимого
			/// </summary>
			public Double VerticalOffset
			{
				get { return ContentOffsetY * ContentScale; }
			}

			//
			// ОПЕРАЦИИ
			//
			/// <summary>
			/// Текущая операция мышью
			/// </summary>
			public TViewHandling OperationCurrent
			{
				get { return mOperationCurrent; }
			}

			/// <summary>
			/// Предыдущая операция мышью
			/// </summary>
			public TViewHandling OperationPreview
			{
				get { return mOperationPreview; }
			}

			/// <summary>
			/// Описание текущей операции
			/// </summary>
			public String OperationDesc
			{
				get { return mOperationDesc; }
			}

			//
			// УВЕЛИЧЕНИЕ РЕГИОНА
			//
			/// <summary>
			/// Возможность увеличение прямоугольной области
			/// </summary>
			[Description("Возможность увеличение прямоугольной области")]
			public Boolean ZoomingIsSupport
			{
				get { return mZoomingIsSupport; }
				set { mZoomingIsSupport = value; }
			}

			/// <summary>
			/// Начало увеличение прямоугольной области
			/// </summary>
			public Boolean ZoomingStarting
			{
				get { return mZoomingStarting; }
			}

			/// <summary>
			/// Минимальное смещение для увеличения области
			/// </summary>
			[Description("Минимальное смещение для увеличения области")]
			public Single ZoomingDragCorrect
			{
				get { return mZoomingDragCorrect; }
				set { mZoomingDragCorrect = value; }
			}

			/// <summary>
			/// Текущий прямоугольник увеличение региона
			/// </summary>
			public Rect ZoomingRect
			{
				get { return mZoomingRect; }
			}

			//
			// ВЫБОР РЕГИОНА
			//
			/// <summary>
			/// Возможность выбора прямоугольной области
			/// </summary>
			[Description("Возможность выбора прямоугольной области")]
			public Boolean SelectingIsSupport
			{
				get { return mSelectingIsSupport; }
				set { mSelectingIsSupport = value; }
			}

			/// <summary>
			/// Минимальное смещение для выбора области
			/// </summary>
			[Description("Минимальное смещение для выбора области")]
			public Single SelectingDragCorrect
			{
				get { return mSelectingDragCorrect; }
				set { mSelectingDragCorrect = value; }
			}

			/// <summary>
			/// Выбора области справа налево
			/// </summary>
			public Boolean SelectingRightToLeft
			{
				get { return mSelectingRightToLeft; }
				set { mSelectingRightToLeft = value; }
			}

			/// <summary>
			/// Текущий прямоугольник выбора региона
			/// </summary>
			public Rect SelectingRect
			{
				get { return mSelectingRect; }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusContentViewer()
			{
				this.Loaded += OnContentViewerLoaded;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="content">Элемент контент</param>
			//---------------------------------------------------------------------------------------------------------
			public LotusContentViewer(FrameworkElement content)
			{
				Content = content;
				this.Loaded += OnContentViewerLoaded;
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных трансформации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void InitContentTransformation()
			{
				//
				// Setup the transform on the mContent so that we can scale it by 'ContentScale'.
				//
				this.mContentScaleTransform = new ScaleTransform(this.ContentScale, this.ContentScale);

				//
				// Setup the transform on the mContent so that we can translate it by 'ContentOffsetX' and 'ContentOffsetY'.
				//
				this.mContentOffsetTransform = new TranslateTransform();
				UpdateTranslationX();
				UpdateTranslationY();

				//
				// Setup a transform group to contain the translation and scale transforms, and then
				// assign this to the mContent's 'RenderTransform'.
				//
				mContentTotalTransform = new TransformGroup();
				mContentTotalTransform.Children.Add(this.mContentOffsetTransform);
				mContentTotalTransform.Children.Add(this.mContentScaleTransform);
				if (mContent == null)
				{
					mContent = Content as FrameworkElement;
					if(mContent != null)
					{
						mContent.RenderTransform = mContentTotalTransform;
					}
				}
				else
				{
					mContent.RenderTransform = mContentTotalTransform;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Увеличение в заданном масштабе и перемещение указанную точку фокуса до центра окна просмотра
			/// </summary>
			/// <param name="new_сontent_scale">Новый масштаб</param>
			/// <param name="content_zoom_focus">Точка масштабирования в координатах контента</param>
			/// <param name="callback">Метод обратного вызова</param>
			//---------------------------------------------------------------------------------------------------------
			private void AnimatedZoomPointToViewportCenter(Double new_сontent_scale, Point content_zoom_focus, EventHandler callback)
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
				// When zooming about a point make updates to ContentScale also update mContent offset.
				//
				mEnableContentOffsetUpdateFromScale = true;

				XAnimationHelper.StartAnimation(this, ContentScaleProperty, new_сontent_scale, AnimationDuration,
					delegate (Object sender, EventArgs args)
					{
						mEnableContentOffsetUpdateFromScale = false;

						if (callback != null)
						{
							callback(this, EventArgs.Empty);
						}
					});

				XAnimationHelper.StartAnimation(this, ViewportZoomFocusXProperty, ViewportWidth / 2, AnimationDuration);
				XAnimationHelper.StartAnimation(this, ViewportZoomFocusYProperty, ViewportHeight / 2, AnimationDuration);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Увеличение в заданном масштабе и перемещение указанную точку фокуса до центра окна просмотра
			/// </summary>
			/// <param name="new_сontent_scale">Новый масштаб</param>
			/// <param name="content_zoom_focus">Точка масштабирования в координатах контента</param>
			//---------------------------------------------------------------------------------------------------------
			private void ZoomPointToViewportCenter(Double new_сontent_scale, Point content_zoom_focus)
			{
				new_сontent_scale = Math.Min(Math.Max(new_сontent_scale, MinContentScale), MaxContentScale);

				XAnimationHelper.CancelAnimation(this, ContentScaleProperty);
				XAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
				XAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

				ContentScale = new_сontent_scale;
				ContentOffsetX = content_zoom_focus.X - (ContentViewportWidth / 2);
				ContentOffsetY = content_zoom_focus.Y - (ContentViewportHeight / 2);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сброс фокус видовой экрана в центре области просмотра
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void ResetViewportZoomFocus()
			{
				ViewportZoomFocusX = ViewportWidth / 2;
				ViewportZoomFocusY = ViewportHeight / 2;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление размера области просмотра от заданного размера
			/// </summary>
			/// <param name="new_size">Новый размер видового экрана</param>
			//---------------------------------------------------------------------------------------------------------
			private void UpdateViewportSize(Size new_size)
			{
				if (mViewportScroll == new_size)
				{
					//
					// The viewport is already the specified size.
					//
					return;
				}

				mViewportScroll = new_size;

				//
				// Update the viewport size in mContent coordiates.
				//
				UpdateContentViewportSize();

				//
				// Initialise the mContent zoom focus point.
				//
				UpdateContentZoomFocusX();
				UpdateContentZoomFocusY();

				//
				// Reset the viewport zoom focus to the center of the viewport.
				//
				ResetViewportZoomFocus();

				//
				// Update mContent offset from itself when the size of the viewport changes.
				// This ensures that the mContent offset remains properly clamped to its valid range.
				//
				ContentOffsetX = ContentOffsetX;
				ContentOffsetY = ContentOffsetY;

				if (mScrollOwner != null)
				{
					//
					// Tell that owning ScrollViewer that scrollbar data has changed.
					//
					mScrollOwner.InvalidateScrollInfo();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление размера области просмотра вследствие изменения масштаба или размеров контента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void UpdateContentViewportSize()
			{
				ContentViewportWidth = ViewportWidth / ContentScale;
				ContentViewportHeight = ViewportHeight / ContentScale;

				mConstrainedContentViewportWidth = Math.Min(ContentViewportWidth, mUnScaledExtent.Width);
				mConstrainedContentViewportHeight = Math.Min(ContentViewportHeight, mUnScaledExtent.Height);

				UpdateTranslationX();
				UpdateTranslationY();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление координаты Х трансформации смещения контента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void UpdateTranslationX()
			{
				var scaled_сontent_width = mUnScaledExtent.Width * ContentScale;
				if (scaled_сontent_width < ViewportWidth)
				{
					//
					// Когда содержание может поместиться целиком внутри окна просмотра, то перемещаем в центр
					//
					mContentOffsetTransform.X = (ContentViewportWidth - mUnScaledExtent.Width) / 2;
				}
				else
				{
					mContentOffsetTransform.X = -ContentOffsetX;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			///  Обновление координаты Y трансформации смещения контента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void UpdateTranslationY()
			{
				var scaled_content_height = mUnScaledExtent.Height * ContentScale;
				if (scaled_content_height < ViewportHeight)
				{
					//
					// Когда содержание может поместиться целиком внутри окна просмотра, то перемещаем в центр
					//
					mContentOffsetTransform.Y = (ContentViewportHeight - mUnScaledExtent.Height) / 2;
				}
				else
				{
					mContentOffsetTransform.Y = -ContentOffsetY;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление X координаты точки фокусировки области просмотра
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void UpdateContentZoomFocusX()
			{
				ContentZoomFocusX = ContentOffsetX + (mConstrainedContentViewportWidth / 2);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление Y координаты точки фокусировки области просмотра
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void UpdateContentZoomFocusY()
			{
				ContentZoomFocusY = ContentOffsetY + (mConstrainedContentViewportHeight / 2);
			}
			#endregion

			#region ======================================= МЕТОДЫ IScrollInfo ========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Величина горизонтальной прокрутки
			/// </summary>
			/// <param name="offset">Величина, на которую содержимое смещается по горизонтали от окна просмотра</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetHorizontalOffset(Double offset)
			{
				if (mDisableScrollOffsetSync)
				{
					return;
				}

				try
				{
					mDisableScrollOffsetSync = true;

					ContentOffsetX = offset / ContentScale;
				}
				finally
				{
					mDisableScrollOffsetSync = false;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Величина вертикальной прокрутки
			/// </summary>
			/// <param name="offset">Величина, на которую содержимое смещается по вертикали от окна просмотра</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetVerticalOffset(Double offset)
			{
				if (mDisableScrollOffsetSync)
				{
					return;
				}

				try
				{
					mDisableScrollOffsetSync = true;

					ContentOffsetY = offset / ContentScale;
				}
				finally
				{
					mDisableScrollOffsetSync = false;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокрутку вверх в содержимом на одну логическую единицу
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void LineUp()
			{
				ContentOffsetY -= ContentViewportHeight / 10;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокрутку вниз в содержимом на одну логическую единицу
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void LineDown()
			{
				ContentOffsetY += ContentViewportHeight / 10;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокрутка влево в содержимом на одну логическую единицу
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void LineLeft()
			{
				ContentOffsetX -= ContentViewportWidth / 10;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокрутка вправо в содержимом на одну логическую единицу
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void LineRight()
			{
				ContentOffsetX += ContentViewportWidth / 10;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокрутка вверх в содержимом на одну логическую страницу
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void PageUp()
			{
				ContentOffsetY -= ContentViewportHeight;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокрутка вниз в содержимом на одну логическую страницу
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void PageDown()
			{
				ContentOffsetY += ContentViewportHeight;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокрутка влево в содержимом на одну логическую страницу
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void PageLeft()
			{
				ContentOffsetX -= ContentViewportWidth;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокрутка вправо в содержимом на одну логическую страницу
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void PageRight()
			{
				ContentOffsetX += ContentViewportWidth;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокручивает содержимое вниз после нажатия пользователем колесика мыши
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void MouseWheelDown()
			{
				if (IsMouseWheelScrollingEnabled)
				{
					LineDown();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокручивает содержимое влево после нажатия пользователем колесика мыши
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void MouseWheelLeft()
			{
				if (IsMouseWheelScrollingEnabled)
				{
					LineLeft();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокручивает содержимое вправо после нажатия пользователем колесика мыши
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void MouseWheelRight()
			{
				if (IsMouseWheelScrollingEnabled)
				{
					LineRight();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Прокручивает содержимое вверх после нажатия пользователем колесика мыши
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void MouseWheelUp()
			{
				if (IsMouseWheelScrollingEnabled)
				{
					LineUp();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Принудительно прокручивание содержимое пока координатное пространство объекта Visual не станет видимым
			/// </summary>
			/// <param name="visual">Объект который становится видимым</param>
			/// <param name="rectangle">Ограничивающий прямоугольник, идентифицирующий пространство координат, которое необходимо сделать видимым</param>
			/// <returns>Прямоугольник который является видимым</returns>
			//---------------------------------------------------------------------------------------------------------
			public Rect MakeVisible(Visual visual, Rect rectangle)
			{
				if (mContent.IsAncestorOf(visual))
				{
					Rect transformedRect = visual.TransformToAncestor(mContent).TransformBounds(rectangle);
					var viewportRect = new Rect(ContentOffsetX, ContentOffsetY, ContentViewportWidth, ContentViewportHeight);
					if (!transformedRect.Contains(viewportRect))
					{
						Double horizOffset = 0;
						Double vertOffset = 0;

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

			#region ======================================= ПЕРЕГРУЖЕННЫЕ МЕТОДЫ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выполняет построение визуального дерева текущего шаблона (если это необходимо) и возвращает значение,
			/// указывающее, было ли визуальное дерево перестроено данным вызовом
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void OnApplyTemplate()
			{
				base.OnApplyTemplate();

				mContent = this.Template.FindName("PART_Content", this) as FrameworkElement;
				//mContent = this.Content as FrameworkElement;
				if (mContent != null)
				{
					InitContentTransformation();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Опредилить размеры
			/// </summary>
			/// <remarks>
			/// Метод, который по заданному available_size определяет желаемые размеры и выставляет их в this.DesiredSize.
			/// В описании к методу написано, что результирующий DesiredSize может быть > availableSize, но для наследников FrameworkElement это не так.
			/// </remarks>
			/// <param name="constraint">Имеющиеся размеры</param>
			/// <returns>Размер элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			protected override Size MeasureOverride(Size constraint)
			{
				if (mContent == null)
				{
					mContent = this.Content as FrameworkElement;
				}

				if (this.mContentScaleTransform == null)
				{
					InitContentTransformation();
				}

				var infinite_size = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
				Size child_size = base.MeasureOverride(infinite_size);

				if (child_size != mUnScaledExtent)
				{
					//
					// Use the size of the child as the un-scaled extent mContent.
					//
					mUnScaledExtent = child_size;

					if (mScrollOwner != null)
					{
						mScrollOwner.InvalidateScrollInfo();
					}
				}

				//
				// Update the size of the viewport onto the mContent based on the passed in 'constraint'.
				//
				UpdateViewportSize(constraint);

				var width = constraint.Width;
				var height = constraint.Height;

				if (Double.IsInfinity(width))
				{
					//
					// Make sure we don't return infinity!
					//
					width = child_size.Width;
				}

				if (Double.IsInfinity(height))
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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончательно определить размеры
			/// </summary>
			/// <param name="arrangeBounds">Требуемые размеры</param>
			/// <returns>Размер элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			protected override Size ArrangeOverride(Size arrangeBounds)
			{
				Size size = base.ArrangeOverride(this.DesiredSize);

				if (mContent == null)
				{
					mContent = this.Content as FrameworkElement;
				}

				if (mContent.DesiredSize != mUnScaledExtent)
				{
					//
					// Use the size of the child as the un-scaled extent mContent.
					//
					mUnScaledExtent = mContent.DesiredSize;

					if (mScrollOwner != null)
					{
						mScrollOwner.InvalidateScrollInfo();
					}
				}

				//
				// Update the size of the viewport onto the mContent based on the passed in 'arrangeBounds'.
				//
				UpdateViewportSize(arrangeBounds);

				NotifyPropertyChanged(PropertyArgsExtentWidth);
				NotifyPropertyChanged(PropertyArgsExtentHeight);
				NotifyPropertyChanged(PropertyArgsViewportHeight);
				NotifyPropertyChanged(PropertyArgsViewportWidth);

				return size;
			}
			#endregion

			#region ======================================= МЕТОДЫ АНИМАЦИИ И МАСШТАБИРОВАНИЯ =========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Анимация масштабирования указанной области контента
			/// </summary>
			/// <param name="new_scale">Масштаб</param>
			/// <param name="content_rect">Прямоугольник области контента</param>
			//---------------------------------------------------------------------------------------------------------
			public void AnimatedZoomTo(Double new_scale, Rect content_rect)
			{
				AnimatedZoomPointToViewportCenter(new_scale, new Point(content_rect.X + (content_rect.Width / 2), content_rect.Y + (content_rect.Height / 2)),
					delegate (Object sender, EventArgs args)
					{
						//
						// At the end of the animation, ensure that we are snapped to the specified mContent offset.
						// Due to zooming in on the mContent focus point and rounding errors, the mContent offset may
						// be slightly off what we want at the end of the animation and this bit of code corrects it.
						//
						ContentOffsetX = content_rect.X;
						ContentOffsetY = content_rect.Y;
					});
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Анимация масштабирования указанной области контента
			/// </summary>
			/// <param name="content_rect">Прямоугольник области контента</param>
			//---------------------------------------------------------------------------------------------------------
			public void AnimatedZoomTo(Rect content_rect)
			{
				var scale_x = ContentViewportWidth / content_rect.Width;
				var scale_y = ContentViewportHeight / content_rect.Height;
				var new_scale = ContentScale * Math.Min(scale_x, scale_y);

				AnimatedZoomPointToViewportCenter(new_scale, new Point(content_rect.X + (content_rect.Width / 2), content_rect.Y + (content_rect.Height / 2)), null);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Масштабирование указанной области контента
			/// </summary>
			/// <param name="content_rect">Прямоугольник области контента</param>
			//---------------------------------------------------------------------------------------------------------
			public void ZoomTo(Rect content_rect)
			{
				var scale_x = ContentViewportWidth / content_rect.Width;
				var scale_y = ContentViewportHeight / content_rect.Height;
				var new_scale = ContentScale * Math.Min(scale_x, scale_y);

				ZoomPointToViewportCenter(new_scale, new Point(content_rect.X + (content_rect.Width / 2), content_rect.Y + (content_rect.Height / 2)));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Мгновенное центрирование вида на указанной точке в координатах контента
			/// </summary>
			/// <param name="content_offset">Точка</param>
			//---------------------------------------------------------------------------------------------------------
			public void SnapContentOffsetTo(Point content_offset)
			{
				XAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
				XAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

				ContentOffsetX = content_offset.X;
				ContentOffsetY = content_offset.Y;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Мгновенное центрирование вида на указанной точке в координатах контента
			/// </summary>
			/// <param name="content_point">Точка</param>
			//---------------------------------------------------------------------------------------------------------
			public void SnapTo(Point content_point)
			{
				XAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
				XAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

				ContentOffsetX = content_point.X - (ContentViewportWidth / 2);
				ContentOffsetY = content_point.Y - (ContentViewportHeight / 2);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Анимация центрирования вида на указанной точке в координатах контента
			/// </summary>
			/// <param name="content_point">Точка</param>
			//---------------------------------------------------------------------------------------------------------
			public void AnimatedSnapTo(Point content_point)
			{
				var newX = content_point.X - (ContentViewportWidth / 2);
				var newY = content_point.Y - (ContentViewportHeight / 2);

				XAnimationHelper.StartAnimation(this, ContentOffsetXProperty, newX, AnimationDuration);
				XAnimationHelper.StartAnimation(this, ContentOffsetYProperty, newY, AnimationDuration);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Анимация масштабирования с центром в указанной точке в координатах контента
			/// </summary>
			/// <param name="new_сontent_scale">Новый масштаб</param>
			/// <param name="content_zoom_focus">Точка масштабирования</param>
			//---------------------------------------------------------------------------------------------------------
			public void AnimatedZoomAboutPoint(Double new_сontent_scale, Point content_zoom_focus)
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
				// When zooming about a point make updates to ContentScale also update mContent offset.
				//
				mEnableContentOffsetUpdateFromScale = true;

				XAnimationHelper.StartAnimation(this, ContentScaleProperty, new_сontent_scale, AnimationDuration,
					delegate (Object sender, EventArgs args)
					{
						mEnableContentOffsetUpdateFromScale = false;

						ResetViewportZoomFocus();
					});
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Масштабирование с центром в указанной точке в координатах контента
			/// </summary>
			/// <param name="new_content_scale">Новый масштаб</param>
			/// <param name="content_zoom_focus">Точка масштабирования</param>
			//---------------------------------------------------------------------------------------------------------
			public void ZoomAboutPoint(Double new_content_scale, System.Windows.Point content_zoom_focus)
			{
				new_content_scale = Math.Min(Math.Max(new_content_scale, MinContentScale), MaxContentScale);

				var screenSpaceZoomOffsetX = (content_zoom_focus.X - ContentOffsetX) * ContentScale;
				var screenSpaceZoomOffsetY = (content_zoom_focus.Y - ContentOffsetY) * ContentScale;
				var contentSpaceZoomOffsetX = screenSpaceZoomOffsetX / new_content_scale;
				var contentSpaceZoomOffsetY = screenSpaceZoomOffsetY / new_content_scale;
				var newContentOffsetX = content_zoom_focus.X - contentSpaceZoomOffsetX;
				var newContentOffsetY = content_zoom_focus.Y - contentSpaceZoomOffsetY;

				XAnimationHelper.CancelAnimation(this, ContentScaleProperty);
				XAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
				XAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

				ContentScale = new_content_scale;
				ContentOffsetX = newContentOffsetX;
				ContentOffsetY = newContentOffsetY;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Анимация масштабирования по центру области просмотра
			/// </summary>
			/// <param name="content_scale">Масштаб</param>
			//---------------------------------------------------------------------------------------------------------
			public void AnimatedZoomTo(Double content_scale)
			{
				var zoom_center = new Point(ContentOffsetX + (ContentViewportWidth / 2), ContentOffsetY + (ContentViewportHeight / 2));
				AnimatedZoomAboutPoint(content_scale, zoom_center);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Mасштабированиe по центру области просмотра
			/// </summary>
			/// <param name="content_scale">Масштаб</param>
			//---------------------------------------------------------------------------------------------------------
			public void ZoomTo(Double content_scale)
			{
				var zoom_сenter = new Point(ContentOffsetX + (ContentViewportWidth / 2), ContentOffsetY + (ContentViewportHeight / 2));
				ZoomAboutPoint(content_scale, zoom_сenter);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Анимация масштабирования по размеру содержимого
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void AnimatedScaleToFit()
			{
				AnimatedZoomTo(new Rect(0, 0, mContent.ActualWidth, mContent.ActualHeight));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Масштабирование по размеру содержимого
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void ScaleToFit()
			{
				if (mContent == null)
				{
					throw new ApplicationException("PART_Content was not found in the LotusContentViewer visual template!");
				}

				ZoomTo(new Rect(0, 0, mContent.ActualWidth, mContent.ActualHeight));
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С УВЕЛИЧЕНИЕМ РЕГИОНА =======================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Первичная инициализация данных для работы с увеличением региона
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void InitZoomingRegion()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало операции увеличения региона
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void StartZoomingRegion()
			{
				if (mZoomingIsSupport)
				{
					mZoomingStarting = true;
					mZoomingStartPoint = new Point(MousePositionLeftDown.X, MousePositionLeftDown.Y);
					mZoomingRectCorrect.X = MousePositionLeftDown.X - mZoomingDragCorrect / 2;
					mZoomingRectCorrect.Y = MousePositionLeftDown.Y - mZoomingDragCorrect / 2;
					mZoomingRectCorrect.Width = mZoomingDragCorrect;
					mZoomingRectCorrect.Height = mZoomingDragCorrect;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Операции увеличения региона (вызывается в MouseMove)
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void ProcessZoomingRegion()
			{
				if (mZoomingIsSupport)
				{
					// Если есть выход за пределы корректировочного прямоугольника
					if (!mZoomingRectCorrect.Contains(MousePositionCurrent))
					{
						if (mOperationCurrent != TViewHandling.ZoomingRegion)
						{
							mOperationCurrent = TViewHandling.ZoomingRegion;
							mOperationDesc = "УВЕЛИЧЕНИЕ РЕГИОНА";
							NotifyPropertyChanged(PropertyArgsOperationDesc);
						}

						if (mZoomingStartPoint.X < MousePositionCurrent.X)
						{
							mZoomingLeftUpPoint.X = mZoomingStartPoint.X;
						}
						else
						{
							mZoomingLeftUpPoint.X = MousePositionCurrent.X;
						}

						if (mZoomingStartPoint.Y < MousePositionCurrent.Y)
						{
							mZoomingLeftUpPoint.Y = mZoomingStartPoint.Y;
						}
						else
						{
							mZoomingLeftUpPoint.Y = MousePositionCurrent.Y;
						}

						mZoomingRect.X = mZoomingLeftUpPoint.X;
						mZoomingRect.Y = mZoomingLeftUpPoint.Y;
						mZoomingRect.Width = Math.Abs(mZoomingStartPoint.X - MousePositionCurrent.X);
						mZoomingRect.Height = Math.Abs(mZoomingStartPoint.Y - MousePositionCurrent.Y);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание операции увеличения региона
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void EndZoomingRegion()
			{
				if (mZoomingIsSupport)
				{
					this.AnimatedZoomTo(mZoomingRect);
				}

				mZoomingStarting = false;
				mOperationCurrent = TViewHandling.None;
				mOperationDesc = "";
				NotifyPropertyChanged(PropertyArgsOperationDesc);
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С ВЫДЕЛЕНИЕМ РЕГИОНА ========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Первичная инициализация данных для работы с выделением региона
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void InitSelectingRegion()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало операции выделения региона
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void StartSelectingRegion()
			{
				if (mSelectingIsSupport)
				{
					mSelectingStarting = true;
					mSelectingStartPoint = new Point(MousePositionLeftDown.X, MousePositionLeftDown.Y);
					mSelectingRectCorrect.X = MousePositionLeftDown.X - mSelectingDragCorrect / 2;
					mSelectingRectCorrect.Y = MousePositionLeftDown.Y - mSelectingDragCorrect / 2;
					mSelectingRectCorrect.Width = mSelectingDragCorrect;
					mSelectingRectCorrect.Height = mSelectingDragCorrect;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Операция выделения региона (вызывается в MouseMove)
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void ProcessSelectingRegion()
			{
				if (mSelectingIsSupport)
				{
					// Если есть выход за пределы корректировочного прямоугольника
					if (!mSelectingRectCorrect.Contains(MousePositionCurrent))
					{
						if (mOperationCurrent != TViewHandling.SelectingRegion)
						{
							mOperationCurrent = TViewHandling.SelectingRegion;
							mOperationDesc = "ВЫДЕЛЕНИЕ РЕГИОНА" + mSelectingStartPoint.ToString();
							NotifyPropertyChanged(PropertyArgsOperationDesc);
						}

						if (mSelectingStartPoint.X < MousePositionCurrent.X)
						{
							mSelectingLeftUpPoint.X = mSelectingStartPoint.X;
							mSelectingRightToLeft = false;
						}
						else
						{
							mSelectingLeftUpPoint.X = MousePositionCurrent.X;
							mSelectingRightToLeft = true;
						}

						if (mSelectingStartPoint.Y < MousePositionCurrent.Y)
						{
							mSelectingLeftUpPoint.Y = mSelectingStartPoint.Y;
						}
						else
						{
							mSelectingLeftUpPoint.Y = MousePositionCurrent.Y;
						}

						mSelectingRect.X = (Single)mSelectingLeftUpPoint.X;
						mSelectingRect.Y = (Single)mSelectingLeftUpPoint.Y;
						mSelectingRect.Width = (Single)Math.Abs(mSelectingStartPoint.X - MousePositionCurrent.X);
						mSelectingRect.Height = (Single)Math.Abs(mSelectingStartPoint.Y - MousePositionCurrent.Y);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание операции выделения региона
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void EndSelectingRegion()
			{
				mSelectingStarting = false;
				mOperationCurrent = TViewHandling.None;
				mOperationDesc = "";
				NotifyPropertyChanged(PropertyArgsOperationDesc);
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С ПЕРЕМЕЩЕНИЕМ ОБЛАСТИ ПРОСМОТРА ============
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало операции перемещения области просмотра
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void StartPanning()
			{
				// Смещаем смотровое окно
				this.Cursor = Cursors.SizeAll;
				mOperationCurrent = TViewHandling.Panning;
				mOperationDesc = "СМЕЩЕНИЕ ОБЛАСТИ";
				NotifyPropertyChanged(PropertyArgsOperationDesc);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Операция перемещения области просмотра (вызывается в MouseMove)
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void ProcessPanning()
			{
				// Смещаем смотровое окно
				var drag_offset = new Vector((MousePositionCurrent - MousePositionMiddleDown).X,
					(MousePositionCurrent - MousePositionMiddleDown).Y);

				this.ContentOffsetX -= drag_offset.X;
				this.ContentOffsetY -= drag_offset.Y;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание операции перемещения области просмотра
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void EndPanning()
			{
				this.Cursor = Cursors.Arrow;
				mOperationCurrent = TViewHandling.None;
				mOperationDesc = "";
				NotifyPropertyChanged(PropertyArgsOperationDesc);
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ - КОНТЕНТ =============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Смещение окна просмотра
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void OnContentViewerContentOffsetChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение размеров окна просмотра
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void OnContentViewerContentSizeChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение масштаба контента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void OnContentViewerContentScaleChanged()
			{
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ - ДЕЙСТВИЯ ============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Элемент загружен
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void OnContentViewerLoaded(Object sender, RoutedEventArgs args)
			{
				if (mContent == null)
				{
					mContent = Content as FrameworkElement;
				}

				InitContentTransformation();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Нажатия кнопки мыши
			/// </summary>
			/// <param name="e">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnMouseDown(MouseButtonEventArgs e)
			{
				base.OnMouseDown(e);

				var status = mContent.Focus();
				Keyboard.Focus(mContent);

				// 1) Получаем позиции курсора в координатах канвы
				MousePositionCurrent = mContentTotalTransform.Inverse.Transform(e.GetPosition(this)).ToVector2Df();

				// 2) Сохраняем текущую операцию
				mOperationPreview = mOperationCurrent;

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
					if (ContextMenu != null)
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
				if (mOperationCurrent != TViewHandling.None)
				{
					CaptureMouse();
					e.Handled = true;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение курсора мыши
			/// </summary>
			/// <param name="e">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnMouseMove(MouseEventArgs e)
			{
				base.OnMouseMove(e);

				// Получаем текущие координаты
				var current_content = mContentTotalTransform.Inverse.Transform(e.GetPosition(this)).ToVector2Df();

				// Смотрим смещение
				MouseDeltaCurrent = current_content - MousePositionCurrent;

				// Обновляем координаты
				MousePositionCurrent = current_content;

				// Если зажата левая кнопка мыши
				if (e.LeftButton == MouseButtonState.Pressed)
				{
					// Увеличение региона
					if (mZoomingStarting)
					{
						ProcessZoomingRegion();
					}
					else
					{
						if (mSelectingStarting)
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
						if (mOperationCurrent == TViewHandling.Panning)
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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отпускание кнопки мыши
			/// </summary>
			/// <param name="e">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnMouseUp(MouseButtonEventArgs e)
			{
				base.OnMouseUp(e);

				if (e.ChangedButton == MouseButton.Left)
				{
					if (mOperationCurrent == TViewHandling.ZoomingRegion)
					{
						EndZoomingRegion();
					}
					else
					{
						if (mOperationCurrent == TViewHandling.SelectingRegion)
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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вращение колеса мыши
			/// </summary>
			/// <param name="e">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnMouseWheel(MouseWheelEventArgs e)
			{
				base.OnMouseWheel(e);

				e.Handled = true;

				if (e.Delta > 0)
				{
					Point curContentMousePoint = e.GetPosition(mContent);
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
					Point curContentMousePoint = e.GetPosition(mContent);
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