using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Lotus.Windows
{
    /** \addtogroup WindowsCommonGDI
	*@{*/
    /// <summary>
    /// Статический класс для реализации методов конвертации.
    /// </summary>
    /// <remarks>
    /// Используется для некоторых типовых преобразований.
    /// </remarks>
    public static class XDrawingConverters
    {
        /// <summary>
        /// Конвертация списка векторов.
        /// </summary>
        /// <param name="values">Список векторов одинарной точности.</param>
        /// <returns>Список точек System.Drawing.</returns>
        public static Point[] ConvertToDrawingPoints(this IList<Vector2> values)
        {
            var list = new Point[values.Count];
            for (var i = 0; i < values.Count; i++)
            {
                list[i] = new Point((int)values[i].X, (int)values[i].Y);
            }

            return list;
        }

        /// <summary>
        /// Конвертация списка векторов.
        /// </summary>
        /// <param name="values">Список векторов одинарной точности.</param>
        /// <returns>Список точек System.Drawing.</returns>
        public static PointF[] ConvertToDrawingPointsF(this IList<Vector2> values)
        {
            var list = new PointF[values.Count];
            for (var i = 0; i < values.Count; i++)
            {
                list[i] = new PointF(values[i].X, values[i].Y);
            }

            return list;
        }
    }
    /**@}*/
}