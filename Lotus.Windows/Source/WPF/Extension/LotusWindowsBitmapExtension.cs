//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Методы расширения
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsBitmapExtension.cs
*		Методы расширения для работы с типом BitmapSource.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFExtension
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[StructLayout(LayoutKind.Explicit)]
		public struct PixelColor
		{
			// 32 bit BGRA 
			[FieldOffset(0)] public UInt32 ColorBGRA;
			// 8 bit components
			[FieldOffset(0)] public byte Blue;
			[FieldOffset(1)] public byte Green;
			[FieldOffset(2)] public byte Red;
			[FieldOffset(3)] public byte Alpha;
		}


		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Статический класс реализующий методы расширения для типа <see cref="BitmapSource"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XWindowsBitmapExtension
		{
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// 
			/// </summary>
			/// <param name="bitmap"></param>
			/// <returns></returns>
			//---------------------------------------------------------------------------------------------------------
			public static PixelColor[,] GetPixels(this BitmapSource bitmap)
			{
				var source = bitmap;
				if (bitmap.Format != PixelFormats.Bgra32)
				{
					source = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
				}

				var height = source.PixelHeight;
				var width = source.PixelWidth;
				var stride = source.PixelWidth * 4;

				var pixelBytes = new byte[height * width * 4];
				source.CopyPixels(pixelBytes, stride, 0);

				PixelColor[,] pixels = new PixelColor[source.PixelWidth, source.PixelHeight];
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						pixels[x, y] = new PixelColor
						{
							Blue = pixelBytes[(y * width + x) * 4 + 0],
							Green = pixelBytes[(y * width + x) * 4 + 1],
							Red = pixelBytes[(y * width + x) * 4 + 2],
							Alpha = pixelBytes[(y * width + x) * 4 + 3],
						};
					}
				}

				return pixels;
			}


			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// 
			/// </summary>
			/// <param name="bitmap"></param>
			/// <returns></returns>
			//---------------------------------------------------------------------------------------------------------
			public static System.Drawing.Color[] GetPixels(this Bitmap bitmap)
			{
				var height = bitmap.Height;
				var width = bitmap.Width;

				System.Drawing.Color[] pixels= new System.Drawing.Color[width * height];
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						pixels[x + (y * width)] = bitmap.GetPixel(x, y);
					}
				}

				return pixels;
			}

			public static void PutPixels(this WriteableBitmap bitmap, PixelColor[,] pixels, int x, int y)
			{
				int width = pixels.GetLength(0);
				int height = pixels.GetLength(1);
				bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, x, y);
			}

			public static void PutPixels(this Bitmap bitmap, System.Drawing.Color[]pixels, int x, int y, int width)
			{
				int height = pixels.GetLength(0) / width;

				for (int y1 = 0; y1 < height; y1++)
				{
					for (int x1 = 0; x1 < width; x1++)
					{
						var pixel = pixels[x1 + (y1 * width)];
						bitmap.SetPixel(x1 + x, y1 + y, pixel);
					}
				}
			}
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================