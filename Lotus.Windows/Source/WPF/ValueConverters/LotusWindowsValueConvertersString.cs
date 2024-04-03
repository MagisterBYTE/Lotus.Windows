using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFValueConverters
	*@{*/
    /// <summary>
    /// Конвертер строки(как пути) в источник изображения которое находится по данному пути.
    /// </summary>
    [ValueConversion(typeof(string), typeof(BitmapSource))]
    public class StringToBitmapSourceConverter : IValueConverter
    {
        #region Properties
        /// <summary>
        /// Директория по умолчанию.
        /// </summary>
        /// <remarks>
        /// Если значение установлено то комбинируется имя файла и путь директории.
        /// </remarks>
        public string ImageDirectory { get; set; }
        #endregion

        #region Methods 
        /// <summary>
        /// Конвертер строки(как пути) в источник изображения.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Тип BitmapSource.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(ImageDirectory))
            {
                try
                {
                    return new BitmapImage(new Uri((string)value));
                }
                catch (Exception)
                {

                    return null!;
                }

            }
            else
            {
                var image_path = System.IO.Path.Combine(ImageDirectory, (string)value);
                return new BitmapImage(new Uri(image_path));
            }
        }

        /// <summary>
        /// Конвертация типа BitmapSource в путь.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Путь.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
        #endregion
    }
    /**@}*/
}