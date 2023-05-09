﻿//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Имплементация модуля базового ядра
// Подраздел: Подсистема сервисов OS
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusBaseServiceFileDialogsWindows.cs
*		Реализации диалогов открытия/сохранения файлов и директории для Windows.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
//=====================================================================================================================
namespace Lotus
{
	namespace Core
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup CoreServiceOS
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Реализации диалогов открытия/сохранения файлов и директории для Windows
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CFileDialogsWindows : ILotusFileDialogs
		{
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение соответствующего фильтра по указанному расширению файла
			/// </summary>
			/// <param name="extension">Расширение файла без точки</param>
			/// <returns>Фильтр</returns>
			//---------------------------------------------------------------------------------------------------------
			private static String GetFilterFromExt(String extension)
			{
				String result = "";
				switch (extension.ToLower())
				{
					case XFileExtension.TXT:
						{
							result = XFileDialog.TXT_FILTER;
						}
						break;
					case XFileExtension.XML:
						{
							result = XFileDialog.XML_FILTER;
						}
						break;
					case XFileExtension.JSON:
						{
							result = XFileDialog.JSON_FILTER;
						}
						break;
					case XFileExtension.LUA:
						{
							result = XFileDialog.LUA_FILTER;
						}
						break;
					case XFileExtension.BIN:
						{
							result = XFileDialog.BIN_FILTER;
						}
						break;
					case XFileExtension.BYTES:
						{
							result = XFileDialog.BYTES_FILTER;
						}
						break;
					default:
						break;
				}

				return (result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Показ диалога для открытия файла
			/// </summary>
			/// <param name="title">Заголовок диалога</param>
			/// <param name="directory">Директория для открытия файла</param>
			/// <param name="extension">Расширение файла без точки или список расширений или null</param>
			/// <returns>Полное имя существующего файла или null</returns>
			//---------------------------------------------------------------------------------------------------------
			public String Open(String title, String directory, String extension)
			{
				// Конфигурация диалога
				Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
				dialog.Title = title;
				dialog.InitialDirectory = directory;
				
				if(extension == null)
				{
					dialog.DefaultExt = XFileDialog.DefaultExt;
				}
				else
				{
					// Это фильтр
					if (extension.Contains("*"))
					{
						dialog.Filter = extension;
					}
					else
					{
						dialog.Filter = GetFilterFromExt(extension);
						dialog.DefaultExt = extension[0] == XChar.Dot ? extension : XChar.Dot + extension;
					}
				}

				// Показываем диалог открытия
				Nullable<Boolean> result = dialog.ShowDialog();

				// Если успешно
				if (result == true)
				{
					return (dialog.FileName);
				}

				return ("");

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Показ диалога для сохранения файла
			/// </summary>
			/// <param name="title">Заголовок диалога</param>
			/// <param name="directory">Директория для сохранения файла</param>
			/// <param name="default_name">Имя файла по умолчанию</param>
			/// <param name="extension">Расширение файла без точки</param>
			/// <returns>Полное имя файла или null</returns>
			//---------------------------------------------------------------------------------------------------------
			public String Save(String title, String directory, String default_name, String extension)
			{
				// Конфигурация диалога
				Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
				dialog.DefaultExt = extension[0] == XChar.Dot ? extension : XChar.Dot + extension;
				dialog.Title = title;
				dialog.InitialDirectory = directory;
				dialog.FileName = default_name;
				dialog.Filter = GetFilterFromExt(extension);

				// Показываем диалог открытия
				Nullable<Boolean> result = dialog.ShowDialog();

				// Если успешно
				if (result == true)
				{
					return (dialog.FileName);
				}

				return ("");
			}
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================