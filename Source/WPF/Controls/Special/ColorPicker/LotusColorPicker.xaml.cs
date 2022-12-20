//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Специальные элементы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusColorPicker.xaml.cs
*		Элемент для выбора цвета.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup WindowsWPFControlsSpecial
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент для выбора цвета
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusColorPicker : UserControl
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Свойство цвет
			/// </summary>
			public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), 
				typeof(Color), typeof(LotusColorPicker), 
				new FrameworkPropertyMetadata(Colors.Black, new PropertyChangedCallback(OnColorChanged)));

			/// <summary>
			/// Красная компонента цвета
			/// </summary>
			public static readonly DependencyProperty RedProperty = DependencyProperty.Register(nameof(Red), 
				typeof(Byte), typeof(LotusColorPicker),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorRGBChanged)));

			/// <summary>
			/// Зеленая компонента цвета
			/// </summary>
			public static readonly DependencyProperty GreenProperty = DependencyProperty.Register(nameof(Green), 
				typeof(Byte), typeof(LotusColorPicker),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorRGBChanged)));

			/// <summary>
			/// Синия компонента цвета
			/// </summary>
			public static readonly DependencyProperty BlueProperty = DependencyProperty.Register(nameof(Blue), 
				typeof(Byte), typeof(LotusColorPicker),
				new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorRGBChanged)));

			/// <summary>
			/// Событие изменения цвета
			/// </summary>
			public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent(nameof(ColorChanged),
				RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(LotusColorPicker));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения цвета
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
			{
				Color old_color = (Color)args.OldValue;
				Color new_сolor = (Color)args.NewValue;
				LotusColorPicker color_picker = (LotusColorPicker)sender;
				color_picker.Red = new_сolor.R;
				color_picker.Green = new_сolor.G;
				color_picker.Blue = new_сolor.B;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения компонентов цвета
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void OnColorRGBChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
			{
				LotusColorPicker color_picker = (LotusColorPicker)sender;
				Color color = color_picker.Color;
				if (args.Property == RedProperty)
					color.R = (Byte)args.NewValue;
				else if (args.Property == GreenProperty)
					color.G = (Byte)args.NewValue;
				else if (args.Property == BlueProperty)
					color.B = (Byte)args.NewValue;

				color_picker.Color = color;
			}
			#endregion

			#region ======================================= ДАННЫЕ ====================================================

			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Цвет
			/// </summary>
			public Color Color
			{
				get { return (Color)GetValue(ColorProperty); }
				set { SetValue(ColorProperty, value); }
			}

			/// <summary>
			/// Красная компонента цвета
			/// </summary>
			public Byte Red
			{
				get { return (Byte)GetValue(RedProperty); }
				set { SetValue(RedProperty, value); }
			}

			/// <summary>
			/// Зеленая компонента цвета
			/// </summary>
			public Byte Green
			{
				get { return (Byte)GetValue(GreenProperty); }
				set { SetValue(GreenProperty, value); }
			}

			/// <summary>
			/// Синия компонента цвета
			/// </summary>
			public Byte Blue
			{
				get { return (Byte)GetValue(BlueProperty); }
				set { SetValue(BlueProperty, value); }
			}

			/// <summary>
			/// Событие изменения цвета
			/// </summary>
			public event RoutedPropertyChangedEventHandler<Color> ColorChanged
			{
				add { AddHandler(ColorChangedEvent, value); }
				remove { RemoveHandler(ColorChangedEvent, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusColorPicker()
			{
				InitializeComponent();
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================