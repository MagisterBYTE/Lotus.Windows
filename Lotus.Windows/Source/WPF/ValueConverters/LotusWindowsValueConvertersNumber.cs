using System;
using System.Globalization;
using System.Windows.Data;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFValueConverters
	*@{*/
    /// <summary>
    /// Универсальный конвертор для числовых значений в тип Double.
    /// </summary>
    public class NumberToDoubleConverter : IValueConverter
    {
        #region Methods 
        /// <summary>
        /// Конвертация числового значения в тип Double.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Значение.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value);
        }

        /// <summary>
        /// Конвертация объекта типа Double в соответствующий числовой тип.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр(реальный тип для преобразования).</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Значение.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return XConverter.ToNumber(targetType, (double)value);
        }
        #endregion
    }

    /// <summary>
    /// Универсальный конвертор для числовых значений в тип Decimal.
    /// </summary>
    public class NumberToDecimalConverter : IValueConverter
    {
        #region Methods 
        /// <summary>
        /// Конвертация числового значения в тип Decimal.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Значение.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDecimal(value);
        }

        /// <summary>
        /// Конвертация объекта типа Decimal в соответствующий числовой тип.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр(реальный тип для преобразования).</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Значение.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return XConverter.ToNumber(targetType, (decimal)value);
        }
        #endregion
    }
    /**@}*/
}