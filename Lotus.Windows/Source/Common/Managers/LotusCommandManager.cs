using System.Collections.Generic;
using System.Windows.Input;

namespace Lotus.Windows
{
    /** \addtogroup WindowsCommonManagers
	*@{*/
    /// <summary>
    /// Центральный менеджер основных команд приложения.
    /// </summary>
    public static class XCommandManager
    {
        #region Fields
        /// <summary>
        /// Список всех команд по ключу имени команды.
        /// </summary>
        public static readonly Dictionary<string, CommandBinding> CommandBindings = [];
        #endregion

        #region КОМАНДЫ РАБОТЫ С ФАЙЛОМ 
        /// <summary>
        /// Новый.
        /// </summary>
        public static readonly RoutedIconUICommand FileNew = new("Новый…", nameof(FileNew),
            Properties.Resources.Oxygen_document_new_32.ToBitmapSource());

        /// <summary>
        /// Открыть.
        /// </summary>
        public static readonly RoutedIconUICommand FileOpen = new("Открыть…", nameof(FileOpen),
            Properties.Resources.Oxygen_document_open_32.ToBitmapSource());

        /// <summary>
        /// Сохранить.
        /// </summary>
        public static readonly RoutedIconUICommand FileSave = new("Сохранить", nameof(FileSave),
            Properties.Resources.Oxygen_document_save_32.ToBitmapSource());

        /// <summary>
        /// Сохранить как.
        /// </summary>
        public static readonly RoutedIconUICommand FileSaveAs = new("Сохранить как", nameof(FileSaveAs),
            Properties.Resources.Oxygen_document_save_as_32.ToBitmapSource());

        /// <summary>
        /// Печать.
        /// </summary>
        public static readonly RoutedIconUICommand FilePrint = new("Печать", nameof(FilePrint),
            Properties.Resources.Oxygen_document_print_32.ToBitmapSource());

        /// <summary>
        /// Экспорт.
        /// </summary>
        public static readonly RoutedIconUICommand FileExport = new("Экспорт", nameof(FileExport),
            Properties.Resources.Oxygen_document_export_32.ToBitmapSource());

        /// <summary>
        /// Закрыть.
        /// </summary>
        public static readonly RoutedIconUICommand FileClose = new("Закрыть", nameof(FileClose),
            Properties.Resources.Oxygen_document_close_32.ToBitmapSource());
        #endregion

        #region КОМАНДЫ РАБОТЫ С РЕПОЗИТОРИЕМ 
        /// <summary>
        /// Создать.
        /// </summary>
        public static readonly RoutedIconUICommand RepCreate = new("Создать…", nameof(RepCreate),
            Properties.Resources.Fatcow_database_add_32.ToBitmapSource());

        /// <summary>
        /// Присоединить.
        /// </summary>
        public static readonly RoutedIconUICommand RepAttach = new("Присоединить…", nameof(RepAttach),
            Properties.Resources.Fatcow_database_connect_32.ToBitmapSource());

        /// <summary>
        /// Получить.
        /// </summary>
        public static readonly RoutedIconUICommand RepGet = new("Получить…", nameof(RepGet),
            Properties.Resources.Fatcow_database_connect_32.ToBitmapSource());

        /// <summary>
        /// Восстановить.
        /// </summary>
        public static readonly RoutedIconUICommand RepRestore = new("Восстановить", nameof(RepRestore),
            Properties.Resources.Fatcow_database_refresh_32.ToBitmapSource());

        /// <summary>
        /// Сохранить.
        /// </summary>
        public static readonly RoutedIconUICommand RepSave = new("Сохранить", nameof(RepSave),
            Properties.Resources.Fatcow_database_save_32.ToBitmapSource());

        /// <summary>
        /// Закрыть.
        /// </summary>
        public static readonly RoutedIconUICommand RepClose = new("Закрыть", nameof(RepClose),
            Properties.Resources.Fatcow_database_go_32.ToBitmapSource());
        #endregion

