//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Конвертеры данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsValueConvertersBoolean.cs
*		Конвертеры типа Boolean в различные типы данных.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Automation;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/**
         * \defgroup WindowsWPFValueConverters Конвертеры данных
         * \ingroup WindowsWPF
         * \brief Конвертеры данных.
         * @{
         */
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер логического значения в соответствующую графическую пиктограмму
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(Boolean), typeof(BitmapSource))]
		public class BooleanToBitmapSourceConverter : IValueConverter
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Пиктограмма для истинного значения
			/// </summary>
			public BitmapSource BitmapYes { get; set; }

			/// <summary>
			/// Пиктограмма для ложного значения
			/// </summary>
			public BitmapSource BitmapNo { get; set; }
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в тип Image
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Тип Image</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var val = (Boolean)value;
				if (val)
				{
					return BitmapYes;
				}
				else
				{
					return BitmapNo;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация типа Image в логическое значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Логический тип</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				return null!;
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер логического значения в обратное значение
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(Boolean), typeof(Boolean))]
		public class BooleanInverseConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в обратное значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Обратное значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var val = (Boolean)value;
				return !val;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в обратное значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>братное значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var val = (Boolean)value;
				return val;
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер логического значения в три состояния выбора
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(Boolean), typeof(ToggleState))]
		public class BooleanToToggleStateConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в состояние выбора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Cостояние выбора</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				if (value is bool?)
				{
					var nullableBoolValue = (bool?)value;
					if (nullableBoolValue.HasValue)
						return nullableBoolValue.Value ? ToggleState.On : ToggleState.Off;
					return ToggleState.Indeterminate;
				}
				else if (value is bool)
					return ((bool)value) ? ToggleState.On : ToggleState.Off;

				return ToggleState.Indeterminate;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация состояние выбора в логическое значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Логическое значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				if (value is ToggleState toggleStat)
				{
					switch (toggleStat)
					{
						case ToggleState.On:
							return true;
						case ToggleState.Off:
							return false;
					}
				}
				return ToggleState.On;
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер логического значения в тип <see cref="Visibility"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(Boolean), typeof(Visibility))]
		public class BooleanToVisibilityConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в состояние выбора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Cостояние выбора</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Boolean)value;
				if (status)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Hidden;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация состояние выбора в логическое значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Логическое значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Visibility)value;
				if (status == Visibility.Visible)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер логического значения в тип <see cref="Visibility"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(Boolean), typeof(Visibility))]
		public class BooleanInverseToVisibilityConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в состояние выбора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Cостояние выбора</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Boolean)value;
				if (!status)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Hidden;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация состояние выбора в логическое значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Логическое значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Visibility)value;
				if (status == Visibility.Visible)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер логического значения в тип <see cref="Visibility"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(Boolean?), typeof(Visibility))]
		public class BooleanNullToVisibilityConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в состояние выбора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Cостояние выбора</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Boolean?)value;
				if (status.HasValue && status.Value)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Hidden;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация состояние выбора в логическое значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Логическое значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Visibility)value;
				if (status == Visibility.Visible)
				{
					return new Boolean?(true);
				}
				else
				{
					return new Boolean?(false);
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер типа <see cref="Visibility"/> в логического значения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(Visibility), typeof(Boolean?))]
		public class VisibilityToBooleanNullConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в состояние выбора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Cостояние выбора</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Visibility)value;
				if (status == Visibility.Visible)
				{
					return new Boolean?(true);
				}
				else
				{
					return new Boolean?(false);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация состояние выбора в логическое значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Логическое значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Boolean?)value;
				if (status.HasValue && status.Value)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Hidden;
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер логического значения в тип <see cref="Visibility"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(Boolean), typeof(Visibility))]
		public class BooleanTrueToCollapsedConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в состояние выбора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Cостояние выбора</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Boolean)value;
				if (status)
				{
					return Visibility.Collapsed;
				}
				else
				{
					return Visibility.Visible;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация состояние выбора в логическое значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Логическое значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Visibility)value;
				if (status == Visibility.Collapsed)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер логического значения в тип <see cref="Visibility"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(Boolean), typeof(Visibility))]
		public class BooleanFalseToCollapsedConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация логического значения в состояние выбора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Cостояние выбора</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Boolean)value;
				if (status == false)
				{
					return Visibility.Collapsed;
				}
				else
				{
					return Visibility.Visible;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация состояние выбора в логическое значение
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Логическое значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
			{
				var status = (Visibility)value;
				if (status == Visibility.Collapsed)
				{
					return false;
				}
				else
				{
					return true;
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