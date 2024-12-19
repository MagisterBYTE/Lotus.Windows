using System;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup CoreViewModel
    *@{*/
    /// <summary>
    /// Класс инкапсулирующий элемент контекстного меню для Windows.
    /// </summary>
    public class CUIContextMenuItemWindows : CUIContextMenuItem
    {
        #region Fields
        public System.Windows.Controls.MenuItem MenuItem;
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public CUIContextMenuItemWindows()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="viewModel">Элемент ViewModel.</param>
        public CUIContextMenuItemWindows(ILotusViewModel viewModel)
            : this(viewModel, string.Empty, null, null)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="viewModel">Элемент ViewModel.</param>
        /// <param name="name">Имя элемента меню.</param>
        public CUIContextMenuItemWindows(ILotusViewModel viewModel, string name)
            : this(viewModel, name, null, null)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="name">Имя элемента меню.</param>
        /// <param name="onAction">Обработчик событие основного действия.</param>
        public CUIContextMenuItemWindows(string name, Action<ILotusViewModel> onAction)
            : this(null, name, onAction, null)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="viewModel">Элемент ViewModel.</param>
        /// <param name="name">Имя элемента меню.</param>
        /// <param name="onAction">Обработчик событие основного действия.</param>
        public CUIContextMenuItemWindows(ILotusViewModel viewModel, string name, Action<ILotusViewModel> onAction)
            : this(viewModel, name, onAction, null)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="viewModel">Элемент ViewModel.</param>
        /// <param name="name">Имя элемента меню.</param>
        /// <param name="onAction">Обработчик событие основного действия.</param>
        /// <param name="onAfterAction">Дополнительный обработчик события после основного действия.</param>
        public CUIContextMenuItemWindows(ILotusViewModel? viewModel, string name, Action<ILotusViewModel>? onAction,
            Action<ILotusViewModel>? onAfterAction)
            : base(viewModel, name, onAction, onAfterAction)
        {
            CreateMenuItem(name, null);
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="name">Имя элемента меню.</param>
        /// <param name="onAction">Обработчик событие основного действия.</param>
        /// <param name="icon">Графическая иконка.</param>
        public CUIContextMenuItemWindows(string name, Action<ILotusViewModel> onAction, System.Drawing.Bitmap icon)
        {
            OnAction = onAction;
            CreateMenuItem(name, icon);
        }
        #endregion

        #region ILotusDuplicate methods
        /// <summary>
        /// Получение дубликата объекта.
        /// </summary>
        /// <param name="parameters">Параметры дублирования объекта.</param>
        /// <returns>Дубликат объекта.</returns>
        public override CUIContextMenuItem Duplicate(CParameters? parameters = null)
        {
            var item = new CUIContextMenuItemWindows
            {
                ViewModel = ViewModel,
                OnAction = OnAction,
                OnAfterAction = OnAfterAction
            };
            item.CreateMenuItem(MenuItem);
            return item;
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Создание элемента меню.
        /// </summary>
        /// <param name="name">Имя элемента меню.</param>
        /// <param name="icon">Графическая иконка.</param>
        public void CreateMenuItem(string name, System.Drawing.Bitmap? icon)
        {
            if (MenuItem == null)
            {
                MenuItem = new System.Windows.Controls.MenuItem
                {
                    Header = name
                };
                MenuItem.Click += OnItemClick;
                if (icon != null)
                {
                    MenuItem.Icon = new System.Windows.Controls.Image
                    {
                        Source = icon.ToBitmapSource(),
                        Width = 16,
                        Height = 16
                    };
                }
            }
        }

        /// <summary>
        /// Создание элемента меню.
        /// </summary>
        /// <param name="menuItem">Элемента меню.</param>
        public void CreateMenuItem(System.Windows.Controls.MenuItem menuItem)
        {
            if (MenuItem == null)
            {
                MenuItem = new System.Windows.Controls.MenuItem
                {
                    Header = menuItem.Header,
                    Icon = menuItem.Icon
                };
                MenuItem.Click += OnItemClick;
            }
            else
            {
                MenuItem.Header = menuItem.Header;
                MenuItem.Icon = menuItem.Icon;
            }
        }
        #endregion

        #region Event handler
        /// <summary>
        /// Обработка события щелчка на элементе меню.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnItemClick(object sender, System.Windows.RoutedEventArgs args)
        {
            if (OnAction != null)
            {
                OnAction(ViewModel!);
            }
            if (OnAfterAction != null)
            {
                OnAfterAction(ViewModel!);
            }
        }
        #endregion
    }

    /// <summary>
    /// Класс инкапсулирующий контекстное меню для Windows.
    /// </summary>
    public class CUIContextMenuWindows : CUIContextMenu
    {
        #region Static fields
        /// <summary>
        /// Элемент меню - загрузить объект из файла.
        /// </summary>
        public readonly static new CUIContextMenuItemWindows Load = new("Загрузить...",
            OnLoadItemClick, XResources.Oxygen_document_open_32);

        /// <summary>
        /// Элемент меню - сохранить объект в файл.
        /// </summary>
        public readonly static new CUIContextMenuItemWindows Save = new("Сохранить...",
            OnSaveItemClick, XResources.Oxygen_document_save_32);

        /// <summary>
        /// Элемент меню - удалить объект.
        /// </summary>
        public readonly static new CUIContextMenuItemWindows Remove = new("Удалить",
            OnRemoveItemClick, XResources.Oxygen_list_remove_32);

        /// <summary>
        /// Элемент меню - дублировать объект.
        /// </summary>
        public readonly static new CUIContextMenuItemWindows Duplicate = new("Дублировать",
            OnDuplicateItemClick, XResources.Oxygen_tab_duplicate_32);

        /// <summary>
        /// Элемент меню - переместить объект вверх.
        /// </summary>
        public readonly static new CUIContextMenuItemWindows MoveUp = new("Переместить вверх",
            OnMoveUpItemClick, XResources.Oxygen_arrow_up_22);

        /// <summary>
        /// Элемент меню - переместить объект вниз.
        /// </summary>
        public readonly static new CUIContextMenuItemWindows MoveDown = new("Переместить вниз",
            OnMoveDownItemClick, XResources.Oxygen_arrow_down_22);

        /// <summary>
        /// Элемент меню - не учитывать объект в расчетах.
        /// </summary>
        public readonly static new CUIContextMenuItemWindows NotCalculation = new("Не учитывать в расчетах",
            OnNotCalculationItemClick, XResources.Oxygen_user_busy_32);
        #endregion

        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public CUIContextMenuWindows()
            : this(null, null)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="viewModel">Элемент ViewModel.</param>
        public CUIContextMenuWindows(ILotusViewModel viewModel)
            : this(viewModel, null)
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="viewModel">Элемент ViewModel.</param>
        /// <param name="items">Набор элементов меню.</param>
        public CUIContextMenuWindows(ILotusViewModel? viewModel, params CUIContextMenuItem[]? items)
            : base(viewModel, items)
        {
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Добавление элемента меню.
        /// </summary>
        /// <param name="name">Имя элемента меню.</param>
        /// <param name="onAction">Обработчик событие основного действия.</param>
        public override void AddItem(string name, Action<ILotusViewModel> onAction)
        {
            Items.Add(new CUIContextMenuItemWindows(name, onAction));
        }

        /// <summary>
        /// Добавление элемента меню.
        /// </summary>
        /// <param name="name">Имя элемента меню.</param>
        /// <param name="onAction">Обработчик событие основного действия.</param>
        /// <param name="onAfterAction">Дополнительный обработчик события после основного действия.</param>
        public override void AddItem(string name, Action<ILotusViewModel> onAction, Action<ILotusViewModel> onAfterAction)
        {
            Items.Add(new CUIContextMenuItemWindows(null, name, onAction, onAfterAction));
        }

        /// <summary>
        /// Добавление элемента меню.
        /// </summary>
        /// <param name="name">Имя элемента меню.</param>
        /// <param name="onAction">Обработчик события элемента меню.</param>
        /// <param name="icon">Иконка элемента меню.</param>
        public virtual void AddItem(string name, Action<ILotusViewModel> onAction, System.Drawing.Bitmap icon)
        {
            Items.Add(new CUIContextMenuItemWindows(name, onAction, icon));
        }

        /// <summary>
        /// Установка команд для контекстного меню по умолчанию.
        /// </summary>
        /// <param name="contextMenu">Контекстное меню.</param>
        public void SetCommandsDefault(System.Windows.Controls.ContextMenu contextMenu)
        {
            if (ViewModel == null) return;

            // Устанавливаем/обновляем модель
            if (IsCreatedItems == false)
            {
                for (var i = 0; i < Items.Count; i++)
                {
                    var item = Items[i] as CUIContextMenuItemWindows;
                    if (item != null)
                    {
                        // Если у экземпляра меню есть уже родитель то удаляем
                        if (item.MenuItem.Items != null)
                        {
                            var item_collection = item.MenuItem.Items;
                            item_collection.Remove(item.MenuItem);
                        }

                        Items[i].ViewModel = ViewModel;
                        contextMenu.Items.Add(item.MenuItem);
                    }
                }

                IsCreatedItems = true;
            }
            else
            {
                // Устанавливаем/обновляем модель
                for (var i = 0; i < Items.Count; i++)
                {
                    Items[i].ViewModel = ViewModel;
                }
            }
        }
        #endregion
    }
    /**@}*/
}