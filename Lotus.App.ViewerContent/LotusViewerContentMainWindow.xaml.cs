//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
//---------------------------------------------------------------------------------------------------------------------
using AvalonDock.Layout;
using Xceed.Wpf.Toolkit.PropertyGrid;
//---------------------------------------------------------------------------------------------------------------------
using Fluent;
//---------------------------------------------------------------------------------------------------------------------
//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Drive.v3;
//using Google.Apis.Drive.v3.Data;
//using Google.Apis.Services;
//using Google.Apis.Util.Store;
//---------------------------------------------------------------------------------------------------------------------
//using YandexDisk.Client;
//using YandexDisk.Client.Http;
//using YandexDisk.Client.Clients;
//using YandexDisk.Client.Protocol;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Windows;
//=====================================================================================================================
namespace Lotus
{
	//-----------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	//-----------------------------------------------------------------------------------------------------------------
	public partial class ViewerContentMainWindow : RibbonWindow, INotifyPropertyChanged
	{
		#region =========================================== КОНСТРУКТОРЫ ==============================================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public ViewerContentMainWindow()
		{
			this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
			InitializeComponent();
		}
		#endregion

		#region =========================================== ОБЩИЕ МЕТОДЫ ==============================================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Создание документа
		/// </summary>
		/// <param name="file">Файл</param>
		/// <param name="content">Контент</param>
		/// <returns>Документ</returns>
		//-------------------------------------------------------------------------------------------------------------
		private LayoutDocument CreatePaneDocument(CFileSystemFile file, object content)
		{
			// Создаем документ
			var layout_document = new LayoutDocument();
			layout_document.CanFloat = true;
			layout_document.Title = file.Info.Name;
			layout_document.ToolTip = file.Info.FullName;

			// Присваиваем контент
			layout_document.Content = content;

			// Вставляем в начала
			layoutDocumentPane.InsertChildAt(0, layout_document);

			// Активируем
			layout_document.IsSelected = true;

			return layout_document;
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Активация инструментов для просмотра и редактирования текста
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		private void SetActiveTextEditor()
		{
			ribbonToolsContextualText.Visibility = Visibility.Visible;
			ribbonTabTextEditor.IsSelected = true;

			ribbonToolsContextualImage.Visibility = Visibility.Collapsed;
			ribbonToolsContextualContent3D.Visibility = Visibility.Collapsed;
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Активация инструментов для просмотра и редактирования изображения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		private void SetActiveImageEditor()
		{
			ribbonToolsContextualImage.Visibility = Visibility.Visible;
			ribbonTabImageEditor.IsSelected = true;

			ribbonToolsContextualText.Visibility = Visibility.Collapsed;
			ribbonToolsContextualContent3D.Visibility = Visibility.Collapsed;
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Активация инструментов для просмотра и редактирования 3D контента
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		private void SetActiveContent3DEditor()
		{
			ribbonToolsContextualContent3D.Visibility = Visibility.Visible;
			ribbonTabContent3DEditor.IsSelected = true;

			ribbonToolsContextualText.Visibility = Visibility.Collapsed;
			ribbonToolsContextualImage.Visibility = Visibility.Collapsed;
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ГЛАВНОЕ ОКНО ========================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Загрузка основного окна и готовность к представлению
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnMainWindow_Loaded(object sender, RoutedEventArgs args)
		{
			// Главное окно приложения
			// Создаём менеджер репозиториев
			//mRepositoryDispatcher = new CRepositoryDispatcher();
			//mRepositoryDispatcher.LoadAllRepositories(Path.Combine(XApplicationManager.GetPathDirectoryData(), "Handbook"), true);

			//// Присваиваем иконки панелям
			//layoutAnchorableTreeModel.IconSource = XResources.Fatcow_folders_explorer_32.ToBitmapSource();
			//layoutAnchorableInspectorProperties.IconSource = XResources.NuoveXT_document_properties_16.ToBitmapSource();
			//layoutAnchorableLogger.IconSource = XResources.Oxygen_utilities_log_viewer_16.ToBitmapSource();

			//// Устанавливаем глобальные данные по элементам управления
			//XWindowManager.ExploreModel = treeModel;
			XWindowManager.PropertyInspector = inspectorProperty;
			//XWindowManager.RepositoryDispatcher = mRepositoryDispatcher;
			XLogger.Logger = logger;

			//// Устанавливаем презентаторы данных
			//treeModel.TextPresenter = flowDoc;
			//treeModel.TablePresenter = dataPresent;
			//treeModel.IsNotifySelectedInspector = true;

			// Присваиваем команды
			CommandBindings.Add(new CommandBinding(XCommandManager.FileNew, OnFileNew));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileOpen, OnFileOpen));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileSave, OnFileSave));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileSaveAs, OnFileSaveAs));
			CommandBindings.Add(new CommandBinding(XCommandManager.FilePrint, OnFilePrint));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileExport, OnFileExport));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileClose, OnFileClose));

			dockingManager.Theme = new AvalonDock.Themes.AeroTheme();
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Закрытие основного окна
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnMainWindow_Closing(object sender, CancelEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Закрытие основного окна
		/// </summary>
		/// <remarks>
		/// Применяется при закрытие другим способом
		/// </remarks>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnMainWindowClose(object sender, RoutedEventArgs args)
		{
			Close();
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - РЕДАКТИРОВАНИЕ ======================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Быстрое сохранение
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditSave(object sender, RoutedEventArgs args)
		{
			//XManager.SaveAllProject();
			//XManager.SaveAllDocument();
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Копирование в буфер обмена
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditCopy(object sender, RoutedEventArgs args)
		{
			//XManager.Editor.Copy();
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вырезать объект в буфер обмена
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditCut(object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вставка из буфера обмена
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditPaste(object sender, RoutedEventArgs args)
		{
			//XManager.Editor.Paste();
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Отмена последнего действия
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditUndo(object sender, RoutedEventArgs args)
		{
			//XManager.MementoManager.Undo();
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Повтор отменённого действия
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditRedo(object sender, RoutedEventArgs args)
		{
			//XManager.MementoManager.Redo();
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Общие обновление
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditRefresh(object sender, RoutedEventArgs args)
		{

		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ФАЙЛ ================================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Создание файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileNew(object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Открытие файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileOpen(object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сохранение файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileSave(object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сохраннее файла под другим имением
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileSaveAs(object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Печать файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFilePrint(object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Экспорт файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileExport(object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Закрытие файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileClose(object sender, RoutedEventArgs args)
		{

		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - РАБОТА С YANDEX DISK ================
		
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - РАБОТА С GOOGLE DISK ================
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - МЕНЕДЖЕР ОКОН =======================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Активация вкладки
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnDockingManager_ActiveContentChanged(object sender, EventArgs args)
		{
			if(dockingManager.ActiveContent is LotusViewerText viewerText)
			{
				if(ribbonTabTextEditor.TextViewEditor != viewerText)
				{
					ribbonTabTextEditor.TextViewEditor = viewerText;
				}

				SetActiveTextEditor();
			}
			if (dockingManager.ActiveContent is LotusViewerImage viewerImage)
			{
				if (ribbonTabImageEditor.ImageViewEditor != viewerImage)
				{
					ribbonTabImageEditor.ImageViewEditor = viewerImage;
				}

				SetActiveImageEditor();
			}

			if (dockingManager.ActiveContent is LotusViewerContent3D viewerContent3D)
			{
				if (ribbonTabContent3DEditor.Content3DViewEditor != viewerContent3D)
				{
					ribbonTabContent3DEditor.Content3DViewEditor = viewerContent3D;
				}

				SetActiveContent3DEditor();
			}
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ОБОЗРЕВАТЕЛЬ ФАЙЛОВОЙ СИСТЕМ ========
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Выбор источника данных для обозревателя файловой системы
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnButtonFileSystemSourceOpen_Click(object sender, RoutedEventArgs args)
		{
			var folder_browser = new System.Windows.Forms.FolderBrowserDialog();
			if(folder_browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (comboFileSystemSource.Items.IndexOf(folder_browser.SelectedPath) == -1)
				{
					comboFileSystemSource.Items.Add(folder_browser.SelectedPath);
				}
			}

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Восстановление источника данных для обозревателя файловой системы
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnButtonFileSystemSourceRefresh_Click(object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Выбор источника данных для обозревателя файловой системы
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnComboFileSystemSource_SelectionChanged(object sender, SelectionChangedEventArgs args)
		{
			//busyIndicator.IsBusy = true;

			if (comboFileSystemSource.SelectedItem != null)
			{
				Task.Factory.StartNew(() =>
				{
					Dispatcher.BeginInvoke((Action)delegate
					{
						var full_path = comboFileSystemSource.SelectedItem.ToString();
						var directory = new CFileSystemDirectory(full_path);
						directory.GetFileSystemItemsTwoLevel();

						//Thread.Sleep(1000);
						//busyIndicator.BusyContent = "Получение директорий";

						var view_file_system = new CollectionViewModelFSWin();
						view_file_system.IsNotify = true;
						view_file_system.Source = directory;

						//Thread.Sleep(1000);
						//busyIndicator.BusyContent = "Создание визуальной модели";


						treeExploreFileSystem.ItemTemplateSelector = CFileSystemEntityDataSelector.Instance;
						treeExploreFileSystem.ItemsSource = view_file_system;

						//Thread.Sleep(1000);
						//busyIndicator.BusyContent = "Отображение в компаненте";

					}, System.Windows.Threading.DispatcherPriority.Normal);

				}).ContinueWith((task) =>
				{
					//busyIndicator.IsBusy = false;
				},
				TaskScheduler.FromCurrentSynchronizationContext());
			}

			//busyIndicator.IsBusy = false;
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Представление элемента отображения в отдельном контексте
		/// </summary>
		/// <param name="item">Элемент отображения</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnTreeExploreFileSystem_PresentedItem(ILotusViewModelHierarchy item)
		{
			if(item.Model is CFileSystemFile file)
			{
				//
				// Изображения
				//
				if (LotusViewerImage.IsSupportFormatFile(file.Info.Extension.ToLower()))
				{
					// Редактор изображений
					var viewerImage = new LotusViewerImage();

					// Присоединяем к ленте и активируем
					ribbonTabImageEditor.ImageViewEditor = viewerImage;
					SetActiveImageEditor();

					// Создаем документ
					CreatePaneDocument(file, viewerImage);

					// Загружаем изображение
					viewerImage.OpenFile(file.FullName, null);
				}

				//
				// 3D контент
				//
				if (LotusViewerContent3D.IsSupportFormatFile(file.Info.Extension.ToLower()))
				{
					// Редактор 3D контента
					var viewerContent3D = new LotusViewerContent3D();

					// Присоединяем к ленте и активируем
					ribbonTabContent3DEditor.Content3DViewEditor = viewerContent3D;
					SetActiveContent3DEditor();

					// Создаем документ
					CreatePaneDocument(file, viewerContent3D);

					// Загружаем файл контента
					var parameters = new CParameters();
					parameters.AddObject("tree_view_model_structure", treeViewModel, false);
					viewerContent3D.OpenFile(file.FullName, parameters);

					var templateSelector = Resources["HelixToolkitDataSelectorKey"] as DataTemplateSelector;
					treeViewModel.ItemTemplateSelector = templateSelector;
					//treeViewModel.ItemsSource = viewerContent3D.Scene;
				}

				//
				// Текстовые документы
				//
				if (LotusViewerText.IsSupportFormatFile(file.Info.Extension.ToLower()))
				{
					// Текстовый редактор
					var viewerText = new LotusViewerText();

					// Присоединяем к ленте и активируем
					ribbonTabTextEditor.TextViewEditor = viewerText;
					SetActiveTextEditor();

					// Создаем документ
					CreatePaneDocument(file, viewerText);

					// Открываем файл
					viewerText.OpenFile(file.Info.FullName, null);
				}
			}
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ОБОЗРЕВАТЕЛЬ ПРОЕКТА ================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Выбор элемента в обозревателе проекта
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorerSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
		{
			//inspectorProperties.SelectedObject = args.NewValue;

			//// Если выбран проект
			//IBaseProject project = args.NewValue as IBaseProject;
			//if (project != null)
			//{
			//	SelectedProject = project;
			//	return;
			//}

			//// Если выбран документ
			//IBaseDocument doc = args.NewValue as IBaseDocument;
			//if (doc != null)
			//{
			//	SelectedDocument = doc;
			//	return;
			//}

			//IBaseElement element = args.NewValue as IBaseElement;
			//if (element != null)
			//{
			//	SelectedElement = element;
			//	XManager.Editor.Add(element);
			//}

			//// Удаляем
			//if (args.OldValue != null)
			//{
			//	IBaseElement remove_element = args.OldValue as IBaseElement;
			//	XManager.Editor.Remove(remove_element);
			//}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Двойной шелчок по элементу в обозревателе проекта
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorerItemDoubleClick(object sender, MouseButtonEventArgs args)
		{
			//IBaseDocument doc = solutionExplorer.SelectedItem as IBaseDocument;
			//if (doc != null)
			//{
			//	XCadManager.Canvas.PresenterDocument = doc;
			//	XCadManager.Canvas.Update();
			//}

			//ICadShape element = solutionExplorer.SelectedItem as ICadShape;
			//if (element != null)
			//{
			//	XCadManager.CanvasViewer.AnimatedZoomTo(element.BoundsRect);
			//	element.SetHandleRects();
			//}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Начало перетаскивания элемента в обозревателе проекта
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorerDragStart(object sender, DragEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Событие постоянно возникает при перетаскивании данных над объектом-приемником
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorerDragOver(object sender, DragEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Проверка типа перетаскиваемых данных и определение типа разрешаемой операции
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorerDragEnter(object sender, DragEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Событие постоянно возникает при покидании объекта-приемника
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorerDragLeave(object sender, DragEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Событие возникает, когда данные сбрасываются над объектом-приемником; по умолчанию это происходит 
		/// при отпускании кнопки мыши.
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorerDrop(object sender, DragEventArgs args)
		{
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ИНСПЕКТОР СВОЙСТВ ===================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Изменение выбранного объекта в инспекторе свойств
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnInspectorPropertiesObjectChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Подготовка свойства для отрисовки в инспекторе свойств
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnInspectorPropertiesPrepareProperty(object sender, PropertyItemEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Изменение выбранного свойства в инспекторе свойств
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnInspectorPropertiesPropertyValueChanged(object sender, PropertyValueChangedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Переход от одного свойства к другому в инспекторе свойств
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnInspectorPropertiesPropertySelectedChanged(object sender, RoutedPropertyChangedEventArgs<PropertyItemBase> args)
		{

		}
		#endregion

		#region =========================================== ДАННЫЕ INotifyPropertyChanged =============================
		/// <summary>
		/// Событие срабатывает ПОСЛЕ изменения свойства
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вспомогательный метод для нотификации изменений свойства.
		/// </summary>
		/// <param name="property_name">Имя свойства</param>
		//-------------------------------------------------------------------------------------------------------------
		public void NotifyPropertyChanged(string property_name = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property_name));
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вспомогательный метод для нотификации изменений свойства.
		/// </summary>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		public void NotifyPropertyChanged(PropertyChangedEventArgs args)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, args);
			}
		}

		#endregion
	}
}
//=====================================================================================================================