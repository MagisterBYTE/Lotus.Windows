using System;
using System.IO;

using Lotus.Core;

namespace Lotus.Windows
{
    /**
     * \defgroup WindowsCommon Общая подсистема
	 * \ingroup Windows
	 * \brief Общая подсистема содержит код развивающий в целом платформу Windows.
	 * \defgroup WindowsCommonManagers Подсистема центральных менеджеров
	 * \ingroup WindowsCommon
	 * \brief Подсистема центральных менеджеров.
	 * @{
	 */
    /// <summary>
    /// Центральный менеджер приложения.
    /// </summary>
    public static class XApplicationManager
    {
        #region Fields
        // Доступ к ресурсам
        private static string _directoryData = "Data";
        private static string _directorySettings = "Settings";
        private static string _directoryPlugins = "Plugins";
        private static string _projectName;
        private static string _currentDirectory;
        #endregion

        #region Properties
        /// <summary>
        /// Имя директории для доступа к основным данным.
        /// </summary>
        public static string DirectoryData
        {
            get { return _directoryData; }
            set
            {
                _directoryData = value;
            }
        }

        /// <summary>
        /// Имя директории для доступа к настройкам.
        /// </summary>
        public static string DirectorySettings
        {
            get { return _directorySettings; }
            set
            {
                _directorySettings = value;
            }
        }

        /// <summary>
        /// Имя директории для доступа к плагинам.
        /// </summary>
        public static string DirectoryPlugins
        {
            get { return _directoryPlugins; }
            set
            {
                _directoryPlugins = value;
            }
        }

        /// <summary>
        /// Имя проекта/приложения.
        /// </summary>
        public static string ProjectName
        {
            get
            {
                if (string.IsNullOrEmpty(_projectName))
                {
                    _projectName = (System.Reflection.Assembly.GetEntryAssembly()!.GetName().Name)!;
                }
                return _projectName;
            }
        }
        #endregion

        #region Get methods
        /// <summary>
        /// Получение полного пути относительно проекта или приложения.
        /// </summary>
        /// <returns>Путь.</returns>
        public static string GetPath()
        {
            if (string.IsNullOrEmpty(_currentDirectory))
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

        /// <summary>
        /// Получение полного пути для директории данных проекта.
        /// </summary>
        /// <returns>Полный путь для директории данных проекта.</returns>
        public static string GetPathDirectoryData()
        {
            return Path.Combine(GetPath(), _directoryData);
        }

        /// <summary>
        /// Получение полного пути для файла данных проекта.
        /// </summary>
        /// <param name="file_name">Имя файла.</param>
        /// <returns>Полный путь к файлу данных проекта.</returns>
        public static string GetPathFileData(string file_name)
        {
            var path = Path.Combine(GetPath(), _directoryData, file_name);
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return path;
        }
        #endregion
    }
    /**@}*/
}