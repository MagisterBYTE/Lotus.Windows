//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Имплементация модуля базового ядра
// Подраздел: Подсистема файловой системы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusFileSystemDataViewWindows.cs
*		Специализация ViewModel для работы с объектам файловой системы для Windows.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Windows.Media;
//=====================================================================================================================
namespace Lotus
{
	namespace Core
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup CoreFileSystem
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс реализующий ViewModel для элемента файловой системы для Windows
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class ViewModelFSFileWin : ViewModelFileSystemFile
		{
			#region ======================================= ДАННЫЕ ====================================================
			protected internal ImageSource _iconSource;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Графическая иконка связанная с данным элементом файловой системы
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
								(UInt32)(Windows.TShellAttribute.Icon | Windows.TShellAttribute.SmallIcon));

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

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="model">Модель</param>
			/// <param name="parent_item">Родительский узел</param>
			//---------------------------------------------------------------------------------------------------------
			public ViewModelFSFileWin(ILotusFileSystemEntity model, ILotusViewModelHierarchy? parent_item)
				: base(model, parent_item)
			{
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка контекстного меню
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void SetContextMenu()
			{
				_contextMenuUI = new CUIContextMenuWindows
				{
					ViewModel = this
				};
				_contextMenuUI.AddItem("Показать в проводнике", (ILotusViewModel view_model) =>
				{
					Windows.XNative.ShellExecute(IntPtr.Zero,
						"explore",
						Model.FullName,
						"",
						"",
						Windows.TShowCommands.SW_NORMAL);
				});
				_contextMenuUI.AddItem(CUIContextMenuWindows.Remove.Duplicate());
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие контекстного меню
			/// </summary>
			/// <param name="contextMenu">Контекстное меню</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OpenContextMenu(System.Object contextMenu)
			{
				if (contextMenu is System.Windows.Controls.ContextMenu window_context_menu)
				{
					((CUIContextMenuWindows)_contextMenuUI).SetCommandsDefault(window_context_menu);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Построение дочерней иерархии согласно источнику данных
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void BuildFromModel()
			{
				Clear();
				CollectionViewModelFSWin.BuildFromParent(this, _owner!);
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Коллекция для отображения элементов файловой системы для Windows
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CollectionViewModelFSWin : CollectionViewModelHierarchy<ViewModelFSFileWin, ILotusFileSystemEntity>
		{
			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CollectionViewModelFSWin()
				: base(String.Empty)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя коллекции</param>
			//---------------------------------------------------------------------------------------------------------
			public CollectionViewModelFSWin(String name)
				: base(name)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя коллекции</param>
			/// <param name="source">Источник данных</param>
			//---------------------------------------------------------------------------------------------------------
			public CollectionViewModelFSWin(String name, ILotusFileSystemEntity source)
				: base(name, source)
			{
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusCollectionViewModelHierarchy =================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание конкретной ViewModel для указанной модели
			/// </summary>
			/// <param name="model">Модель</param>
			/// <param name="parent">Родительский элемент ViewModel</param>
			/// <returns>ViewModel</returns>
			//---------------------------------------------------------------------------------------------------------
			public override ILotusViewModelHierarchy CreateViewModelHierarchy(System.Object model, ILotusViewModelHierarchy? parent)
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
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================