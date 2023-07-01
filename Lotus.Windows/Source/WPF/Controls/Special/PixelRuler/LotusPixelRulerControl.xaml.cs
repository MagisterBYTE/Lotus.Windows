//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Специальные элементы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusPixelRulerControl.xaml.cs
*		Элемент - Линейка.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFControls
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Единица измерения линейки
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(EnumToStringConverter<TRulerDimensionType>))]
		public enum TRulerDimensionType
		{
			/// <summary>
			/// Аппаратно-независимые единицы (96 точек на дюйм)
			/// </summary>
			[Description("Аппаратная единица")]
			DeviceUnit,

			/// <summary>
			/// Миллиметр
			/// </summary>
			[Description("Миллиметр")]
			Milliliter,

			/// <summary>
			/// Сантиметр
			/// </summary>
			[Description("Сантиметр")]
			Centimeter,

			/// <summary>
			/// Пользовательская единица измерения
			/// </summary>
			/// <remarks>
			/// Будет считается как масштаб к единице измерения миллиметр
			/// </remarks>
			[Description("Пользовательская единица")]
			User
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Расположение маркеров линейки
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(EnumToStringConverter<TRulerDimensionType>))]
		public enum TRulerMarksLocation
		{
			/// <summary>
			/// Сверху
			/// </summary>
			[Description("Сверху")]
			Up,

			/// <summary>
			/// Снизу
			/// </summary>
			[Description("Снизу")]
			Down
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент - Линейка
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusPixelRulerControl : FrameworkElement
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Длина линейки
			/// </summary>
			public static readonly DependencyProperty LengthProperty =
				DependencyProperty.Register(nameof(Length), typeof(Double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(20D, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Авто размер линейки
			/// </summary>
			public static readonly DependencyProperty AutoSizeProperty =
				DependencyProperty.Register(nameof(AutoSize), typeof(Boolean), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Масштаб основных единиц
			/// </summary>
			public static readonly DependencyProperty ZoomProperty =
				DependencyProperty.Register(nameof(Zoom), typeof(Double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata((Double)1.0, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Малый шаг маркеров основных единиц
			/// </summary>
			public static readonly DependencyProperty SmallStepProperty =
				DependencyProperty.Register(nameof(SmallStep), typeof(Double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata((Double)5.0, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Основной шаг маркеров основных единиц
			/// </summary>
			public static readonly DependencyProperty StepProperty =
				DependencyProperty.Register(nameof(Step), typeof(Double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata((Double)10, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Смещение единиц основных единиц
			/// </summary>
			public static readonly DependencyProperty OffsetProperty =
				DependencyProperty.Register(nameof(Offset), typeof(Double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata((Double)0.0, FrameworkPropertyMetadataOptions.AffectsRender));


			/// <summary>
			/// Дополнительное начальное фиксированное смещение для отрисовки основных единиц
			/// </summary>
			public static readonly DependencyProperty UnitFixedOffsetProperty =
				DependencyProperty.Register(nameof(UnitFixedOffset), typeof(Double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata((Double)0.0, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Начальное смещение для отрисовки основных единиц
			/// </summary>
			public static readonly DependencyProperty UnitStartOffsetProperty =
				DependencyProperty.Register(nameof(UnitStartOffset), typeof(Double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata((Double)0.0, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Полная длина отрисовки
			/// </summary>
			public static readonly DependencyProperty UnitLengthProperty =
				DependencyProperty.Register(nameof(UnitLength), typeof(Double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata((Double)0.0, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Расположение маркеров
			/// </summary>
			public static readonly DependencyProperty MarksLocationProperty =
				DependencyProperty.Register(nameof(MarksLocation), typeof(TRulerMarksLocation), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(TRulerMarksLocation.Up, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Единица измерения
			/// </summary>
			public static readonly DependencyProperty DimensionTypeProperty =
				DependencyProperty.Register(nameof(DimensionType), typeof(TRulerDimensionType), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(TRulerDimensionType.DeviceUnit, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Единица измерения
			/// </summary>
			public static readonly DependencyProperty DimensionUserProperty =
				DependencyProperty.Register(nameof(DimensionUser), typeof(Double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			private Typeface mTypefaceNumber;
			private Double mSegmentHeight;
			private Pen mThinPen = new Pen(Brushes.Black, 1.0);
			private Pen mBorderPen = new Pen(Brushes.Gray, 1.0);
			private ScaleTransform mTransformScale;
			private TranslateTransform mTransformOffset;
			private TransformGroup mTransformGroup;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Получает или задает длину линейки. Если <see cref="AutoSize"/> свойство установлено в ложное (по умолчанию) 
			/// это фиксированная длина. В противном случае длина вычисляется на основе фактической ширины элемента линейки
			/// </summary>
			public Double Length
			{
				get
				{
					if (AutoSize)
					{
						return ActualWidth;
					}
					else
					{
						return (Double)GetValue(LengthProperty);
					}
				}
				set { SetValue(LengthProperty, value); }
			}

			/// <summary>
			/// Получает или задает поведение AutoSize линейки.
			/// - False (по умолчанию): длина результатам линейки от <see cref="Length"/> собственной ширины элемента.
			/// Если размер окна изменяется, например, шире, чем длина линейки, то будет показано свободное пространство в конце линейки.
			/// Масштабирование не делается.
			/// - True: длина линейки всегда доводится до фактической ширины родительского элемента. Это гарантирует, что линейка будет
			/// показана на всю ширину родительского элемента.
			/// </summary>
			public Boolean AutoSize
			{
				get { return (Boolean)GetValue(AutoSizeProperty); }
				set
				{
					SetValue(AutoSizeProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Масштаб отображения основных единиц
			/// </summary>
			public Double Zoom
			{
				get { return (Double)GetValue(ZoomProperty); }
				set
				{
					SetValue(ZoomProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Малый шаг отображения основных единиц
			/// </summary>
			public Double SmallStep
			{
				get { return (Double)GetValue(SmallStepProperty); }
				set
				{
					SetValue(SmallStepProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Основной шаг отображения основных единиц
			/// </summary>
			public Double Step
			{
				get { return (Double)GetValue(StepProperty); }
				set
				{
					SetValue(StepProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Смещение основных единиц
			/// </summary>
			public Double Offset
			{
				get { return (Double)GetValue(OffsetProperty); }
				set
				{
					SetValue(OffsetProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Дополнительное начальное фиксированное смещение для отрисовки основных единиц
			/// </summary>
			/// <remarks>
			/// Применяется для выравнивания относительно других элементов и определения нулю отсчета
			/// Измеряется в аппаратно-независимых единицах
			/// </remarks>
			public Double UnitFixedOffset
			{
				get { return (Double)GetValue(UnitFixedOffsetProperty); }
				set
				{
					SetValue(UnitFixedOffsetProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Начальное масштабируемое смещение для отрисовки основных единиц
			/// </summary>
			/// <remarks>
			/// Применяется для выравнивания относительно других элементов и определения нулю отсчета
			/// Измеряется в аппаратно-независимых единицах
			/// </remarks>
			public Double UnitStartOffset
			{
				get { return (Double)GetValue(UnitStartOffsetProperty); }
				set
				{
					SetValue(UnitStartOffsetProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Cмещение для отрисовки основных единиц
			/// </summary>
			public Double UnitOffset
			{
				get { return mTransformOffset.X; }
				set
				{
					mTransformOffset.X = value;
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Полная длина отрисовки
			/// </summary>
			public Double UnitLength
			{
				get { return (Double)GetValue(UnitLengthProperty); }
				set
				{
					SetValue(UnitLengthProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Масштаб увеличения по X
			/// </summary>
			public Double UnitScaleX
			{
				get { return mTransformScale.ScaleX; }
				set
				{
					mTransformScale.ScaleX = value;
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Масштаб увеличения по Y
			/// </summary>
			public Double UnitScaleY
			{
				get { return mTransformScale.ScaleY; }
				set
				{
					mTransformScale.ScaleY = value;
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Смещение точки при увеличения по X
			/// </summary>
			public Double UnitScaleCenterX
			{
				get { return mTransformScale.CenterX; }
				set
				{
					mTransformScale.CenterX = value;
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Смещение точки при увеличения по Y
			/// </summary>
			public Double UnitScaleCenterY
			{
				get { return mTransformScale.CenterY; }
				set
				{
					mTransformScale.CenterY = value;
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Расположение маркеров
			/// </summary>
			public TRulerMarksLocation MarksLocation
			{
				get { return (TRulerMarksLocation)GetValue(MarksLocationProperty); }
				set { SetValue(MarksLocationProperty, value); }
			}

			/// <summary>
			/// Единица измерения
			/// </summary>
			public TRulerDimensionType DimensionType
			{
				get { return (TRulerDimensionType)GetValue(DimensionTypeProperty); }
				set 
				{
					SetValue(DimensionTypeProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Масштаб пользовательской единицы измерения к миллиметру
			/// </summary>
			public Double DimensionUser
			{
				get { return (Double)GetValue(DimensionUserProperty); }
				set
				{
					SetValue(DimensionUserProperty, value);
					InvalidateVisual();
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Статический конструктор
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			static LotusPixelRulerControl()
			{
				HeightProperty.OverrideMetadata(typeof(LotusPixelRulerControl), new FrameworkPropertyMetadata(20.0));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusPixelRulerControl()
			{
				InitializeComponent();
				mTypefaceNumber = new Typeface("Arial");
				mSegmentHeight = Height - 10;
				UnitStartOffset = 5;
				SnapsToDevicePixels = true;
				mTransformGroup = new TransformGroup();
				mTransformOffset = new TranslateTransform(0, 0);
				mTransformScale = new ScaleTransform();
				mTransformGroup.Children.Add(mTransformScale);
				mTransformGroup.Children.Add(mTransformOffset);
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод в аппаратно-независимые единицы
			/// </summary>
			/// <param name="value">Значение</param>
			/// <returns>Размер в аппаратно-независимых единицах</returns>
			//---------------------------------------------------------------------------------------------------------
			private Double ToDeviceUnits(Double value)
			{
				switch (DimensionType)
				{
					case TRulerDimensionType.DeviceUnit: return value;
					case TRulerDimensionType.Milliliter: return value * 3.77952;
					case TRulerDimensionType.Centimeter: return value * 37.795;
					case TRulerDimensionType.User: return value * 3.77952 / DimensionUser;
					default:
						break;
				}

				return value;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод аппаратно-независимых единиц в соответствующую единицу измерения
			/// </summary>
			/// <param name="device_unit">Размер в аппаратно-независимых единицах</param>
			/// <returns>Значение в соответствующих единицах измерения</returns>
			//---------------------------------------------------------------------------------------------------------
			private Double ToValueDimension(Double device_unit)
			{
				switch (DimensionType)
				{
					case TRulerDimensionType.DeviceUnit: return device_unit;
					case TRulerDimensionType.Milliliter: return device_unit * 0.26458;
					case TRulerDimensionType.Centimeter: return device_unit * 0.02645;
					case TRulerDimensionType.User: return device_unit * 0.26458 * DimensionUser;
					default:
						break;
				}

				return device_unit;
			}
			#endregion

			#region ======================================= ПЕРЕГРУЖЕННЫЕ МЕТОДЫ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование элемента
			/// </summary>
			/// <param name="drawingContext">Контекст команд рисования</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnRender(DrawingContext drawingContext)
			{
				base.OnRender(drawingContext);

				var bounds = new Rect(0, 0, ActualWidth, ActualHeight);
				drawingContext.DrawRectangle(null, mBorderPen, bounds);
				var rect_clip = new RectangleGeometry(bounds);

				// 0) Начальное смещение
				var x = UnitFixedOffset * (1.0 / Zoom) + UnitStartOffset;

				// 1) Количество отрисовок
				var small_step = ToDeviceUnits(SmallStep);
				var step = ToDeviceUnits(Step);
				var count = (Int32)(UnitLength /small_step + 0.5);

				// 3) Размер шрифта и пера
				mThinPen.Thickness = 1.0 / Zoom * 1;
				mBorderPen.Thickness = 1.0 / Zoom * 1;
				var size_text = 1.0 / Zoom * 12;
				var segment_height = 1.0 / Zoom * mSegmentHeight;

				// 4) Рисуем
				drawingContext.PushClip(rect_clip);
				drawingContext.PushTransform(mTransformGroup);
				
				Double current_length = 0;
				Double current_value = 0;
				for (var i = 0; i < count; i++)
				{
					current_length = small_step * i;
					current_value = SmallStep * i;

					// 5) Если делится без остатка на основной шаг то рисуем его
					var is_main = ((Int32)current_value % (Int32)Step) == 0;

					Double start_height;
					Double end_height;
					if (MarksLocation == TRulerMarksLocation.Up)
					{
						start_height = 0;

						// Определение шага
						end_height = is_main ? segment_height : segment_height / 2;
					}
					else
					{
						start_height = Height;

						// Определение шага
						end_height = is_main ? segment_height : segment_height * 1.5;
					}

					var p1 = new Point(x + current_length, start_height);
					var p2 = new Point(x + current_length, end_height);

					drawingContext.DrawLine(mThinPen, p1, p2);

					// 6) Рисуем текст
					if(is_main || (Zoom > 5 && is_main == false))
					{
						var number = new FormattedText((i * SmallStep + Offset).ToString("F0"),
							CultureInfo.CurrentCulture, 
							FlowDirection.LeftToRight,
							mTypefaceNumber, size_text, 
							Brushes.DimGray,
							VisualTreeHelper.GetDpi(this).PixelsPerDip);

						number.SetFontWeight(FontWeights.Regular);
						number.TextAlignment = TextAlignment.Center;

						if (MarksLocation == TRulerMarksLocation.Up)
						{
							drawingContext.DrawText(number, new Point(x + current_length, (Height * 1.0 / Zoom) - number.Height));
						}
						else
						{
							drawingContext.DrawText(number, new Point(x + current_length, (Height * 1.0 / Zoom) - segment_height - number.Height));
						}
					}
				}
				drawingContext.Pop();
				drawingContext.Pop();
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================