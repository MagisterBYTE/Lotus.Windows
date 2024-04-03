using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFValueConverters
	*@{*/
    /// <summary>
    /// Конвертер типа Rect в строку.
    /// </summary>
    [ValueConversion(typeof(Size), typeof(string))]
    public class RectToStringConverter : IValueConverter
    {
        /// <summary>
        /// Конвертация типа Rect в строковый тип.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Строка.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (Rect)value;
            return "X = " + val.X.ToString("F2") + "; Y = " + val.Y.ToString("F2") + "; Width = " +
                val.Width.ToString("F2") + "; Height = " + val.Height.ToString("F2") + ";";
        }

        /// <summary>
        /// Конвертация строкового типа в тип Rect.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>тип Rect.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
    }
    /**@}*/
}