//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Общая подсистема
// Подраздел: Подсистема работы с GDI
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsGDIBitmapExtension.cs
*		Статический класс для реализации методов расширений для типа Bitmap
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Drawing;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsCommonGDI
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Статический класс для реализации методов расширений для типа <see cref="Bitmap"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XBitmapExtension
		{
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация объекта <see cref="Bitmap"/> в объект WPF <see cref="System.Windows.Media.Imaging.BitmapSource"/>.
			/// </summary>
			/// <remarks>
			/// Использует GDI для выполнения преобразования
			/// </remarks> 
			/// <param name="source">Источник изображения</param>
			/// <param name="width">Ширина требуемого изображения</param>
			/// <param name="height">Высота требуемого изображения</param> 
			/// <returns>Объект BitmapSource</returns>
			//---------------------------------------------------------------------------------------------------------
			public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource(this Bitmap source, int width, int height)
			{
				var h_bitmap = source.GetHbitmap();
				var result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bitmap, IntPtr.Zero,
					System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(width, height));

				XNative.DeleteObject(h_bitmap);

				return result;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary> 
			/// Конвертация объекта <see cref="Bitmap"/> в объект WPF <see cref="System.Windows.Media.Imaging.BitmapSource"/>.
			/// </summary> 
			/// <remarks>
			/// Использует GDI для выполнения преобразования
			/// </remarks> 
			/// <param name="source">Источник изображения</param>
			/// <returns>Объект BitmapSource</returns>
			//---------------------------------------------------------------------------------------------------------
			public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource(this Bitmap source)
			{
				var h_bitmap = source.GetHbitmap();
				var result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bitmap, IntPtr.Zero,
					System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

				XNative.DeleteObject(h_bitmap);

				return result;
			}
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================