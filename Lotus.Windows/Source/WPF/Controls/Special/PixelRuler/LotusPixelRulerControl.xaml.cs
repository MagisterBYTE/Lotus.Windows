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
				DependencyProperty.Register(nameof(Length), typeof(double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(20D, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Авто размер линейки
			/// </summary>
			public static readonly DependencyProperty AutoSizeProperty =
				DependencyProperty.Register(nameof(AutoSize), typeof(bool), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Масштаб основных единиц
			/// </summary>
			public static readonly DependencyProperty ZoomProperty =
				DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Малый шаг маркеров основных единиц
			/// </summary>
			public static readonly DependencyProperty SmallStepProperty =
				DependencyProperty.Register(nameof(SmallStep), typeof(double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Основной шаг маркеров основных единиц
			/// </summary>
			public static readonly DependencyProperty StepProperty =
				DependencyProperty.Register(nameof(Step), typeof(double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata((double)10, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Смещение единиц основных единиц
			/// </summary>
			public static readonly DependencyProperty OffsetProperty =
				DependencyProperty.Register(nameof(Offset), typeof(double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));


			/// <summary>
			/// Дополнительное начальное фиксированное смещение для отрисовки основных единиц
			/// </summary>
			public static readonly DependencyProperty UnitFixedOffsetProperty =
				DependencyProperty.Register(nameof(UnitFixedOffset), typeof(double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Начальное смещение для отрисовки основных единиц
			/// </summary>
			public static readonly DependencyProperty UnitStartOffsetProperty =
				DependencyProperty.Register(nameof(UnitStartOffset), typeof(double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

			/// <summary>
			/// Полная длина отрисовки
			/// </summary>
			public static readonly DependencyProperty UnitLengthProperty =
				DependencyProperty.Register(nameof(UnitLength), typeof(double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

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
				DependencyProperty.Register(nameof(DimensionUser), typeof(double), typeof(LotusPixelRulerControl),
				new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			protected internal Typeface _typefaceNumber;
			protected internal double _segmentHeight;
			protected internal Pen _thinPen = new Pen(Brushes.Black, 1.0);
			protected internal Pen _borderPen = new Pen(Brushes.Gray, 1.0);
			protected internal ScaleTransform _transformScale;
			protected internal TranslateTransform _transformOffset;
			protected internal TransformGroup _transformGroup;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Получает или задает длину линейки. Если <see cref="AutoSize"/> свойство установлено в ложное (по умолчанию) 
			/// это фиксированная длина. В противном случае длина вычисляется на основе фактической ширины элемента линейки
			/// </summary>
			public double Length
			{
				get
				{
					if (AutoSize)
					{
						return ActualWidth;
					}
					else
					{
						return (double)GetValue(LengthProperty);
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
			public bool AutoSize
			{
				get { return (bool)GetValue(AutoSizeProperty); }
				set
				{
					SetValue(AutoSizeProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Масштаб отображения основных единиц
			/// </summary>
			public double Zoom
			{
				get { return (double)GetValue(ZoomProperty); }
				set
				{
					SetValue(ZoomProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Малый шаг отображения основных единиц
			/// </summary>
			public double SmallStep
			{
				get { return (double)GetValue(SmallStepProperty); }
				set
				{
					SetValue(SmallStepProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Основной шаг отображения основных единиц
			/// </summary>
			public double Step
			{
				get { return (double)GetValue(StepProperty); }
				set
				{
					SetValue(StepProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Смещение основных единиц
			/// </summary>
			public double Offset
			{
				get { return (double)GetValue(OffsetProperty); }
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
			public double UnitFixedOffset
			{
				get { return (double)GetValue(UnitFixedOffsetProperty); }
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
			public double UnitStartOffset
			{
				get { return (double)GetValue(UnitStartOffsetProperty); }
				set
				{
					SetValue(UnitStartOffsetProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Cмещение для отрисовки основных единиц
			/// </summary>
			public double UnitOffset
			{
				get { return _transformOffset.X; }
				set
				{
					_transformOffset.X = value;
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Полная длина отрисовки
			/// </summary>
			public double UnitLength
			{
				get { return (double)GetValue(UnitLengthProperty); }
				set
				{
					SetValue(UnitLengthProperty, value);
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Масштаб увеличения по X
			/// </summary>
			public double UnitScaleX
			{
				get { return _transformScale.ScaleX; }
				set
				{
					_transformScale.ScaleX = value;
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Масштаб увеличения по Y
			/// </summary>
			public double UnitScaleY
			{
				get { return _transformScale.ScaleY; }
				set
				{
					_transformScale.ScaleY = value;
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Смещение точки при увеличения по X
			/// </summary>
			public double UnitScaleCenterX
			{
				get { return _transformScale.CenterX; }
				set
				{
					_transformScale.CenterX = value;
					InvalidateVisual();
				}
			}

			/// <summary>
			/// Смещение точки при увеличения по Y
			/// </summary>
			public double UnitScaleCenterY
			{
				get { return _transformScale.CenterY; }
				set
				{
					_transformScale.CenterY = value;
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
			public double DimensionUser
			{
				get { return (double)GetValue(DimensionUserProperty); }
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
				_typefaceNumber = new Typeface("Arial");
				_segmentHeight = Height - 10;
				UnitStartOffset = 5;
				SnapsToDevicePixels = true;
				_transformGroup = new TransformGroup();
				_transformOffset = new TranslateTransform(0, 0);
				_transformScale = new ScaleTransform();
				_transformGroup.Children.Add(_transformScale);
				_transformGroup.Children.Add(_transformOffset);
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
			private double ToDeviceUnits(double value)
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
			private double ToValueDimension(double device_unit)
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
				drawingContext.DrawRectangle(null, _borderPen, bounds);
				var rect_clip = new RectangleGeometry(bounds);

				// 0) Начальное смещение
				var x = UnitFixedOffset * (1.0 / Zoom) + UnitStartOffset;

				// 1) Количество отрисовок
				var small_step = ToDeviceUnits(SmallStep);
				_ = ToDeviceUnits(Step);
				var count = (int)(UnitLength /small_step + 0.5);

				// 3) Размер шрифта и пера
				_thinPen.Thickness = 1.0 / Zoom * 1;
				_borderPen.Thickness = 1.0 / Zoom * 1;
				var size_text = 1.0 / Zoom * 12;
				var segment_height = 1.0 / Zoom * _segmentHeight;

				// 4) Рисуем
				drawingContext.PushClip(rect_clip);
				drawingContext.PushTransform(_transformGroup);

				double current_length;
				double current_value;
				for (var i = 0; i < count; i++)
				{
					current_length = small_step * i;
					current_value = SmallStep * i;

					// 5) Если делится без остатка на основной шаг то рисуем его
					var is_main = ((int)current_value % (int)Step) == 0;

					double start_height;
					double end_height;
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

					drawingContext.DrawLine(_thinPen, p1, p2);

					// 6) Рисуем текст
					if(is_main || (Zoom > 5 && is_main == false))
					{
						var number = new FormattedText((i * SmallStep + Offset).ToString("F0"),
							CultureInfo.CurrentCulture, 
							FlowDirection.LeftToRight,
							_typefaceNumber, size_text, 
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