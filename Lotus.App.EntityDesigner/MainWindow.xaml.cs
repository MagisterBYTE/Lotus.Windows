//=====================================================================================================================
using AvalonDock;
using Fluent;
using Lotus.Core;
using Lotus.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lotus.EntityDesigner
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ГЛАВНОЕ ОКНО ========================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Загрузка основного окна и готовность к представлению
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnMainWindow_Loaded(Object sender, RoutedEventArgs args)
		{
			// Устанавливаем глобальные данные по элементам управления
			XWindowManager.PropertyInspector = propertyInspector;

			// Присваиваем команды
			CommandBindings.Add(new CommandBinding(XCommandManager.FileNew, OnFileNew));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileOpen, OnFileOpen));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileSave, OnFileSave));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileSaveAs, OnFileSaveAs));
			CommandBindings.Add(new CommandBinding(XCommandManager.FilePrint, OnFilePrint));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileExport, OnFileExport));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileClose, OnFileClose));
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Закрытие основного окна
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnMainWindow_Closing(Object sender, CancelEventArgs args)
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
		private void OnMainWindowClose(Object sender, RoutedEventArgs args)
		{
			Close();
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
		private void OnFileNew(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Открытие файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileOpen(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сохранение файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileSave(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сохраннее файла под другим имением
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileSaveAs(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Печать файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFilePrint(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Экспорт файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileExport(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Закрытие файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileClose(Object sender, RoutedEventArgs args)
		{

		}
		#endregion
	}
}
//=====================================================================================================================
