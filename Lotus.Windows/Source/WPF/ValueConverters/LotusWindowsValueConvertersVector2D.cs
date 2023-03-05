//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Конвертеры данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsValueConvertersVector2D.cs
*		Конвертеры 2D векторов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Globalization;
using System.Windows.Data;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Maths;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup WindowsWPFValueConverters
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Универсальный конвертор типа Vector2D между различными типами представлений 
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class Vector2DToVector2DConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация объекта вектор в объект типа <see cref="Vector2D"/>
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Объект <see cref="Vector2D"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				if (value is Vector2D)
				{
					return (value);
				}

				if (value is Vector2Df)
				{
					Vector2Df v = (Vector2Df)value;
					return (new Vector2D(v.X, v.Y));
				}

				if (value is System.Windows.Vector)
				{
					System.Windows.Vector v = (System.Windows.Vector)value;
					return (new Vector2D(v.X, v.Y));
				}

				if (value is System.Windows.Point)
				{
					System.Windows.Point v = (System.Windows.Point)value;
					return (new Vector2D(v.X, v.Y));
				}

#if USE_ASSIMP
				if (value is Assimp.Vector2D)
				{
					Assimp.Vector2D v = (Assimp.Vector2D)value;
					return (new Vector2D(v.X, v.Y));
				}
#endif

#if USE_SHARPDX
				if (value is SharpDX.Vector2)
				{
					SharpDX.Vector2 v = (SharpDX.Vector2)value;
					return (new Vector2D(v.X, v.Y));
				}
#endif

				return (Vector2D.Zero);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация объекта типа <see cref="Vector2D"/> в объект вектора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр(реальный тип для преобразования)</param>
			/// <param name="culture">Культура</param>
			/// <returns>Объект вектора</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				Type real_type = (Type)parameter;
				if (real_type == null) real_type = target_type;

				if (real_type == typeof(Vector2D))
				{
					return (value);
				}

				if (real_type == typeof(Vector2Df))
				{
					Vector2D v = (Vector2D)value;
					return (new Vector2Df((Single)v.X, (Single)v.Y));
				}

				if (real_type == typeof(System.Windows.Vector))
				{
					Vector2D v = (Vector2D)value;
					return (new System.Windows.Vector(v.X, v.Y));
				}

				if (real_type == typeof(System.Windows.Point))
				{
					Vector2D v = (Vector2D)value;
					return (new System.Windows.Point(v.X, v.Y));
				}

#if USE_ASSIMP
				if (real_type == typeof(Assimp.Vector2D))
				{
					Vector2D v = (Vector2D)value;
					return (new Assimp.Vector2D((Single)v.X, (Single)v.Y));
				}
#endif

#if USE_SHARPDX
				if (real_type == typeof(SharpDX.Vector2))
				{
					Vector2D v = (Vector2D)value;
					return (new SharpDX.Vector2((Single)v.X, (Single)v.Y));
				}
#endif
				return (value);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================