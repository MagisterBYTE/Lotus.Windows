using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFValueConverters
	*@{*/
    /// <summary>
    /// Конвертер типа Point в строку.
    /// </summary>
    [ValueConversion(typeof(Point), typeof(string))]
    public class PointToStringConverter : IValueConverter
    {
        #region Methods 
        /// <summary>
        /// Конвертация типа Point в строковый тип.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Строка.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (Point)value;
            return "X = " + val.X.ToString("F2") + "; Y = " + val.Y.ToString("F2") + ";";
        }

        /// <summary>
        /// Конвертация строкового типа в тип Point.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>тип Point.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
        #endregion
    }
    /**@}*/
}