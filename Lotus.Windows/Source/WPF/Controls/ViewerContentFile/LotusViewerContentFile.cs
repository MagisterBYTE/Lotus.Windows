using Lotus.Core;

namespace Lotus.Windows
{
    /**
     * \defgroup WindowsWPFControlsViewerFiles Просмотр и редактирование файлов
     * \ingroup WindowsWPFControls
     * \brief Просмотр и редактирование файлов.
     * @{
     */
    /// <summary>
    /// Определение основного интерфейса для просмотра и редактирования файлов.
    /// </summary>
    public interface ILotusViewerContentFile
    {
        #region Properties
        /// <summary>
        /// Имя файла.
        /// </summary>
        string FileName { get; set; }
        #endregion

        #region Methods 
        /// <summary>
        /// Создание нового файла с указанным именем и параметрами.
        /// </summary>
        /// <param name="file_name">Имя файла.</param>
        /// <param name="parameters_create">Параметры создания файла.</param>
        void NewFile(string file_name, CParameters? parameters_create);

        /// <summary>
        /// Открытие указанного файла.
        /// </summary>
        /// <param name="file_name">Полное имя файла.</param>
        /// <param name="parameters_open">Параметры открытия файла.</param>
        void OpenFile(string file_name, CParameters? parameters_open);

        /// <summary>
        /// Сохранения файла.
        /// </summary>
        void SaveFile();

        /// <summary>
        /// Сохранение файла под новым именем и параметрами.
        /// </summary>
        /// <param name="file_name">Полное имя файла.</param>
        /// <param name="parameters_save">Параметры сохранения файла.</param>
        void SaveAsFile(string file_name, CParameters? parameters_save);

        /// <summary>
        /// Печать файла.
        /// </summary>
        /// <param name="parameters_print">Параметры печати файла.</param>
        void PrintFile(CParameters? parameters_print);

        /// <summary>
        /// Экспорт файла под указанным именем и параметрами.
        /// </summary>
        /// <param name="file_name">Полное имя файла.</param>
        /// <param name="parameters_export">Параметры для экспорта файла.</param>
        void ExportFile(string file_name, CParameters? parameters_export);

        /// <summary>
        /// Закрытие файла.
        /// </summary>
        void CloseFile();
        #endregion
    }
    /**@}*/
}