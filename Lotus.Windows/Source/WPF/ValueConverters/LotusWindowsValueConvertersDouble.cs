using System;
using System.Globalization;
using System.Windows.Data;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFValueConverters
	*@{*/
    /// <summary>
    /// Конвертер вещественного типа в строку.
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToStringConverter : IValueConverter
    {
        #region Methods 
        /// <summary>
        /// Конвертация вещественного типа в строковый тип.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Строка.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)value;

            if (parameter != null)
            {
                return val.ToString(parameter.ToString());
            }
            else
            {
                return val.ToString("G");
            }
        }

        /// <summary>
        /// Конвертация строкового типа в вещественный тип.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Вещественный тип.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string)value;

            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }
            else
            {
                str = str.Trim();
                return XNumbers.ParseDouble(str);
            }
        }
        #endregion
    }

    /// <summary>
    /// Конвертер для изменения вещественного значения через параметр.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class DoubleOffsetConverter : IValueConverter
    {
        /// <summary>
        /// Изменение вещественного значения через параметр.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Вещественный тип.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)value;

            if (parameter != null)
            {
                return val - XNumbers.ParseDouble(parameter.ToString()!);
            }
            else
            {
                return val;
            }
        }

        /// <summary>
        /// Изменение вещественного значения через параметр.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Вещественный тип.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)value;

            if (parameter != null)
            {
                return val + XNumbers.ParseDouble(parameter.ToString()!);
            }
            else
            {
                return val;
            }
        }
    }
    /**@}*/
}