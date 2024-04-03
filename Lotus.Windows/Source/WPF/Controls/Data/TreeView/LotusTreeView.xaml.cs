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
			protected internal bool _isNotifySelectedInspector;
			protected internal TreeViewItem _selectedItem;

			// Параметры представления
			protected internal bool _isPresentPolicyDefault;
			protected internal Type _presentOnlyType;
			protected internal bool _sendViewPresented;

			// Модель перетаскивания
			protected internal TreeViewItem? _draggedItem;
			protected internal bool _isDragging;
			protected internal Point _dragLastMouseDown;
			protected internal Popup _popupHand;

			// События
			protected internal Action<ILotusViewModelHierarchy> _onPresentedItem;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Информирование инспектора свойств о смене выбранного элемента
			/// </summary>
			public bool IsNotifySelectedInspector
			{
				get { return _isNotifySelectedInspector; }
				set
				{
					_isNotifySelectedInspector = value;
					NotifyPropertyChanged(PropertyArgsIsNotifySelectedInspector);
				}
			}

			/// <summary>
			/// Коллекция для иерархического отображения
			/// </summary>
			public ILotusCollectionViewModelHierarchy? CollectionViewModelHierarchy
			{
				get { return ItemsSource as ILotusCollectionViewModelHierarchy; }
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
			public bool IsPresentPolicyDefault
			{
				get { return _isPresentPolicyDefault; }
				set
				{
					_isPresentPolicyDefault = value;
					NotifyPropertyChanged(PropertyArgsIsPresentPolicyDefault);
				}
			}

			/// <summary>
			/// Иметь представление может только один тип
			/// </summary>
			public Type PresentOnlyType
			{
				get { return _presentOnlyType; }
				set
				{
					_presentOnlyType = value;
					NotifyPropertyChanged(PropertyArgsIsPresentPolicyDefault);
				}
			}

			/// <summary>
			/// Статус вызова метода <see cref="ILotusModelPresented"/> при активации по двойному щелчку
			/// </summary>
			public bool SendViewPresented
			{
				get { return _sendViewPresented; }
				set
				{
					_sendViewPresented = value;
					NotifyPropertyChanged(PropertyArgsSendViewPresented);
				}
			}

			//
			// МОДЕЛЬ ПЕРЕТАСКИВАНИЯ
			//
			/// <summary>
			/// Статус перетаскивания модели
			/// </summary>
			public bool IsDragging
			{
				get { return _isDragging; }
				set
				{
					_isDragging = value;
					NotifyPropertyChanged(PropertyArgsIsDragging);
				}
			}

			/// <summary>
			/// Перетаскиваемая модель
			/// </summary>
			public TreeViewItem? DraggedItem
			{
				get { return _draggedItem; }
				set { _draggedItem = value; }
			}

			//
			// СОБЫТИЯ
			//
			/// <summary>
			/// Событие представления элемента отображения
			/// </summary>
			public Action<ILotusViewModelHierarchy> OnPresentedItem
			{
				get { return _onPresentedItem; }
				set { _onPresentedItem = value; }
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

			#region Main methods

			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ TreeView ==============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка компонента
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_Loaded(object sender, RoutedEventArgs args)
			{
				_popupHand = (Resources["popupHandKey"] as Popup)!;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор элемента
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
			{
				// Проверка на иерархическую модель
				if (args.NewValue is ILotusViewModelHierarchy new_item)
				{
					if (CollectionViewModelHierarchy != null)
					{
						CollectionViewModelHierarchy.ISelectedViewModel = new_item;
					}
					new_item.IsSelected = true;
				}

				// Выключаем выбор для предыдущего элемента
				if (args.OldValue is ILotusViewModelHierarchy old_item)
				{
					old_item.IsSelected = false;
				}


				if (IsNotifySelectedInspector && XWindowManager.PropertyInspector != null 
					&& args.NewValue is ILotusViewModelHierarchy new_item_data && new_item_data.Model != null)
				{
					XWindowManager.PropertyInspector.SelectedObject = new_item_data.Model;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Щелчок левой кнопки
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
			{
				if (treeExplorer.IsMouseOver && AllowDrop)
				{
					_dragLastMouseDown = args.GetPosition(treeExplorer);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отпускания левой кнопки мыши
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
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
			private void OnTreeView_PreviewMouseMove(object sender, MouseEventArgs args)
			{
				// Получаем позицию курсора
				Point mouse_pos = args.GetPosition(treeExplorer);
				Vector diff = _dragLastMouseDown - mouse_pos;

				// Проверяем смещение
				if (AllowDrop && args.LeftButton == MouseButtonState.Pressed &&
					(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
					Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
				{
					// Получаем визуальный элемент
					TreeViewItem? drag_tree_view_item = (args.OriginalSource as DependencyObject)?.FindVisualParent<TreeViewItem>();

					if (drag_tree_view_item == null)
					{
						return;
					}

					IsDragging = true;

					// Получаем данные
					DraggedItem = drag_tree_view_item;

					var drag_data = new DataObject(nameof(ILotusViewModelHierarchy), drag_tree_view_item.DataContext);

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
			private void OnTreeView_DragEnter(object sender, DragEventArgs args)
			{
				// Если перетаскиваемая объект не содержит модель или тому над которым происходит перетаскивание 
				if (!args.Data.GetDataPresent(nameof(ILotusViewModelHierarchy)) || sender == args.Source)
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
			private void OnTreeView_DragOver(object sender, DragEventArgs args)
			{
				if (args.Data.GetDataPresent(nameof(ILotusViewModelHierarchy)))
				{
					var view_model = args.Data.GetData(nameof(ILotusViewModelHierarchy)) as ILotusViewModelHierarchy;

					// Над этим элементом находится перетаскиваемый объект
					TreeViewItem? over_item = (args.OriginalSource as DependencyObject)?.FindVisualParent<TreeViewItem>();
					if (over_item != null)
					{
						var over_view_model = over_item.DataContext as ILotusViewModelHierarchy;
						if (over_view_model != null && over_view_model.IsSupportViewModel(view_model!))
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
			private void OnTreeView_DragLeave(object sender, DragEventArgs args)
			{
				// Method intentionally left empty.
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Постоянная информация о перетаскивания
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_GiveFeedback(object sender, GiveFeedbackEventArgs args)
			{
				var popup_size = new Size(_popupHand.ActualWidth, _popupHand.ActualHeight);

				var mouse_pos = new Win32Point();
				XNative.GetCursorPos(ref mouse_pos);
				var cursopr_pos = new Point(mouse_pos.X, mouse_pos.Y);

				_popupHand.PlacementRectangle = new Rect(cursopr_pos, popup_size);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Событие возникает, когда данные сбрасываются над объектом-приемником; по умолчанию это происходит 
			/// при отпускании кнопки мыши
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeView_Drop(object sender, DragEventArgs args)
			{
				if (args.Data.GetDataPresent(nameof(ILotusViewModelHierarchy)))
				{
					var view_model = (args.Data.GetData(nameof(ILotusViewModelHierarchy)) as ILotusViewModelHierarchy)!;

					// Над этим элементом находится перетаскиваемый объект
					TreeViewItem? over_item = (args.OriginalSource as DependencyObject)?.FindVisualParent<TreeViewItem>();
					if (over_item != null)
					{
						var over_view_model = over_item.DataContext as ILotusViewModelHierarchy;
						if (over_view_model != null && over_view_model.IsSupportViewModel(view_model!))
						{
							// Удаляем с предыдущего элемента
							view_model.IParent?.IViewModels.Remove(view_model);

							// Добавляем в текущий
							view_model.IParent = over_view_model;
							over_view_model.IViewModels.Add(view_model);

							// Уведомляем данные
							if(over_view_model.Model is ILotusOwnerObject owner_object)
							{
								owner_object.AttachOwnedObject((view_model.Model as ILotusOwnedObject)!, true);
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
			private void OnTreeViewItem_Selected(object sender, RoutedEventArgs args)
			{
				if(args.Source is TreeViewItem view_item)
				{
					_selectedItem = view_item;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Формирование контекстного меню у элемента
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTreeViewItem_ContextMenuOpening(object sender, ContextMenuEventArgs args)
			{
				TreeViewItem? item_sender = (sender as DependencyObject)?.FindVisualParent<TreeViewItem>();
				TreeViewItem? item_source = (args.OriginalSource as DependencyObject)?.FindVisualParent<TreeViewItem>();
				if (item_sender != null && item_source != null)
				{
					// Делаем событие обработанным чтобы оно не поднималось вверх
					// В случае необходимости меню открываем вручную
					args.Handled = true;

					// Только если оригинальный источник и текущий совпадает
					// В иных случая это означает что событие обрабатывается иным элементом
					if (item_sender == item_source)
					{
						var support_contex_menu = item_source.DataContext as ILotusViewModelHierarchy;
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
			private void OnTreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs args)
			{
				TreeViewItem? item = (args.OriginalSource as DependencyObject)?.FindVisualParent<TreeViewItem>();

				if (_selectedItem != args.Source) return;

				if (item == null || item.DataContext == null) return;

				if (item.DataContext is ILotusViewModelHierarchy view_item_presented)
				{
					// Событие обработано
					args.Handled = true;

					var handled = false;
					if (IsPresentPolicyDefault && (_presentOnlyType == null || view_item_presented.Model.GetType().IsAssignableFrom(_presentOnlyType)))
					{
						if (CollectionViewModelHierarchy != null)
						{
							// Если уже был выбран какой либо элемент
							if (CollectionViewModelHierarchy.IPresentedViewModel != null)
							{
								// И не совпадает
								if (CollectionViewModelHierarchy.IPresentedViewModel != view_item_presented)
								{
									CollectionViewModelHierarchy.IPresentedViewModel.IsPresented = false;
									CollectionViewModelHierarchy.IPresentedViewModel = view_item_presented;
									CollectionViewModelHierarchy.IPresentedViewModel.IsPresented = true;

									_onPresentedItem?.Invoke(view_item_presented);
								}
								else
								{
									// Если совпадают то нет
									CollectionViewModelHierarchy.IPresentedViewModel.IsPresented = false;
									CollectionViewModelHierarchy.IPresentedViewModel = null;
								}
							}
							else
							{
								CollectionViewModelHierarchy.IPresentedViewModel = view_item_presented;
								CollectionViewModelHierarchy.IPresentedViewModel.IsPresented = true;

								_onPresentedItem?.Invoke(view_item_presented);
							}
						}

						handled = true;
					}
					if (SendViewPresented && handled == false && view_item_presented.Model is ILotusModelPresented view_presented)
					{
						view_presented.SetModelPresented(view_item_presented, true);
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
			private void OnTreeViewItem_KeyDown(object sender, KeyEventArgs args)
			{
				if (args.Key == Key.F2)
				{
					TreeViewItem? item = (args.OriginalSource as DependencyObject)?.FindVisualParent<TreeViewItem>();
					if (item != null && item.DataContext != null)
					{
						// Делаем событие обработанным чтобы оно не поднималось вверх
						// В случае необходимости меню открываем вручную
						args.Handled = true;

						if (item.DataContext is ILotusViewModelHierarchy view_item_hierarchy)
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
			private void OnTreeViewItem_Expanded(object sender, RoutedEventArgs args)
			{
				// Method intentionally left empty.
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler? PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(string property_name = "")
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(PropertyChangedEventArgs args)
			{
				PropertyChanged?.Invoke(this, args);
			}
			#endregion

		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
