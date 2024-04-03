using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFValueConverters
	*@{*/
    /// <summary>
    /// Конвертер типа Size в строку.
    /// </summary>
    [ValueConversion(typeof(Size), typeof(string))]
    public class SizeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Конвертация типа Size в строковый тип.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Строка.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (Size)value;
            return "Ширина = " + val.Width.ToString("F0") + "; Высота = " + val.Height.ToString("F0") + ";";
        }

        /// <summary>
        /// Конвертация строкового типа в тип Size.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>тип Size.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
    }
    /**@}*/
}