using System.Windows;
using System.Windows.Media;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFExtension
	*@{*/
    /// <summary>
    /// Статический класс реализующий методы расширения для типа <see cref="Geometry"/>.
    /// </summary>
    public static class XWindowsGeometryExtension
    {
        /// <summary>
        /// Установка позиции геометрии.
        /// </summary>
        /// <param name="geometry">Геометрия.</param>
        /// <param name="x">Позиция по X.</param>
        /// <param name="y">Позиция по Y.</param>
        public static void SetPosition(this Geometry geometry, double x, double y)
        {
            var transformGroup = geometry.Transform as TransformGroup;
            if (transformGroup is not null)
            {
                var translate_in_group = transformGroup.Children[0] as TranslateTransform;
                if (translate_in_group is not null)
                {
                    translate_in_group.X = x;
                    translate_in_group.Y = y;
                    return;
                }
            }

            var translate = geometry.Transform as TranslateTransform;
            if (translate is not null)
            {
                translate.X = x;
                translate.Y = y;
            }
        }

        /// <summary>
        /// Установка позиции геометрии.
        /// </summary>
        /// <param name="geometry">Геометрия.</param>
        /// <param name="point">Позиция.</param>
        public static void SetPosition(this Geometry geometry, Point point)
        {
            var transformGroup = geometry.Transform as TransformGroup;
            if (transformGroup is not null)
            {
                var translate_in_group = transformGroup.Children[0] as TranslateTransform;
                if (translate_in_group is not null)
                {
                    translate_in_group.X = point.X;
                    translate_in_group.Y = point.Y;
                    return;
                }
            }

            var translate = geometry.Transform as TranslateTransform;
            if (translate is not null)
            {
                translate.X = point.X;
                translate.Y = point.Y;
            }
        }

        /// <summary>
        /// Установка угла поворота геометрии.
        /// </summary>
        /// <param name="geometry">Геометрия.</param>
        /// <param name="angle">Угол поворота.</param>
        public static void SetAngle(this Geometry geometry, double angle)
        {
            var transformGroup = geometry.Transform as TransformGroup;
            if (transformGroup is not null)
            {
                var rotation_in_group = transformGroup.Children[1] as RotateTransform;
                if (rotation_in_group is not null)
                {
                    var bounds_rect = Rect.Empty;
                    rotation_in_group.Angle = angle;
                    rotation_in_group.CenterX = bounds_rect.Left + bounds_rect.Width / 2;
                    rotation_in_group.CenterY = bounds_rect.Top + bounds_rect.Height / 2;
                    return;
                }
            }

            var rotation = geometry.Transform as RotateTransform;
            if (rotation is not null)
            {
                rotation.Angle = angle;
            }
        }
    }
    /**@}*/
}