        #region КОМАНДЫ РАБОТЫ С ЗАПИСЬЮ 
        /// <summary>
        /// Добавить запись.
        /// </summary>
        public static readonly RoutedIconUICommand RecordAdd = new("Добавить", nameof(RecordAdd),
            Properties.Resources.Oxygen_list_add_32.ToBitmapSource());

        /// <summary>
        /// Удалить запись.
        /// </summary>
        public static readonly RoutedIconUICommand RecordRemove = new("Удалить", nameof(RecordRemove),
            Properties.Resources.Oxygen_list_remove_32.ToBitmapSource());

        /// <summary>
        /// Дублировать запись.
        /// </summary>
        public static readonly RoutedIconUICommand RecordDuplicate = new("Дублировать", nameof(RecordDuplicate),
            Properties.Resources.Oxygen_tab_duplicate_32.ToBitmapSource());

        /// <summary>
        /// Не учитывать запись.
        /// </summary>
        public static readonly RoutedIconUICommand RecordNotCalculation = new("Не учитывать", nameof(RecordNotCalculation),
            Properties.Resources.Oxygen_user_busy_32.ToBitmapSource());

        /// <summary>
        /// Переместить запись вверх.
        /// </summary>
        public static readonly RoutedIconUICommand RecordMoveUp = new("Вверх", nameof(RecordMoveUp),
            Properties.Resources.Oxygen_arrow_up_22.ToBitmapSource());

        /// <summary>
        /// Переместить запись вниз.
        /// </summary>
        public static readonly RoutedIconUICommand RecordMoveDown = new("Вниз", nameof(RecordMoveDown),
            Properties.Resources.Oxygen_arrow_down_22.ToBitmapSource());
        #endregion

        #region КОМАНДЫ ОБЩЕГО РЕДАКТИРОВАНИЯ 
        /// <summary>
        /// Скопировать.
        /// </summary>
        public static readonly RoutedIconUICommand EditCopy = new("Скопировать", nameof(EditCopy),
            Properties.Resources.Oxygen_edit_copy_32.ToBitmapSource());

        /// <summary>
        /// Вырезать.
        /// </summary>
        public static readonly RoutedIconUICommand EditCut = new("Вырезать", nameof(EditCut),
            Properties.Resources.Oxygen_edit_cut_32.ToBitmapSource());

        /// <summary>
        /// Вставить.
        /// </summary>
        public static readonly RoutedIconUICommand EditPaste = new("Вставить", nameof(EditPaste),
            Properties.Resources.Oxygen_edit_paste_32.ToBitmapSource());

        /// <summary>
        /// Отменить.
        /// </summary>
        public static readonly RoutedIconUICommand EditUndo = new("Отменить", nameof(EditUndo),
            Properties.Resources.Oxygen_edit_undo_32.ToBitmapSource());

        /// <summary>
        /// Повторить.
        /// </summary>
        public static readonly RoutedIconUICommand EditRedo = new("Повторить", nameof(EditRedo),
            Properties.Resources.Oxygen_edit_redo_32.ToBitmapSource());
        #endregion

        #region Initial methods
        /// <summary>
        /// Добавление и связывание команды с обработчиком действия.
        /// </summary>
        /// <param name="command">Команда.</param>
        /// <param name="executed">Обработчик исполнения команды.</param>
        public static void AddCommandBinding(RoutedIconUICommand command, ExecutedRoutedEventHandler executed)
        {
            CommandBindings.Add(command.Name, new CommandBinding(command, executed));
        }

        /// <summary>
        /// Добавление и связывание команды с обработчиком действия.
        /// </summary>
        /// <param name="command">Команда.</param>
        /// <param name="executed">Обработчик исполнения команды.</param>
        /// <param name="canExecute">Обработчик возможности исполнения команды.</param>
        public static void AddCommandBinding(RoutedIconUICommand command, ExecutedRoutedEventHandler executed,
            CanExecuteRoutedEventHandler canExecute)
        {
            CommandBindings.Add(command.Name, new CommandBinding(command, executed, canExecute));
        }
        #endregion
    }
    /**@}*/
}