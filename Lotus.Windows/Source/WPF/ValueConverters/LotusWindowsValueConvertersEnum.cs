using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFValueConverters
	*@{*/
    /// <summary>
    /// Конвертер для <see cref="Enum"/> в целочисленный тип.
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(int))]
    public class EnumToIntConverter : IValueConverter
    {
        /// <summary>
        /// Конвертация типа Enum в целочисленный тип.
        /// </summary>
        /// <param name="value">Значение тип Enum.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Целочисленный тип.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            var values = Enum.GetValues(value.GetType());
            for (var i = 0; i < values.Length; i++)
            {
                if (values.GetValue(i)!.ToString() == value.ToString())
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// Конвертация целочисленного типа в тип Enum.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Тип Enum.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var values = Enum.GetValues(targetType);
            var index_value = 0;
            if (value != null)
            {
                index_value = (int)value;
            }

            return values.GetValue(index_value)!;
        }
    }

    /// <summary>
    /// Конвертер для <see cref="Enum"/>, преобразовывающий Enum к строке с учетом атрибута <see cref="DescriptionAttribute"/>.
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToStringConverter : IValueConverter
    {
        public static readonly EnumToStringConverter Instance = new EnumToStringConverter();

        /// <summary>
        /// Конвертация типа Enum в строковый тип.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Строка.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            var type_enum = value.GetType();
            return XEnum.GetDescriptionOrName(type_enum, (Enum)value);
        }

        /// <summary>
        /// Конвертация строкового типа в тип Enum.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Тип Enum.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null!;
            }
            return XEnum.ConvertFromDescriptionOrName(targetType, value.ToString()!);
        }
    }
    /**@}*/
}