//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsColorManager.cs
*		Менеджер для работы с цветом и сплошными кистями WPF.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup WindowsWPFCommon
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Статический класс для работы с цветом и сплошными кистями
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XWindowsColorManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			internal static List<KeyValuePair<String, Color>> mKnownColors;
			internal static List<KeyValuePair<String, SolidColorBrush>> mKnownBrushes;
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Инициализация данных
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public static void Init()
			{
				mKnownColors = new List<KeyValuePair<String, Color>>();
				mKnownBrushes = new List<KeyValuePair<String, SolidColorBrush>>();

				Type color_type = typeof(Colors);
				Type brush_type = typeof(Brushes);

				PropertyInfo[] arr_colors = color_type.GetProperties(BindingFlags.Public | BindingFlags.Static);
				PropertyInfo[] arr_brushes = brush_type.GetProperties(BindingFlags.Public | BindingFlags.Static);

				for (Int32 i = 0; i < arr_colors.Length; i++)
				{
					mKnownColors.Add(new KeyValuePair<String, Color>(arr_colors[i].Name, (Color)arr_colors[i].GetValue(null, null)));
				}

				for (Int32 i = 0; i < arr_brushes.Length; i++)
				{
					mKnownBrushes.Add(new KeyValuePair<String, SolidColorBrush>(arr_brushes[i].Name, (SolidColorBrush)arr_brushes[i].GetValue(null, null)));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение имени цвета или пустой строки
			/// </summary>
			/// <param name="color">Цвет</param>
			/// <returns>Имя цвета</returns>
			//---------------------------------------------------------------------------------------------------------
			public static String GetKnownColorName(Color color)
			{
				String result = String.Empty;

				for (Int32 i = 0; i < mKnownColors.Count; i++)
				{
					if (Color.AreClose(mKnownColors[i].Value, color))
					{
						return (mKnownColors[i].Key);
					}
				}

				return (result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение имени сплошной кисти или пустой строки
			/// </summary>
			/// <param name="brush">Сплошная кисть</param>
			/// <returns>Имя кисти</returns>
			//---------------------------------------------------------------------------------------------------------
			public static String GetKnownBrushName(SolidColorBrush brush)
			{
				String result = String.Empty;

				for (Int32 i = 0; i < mKnownColors.Count; i++)
				{
					if (Color.AreClose(mKnownBrushes[i].Value.Color, brush.Color))
					{
						return (mKnownColors[i].Key);
					}
				}

				return (result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение цвета через имя
			/// </summary>
			/// <param name="color_name">Стандартное имя цвета</param>
			/// <returns>Найденный цвет или белый цвет если не нашли</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Color GetColorByName(String color_name)
			{
				Color result = Colors.White;

				if (mKnownColors != null)
				{
					for (Int32 i = 0; i < mKnownColors.Count; i++)
					{
						if (mKnownColors[i].Key == color_name)
						{
							return (mKnownColors[i].Value);
						}
					}
				}

				return (result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение кисти через имя
			/// </summary>
			/// <param name="brush_name">Стандартное имя кисти</param>
			/// <returns>Найденную кисть или белый цвет кисти если не нашли</returns>
			//---------------------------------------------------------------------------------------------------------
			public static SolidColorBrush GetBrushByName(String brush_name)
			{
				SolidColorBrush result = Brushes.White;

				if (mKnownBrushes != null)
				{

					for (Int32 i = 0; i < mKnownBrushes.Count; i++)
					{
						if (mKnownBrushes[i].Key == brush_name)
						{
							return (mKnownBrushes[i].Value);
						}
					}
				}

				return (result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение кисти через цвет
			/// </summary>
			/// <param name="color">Цвет</param>
			/// <returns>Найденную кисть или новую кисть на основе цвета</returns>
			//---------------------------------------------------------------------------------------------------------
			public static SolidColorBrush GetBrushByColor(Color color)
			{
				if (mKnownBrushes != null)
				{
					for (Int32 i = 0; i < mKnownBrushes.Count; i++)
					{
						if (Color.AreClose(mKnownBrushes[i].Value.Color, color))
						{
							return (mKnownBrushes[i].Value);
						}
					}

					return (new SolidColorBrush(color));
				}
				else
				{
					return (Brushes.White);
				}
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================