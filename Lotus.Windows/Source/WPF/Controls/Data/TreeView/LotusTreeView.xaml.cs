//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для работы с данными
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusTreeView.xaml.cs
*		Дерево для отображения иерархической информации с поддержкой иерархических моделей данных.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Effects;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFControlsData
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Дерево для отображения иерархической информации с поддержкой иерархических моделей данных
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusTreeView : TreeView, INotifyPropertyChanged
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static readonly PropertyChangedEventArgs PropertyArgsIsNotifySelectedInspector = new PropertyChangedEventArgs(nameof(IsNotifySelectedInspector));
			protected static readonly PropertyChangedEventArgs PropertyArgsIsDragging = new PropertyChangedEventArgs(nameof(IsDragging));

			protected static readonly PropertyChangedEventArgs PropertyArgsIsPresentPolicyDefault = new PropertyChangedEventArgs(nameof(PropertyArgsIsPresentPolicyDefault));
			protected static readonly PropertyChangedEventArgs PropertyArgsSendViewPresented = new PropertyChangedEventArgs(nameof(SendViewPresented));
			#endregion

			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			protected Boolean mIsNotifySelectedInspector;
			protected TreeViewItem mSelectedItem;

			// Параметры представления
			protected Boolean mIsPresentPolicyDefault;
			protected Type mPresentOnlyType;
			protected Boolean mSendViewPresented;

			// Модель перетаскивания
			protected TreeViewItem mDraggedItem;
			protected Boolean mIsDragging;
			protected Point mDragLastMouseDown;
			protected Popup mPopupHand;

			// События
			protected Action<ILotusViewItemHierarchy> mOnPresentedItem;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Информирование инспектора свойств о смене выбранного элемента
			/// </summary>
			public Boolean IsNotifySelectedInspector
			{
				get { return (mIsNotifySelectedInspector); }
				set
				{
					mIsNotifySelectedInspector = value;
					NotifyPropertyChanged(PropertyArgsIsNotifySelectedInspector);
				}
			}

			/// <summary>
			/// Коллекция для иерархического отображения
			/// </summary>
			public ILotusCollectionViewHierarchy CollectionViewHierarchy
			{
				get { return (ItemsSource as ILotusCollectionViewHierarchy); }
			}

			//
			// ПАРАМЕТРЫ ПРЕДСТАВЛЕНИЯ
			//
			/// <summary>
			/// Статус представления элементов по умолчанию
			/// </summary>
			/// <remarks>
			/// Статус представления элементов по умолчанию означает что представлен может быть только один, 
			/// при активации по двойному щелчку он активируется а предыдущий сбрасывается
			/// </remarks>
			public Boolean IsPresentPolicyDefault
			{
				get { return (mIsPresentPolicyDefault); }
				set
				{
					mIsPresentPolicyDefault = value;
					NotifyPropertyChanged(PropertyArgsIsPresentPolicyDefault);
				}
			}

			/// <summary>
			/// Иметь представление может только один тип
			/// </summary>
			public Type PresentOnlyType
			{
				get { return (mPresentOnlyType); }
				set
				{
					mPresentOnlyType = value;
					NotifyPropertyChanged(PropertyArgsIsPresentPolicyDefault);
				}
			}

			/// <summary>
			/// Статус вызова метода <see cref="ILotusViewPresented"/> при активации по двойному щелчку
			/// </summary>
			public Boolean SendViewPresented
			{
				get { return (mSendViewPresented); }
				set
				{
					mSendViewPresented = value;
					NotifyPropertyChanged(PropertyArgsSendViewPresented);
				}
			}

			//
			// МОДЕЛЬ ПЕРЕТАСКИВАНИЯ
			//
			/// <summary>
			/// Статус перетаскивания модели
			/// </summary>
			public Boolean IsDragging
			{
				get { return (mIsDragging); }
				set
				{
					mIsDragging = value;
					NotifyPropertyChanged(PropertyArgsIsDragging);
				}
			}

			/// <summary>
			/// Перетаскиваемая модель
			/// </summary>
			public TreeViewItem DraggedItem
			{
				get { return (mDraggedItem); }
				set { mDraggedItem = value; }
			}

			//
			// СОБЫТИЯ
			//
			/// <summary>
			/// Событие представления элемента отображения
			/// </summary>
			public Action<ILotusViewItemHierarchy> OnPresentedItem
			{
				get { return (mOnPresentedItem); }
				set { mOnPresentedItem = value; }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusTreeView()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(TreeView));
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================

			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ TreeView ==============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка компонента
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_Loaded(Object sender, RoutedEventArgs args)
			{
				mPopupHand = this.Resources["popupHandKey"] as Popup;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор элемента
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_SelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> args)
			{
				// Проверка на иерархическую модель
				if (args.NewValue is ILotusViewItemHierarchy new_item)
				{
					if (CollectionViewHierarchy != null)
					{
						CollectionViewHierarchy.ISelectedViewItem = new_item;
					}
					new_item.IsSelected = true;
				}

				// Выключаем выбор для предыдущего элемента
				if (args.OldValue is ILotusViewItemHierarchy old_item)
				{
					old_item.IsSelected = false;
				}


				if (IsNotifySelectedInspector && XWindowManager.PropertyInspector != null)
				{
					if (args.NewValue is ILotusViewItemHierarchy new_item_data && new_item_data.DataContext != null)
					{
						XWindowManager.PropertyInspector.SelectedObject = new_item_data.DataContext;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Щелчок левой кнопки
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_PreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs args)
			{
				if (treeExplorer.IsMouseOver && AllowDrop)
				{
					mDragLastMouseDown = args.GetPosition(treeExplorer);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отпускания левой кнопки мыши
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_PreviewMouseLeftButtonUp(Object sender, MouseButtonEventArgs args)
			{
				IsDragging = false;
				DraggedItem = null;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение курсора мыши
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_PreviewMouseMove(Object sender, MouseEventArgs args)
			{
				// Получаем позицию курсора
				Point mouse_pos = args.GetPosition(treeExplorer);
				Vector diff = mDragLastMouseDown - mouse_pos;

				// Проверяем смещение
				if (AllowDrop && args.LeftButton == MouseButtonState.Pressed &&
					(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
					Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
				{
					// Получаем визуальный элемент
					TreeViewItem drag_tree_view_item = ((DependencyObject)args.OriginalSource).FindVisualParent<TreeViewItem>();

					if (drag_tree_view_item == null)
					{
						return;
					}

					IsDragging = true;

					// Получаем данные
					DraggedItem = drag_tree_view_item;

					var drag_data = new DataObject(nameof(ILotusViewItemHierarchy), drag_tree_view_item.DataContext);

					if (Keyboard.IsKeyDown(Key.LeftCtrl))
					{
						// Копируем
						DragDrop.DoDragDrop(drag_tree_view_item, drag_data, DragDropEffects.Copy);
					}
					else
					{
						// Переносим
						DragDrop.DoDragDrop(drag_tree_view_item, drag_data, DragDropEffects.Move);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка типа перетаскиваемых данных и определение типа разрешаемой операции
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_DragEnter(Object sender, DragEventArgs args)
			{
				// Если перетаскиваемая объект не содержит модель или тому над которым происходит перетаскивание 
				if (!args.Data.GetDataPresent(nameof(ILotusViewItemHierarchy)) || sender == args.Source)
				{
					args.Effects = DragDropEffects.None;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Событие постоянно возникает при перетаскивании данных над объектом-приемником
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_DragOver(Object sender, DragEventArgs args)
			{
				if (args.Data.GetDataPresent(nameof(ILotusViewItemHierarchy)))
				{
					ILotusViewItemHierarchy view_model = args.Data.GetData(nameof(ILotusViewItemHierarchy)) as ILotusViewItemHierarchy;

					// Над этим элементом находится перетаскиваемый объект
					TreeViewItem over_item = ((DependencyObject)args.OriginalSource).FindVisualParent<TreeViewItem>();
					if (over_item != null)
					{
						ILotusViewItemHierarchy over_view_model = over_item.DataContext as ILotusViewItemHierarchy;
						if (over_view_model != null && over_view_model.IsSupportViewItem(view_model))
						{

						}
						else
						{
							args.Effects = DragDropEffects.None;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Событие постоянно возникает при покидании объекта-приемника
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_DragLeave(Object sender, DragEventArgs args)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Постоянная информация о перетаскивания
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_GiveFeedback(Object sender, GiveFeedbackEventArgs args)
			{
				Size popup_size = new Size(mPopupHand.ActualWidth, mPopupHand.ActualHeight);

				Win32Point mouse_pos = new Win32Point();
				XNative.GetCursorPos(ref mouse_pos);
				Point cursopr_pos = new Point(mouse_pos.X, mouse_pos.Y);

				mPopupHand.PlacementRectangle = new Rect(cursopr_pos, popup_size);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Событие возникает, когда данные сбрасываются над объектом-приемником; по умолчанию это происходит 
			/// при отпускании кнопки мыши
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_Drop(Object sender, DragEventArgs args)
			{
				if (args.Data.GetDataPresent(nameof(ILotusViewItemHierarchy)))
				{
					ILotusViewItemHierarchy view_model = args.Data.GetData(nameof(ILotusViewItemHierarchy)) as ILotusViewItemHierarchy;

					// Над этим элементом находится перетаскиваемый объект
					TreeViewItem over_item = ((DependencyObject)args.OriginalSource).FindVisualParent<TreeViewItem>();
					if (over_item != null)
					{
						ILotusViewItemHierarchy over_view_model = over_item.DataContext as ILotusViewItemHierarchy;
						if (over_view_model != null && over_view_model.IsSupportViewItem(view_model))
						{
							// Удаляем с предыдущего элемента
							view_model.IParent.IViewItems.Remove(view_model);

							// Добавляем в текущий
							view_model.IParent = over_view_model;
							over_view_model.IViewItems.Add(view_model);

							// Уведомляем данные
							if(over_view_model.DataContext is ILotusOwnerObject owner_object)
							{
								owner_object.AttachOwnedObject(view_model.DataContext as ILotusOwnedObject, true);
							}
						}
					}
				}
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ TreeViewItem ==========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Щелчок по элементу
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeViewItem_Selected(Object sender, RoutedEventArgs args)
			{
				if(args.Source is TreeViewItem view_item)
				{
					mSelectedItem = view_item;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Формирование контекстного меню у элемента
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeViewItem_ContextMenuOpening(Object sender, ContextMenuEventArgs args)
			{
				TreeViewItem item_sender = ((DependencyObject)sender).FindVisualParent<TreeViewItem>();
				TreeViewItem item_source = ((DependencyObject)args.OriginalSource).FindVisualParent<TreeViewItem>();
				if (item_sender != null && item_source != null)
				{
					// Делаем событие обработанным чтобы оно не поднималось вверх
					// В случае необходимости меню открываем вручную
					args.Handled = true;

					// Только если оригинальный источник и текущий совпадает
					// В иных случая это означает что событие обрабатывается иным элементом
					if (item_sender == item_source)
					{
						ILotusViewItemHierarchy support_contex_menu = item_source.DataContext as ILotusViewItemHierarchy;
						if (support_contex_menu != null)
						{
							ContextMenu context_menu = item_source.ContextMenu;
							if (context_menu == null)
							{
								context_menu = new ContextMenu();
								item_source.ContextMenu = context_menu;
							}

							support_contex_menu.OpenContextMenu(context_menu);
							context_menu.IsOpen = true;
						}
						else
						{
							// Проверяем просто наличие меню
							ContextMenu context_menu = item_source.ContextMenu;
							if (context_menu != null)
							{
								context_menu.IsOpen = true;
							}
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Двойной щелчок по элементу
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeViewItem_MouseDoubleClick(Object sender, MouseButtonEventArgs args)
			{
				TreeViewItem item = ((DependencyObject)args.OriginalSource).FindVisualParent<TreeViewItem>();

				if (mSelectedItem != args.Source) return;

				if (item == null || item.DataContext == null) return;

				if (item.DataContext is ILotusViewItemHierarchy view_item_presented)
				{
					// Событие обработано
					args.Handled = true;

					Boolean handled = false;
					if (IsPresentPolicyDefault)
					{
						if (mPresentOnlyType == null || view_item_presented.DataContext.GetType().IsAssignableFrom(mPresentOnlyType))
						{
							if (CollectionViewHierarchy != null)
							{
								// Если уже был выбран какой либо элемент
								if (CollectionViewHierarchy.IPresentedViewItem != null)
								{
									// И не совпадает
									if (CollectionViewHierarchy.IPresentedViewItem != view_item_presented)
									{
										CollectionViewHierarchy.IPresentedViewItem.IsPresented = false;
										CollectionViewHierarchy.IPresentedViewItem = view_item_presented;
										CollectionViewHierarchy.IPresentedViewItem.IsPresented = true;

										if (mOnPresentedItem != null)
										{
											mOnPresentedItem(view_item_presented);
										}
									}
									else
									{
										// Если совпадают то нет
										CollectionViewHierarchy.IPresentedViewItem.IsPresented = false;
										CollectionViewHierarchy.IPresentedViewItem = null;
									}
								}
								else
								{
									CollectionViewHierarchy.IPresentedViewItem = view_item_presented;
									CollectionViewHierarchy.IPresentedViewItem.IsPresented = true;

									if (mOnPresentedItem != null)
									{
										mOnPresentedItem(view_item_presented);
									}
								}
							}

							handled = true;
						}
					}
					if (SendViewPresented && handled == false)
					{
						if (view_item_presented.DataContext is ILotusViewPresented view_presented)
						{
							view_presented.SetViewPresented(view_item_presented, true);
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработка события нажатия клавиши
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeViewItem_KeyDown(Object sender, KeyEventArgs args)
			{
				if (args.Key == Key.F2)
				{
					TreeViewItem item = ((DependencyObject)args.OriginalSource).FindVisualParent<TreeViewItem>();
					if (item != null && item.DataContext != null)
					{
						// Делаем событие обработанным чтобы оно не поднималось вверх
						// В случае необходимости меню открываем вручную
						args.Handled = true;

						if (item.DataContext is ILotusViewItemHierarchy view_item_hierarchy)
						{
							view_item_hierarchy.IsEditMode = !view_item_hierarchy.IsEditMode;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработка события раскрытия узла
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeViewItem_Expanded(Object sender, RoutedEventArgs args)
			{

			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(String property_name = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property_name));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(PropertyChangedEventArgs args)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, args);
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
