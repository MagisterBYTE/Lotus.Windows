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
        /// <param name="fileName">Имя файла.</param>
        /// <param name="parametersCreate">Параметры создания файла.</param>
        void NewFile(string fileName, CParameters? parametersCreate);

        /// <summary>
        /// Открытие указанного файла.
        /// </summary>
        /// <param name="fileName">Полное имя файла.</param>
        /// <param name="parametersOpen">Параметры открытия файла.</param>
        void OpenFile(string fileName, CParameters? parametersOpen);

        /// <summary>
        /// Сохранения файла.
        /// </summary>
        void SaveFile();

        /// <summary>
        /// Сохранение файла под новым именем и параметрами.
        /// </summary>
        /// <param name="fileName">Полное имя файла.</param>
        /// <param name="parametersSave">Параметры сохранения файла.</param>
        void SaveAsFile(string fileName, CParameters? parametersSave);

        /// <summary>
        /// Печать файла.
        /// </summary>
        /// <param name="parametersPrint">Параметры печати файла.</param>
        void PrintFile(CParameters? parametersPrint);

        /// <summary>
        /// Экспорт файла под указанным именем и параметрами.
        /// </summary>
        /// <param name="fileName">Полное имя файла.</param>
        /// <param name="parametersExport">Параметры для экспорта файла.</param>
        void ExportFile(string fileName, CParameters? parametersExport);

        /// <summary>
        /// Закрытие файла.
        /// </summary>
        void CloseFile();
        #endregion
    }
    /**@}*/
}