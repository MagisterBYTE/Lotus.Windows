using System;
using System.Windows;
using System.Windows.Media;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFCommon
	*@{*/
    /// <summary>
    /// Статический класс для реализации дополнительной функциональности подсистемы чертежной графики под WPF.
    /// </summary>
    public static class XWindowsGeometryManager
    {
        #region Fields
        /// <summary>
        /// Коэффициент для перевода значения в аппаратно-независимых единиц в миллиметры.
        /// </summary>
        public const double UnitToMM = 0.26458;

        /// <summary>
        /// Коэффициент для перевода значения в аппаратно-независимых единиц в сантиметры.
        /// </summary>
        public const double UnitToСM = 0.026458;

        /// <summary>
        /// Коэффициент для перевода значения в миллиметрах в аппаратно-независимые единицы.
        /// </summary>
        public const double MMToUnit = 3.77952;

        /// <summary>
        /// Коэффициент для перевода значения в сантиметрах в аппаратно-независимые единицы.
        /// </summary>
        public const double CMToUnit = 37.7952;
        #endregion

        #region Convert methods
        /// <summary>
        /// Перевод вектора в миллиметрах в аппаратно-независимые единицы.
        /// </summary>
        /// <param name="millimeter">Вектор в миллиметрах.</param>
        /// <returns>Вектор в аппаратно-независимых единицах.</returns>
        public static Vector ToDeviceUnits(Vector millimeter)
        {
            return new Vector(millimeter.X * MMToUnit, millimeter.Y * MMToUnit);
        }

        /// <summary>
        /// Перевод точки в миллиметрах в аппаратно-независимые единицы.
        /// </summary>
        /// <param name="millimeter">Точка в миллиметрах.</param>
        /// <returns>Точка в аппаратно-независимых единицах.</returns>
        public static Point ToDeviceUnits(ref Point millimeter)
        {
            return new Point((int)(millimeter.X * MMToUnit), (int)(millimeter.Y * MMToUnit));
        }

        /// <summary>
        /// Перевод размера в миллиметрах в аппаратно-независимые единицы.
        /// </summary>
        /// <param name="millimeter">Размер в миллиметрах.</param>
        /// <returns>Размер в аппаратно-независимых единицах.</returns>
        public static Size ToDeviceUnits(ref Size millimeter)
        {
            return new Size(millimeter.Width * MMToUnit, millimeter.Height * MMToUnit);
        }

        /// <summary>
        /// Перевод прямоугольника в миллиметрах в аппаратно-независимые единицы.
        /// </summary>
        /// <param name="millimeter">Прямоугольник в миллиметрах.</param>
        /// <returns>Прямоугольник в аппаратно-независимых единицах.</returns>
        public static Rect ToDeviceUnits(ref Rect millimeter)
        {
            return new Rect(millimeter.X * MMToUnit, millimeter.Y * MMToUnit,
                millimeter.Width * MMToUnit, millimeter.Height * MMToUnit);
        }

        /// <summary>
        /// Перевод рамки в миллиметрах в аппаратно-независимые единицы.
        /// </summary>
        /// <param name="millimeter">Рамка в миллиметрах.</param>
        /// <returns>Рамка в аппаратно-независимых единицах.</returns>
        public static Thickness ToDeviceUnits(Thickness millimeter)
        {
            return new Thickness(millimeter.Left * MMToUnit, millimeter.Top * MMToUnit,
                millimeter.Right * MMToUnit, millimeter.Bottom * MMToUnit);
        }

        /// <summary>
        /// Перевод рамки в миллиметрах в аппаратно-независимые единицы.
        /// </summary>
        /// <param name="millimeter">Рамка в миллиметрах.</param>
        /// <returns>Рамка в аппаратно-независимых единицах.</returns>
        public static Thickness ToDeviceUnits(ref Thickness millimeter)
        {
            return new Thickness(millimeter.Left * MMToUnit, millimeter.Top * MMToUnit,
                millimeter.Right * MMToUnit, millimeter.Bottom * MMToUnit);
        }

        /// <summary>
        /// Перевод размера в аппаратно-независимых единицах в миллиметры.
        /// </summary>
        /// <param name="deviceUnit">Размер в аппаратно-независимых единицах.</param>
        /// <returns>Размер в миллиметрах.</returns>
        public static Size ToMilliliters(in Size deviceUnit)
        {
            return new Size(deviceUnit.Width * UnitToMM, deviceUnit.Height * UnitToMM);
        }

        /// <summary>
        /// Перевод размера в аппаратно-независимых единицах в миллиметры.
        /// </summary>
        /// <param name="deviceUnit">Размер в аппаратно-независимых единицах.</param>
        /// <returns>Размер в миллиметрах.</returns>
        public static Size ToMilliliters(Size deviceUnit)
        {
            return new Size(deviceUnit.Width * UnitToMM, deviceUnit.Height * UnitToMM);
        }

        /// <summary>
        /// Перевод размера в аппаратно-независимых единицах в миллиметры.
        /// </summary>
        /// <param name="deviceUnit">Размер в аппаратно-независимых единицах.</param>
        /// <returns>Размер в миллиметрах.</returns>
        public static Size ToMillilitersRound(Size deviceUnit)
        {
            return new Size(Math.Round(deviceUnit.Width * UnitToMM, 0), Math.Round(deviceUnit.Height * UnitToMM, 0));
        }

        /// <summary>
        /// Перевод прямоугольника в аппаратно-независимых единицах в миллиметры.
        /// </summary>
        /// <param name="deviceUnit">Прямоугольник в аппаратно-независимых единицах.</param>
        /// <returns>Прямоугольник в миллиметрах.</returns>
        public static Rect ToMilliliters(in Rect deviceUnit)
        {
            return new Rect(deviceUnit.X * UnitToMM, deviceUnit.Y * UnitToMM,
                deviceUnit.Width * UnitToMM, deviceUnit.Height * UnitToMM);
        }

        /// <summary>
        /// Перевод рамки в аппаратно-независимых единицах в миллиметры.
        /// </summary>
        /// <param name="deviceUnit">Рамка в аппаратно-независимых единицах.</param>
        /// <returns>Рамка в миллиметрах.</returns>
        public static Thickness ToMilliliters(in Thickness deviceUnit)
        {
            return new Thickness(deviceUnit.Left * UnitToMM,
                                deviceUnit.Top * UnitToMM,
                                deviceUnit.Right * UnitToMM,
                                deviceUnit.Bottom * UnitToMM);
        }

        /// <summary>
        /// Перевод рамки в аппаратно-независимых единицах в миллиметры c округлением.
        /// </summary>
        /// <param name="deviceUnit">Рамка в аппаратно-независимых единицах.</param>
        /// <returns>Рамка в миллиметрах.</returns>
        public static Thickness ToMillilitersRound(ref Thickness deviceUnit)
        {
            return new Thickness(Math.Round(deviceUnit.Left * UnitToMM, 0),
                                Math.Round(deviceUnit.Top * UnitToMM, 0),
                                Math.Round(deviceUnit.Right * UnitToMM, 0),
                                Math.Round(deviceUnit.Bottom * UnitToMM, 0));
        }
        #endregion

        #region Create methods
        /// <summary>
        /// Создание массива линий (количество точек должно быть в два раза больше).
        /// </summary>
        /// <param name="points">Массив точек.</param>
        /// <param name="freeze">Следует ли заморозить геометрию.</param>
        /// <returns>Геометрия.</returns>
        public static Geometry CreateGeometryLine(Point[] points, bool freeze)
        {
            Geometry geometry = new StreamGeometry();
            using (var sgc = ((StreamGeometry)geometry).Open())
            {
                var count = points.Length / 2;
                for (var i = 0; i < count; i++)
                {
                    sgc.BeginFigure(points[i], true, false);
                    sgc.LineTo(points[i + 1], true, false);
                }
            }

            // Следует ли заморозить геометрию для повышения производительности
            if (freeze)
            {
                geometry.Freeze();
            }

            return geometry;
        }

        /// <summary>
        /// Создание полилинии.
        /// </summary>
        /// <param name="points">Массив точек.</param>
        /// <param name="closed">Следует ли замкнуть контур.</param>
        /// <param name="freeze">Следует ли заморозить геометрию.</param>
        /// <returns>Геометрия.</returns>
        public static Geometry CreateGeometryPolyLine(Point[] points, bool closed, bool freeze)
        {
            Geometry geometry = new StreamGeometry();
            using (var ctx = ((StreamGeometry)geometry).Open())
            {
                ctx.BeginFigure(points[0], true, closed);
                ctx.PolyLineTo(points, true, false);
            }
            // Freeze the geometry (make it unmodifiable) 
            // for additional performance benefits. 
            if (freeze)
            {
                geometry.Freeze();
            }

            return geometry;
        }

        /// <summary>
        /// Создание геометрии стрелки.
        /// </summary>
        /// <param name="start">Начало.</param>
        /// <param name="end">Конец.</param>
        /// <param name="headWidth">Ширина.</param>
        /// <param name="headHeight">Высота.</param>
        /// <param name="freeze">Следует ли заморозить геометрию.</param>
        /// <returns>Геометрия.</returns>
        public static Geometry CreateGeometryArrow(Point start, Point end, double headWidth, double headHeight, bool freeze)
        {
            var theta = Math.Atan2(start.Y - end.Y, start.X - end.X);
            var sint = Math.Sin(theta);
            var cost = Math.Cos(theta);

            var pt3 = new Point(
                end.X + (headWidth * cost - headHeight * sint),
                end.Y + (headWidth * sint + headHeight * cost));

            var pt4 = new Point(
                end.X + (headWidth * cost + headHeight * sint),
                end.Y - (headHeight * cost - headWidth * sint));

            Geometry geometry = new StreamGeometry();
            using (var ctx = ((StreamGeometry)geometry).Open())
            {
                ctx.BeginFigure(start, true, false);
                ctx.LineTo(end, true, true);
                ctx.LineTo(pt3, true, true);
                ctx.LineTo(end, true, true);
                ctx.LineTo(pt4, true, true);
            }
            // Freeze the geometry (make it unmodifiable) 
            // for additional performance benefits. 
            if (freeze)
            {
                geometry.Freeze();
            }

            return geometry;
        }
        #endregion
    }
    /**@}*/
}