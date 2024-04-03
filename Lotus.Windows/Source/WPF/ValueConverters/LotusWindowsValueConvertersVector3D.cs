//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Конвертеры данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsValueConvertersVector3D.cs
*		Конвертеры 3D векторов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
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
		/** \addtogroup WindowsWPFValueConverters
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Универсальный конвертор типа Vector3D между различными типами представлений 
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class Vector3DToVector3DConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация объекта вектор в объект типа <see cref="Vector3D"/>
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Объект <see cref="Vector3D"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (value is Vector3D)
				{
					return value;
				}

				if (value is System.Windows.Media.Media3D.Vector3D)
				{
					var v = (System.Windows.Media.Media3D.Vector3D)value;
					return new Vector3D(v.X, v.Y, v.Z);
				}

				if (value is System.Windows.Media.Media3D.Point3D)
				{
					var v = (System.Windows.Media.Media3D.Point3D)value;
					return new Vector3D(v.X, v.Y, v.Z);
				}

#if USE_ASSIMP
				if (value is Assimp.Vector3D)
				{
					Assimp.Vector3D v = (Assimp.Vector3D)value;
					return (new Vector3D(v.X, v.Y, v.Z));
				}
#endif

#if USE_SHARPDX
				if (value is SharpDX.Vector3)
				{
					SharpDX.Vector3 v = (SharpDX.Vector3)value;
					return (new Vector3D(v.X, v.Y, v.Z));
				}
#endif
				return Vector3D.Zero;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация объект типа <see cref="Vector3D"/> в объект вектора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр(реальный тип для преобразования)</param>
			/// <param name="culture">Культура</param>
			/// <returns>Объект вектора </returns>
			//---------------------------------------------------------------------------------------------------------
			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				var real_type = (Type)parameter;
				if (real_type == null) real_type = targetType;

				if (real_type == typeof(Vector3D))
				{
					return value;
				}

				if (real_type == typeof(System.Windows.Media.Media3D.Vector3D))
				{
					var v = (Vector3D)value;
					return new System.Windows.Media.Media3D.Vector3D(v.X, v.Y, v.Z);
				}

				if (real_type == typeof(System.Windows.Media.Media3D.Point3D))
				{
					var v = (Vector3D)value;
					return new System.Windows.Media.Media3D.Point3D(v.X, v.Y, v.Z);
				}

#if USE_ASSIMP
				if (real_type == typeof(Assimp.Vector3D))
				{
					Vector3D v = (Vector3D)value;
					return (new Assimp.Vector3D((Single)v.X, (Single)v.Y, (Single)v.Z));
				}
#endif

#if USE_SHARPDX
				if (real_type == typeof(SharpDX.Vector3))
				{
					Vector3D v = (Vector3D)value;
					return (new SharpDX.Vector3((Single)v.X, (Single)v.Y, (Single)v.Z));
				}
#endif
				return value;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================