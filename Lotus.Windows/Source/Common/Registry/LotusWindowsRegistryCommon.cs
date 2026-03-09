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
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Тип MimeType.</returns>
        public static string GetMimeType(string fileName)
        {
            var mime_type = "application/unknown";
            var ext = System.IO.Path.GetExtension(fileName).ToLower();
            var regKey = Microsoft.Win32.Registry.ClassesRoot!.OpenSubKey(ext)!;
            if (regKey is not null && regKey.GetValue("Content Type") is not null)
                mime_type = regKey.GetValue("Content Type")!.ToString()!;
            return mime_type;
        }
    }
    /**@}*/
}