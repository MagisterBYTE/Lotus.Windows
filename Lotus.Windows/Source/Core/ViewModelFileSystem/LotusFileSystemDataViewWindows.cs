using System;
using System.Windows.Media;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup CoreFileSystem
	*@{*/
    /// <summary>
    /// Класс реализующий ViewModel для элемента файловой системы для Windows.
    /// </summary>
    public class ViewModelFSFileWin : ViewModelFileSystemFile
    {
        #region Fields
        protected internal ImageSource _iconSource;
        #endregion

        #region Properties
        /// <summary>
        /// Графическая иконка связанная с данным элементом файловой системы.
        /// </summary>
        public ImageSource IconSource
        {
            get
            {
                if (_iconSource == null)
                {
                    var full_name = Model.FullName;
                    if (full_name.IsExists())
                    {
                        _iconSource = Windows.XWindowsLoaderBitmap.GetIconFromFileTypeFromShell(full_name,
                            (uint)(TShellAttribute.Icon | TShellAttribute.SmallIcon));

                        if (_iconSource == null)
                        {
                            _iconSource = Windows.XWindowsLoaderBitmap.GetIconFromFileTypeFromExtract(full_name);
                        }
                    }
                }

                return _iconSource!;
            }
            set
            {
                _iconSource = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="model">Модель.</param>
        /// <param name="parent_item">Родительский узел.</param>
        public ViewModelFSFileWin(ILotusFileSystemEntity model, ILotusViewModelHierarchy? parent_item)
            : base(model, parent_item)
        {
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Установка контекстного меню.
        /// </summary>
        public override void SetContextMenu()
        {
            _contextMenuUI = new CUIContextMenuWindows
            {
                ViewModel = this
            };
            _contextMenuUI.AddItem("Показать в проводнике", (ILotusViewModel view_model) =>
            {
                XNative.ShellExecute(IntPtr.Zero,
                    "explore",
                    Model.FullName,
                    "",
                    "",
                    TShowCommands.SW_NORMAL);
            });
            _contextMenuUI.AddItem(CUIContextMenuWindows.Remove.Duplicate());
        }

        /// <summary>
        /// Открытие контекстного меню.
        /// </summary>
        /// <param name="contextMenu">Контекстное меню.</param>
        public override void OpenContextMenu(object contextMenu)
        {
            if (contextMenu is System.Windows.Controls.ContextMenu window_context_menu)
            {
                ((CUIContextMenuWindows)_contextMenuUI).SetCommandsDefault(window_context_menu);
            }
        }

        /// <summary>
        /// Построение дочерней иерархии согласно источнику данных.
        /// </summary>
        public override void BuildFromModel()
        {
            Clear();
            CollectionViewModelFSWin.BuildFromParent(this, _owner!);
        }
        #endregion
    }

    /// <summary>
    /// Коллекция для отображения элементов файловой системы для Windows.
    /// </summary>
    public class CollectionViewModelFSWin : CollectionViewModelHierarchy<ViewModelFSFileWin, ILotusFileSystemEntity>
    {
        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public CollectionViewModelFSWin()
            : base(string.Empty)
        {

        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="name">Имя коллекции.</param>
        public CollectionViewModelFSWin(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="name">Имя коллекции.</param>
        /// <param name="source">Источник данных.</param>
        public CollectionViewModelFSWin(string name, ILotusFileSystemEntity source)
            : base(name, source)
        {
        }
        #endregion

        #region ILotusCollectionViewModelHierarchy methods
        /// <summary>
        /// Создание конкретной ViewModel для указанной модели.
        /// </summary>
        /// <param name="model">Модель.</param>
        /// <param name="parent">Родительский элемент ViewModel.</param>
        /// <returns>ViewModel.</returns>
        public override ILotusViewModelHierarchy CreateViewModelHierarchy(object model, ILotusViewModelHierarchy? parent)
        {
            if (model is CFileSystemFile file)
            {
                return new ViewModelFSFileWin(file, parent);
            }

            if (model is CFileSystemDirectory directory)
            {
                return new ViewModelFSFileWin(directory, parent);
            }

            throw new NotImplementedException("Model must be type <CFileSystemFile> or <CFileSystemDirectory>");
        }
        #endregion
    }
    /**@}*/
}