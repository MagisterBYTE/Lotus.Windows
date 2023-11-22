//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Имплементация модуля базового ядра
// Подраздел: Подсистема отображения данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusViewModelContextMenuWindows.cs
*		Реализация контекстного меню для Windows.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Windows;
//=====================================================================================================================
namespace Lotus
{
	namespace Core
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup CoreViewModel
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс инкапсулирующий элемент контекстного меню для Windows
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CUIContextMenuItemWindows : CUIContextMenuItem
		{
			#region ======================================= ДАННЫЕ ====================================================
			public System.Windows.Controls.MenuItem MenuItem;
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuItemWindows()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="view_model">Элемент ViewModel</param>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuItemWindows(ILotusViewModel view_model)
				: this(view_model, String.Empty, null, null)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="view_model">Элемент ViewModel</param>
			/// <param name="name">Имя элемента меню</param>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuItemWindows(ILotusViewModel view_model, String name)
				: this(view_model, name, null, null)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя элемента меню</param>
			/// <param name="on_action">Обработчик событие основного действия</param>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuItemWindows(String name, Action<ILotusViewModel> on_action)
				: this(null, name, on_action, null)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="view_model">Элемент ViewModel</param>
			/// <param name="name">Имя элемента меню</param>
			/// <param name="on_action">Обработчик событие основного действия</param>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuItemWindows(ILotusViewModel view_model, String name, Action<ILotusViewModel> on_action)
				: this(view_model, name, on_action, null)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="view_model">Элемент ViewModel</param>
			/// <param name="name">Имя элемента меню</param>
			/// <param name="on_action">Обработчик событие основного действия</param>
			/// <param name="on_after_action">Дополнительный обработчик события после основного действия</param>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuItemWindows(ILotusViewModel view_model, String name, Action<ILotusViewModel> on_action,
				Action<ILotusViewModel> on_after_action)
				: base(view_model, name, on_action, on_after_action)
			{
				CreateMenuItem(name, null);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя элемента меню</param>
			/// <param name="on_action">Обработчик событие основного действия</param>
			/// <param name="icon">Графическая иконка</param>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuItemWindows(String name, Action<ILotusViewModel> on_action, System.Drawing.Bitmap icon)
			{
				OnAction = on_action;
				CreateMenuItem(name, icon);
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusDuplicate ====================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение дубликата объекта
			/// </summary>
			/// <param name="parameters">Параметры дублирования объекта</param>
			/// <returns>Дубликат объекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public override CUIContextMenuItem Duplicate(CParameters parameters = null)
			{
				var item = new CUIContextMenuItemWindows();
				item.ViewModel = ViewModel;
				item.OnAction = OnAction;
				item.OnAfterAction = OnAfterAction;
				item.CreateMenuItem(MenuItem);
				return item;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание элемента меню
			/// </summary>
			/// <param name="name">Имя элемента меню</param>
			/// <param name="icon">Графическая иконка</param>
			//---------------------------------------------------------------------------------------------------------
			public void CreateMenuItem(String name, System.Drawing.Bitmap icon)
			{
				if (MenuItem == null)
				{
					MenuItem = new System.Windows.Controls.MenuItem();
					MenuItem.Header = name;
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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание элемента меню
			/// </summary>
			/// <param name="menu_item">Элемента меню</param>
			//---------------------------------------------------------------------------------------------------------
			public void CreateMenuItem(System.Windows.Controls.MenuItem menu_item)
			{
				if (MenuItem == null)
				{
					MenuItem = new System.Windows.Controls.MenuItem();
					MenuItem.Header = menu_item.Header;
					MenuItem.Icon = menu_item.Icon;
					MenuItem.Click += OnItemClick;
				}
				else
				{
					MenuItem.Header = menu_item.Header;
					MenuItem.Icon = menu_item.Icon;
				}
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработка события щелчка на элементе меню
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnItemClick(Object sender, System.Windows.RoutedEventArgs args)
			{
				if (OnAction != null)
				{
					OnAction(ViewModel);
				}
				if (OnAfterAction != null)
				{
					OnAfterAction(ViewModel);
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс инкапсулирующий контекстное меню для Windows
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CUIContextMenuWindows : CUIContextMenu
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			/// <summary>
			/// Элемент меню - загрузить объект из файла
			/// </summary>
			public readonly static new CUIContextMenuItemWindows Load = new CUIContextMenuItemWindows("Загрузить...",
				OnLoadItemClick, XResources.Oxygen_document_open_32);

			/// <summary>
			/// Элемент меню - сохранить объект в файл
			/// </summary>
			public readonly static new CUIContextMenuItemWindows Save = new CUIContextMenuItemWindows("Сохранить...",
				OnSaveItemClick, XResources.Oxygen_document_save_32);

			/// <summary>
			/// Элемент меню - удалить объект
			/// </summary>
			public readonly static new CUIContextMenuItemWindows Remove = new CUIContextMenuItemWindows("Удалить",
				OnRemoveItemClick, XResources.Oxygen_list_remove_32);

			/// <summary>
			/// Элемент меню - дублировать объект
			/// </summary>
			public readonly static new CUIContextMenuItemWindows Duplicate = new CUIContextMenuItemWindows("Дублировать",
				OnDuplicateItemClick, XResources.Oxygen_tab_duplicate_32);

			/// <summary>
			/// Элемент меню - переместить объект вверх
			/// </summary>
			public readonly static new CUIContextMenuItemWindows MoveUp = new CUIContextMenuItemWindows("Переместить вверх",
				OnMoveUpItemClick, XResources.Oxygen_arrow_up_22);

			/// <summary>
			/// Элемент меню - переместить объект вниз
			/// </summary>
			public readonly static new CUIContextMenuItemWindows MoveDown = new CUIContextMenuItemWindows("Переместить вниз",
				OnMoveDownItemClick, XResources.Oxygen_arrow_down_22);

			/// <summary>
			/// Элемент меню - не учитывать объект в расчетах
			/// </summary>
			public readonly static new CUIContextMenuItemWindows NotCalculation = new CUIContextMenuItemWindows("Не учитывать в расчетах",
				OnNotCalculationItemClick, XResources.Oxygen_user_busy_32);
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuWindows()
				: this(null, null)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="view_model">Элемент ViewModel</param>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuWindows(ILotusViewModel view_model)
				: this(view_model, null)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="view_model">Элемент ViewModel</param>
			/// <param name="items">Набор элементов меню</param>
			//---------------------------------------------------------------------------------------------------------
			public CUIContextMenuWindows(ILotusViewModel view_model, params CUIContextMenuItem[] items)
				: base(view_model, items)
			{
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление элемента меню
			/// </summary>
			/// <param name="name">Имя элемента меню</param>
			/// <param name="onAction">Обработчик событие основного действия</param>
			//---------------------------------------------------------------------------------------------------------
			public override void AddItem(String name, Action<ILotusViewModel> onAction)
			{
				Items.Add(new CUIContextMenuItemWindows(name, onAction));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление элемента меню
			/// </summary>
			/// <param name="name">Имя элемента меню</param>
			/// <param name="onAction">Обработчик событие основного действия</param>
			/// <param name="onAfterAction">Дополнительный обработчик события после основного действия</param>
			//---------------------------------------------------------------------------------------------------------
			public override void AddItem(String name, Action<ILotusViewModel> onAction, Action<ILotusViewModel> onAfterAction)
			{
				Items.Add(new CUIContextMenuItemWindows(null, name, onAction, onAfterAction));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление элемента меню
			/// </summary>
			/// <param name="name">Имя элемента меню</param>
			/// <param name="on_action">Обработчик события элемента меню</param>
			/// <param name="icon">Иконка элемента меню</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void AddItem(String name, Action<ILotusViewModel> on_action, System.Drawing.Bitmap icon)
			{
				Items.Add(new CUIContextMenuItemWindows(name, on_action, icon));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка команд для контекстного меню по умолчанию
			/// </summary>
			/// <param name="context_menu">Контекстное меню</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetCommandsDefault(System.Windows.Controls.ContextMenu context_menu)
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
							// Если у экземпляра меню есть уже родитель то удалям
							if (item.MenuItem.Items != null)
							{
								System.Windows.Controls.ItemCollection item_collection = item.MenuItem.Items;
								item_collection.Remove(item.MenuItem);
							}

							Items[i].ViewModel = ViewModel;
							context_menu.Items.Add(item.MenuItem);
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
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================