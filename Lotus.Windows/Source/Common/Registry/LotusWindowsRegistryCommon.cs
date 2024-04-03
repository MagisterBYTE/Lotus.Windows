namespace Lotus.Windows
{

    /**
     * \defgroup WindowsCommonRegistry Подсистема работы с реестром
     * \ingroup WindowsCommon
     * \brief Подсистема работы с реестром.
     * @{
     */
    /// <summary>
    /// Статический класс для работы с реестром.
    /// </summary>
    public static class XRegistry
    {
        /// <summary>
        /// Получение MimeType типа файла по его расширению.
        /// </summary>
        /// <param name="file_name">Имя файла.</param>
        /// <returns>Тип MimeType.</returns>
        public static string GetMimeType(string file_name)
        {
            var mime_type = "application/unknown";
            var ext = System.IO.Path.GetExtension(file_name).ToLower();
            var regKey = Microsoft.Win32.Registry.ClassesRoot!.OpenSubKey(ext)!;
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mime_type = regKey.GetValue("Content Type")!.ToString()!;
            return mime_type;
        }
    }
    /**@}*/
}