using System;
using System.Runtime.InteropServices;

namespace Lotus.Windows
{
    /**
     * \defgroup WindowsCommonNative Подсистема нативного доступа
     * \ingroup WindowsCommon
     * \brief Подсистема нативного доступа.
     * @{
     */
    /// <summary>
    /// Статический класс для реализации доступа к нативным методам Windows.
    /// </summary>
    public static partial class XNative
    {
        #region Fields
        public static ShellFileInfo ShellFileInfoDefault;
        #endregion

        #region Main methods
        /// <summary>
        /// Удаление графического объекта GDI.
        /// </summary>
        /// <param name="hObject">Дескриптор объекта.</param>
        /// <returns>Статус операции.</returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Получение позиции курсора.
        /// </summary>
        /// <param name="point">Позиция курсора.</param>
        /// <returns>Статус операции.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref Win32Point point);

        /// <summary>
        /// Получение указателя на дескриптор экрана.
        /// </summary>
        /// <returns>Дескриптор экрана.</returns>
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// Уничтожение иконки.
        /// </summary>
        /// <param name="handle">Дескриптор иконки.</param>
        /// <returns>Статус операции.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr handle);

        /// <summary>
        /// Инициализирует библиотеку COM для дополнительных функциональных возможностей.
        /// </summary>
        /// <param name="reserved">Зарезервировано.</param>
        /// <returns>Код ошибки.</returns>
        [DllImport("ole32.dll")]
        public static extern uint OleInitialize([In] IntPtr reserved);
        #endregion

        #region SHELL methods
        /// <summary>
        /// Получение иконки связанное с данным файлом.
        /// </summary>
        /// <param name="path">Путь к файлу.</param>
        /// <param name="fileAttributes">Атрибуты файла.</param>
        /// <param name="shellFileInfo">Структура для записи данных.</param>
        /// <param name="fileInfoSize">Размер структуры в байтах.</param>
        /// <param name="flags">Флаги получения данных.</param>
        /// <returns></returns>
        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string path, uint fileAttributes, ref ShellFileInfo shellFileInfo,
            uint fileInfoSize, uint flags);

        /// <summary>
        /// Запуск приложения.
        /// </summary>
        /// <param name="hwnd">Дескриптор родительского окна.</param>
        /// <param name="operation">Операция.</param>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="parameters">Параметры.</param>
        /// <param name="directory">Рабочая директория.</param>
        /// <param name="showCommands">Способ открытия файла.</param>
        /// <returns>Дескриптор приложения.</returns>
        [DllImport("shell32.dll")]
        public static extern IntPtr ShellExecute(IntPtr hwnd, string operation, string fileName, string parameters,
            string directory, TShowCommands showCommands);
        #endregion
    }
    /**@}*/
}