//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsGeometryManager.cs
*		Реализация расширенной функциональности работы с геометрией WPF.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Windows;
using System.Windows.Media;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFCommon
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Статический класс для реализации дополнительной функциональности подсистемы чертежной графики под WPF
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XWindowsGeometryManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			/// <summary>
			/// Коэффициент для перевода значения в аппаратно-независимых единиц в миллиметры
			/// </summary>
			public const Double UnitToMM = 0.26458;

			/// <summary>
			/// Коэффициент для перевода значения в аппаратно-независимых единиц в сантиметры
			/// </summary>
			public const Double UnitToСM = 0.026458;

			/// <summary>
			/// Коэффициент для перевода значения в миллиметрах в аппаратно-независимые единицы
			/// </summary>
			public const Double MMToUnit = 3.77952;

			/// <summary>
			/// Коэффициент для перевода значения в сантиметрах в аппаратно-независимые единицы
			/// </summary>
			public const Double CMToUnit = 37.7952;
			#endregion

			#region ======================================= МЕТОДЫ ПРЕОБРАЗОВАНИЯ ЕДИНИЦ ==============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод вектора в миллиметрах в аппаратно-независимые единицы
			/// </summary>
			/// <param name="millimeter">Вектор в миллиметрах</param>
			/// <returns>Вектор в аппаратно-независимых единицах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Vector ToDeviceUnits(Vector millimeter)
			{
				return new Vector(millimeter.X * MMToUnit, millimeter.Y * MMToUnit);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод точки в миллиметрах в аппаратно-независимые единицы
			/// </summary>
			/// <param name="millimeter">Точка в миллиметрах</param>
			/// <returns>Точка в аппаратно-независимых единицах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Point ToDeviceUnits(ref Point millimeter)
			{
				return new Point((Int32)(millimeter.X * MMToUnit), (Int32)(millimeter.Y * MMToUnit));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод размера в миллиметрах в аппаратно-независимые единицы
			/// </summary>
			/// <param name="millimeter">Размер в миллиметрах</param>
			/// <returns>Размер в аппаратно-независимых единицах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Size ToDeviceUnits(ref Size millimeter)
			{
				return new Size(millimeter.Width * MMToUnit, millimeter.Height * MMToUnit);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод прямоугольника в миллиметрах в аппаратно-независимые единицы
			/// </summary>
			/// <param name="millimeter">Прямоугольник в миллиметрах</param>
			/// <returns>Прямоугольник в аппаратно-независимых единицах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Rect ToDeviceUnits(ref Rect millimeter)
			{
				return new Rect(millimeter.X * MMToUnit, millimeter.Y * MMToUnit,
					millimeter.Width * MMToUnit, millimeter.Height * MMToUnit);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод рамки в миллиметрах в аппаратно-независимые единицы
			/// </summary>
			/// <param name="millimeter">Рамка в миллиметрах</param>
			/// <returns>Рамка в аппаратно-независимых единицах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Thickness ToDeviceUnits(Thickness millimeter)
			{
				return new Thickness(millimeter.Left * MMToUnit, millimeter.Top * MMToUnit,
					millimeter.Right * MMToUnit, millimeter.Bottom * MMToUnit);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод рамки в миллиметрах в аппаратно-независимые единицы
			/// </summary>
			/// <param name="millimeter">Рамка в миллиметрах</param>
			/// <returns>Рамка в аппаратно-независимых единицах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Thickness ToDeviceUnits(ref Thickness millimeter)
			{
				return new Thickness(millimeter.Left * MMToUnit, millimeter.Top * MMToUnit,
					millimeter.Right * MMToUnit, millimeter.Bottom * MMToUnit);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод размера в аппаратно-независимых единицах в миллиметры
			/// </summary>
			/// <param name="device_unit">Размер в аппаратно-независимых единицах</param>
			/// <returns>Размер в миллиметрах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Size ToMilliliters(in Size device_unit)
			{
				return new Size(device_unit.Width * UnitToMM, device_unit.Height * UnitToMM);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод размера в аппаратно-независимых единицах в миллиметры
			/// </summary>
			/// <param name="device_unit">Размер в аппаратно-независимых единицах</param>
			/// <returns>Размер в миллиметрах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Size ToMilliliters(Size device_unit)
			{
				return new Size(device_unit.Width * UnitToMM, device_unit.Height * UnitToMM);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод размера в аппаратно-независимых единицах в миллиметры
			/// </summary>
			/// <param name="device_unit">Размер в аппаратно-независимых единицах</param>
			/// <returns>Размер в миллиметрах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Size ToMillilitersRound(Size device_unit)
			{
				return new Size(Math.Round(device_unit.Width * UnitToMM, 0), Math.Round(device_unit.Height * UnitToMM, 0));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод прямоугольника в аппаратно-независимых единицах в миллиметры
			/// </summary>
			/// <param name="device_unit">Прямоугольник в аппаратно-независимых единицах</param>
			/// <returns>Прямоугольник в миллиметрах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Rect ToMilliliters(in Rect device_unit)
			{
				return new Rect(device_unit.X * UnitToMM, device_unit.Y * UnitToMM,
					device_unit.Width * UnitToMM, device_unit.Height * UnitToMM);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод рамки в аппаратно-независимых единицах в миллиметры
			/// </summary>
			/// <param name="device_unit">Рамка в аппаратно-независимых единицах</param>
			/// <returns>Рамка в миллиметрах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Thickness ToMilliliters(in Thickness device_unit)
			{
				return new Thickness(device_unit.Left * UnitToMM,
									device_unit.Top * UnitToMM,
									device_unit.Right * UnitToMM,
									device_unit.Bottom * UnitToMM);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перевод рамки в аппаратно-независимых единицах в миллиметры c округлением
			/// </summary>
			/// <param name="device_unit">Рамка в аппаратно-независимых единицах</param>
			/// <returns>Рамка в миллиметрах</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Thickness ToMillilitersRound(ref Thickness device_unit)
			{
				return new Thickness(Math.Round(device_unit.Left * UnitToMM, 0),
									Math.Round(device_unit.Top * UnitToMM, 0),
									Math.Round(device_unit.Right * UnitToMM, 0),
									Math.Round(device_unit.Bottom * UnitToMM, 0));
			}
			#endregion

			#region ======================================= МЕТОДЫ СОЗДАНИЯ ГЕОМЕТРИИ =================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание массива линий (количество точек должно быть в два раза больше)
			/// </summary>
			/// <param name="points">Массив точек</param>
			/// <param name="freeze">Следует ли заморозить геометрию</param>
			/// <returns>Геометрия</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Geometry CreateGeometryLine(Point[] points, Boolean freeze)
			{
				Geometry geometry = new StreamGeometry();
				using (StreamGeometryContext sgc = ((StreamGeometry)geometry).Open())
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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание полилинии
			/// </summary>
			/// <param name="points">Массив точек</param>
			/// <param name="closed">Следует ли замкнуть контур</param>
			/// <param name="freeze">Следует ли заморозить геометрию</param>
			/// <returns>Геометрия</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Geometry CreateGeometryPolyLine(Point[] points, Boolean closed, Boolean freeze)
			{
				Geometry geometry = new StreamGeometry();
				using (StreamGeometryContext ctx = ((StreamGeometry)geometry).Open())
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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание геометрии стрелки
			/// </summary>
			/// <param name="start">Начало</param>
			/// <param name="end">Конец</param>
			/// <param name="head_width">Ширина</param>
			/// <param name="head_height">Высота</param>
			/// <param name="freeze">Следует ли заморозить геометрию</param>
			/// <returns>Геометрия</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Geometry CreateGeometryArrow(Point start, Point end, Double head_width, Double head_height, Boolean freeze)
			{
				var theta = Math.Atan2(start.Y - end.Y, start.X - end.X);
				var sint = Math.Sin(theta);
				var cost = Math.Cos(theta);

				var pt3 = new Point(
					end.X + (head_width * cost - head_height * sint),
					end.Y + (head_width * sint + head_height * cost));

				var pt4 = new Point(
					end.X + (head_width * cost + head_height * sint),
					end.Y - (head_height * cost - head_width * sint));

				Geometry geometry = new StreamGeometry();
				using (StreamGeometryContext ctx = ((StreamGeometry)geometry).Open())
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
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================