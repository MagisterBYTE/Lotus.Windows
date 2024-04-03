using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFCommon
	*@{*/
    /// <summary>
    /// Статический класс для работы с цветом и сплошными кистями.
    /// </summary>
    public static class XWindowsColorManager
    {
        #region Fields
        /// <summary>
        /// Словарь цветов по имени цвета.
        /// </summary>
        public static readonly List<KeyValuePair<string, Color>> KnownColors = [];

        /// <summary>
        /// Словарь сплошных кистей по имени цвета.
        /// </summary>
        public static readonly List<KeyValuePair<string, SolidColorBrush>> KnownBrushes = [];
        #endregion

        #region Methods 
        /// <summary>
        /// Инициализация данных.
        /// </summary>
        public static void Init()
        {
            var color_type = typeof(Colors);
            var brush_type = typeof(Brushes);

            var arr_colors = color_type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            var arr_brushes = brush_type.GetProperties(BindingFlags.Public | BindingFlags.Static);

            for (var i = 0; i < arr_colors.Length; i++)
            {
                KnownColors.Add(new KeyValuePair<string, Color>(arr_colors[i].Name, (Color)arr_colors[i].GetValue(null, null)!));
            }

            for (var i = 0; i < arr_brushes.Length; i++)
            {
                KnownBrushes.Add(new KeyValuePair<string, SolidColorBrush>(arr_brushes[i].Name,
                    (SolidColorBrush)arr_brushes[i].GetValue(null, null)!));
            }
        }

        /// <summary>
        /// Получение имени цвета или пустой строки.
        /// </summary>
        /// <param name="color">Цвет.</param>
        /// <returns>Имя цвета.</returns>
        public static string GetKnownColorName(Color color)
        {
            var result = string.Empty;

            for (var i = 0; i < KnownColors.Count; i++)
            {
                if (Color.AreClose(KnownColors[i].Value, color))
                {
                    return KnownColors[i].Key;
                }
            }

            return result;
        }

        /// <summary>
        /// Получение имени сплошной кисти или пустой строки.
        /// </summary>
        /// <param name="brush">Сплошная кисть.</param>
        /// <returns>Имя кисти.</returns>
        public static string GetKnownBrushName(SolidColorBrush brush)
        {
            var result = string.Empty;

            for (var i = 0; i < KnownColors.Count; i++)
            {
                if (Color.AreClose(KnownBrushes[i].Value.Color, brush.Color))
                {
                    return KnownColors[i].Key;
                }
            }

            return result;
        }

        /// <summary>
        /// Получение цвета через имя.
        /// </summary>
        /// <param name="color_name">Стандартное имя цвета.</param>
        /// <returns>Найденный цвет или белый цвет если не нашли.</returns>
        public static Color GetColorByName(string color_name)
        {
            var result = Colors.White;

            if (KnownColors != null)
            {
                for (var i = 0; i < KnownColors.Count; i++)
                {
                    if (KnownColors[i].Key == color_name)
                    {
                        return KnownColors[i].Value;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Получение кисти через имя.
        /// </summary>
        /// <param name="brush_name">Стандартное имя кисти.</param>
        /// <returns>Найденную кисть или белый цвет кисти если не нашли.</returns>
        public static SolidColorBrush GetBrushByName(string brush_name)
        {
            var result = Brushes.White;

            if (KnownBrushes != null)
            {

                for (var i = 0; i < KnownBrushes.Count; i++)
                {
                    if (KnownBrushes[i].Key == brush_name)
                    {
                        return KnownBrushes[i].Value;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Получение кисти через цвет.
        /// </summary>
        /// <param name="color">Цвет.</param>
        /// <returns>Найденную кисть или новую кисть на основе цвета.</returns>
        public static SolidColorBrush GetBrushByColor(Color color)
        {
            if (KnownBrushes != null)
            {
                for (var i = 0; i < KnownBrushes.Count; i++)
                {
                    if (Color.AreClose(KnownBrushes[i].Value.Color, color))
                    {
                        return KnownBrushes[i].Value;
                    }
                }

                return new SolidColorBrush(color);
            }
            else
            {
                return Brushes.White;
            }
        }
        #endregion
    }
    /**@}*/
}