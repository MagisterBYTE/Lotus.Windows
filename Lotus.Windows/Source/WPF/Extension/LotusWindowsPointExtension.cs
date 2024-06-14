using System;
using System.Windows;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFExtension
	*@{*/
    /// <summary>
    /// Статический класс реализующий методы расширения для типа <see cref="Point"/>.
    /// </summary>
    public static class XWindowsPointExtension
    {
        /// <summary>
        /// Сериализация точки в строку.
        /// </summary>
        /// <param name="point">Двухмерная точка.</param>
        /// <returns>Строка данных.</returns>
        public static string SerializeToString(this Point point)
        {
            return string.Format("{0};{1}", point.X, point.Y);
        }

        /// <summary>
        /// Десереализация двухмерной точки из строки.
        /// </summary>
        /// <param name="data">Строка данных.</param>
        /// <returns>Двухмерная точка.</returns>
        public static Point DeserializeFromString(string data)
        {
            var point = new Point();
            var vector_data = data.Split(';');
            point.X = XNumberHelper.ParseDouble(vector_data[0]);
            point.Y = XNumberHelper.ParseDouble(vector_data[1]);
            return point;
        }

        /// <summary>
        /// Аппроксимация равенства двухмерных точек.
        /// </summary>
        /// <param name="point">Первое значение.</param>
        /// <param name="other">Второе значение.</param>
        /// <param name="epsilon">Погрешность.</param>
        /// <returns>Статус равенства значений.</returns>
        public static bool Approximately(this Point point, Point other, double epsilon)
        {
            if ((Math.Abs(point.X - other.X) < epsilon) && (Math.Abs(point.Y - other.Y) < epsilon))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Аппроксимация равенства двухмерных точек.
        /// </summary>
        /// <param name="point">Первое значение.</param>
        /// <param name="other">Второе значение.</param>
        /// <returns>Статус равенства значений.</returns>
        public static bool Approximately(this Point point, Point other)
        {
            if ((Math.Abs(point.X - other.X) < 0.001) && (Math.Abs(point.Y - other.Y) < 0.001))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Преобразование в вектор Maths.Vector2D.
        /// </summary>
        /// <param name="point">Точка.</param>
        /// <returns>Вектор.</returns>
        public static Maths.Vector2D ToVector2D(this Point point)
        {
            return new Maths.Vector2D(point.X, point.Y);
        }

        /// <summary>
        /// Преобразование в вектор Maths.Vector2Df.
        /// </summary>
        /// <param name="point">Точка.</param>
        /// <returns>Вектор.</returns>
        public static Maths.Vector2Df ToVector2Df(this Point point)
        {
            return new Maths.Vector2Df((float)point.X, (float)point.Y);
        }

        /// <summary>
        /// Преобразование в Win32Point.
        /// </summary>
        /// <param name="point">Точка.</param>
        /// <returns>Точка.</returns>
        public static Win32Point ToWin32Point(this Point point)
        {
            var window_point = new Win32Point();
            window_point.X = (int)point.X;
            window_point.Y = (int)point.Y;
            return window_point;
        }
    }
    /**@}*/
}