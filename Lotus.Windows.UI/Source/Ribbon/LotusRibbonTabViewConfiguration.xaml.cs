//=====================================================================================================================
// Проект: Модуль UI платформы Windows
// Раздел: Элементы интерфейса
// Подраздел: Элементы для ленты
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusRibbonTabViewConfiguration.xaml.cs
*		Вкладка ленты для управления видимостью и положением встраиваемых вкладок и документов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Windows;
//---------------------------------------------------------------------------------------------------------------------
using AvalonDock;
using AvalonDock.Layout;
//---------------------------------------------------------------------------------------------------------------------
using Fluent;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFControlsRibbon
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вкладка ленты для управления видимостью и положением встраиваемых вкладок и документов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusRibbonTabViewConfiguration : RibbonTabItem, INotifyPropertyChanged
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Основной менеджер встраиваемых окон и вкладок
			/// </summary>
			public static readonly DependencyProperty DockManagerProperty = DependencyProperty.Register(nameof(DockManager),
				typeof(DockingManager),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Корневая панель
			/// </summary>
			public static readonly DependencyProperty LayoutPanelRootProperty = DependencyProperty.Register(nameof(LayoutPanelRoot),
				typeof(LayoutPanel),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Основная панель
			/// </summary>
			public static readonly DependencyProperty LayoutPanelMainProperty = DependencyProperty.Register(nameof(LayoutPanelMain),
				typeof(LayoutPanel),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Панель расположения документов
			/// </summary>
			public static readonly DependencyProperty LayoutPanelDocsProperty = DependencyProperty.Register(nameof(LayoutPanelDocs),
				typeof(LayoutPanel),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Левая панель группирования
			/// </summary>
			public static readonly DependencyProperty LayoutAnchorablePaneGroupLeftProperty = DependencyProperty.Register(nameof(LayoutAnchorablePaneGroupLeft),
				typeof(LayoutAnchorablePaneGroup),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Левая панель
			/// </summary>
			public static readonly DependencyProperty LayoutAnchorablePaneLeftProperty = DependencyProperty.Register(nameof(LayoutAnchorablePaneLeft),
				typeof(LayoutAnchorablePane),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Правая панель группирования
			/// </summary>
			public static readonly DependencyProperty LayoutAnchorablePaneGroupRightProperty = DependencyProperty.Register(nameof(LayoutAnchorablePaneGroupRight),
				typeof(LayoutAnchorablePaneGroup),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Правая панель
			/// </summary>
			public static readonly DependencyProperty LayoutAnchorablePaneRightProperty = DependencyProperty.Register(nameof(LayoutAnchorablePaneRight),
				typeof(LayoutAnchorablePane),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Нижняя панель группирования
			/// </summary>
			public static readonly DependencyProperty LayoutAnchorablePaneGroupBottomProperty = DependencyProperty.Register(nameof(LayoutAnchorablePaneGroupBottom),
				typeof(LayoutAnchorablePaneGroup),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Нижняя панель
			/// </summary>
			public static readonly DependencyProperty LayoutAnchorablePaneBottomProperty = DependencyProperty.Register(nameof(LayoutAnchorablePaneBottom),
				typeof(LayoutAnchorablePane),
				typeof(LotusRibbonTabViewConfiguration),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Основной менеджер встраиваемых окон и вкладок
			/// </summary>
			[Browsable(false)]
			public DockingManager DockManager
			{
				get { return (DockingManager)GetValue(DockManagerProperty); }
				set { SetValue(DockManagerProperty, value); }
			}

			/// <summary>
			/// Корневая панель
			/// </summary>
			[Browsable(false)]
			public LayoutPanel LayoutPanelRoot
			{
				get { return (LayoutPanel)GetValue(LayoutPanelRootProperty); }
				set { SetValue(LayoutPanelRootProperty, value); }
			}

			/// <summary>
			/// Основная панель
			/// </summary>
			[Browsable(false)]
			public LayoutPanel LayoutPanelMain
			{
				get { return (LayoutPanel)GetValue(LayoutPanelMainProperty); }
				set { SetValue(LayoutPanelMainProperty, value); }
			}

			/// <summary>
			/// Панель расположения документов
			/// </summary>
			[Browsable(false)]
			public LayoutPanel LayoutPanelDocs
			{
				get { return (LayoutPanel)GetValue(LayoutPanelDocsProperty); }
				set { SetValue(LayoutPanelDocsProperty, value); }
			}

			/// <summary>
			/// Левая панель группирования
			/// </summary>
			[Browsable(false)]
			public LayoutAnchorablePaneGroup LayoutAnchorablePaneGroupLeft
			{
				get { return (LayoutAnchorablePaneGroup)GetValue(LayoutAnchorablePaneGroupLeftProperty); }
				set { SetValue(LayoutAnchorablePaneGroupLeftProperty, value); }
			}

			/// <summary>
			/// Левая панель
			/// </summary>
			[Browsable(false)]
			public LayoutAnchorablePane LayoutAnchorablePaneLeft
			{
				get { return (LayoutAnchorablePane)GetValue(LayoutAnchorablePaneLeftProperty); }
				set { SetValue(LayoutAnchorablePaneLeftProperty, value); }
			}

			/// <summary>
			/// Правая панель группирования
			/// </summary>
			[Browsable(false)]
			public LayoutAnchorablePaneGroup LayoutAnchorablePaneGroupRight
			{
				get { return (LayoutAnchorablePaneGroup)GetValue(LayoutAnchorablePaneGroupRightProperty); }
				set { SetValue(LayoutAnchorablePaneGroupRightProperty, value); }
			}

			/// <summary>
			/// Правая панель
			/// </summary>
			[Browsable(false)]
			public LayoutAnchorablePane LayoutAnchorablePaneRight
			{
				get { return (LayoutAnchorablePane)GetValue(LayoutAnchorablePaneRightProperty); }
				set { SetValue(LayoutAnchorablePaneRightProperty, value); }
			}

			/// <summary>
			/// Нижняя панель группирования
			/// </summary>
			[Browsable(false)]
			public LayoutAnchorablePaneGroup LayoutAnchorablePaneGroupBottom
			{
				get { return (LayoutAnchorablePaneGroup)GetValue(LayoutAnchorablePaneGroupBottomProperty); }
				set { SetValue(LayoutAnchorablePaneGroupBottomProperty, value); }
			}

			/// <summary>
			/// Нижняя панель
			/// </summary>
			[Browsable(false)]
			public LayoutAnchorablePane LayoutAnchorablePaneBottom
			{
				get { return (LayoutAnchorablePane)GetValue(LayoutAnchorablePaneBottomProperty); }
				set { SetValue(LayoutAnchorablePaneBottomProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusRibbonTabViewConfiguration()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(RibbonTabItem));
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка элемента
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRibbonTabItem_Loaded(Object sender, RoutedEventArgs args)
			{
				if(DockManager != null)
				{
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Активация определённой вкладки
			/// </summary>
			/// <param name="layout">Вкладка</param>
			/// <param name="pane">Панель</param>
			/// <param name="pane_group">Группа панелей</param>
			/// <param name="layout_panel">Панель макета</param>
			//---------------------------------------------------------------------------------------------------------
			private void ActivePane(LayoutAnchorable layout, LayoutAnchorablePane pane, LayoutAnchorablePaneGroup pane_group,
				LayoutPanel layout_panel)
			{
				if (!layout.IsVisible)
				{
					if (!pane.IsVisible)
					{
						if (!pane_group.IsVisible)
						{
							if (pane_group.Parent == null)
								layout_panel.Children.Add(pane_group);
						}

						if (pane.Parent == null)
							pane_group.Children.Add(pane);
					}

					if (layout.Parent == null)
						pane.Children.Add(layout);
					layout.IsVisible = true;
				}
				else
				{
					if(layout.CanClose)
					{
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Активация определённой вкладки - документа
			/// </summary>
			/// <param name="layout">Вкладка</param>
			/// <param name="pane">Панель</param>
			/// <param name="pane_group">Группа панелей</param>
			/// <param name="layout_panel">Панель макета</param>
			//---------------------------------------------------------------------------------------------------------
			private void ActivePaneDocument(LayoutDocument layout, LayoutDocumentPane pane, LayoutDocumentPaneGroup pane_group,
				LayoutPanel layout_panel)
			{
				if (layout.Parent == null)
				{
					if (pane.Parent == null)
					{
						if (pane_group.Parent == null)
						{
							layout_panel.Children.Add(pane_group);
						}

						pane_group.Children.Add(pane);
					}

					pane.Children.Add(layout);
				}

				layout.IsActive = true;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка сохранённых состояний рабочего пространства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void OnDockingManagerViewLoad()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка вида рабочего пространства
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDockingManagerViewLoad(Object sender, RoutedEventArgs args)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранения вида рабочего пространства
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDockingManagerViewSave(Object sender, RoutedEventArgs args)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление по умолчанию вида рабочего пространства
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDockingManagerViewRestore(Object sender, RoutedEventArgs args)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Активирование определенной вкладки рабочего пространства
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDockingManagerTabActive(Object sender, RoutedEventArgs args)
			{
				var text = (sender as Fluent.Button).Tag.ToString();
				var layout = DockManager.FindName(text) as LayoutAnchorable;
				if (layout != null)
				{
					switch (text)
					{
						case "layoutAnchorableSolutionExplore":
							{
								ActivePane(layout, LayoutAnchorablePaneLeft, LayoutAnchorablePaneGroupLeft, LayoutPanelRoot);
								var index = LayoutPanelRoot.Children.IndexOf(LayoutAnchorablePaneGroupLeft);
								if (index != 0 && index > 0)
								{
									LayoutPanelRoot.MoveChild(index, 0);
								}
							}
							break;
						case "layoutAnchorableInspectorProperties":
							{
								ActivePane(layout, LayoutAnchorablePaneRight, LayoutAnchorablePaneGroupRight, LayoutPanelRoot);
							}
							break;
						case "layoutAnchorableLogger":
							{
								ActivePane(layout, LayoutAnchorablePaneBottom, LayoutAnchorablePaneGroupBottom, LayoutPanelRoot);
							}
							break;
						default:
							break;
					}

					layout.IsActive = true;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие всех вкладок рабочего пространства
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDockingManagerCloseAllTabs(Object sender, EventArgs args)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие всех вкладок рабочего пространства кроме текущей
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDockingManagerCloseOtherTabs(Object sender, EventArgs args)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие документа рабочего пространства
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDockingManagerDocumentClosing(Object sender, EventArgs args)
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
			/// Вспомогательный метод для нотификации изменений свойства.
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
			/// Вспомогательный метод для нотификации изменений свойства.
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