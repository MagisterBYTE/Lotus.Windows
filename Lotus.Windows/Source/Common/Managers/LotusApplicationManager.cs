//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Общая подсистема
// Подраздел: Подсистема центральных менеджеров
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusApplicationManager.cs
*		Центральный менеджер приложения.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.IO;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \defgroup WindowsCommon Общая подсистема
		//! Общая подсистема содержит код развивающий в целом платформу Windows
		//! \ingroup Windows
		//-------------------------------------------------------------------------------------------------------------
		//! \defgroup WindowsCommonManagers Подсистема центральных менеджеров
		//! \ingroup WindowsCommon
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Центральный менеджер приложения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XApplicationManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Доступ к ресурсам
			private static String mDirectoryData = "Data";
			private static String mDirectorySettings = "Settings";
			private static String mDirectoryPlugins = "Plugins";
			private static String mProjectName;
			private static String mCurrentDirectory;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Имя директории для доступа к основным данным
			/// </summary>
			public static String DirectoryData
			{
				get { return (mDirectoryData); }
				set
				{
					mDirectoryData = value;
				}
			}

			/// <summary>
			/// Имя директории для доступа к настройкам
			/// </summary>
			public static String DirectorySettings
			{
				get { return (mDirectorySettings); }
				set
				{
					mDirectorySettings = value;
				}
			}

			/// <summary>
			/// Имя директории для доступа к плагинам
			/// </summary>
			public static String DirectoryPlugins
			{
				get { return (mDirectoryPlugins); }
				set
				{
					mDirectoryPlugins = value;
				}
			}

			/// <summary>
			/// Имя проекта/приложения
			/// </summary>
			public static String ProjectName
			{
				get
				{
					if(String.IsNullOrEmpty(mProjectName))
					{
						mProjectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
					}
					return (mProjectName); 
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ПОЛУЧЕНИЯ ПУТЕЙ ====================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение полного пути относительно проекта или приложения
			/// </summary>
			/// <returns>Путь</returns>
			//---------------------------------------------------------------------------------------------------------
			public static String GetPath()
			{
				if (String.IsNullOrEmpty(mCurrentDirectory))
				{
					// Получаем путь
					String path = Environment.CurrentDirectory;

					// Удаляем все до директории LotusPlatform
					path = path.RemoveFrom("LotusPlatform");

					// Соединяем
					mCurrentDirectory = Path.Combine(path, "Desktop", ProjectName);

					if (Directory.Exists(mCurrentDirectory) == false)
					{
						mCurrentDirectory = Environment.CurrentDirectory;
					}
				}

				return (mCurrentDirectory);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение полного пути для директории данных проекта
			/// </summary>
			/// <returns>Полный путь для директории данных проекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public static String GetPathDirectoryData()
			{
				return (Path.Combine(GetPath(), mDirectoryData));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение полного пути для файла данных проекта
			/// </summary>
			/// <param name="file_name">Имя файла</param>
			/// <returns>Полный путь к файлу данных проекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public static String GetPathFileData(String file_name)
			{
				String path = Path.Combine(GetPath(), mDirectoryData, file_name);
				path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				return (path);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================