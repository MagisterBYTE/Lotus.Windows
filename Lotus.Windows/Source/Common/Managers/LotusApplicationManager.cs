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
// Последнее изменение от 30.04.2023
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
		/**
         * \defgroup WindowsCommon Общая подсистема
         * \ingroup Windows
         * \brief Общая подсистема содержит код развивающий в целом платформу Windows.
         * \defgroup WindowsCommonManagers Подсистема центральных менеджеров
         * \ingroup WindowsCommon
         * \brief Подсистема центральных менеджеров.
         * @{
         */
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Центральный менеджер приложения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XApplicationManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Доступ к ресурсам
			private static String _directoryData = "Data";
			private static String _directorySettings = "Settings";
			private static String _directoryPlugins = "Plugins";
			private static String _projectName;
			private static String _currentDirectory;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Имя директории для доступа к основным данным
			/// </summary>
			public static String DirectoryData
			{
				get { return _directoryData; }
				set
				{
					_directoryData = value;
				}
			}

			/// <summary>
			/// Имя директории для доступа к настройкам
			/// </summary>
			public static String DirectorySettings
			{
				get { return _directorySettings; }
				set
				{
					_directorySettings = value;
				}
			}

			/// <summary>
			/// Имя директории для доступа к плагинам
			/// </summary>
			public static String DirectoryPlugins
			{
				get { return _directoryPlugins; }
				set
				{
					_directoryPlugins = value;
				}
			}

			/// <summary>
			/// Имя проекта/приложения
			/// </summary>
			public static String ProjectName
			{
				get
				{
					if(String.IsNullOrEmpty(_projectName))
					{
						_projectName = (System.Reflection.Assembly.GetEntryAssembly()!.GetName().Name)!;
					}
					return _projectName; 
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
				if (String.IsNullOrEmpty(_currentDirectory))
				{
					// Получаем путь
					var path = Environment.CurrentDirectory;

					// Удаляем все до директории LotusPlatform
					path = path.RemoveFrom("LotusPlatform");

					// Соединяем
					_currentDirectory = Path.Combine(path, "Desktop", ProjectName);

					if (Directory.Exists(_currentDirectory) == false)
					{
						_currentDirectory = Environment.CurrentDirectory;
					}
				}

				return _currentDirectory;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение полного пути для директории данных проекта
			/// </summary>
			/// <returns>Полный путь для директории данных проекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public static String GetPathDirectoryData()
			{
				return Path.Combine(GetPath(), _directoryData);
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
				var path = Path.Combine(GetPath(), _directoryData, file_name);
				path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				return path;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================