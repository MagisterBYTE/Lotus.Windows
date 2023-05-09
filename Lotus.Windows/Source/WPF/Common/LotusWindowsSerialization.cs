//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsSerialization.cs
*		Сериализация базовых классов и данных подсистемы WPF.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
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
		/// Статический класс реализующий методы сериализации базовых классов и данных подсистемы WPF
		/// </summary>
		/// <remarks>
		/// Для обеспечения большей гибкости и универсальности сериализация базовых базовых классов и данных
		/// подсистемы WPF в формате XML предусмотрена только в формате атрибутов элементов XML
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public static class XWindowsSerialization
		{
			#region ======================================= ЗАПИСЬ/ЧТЕНИЕ ИЗ БИНАРНОГО ПОТОКА =========================

			#region ======================================= ЗАПИСЬ ДАННЫХ =============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных двухмерного вектора
			/// </summary>
			/// <param name="writer">Бинарный поток открытый для записи</param>
			/// <param name="vector">Двухмерный вектор</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Write(this BinaryWriter writer, Vector vector)
			{
				writer.Write(vector.X);
				writer.Write(vector.Y);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных двухмерного вектора, оптимизированная версия
			/// </summary>
			/// <param name="writer">Бинарный поток открытый для записи</param>
			/// <param name="vector">Двухмерный вектор</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Write(this BinaryWriter writer, ref Vector vector)
			{
				writer.Write(vector.X);
				writer.Write(vector.Y);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных списка двухмерных векторов
			/// </summary>
			/// <param name="writer">Бинарный поток открытый для записи</param>
			/// <param name="vectors">Список двухмерных векторов</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Write(this BinaryWriter writer, IList<Vector> vectors)
			{
				// Проверка против нулевых значений
				if (vectors == null || vectors.Count == 0)
				{
					writer.Write(XBinaryStreamExtension.ZERO_DATA);
				}
				else
				{
					// Записываем длину
					writer.Write(vectors.Count);

					// Записываем данные по порядку
					for (Int32 i = 0; i < vectors.Count; i++)
					{
						writer.Write(vectors[i].X);
						writer.Write(vectors[i].Y);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных цветового значения
			/// </summary>
			/// <param name="writer">Бинарный поток открытый для записи</param>
			/// <param name="color">Цветовое значение</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Write(this BinaryWriter writer, Color color)
			{
				writer.Write(color.R);
				writer.Write(color.G);
				writer.Write(color.B);
				writer.Write(color.A);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных цветового значения, оптимизированная версия
			/// </summary>
			/// <param name="writer">Бинарный поток открытый для записи</param>
			/// <param name="color">Цветовое значение</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Write(this BinaryWriter writer, ref Color color)
			{
				writer.Write(color.R);
				writer.Write(color.G);
				writer.Write(color.B);
				writer.Write(color.A);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных списка цветовых значений
			/// </summary>
			/// <param name="writer">Бинарный поток открытый для записи</param>
			/// <param name="colors">Список цветовых значений</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Write(this BinaryWriter writer, IList<Color> colors)
			{
				// Проверка против нулевых значений
				if (colors == null || colors.Count == 0)
				{
					writer.Write(XBinaryStreamExtension.ZERO_DATA);
				}
				else
				{
					// Записываем длину
					writer.Write(colors.Count);

					// Записываем данные по порядку
					for (Int32 i = 0; i < colors.Count; i++)
					{
						writer.Write(colors[i].R);
						writer.Write(colors[i].G);
						writer.Write(colors[i].B);
						writer.Write(colors[i].A);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных прямоугольника
			/// </summary>
			/// <param name="writer">Бинарный поток открытый для записи</param>
			/// <param name="rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Write(this BinaryWriter writer, Rect rect)
			{
				writer.Write(rect.X);
				writer.Write(rect.Y);
				writer.Write(rect.Width);
				writer.Write(rect.Height);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных прямоугольника, оптимизированная версия
			/// </summary>
			/// <param name="writer">Бинарный поток открытый для записи</param>
			/// <param name="rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Write(this BinaryWriter writer, ref Rect rect)
			{
				writer.Write(rect.X);
				writer.Write(rect.Y);
				writer.Write(rect.Width);
				writer.Write(rect.Height);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных списка прямоугольников
			/// </summary>
			/// <param name="writer">Бинарный поток открытый для записи</param>
			/// <param name="rects">Список прямоугольников</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Write(this BinaryWriter writer, IList<Rect> rects)
			{
				// Проверка против нулевых значений
				if (rects == null || rects.Count == 0)
				{
					writer.Write(XBinaryStreamExtension.ZERO_DATA);
				}
				else
				{
					// Записываем длину
					writer.Write(rects.Count);

					// Записываем данные по порядку
					for (Int32 i = 0; i < rects.Count; i++)
					{
						writer.Write(rects[i].X);
						writer.Write(rects[i].Y);
						writer.Write(rects[i].Width);
						writer.Write(rects[i].Height);
					}
				}
			}
			#endregion

			#region ======================================= ЧТЕНИЕ ДАННЫХ =============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных двухмерного вектора, оптимизированная версия
			/// </summary>
			/// <param name="reader">Бинарный поток открытый для чтения</param>
			/// <param name="vector">Двухмерный вектор</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Read(this BinaryReader reader, ref Vector vector)
			{
				vector.X = reader.ReadDouble();
				vector.Y = reader.ReadDouble();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных двухмерного вектора
			/// </summary>
			/// <param name="reader">Бинарный поток открытый для чтения</param>
			/// <returns>Двухмерный вектор</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Vector ReadWinVector2D(this BinaryReader reader)
			{
				Vector vector = new Vector(reader.ReadDouble(), reader.ReadDouble());
				return (vector);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных массива двухмерных векторов
			/// </summary>
			/// <param name="reader">Бинарный поток открытый для чтения</param>
			/// <returns>Массив двухмерных векторов</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Vector[] ReadWinVectors2D(this BinaryReader reader)
			{
				// Чтение количество элементов массива
				Int32 count = reader.ReadInt32();

				// Проверка нулевых данных
				if (count == XBinaryStreamExtension.ZERO_DATA)
				{
					return (null);
				}
				else
				{
					// Создаем массив
					Vector[] vectors = new Vector[count];

					// Читаем данные по порядку
					for (Int32 i = 0; i < count; i++)
					{
						vectors[i].X = reader.ReadDouble();
						vectors[i].Y = reader.ReadDouble();
					}

					return (vectors);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных цветового значения, оптимизированная версия
			/// </summary>
			/// <param name="reader">Бинарный поток открытый для чтения</param>
			/// <param name="color">Цветовое значение</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Read(this BinaryReader reader, ref Color color)
			{
				color.R = reader.ReadByte();
				color.G = reader.ReadByte();
				color.B = reader.ReadByte();
				color.A = reader.ReadByte();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных цветового значения
			/// </summary>
			/// <param name="reader">Бинарный поток открытый для чтения</param>
			/// <returns>Цветовое значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Color ReadWinColor(this BinaryReader reader)
			{
				Byte R = reader.ReadByte();
				Byte G = reader.ReadByte();
				Byte B = reader.ReadByte();
				Byte A = reader.ReadByte();
				Color color = Color.FromArgb(A, R, G, B);
				return (color);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных массива цветовых значений
			/// </summary>
			/// <param name="reader">Бинарный поток открытый для чтения</param>
			/// <returns>Массив цветовых значений</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Color[] ReadWinColors(this BinaryReader reader)
			{
				// Чтение количество элементов массива
				Int32 count = reader.ReadInt32();

				// Проверка нулевых данных
				if (count == XBinaryStreamExtension.ZERO_DATA)
				{
					return (null);
				}
				else
				{
					// Создаем массив
					Color[] сolors = new Color[count];

					// Читаем данные по порядку
					for (Int32 i = 0; i < count; i++)
					{
						сolors[i].R = reader.ReadByte();
						сolors[i].G = reader.ReadByte();
						сolors[i].B = reader.ReadByte();
						сolors[i].A = reader.ReadByte();
					}

					return (сolors);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных прямоугольника, оптимизированная версия
			/// </summary>
			/// <param name="reader">Бинарный поток открытый для чтения</param>
			/// <param name="rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Read(this BinaryReader reader, ref Rect rect)
			{
				rect.X = reader.ReadDouble();
				rect.Y = reader.ReadDouble();
				rect.Width = reader.ReadDouble();
				rect.Height = reader.ReadDouble();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных прямоугольника
			/// </summary>
			/// <param name="reader">Бинарный поток открытый для чтения</param>
			/// <returns>Прямоугольник</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Rect ReadWinRect(this BinaryReader reader)
			{
				Rect rect = new Rect(reader.ReadDouble(),
									 reader.ReadDouble(),
									 reader.ReadDouble(),
									 reader.ReadDouble());
				return (rect);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных массива прямоугольников
			/// </summary>
			/// <param name="reader">Бинарный поток открытый для чтения</param>
			/// <returns>Массив прямоугольников</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Rect[] ReadWinRects(this BinaryReader reader)
			{
				// Чтение количество элементов массива
				Int32 count = reader.ReadInt32();

				// Проверка нулевых данных
				if (count == XBinaryStreamExtension.ZERO_DATA)
				{
					return (null);
				}
				else
				{
					// Создаем массив
					Rect[] rects = new Rect[count];

					// Читаем данные по порядку
					for (Int32 i = 0; i < count; i++)
					{
						rects[i].X = reader.ReadDouble();
						rects[i].Y = reader.ReadDouble();
						rects[i].Width = reader.ReadDouble();
						rects[i].Height = reader.ReadDouble();
					}

					return (rects);
				}
			}
			#endregion

			#endregion

			#region ======================================= ЗАПИСЬ/ЧТЕНИЕ ИЗ ПОТОКА XML ===============================

			#region ======================================= ЗАПИСЬ ДАННЫХ =============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись значение цвета в формат атрибутов
			/// </summary>
			/// <param name="xml_writer">Средство записи данных в формат XML</param>
			/// <param name="name">Имя атрибута</param>
			/// <param name="color">Цвет</param>
			//---------------------------------------------------------------------------------------------------------
			public static void WriteWinColorToAttribute(this XmlWriter xml_writer, String name, Color color)
			{
				xml_writer.WriteStartAttribute(name);
				xml_writer.WriteValue(color.SerializeToString());
				xml_writer.WriteEndAttribute();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись данных двухмерного вектора в формат атрибутов
			/// </summary>
			/// <param name="xml_writer">Средство записи данных в формат XML</param>
			/// <param name="name">Имя атрибута</param>
			/// <param name="vector">Двухмерный вектор</param>
			//---------------------------------------------------------------------------------------------------------
			public static void WriteWinVectorToAttribute(this XmlWriter xml_writer, String name, Vector vector)
			{
				xml_writer.WriteStartAttribute(name);
				xml_writer.WriteValue(vector.SerializeToString());
				xml_writer.WriteEndAttribute();
			}
			#endregion

			#region ======================================= ЧТЕНИЕ ДАННЫХ =============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных цветового значения из формата атрибутов
			/// </summary>
			/// <param name="xml_reader">Средство чтения данных формата XML</param>
			/// <param name="name">Имя атрибута</param>
			/// <param name="default_value">Значение по умолчанию в случает отсутствия атрибута</param>
			/// <returns>Цветовое значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Color ReadWinColorFromAttribute(this XmlReader xml_reader, String name, Color default_value)
			{
				String value;
				if ((value = xml_reader.GetAttribute(name)) != null)
				{
					return (XWindowsColorExtension.DeserializeFromString(value));
				}
				return (default_value);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение данных двухмерного вектора из формата атрибутов
			/// </summary>
			/// <param name="xml_reader">Средство чтения данных формата XML</param>
			/// <param name="name">Имя атрибута</param>
			/// <returns>Двухмерный вектор</returns>
			//---------------------------------------------------------------------------------------------------------
			public static Vector ReadWinVectorFromAttribute(this XmlReader xml_reader, String name)
			{
				String value;
				if ((value = xml_reader.GetAttribute(name)) != null)
				{
					return (XWindowsVectorExtension.DeserializeFromString(value));
				}
				return (new Vector(0, 0));
			}
			#endregion

			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================