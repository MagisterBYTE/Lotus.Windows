//=====================================================================================================================
// Проект: Модуль UI платформы Windows
// Раздел: Элементы интерфейса
// Подраздел: Элементы для ленты
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusRibbonTabAppearance.xaml.cs
*		Вкладка ленты для определения внешнего вида приложения (визуальных стилей отображения).
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Windows;
using System.Windows.Controls;
//---------------------------------------------------------------------------------------------------------------------
using AvalonDock;
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
		/// Вкладка ленты для определения внешнего вида приложения (визуальных стилей отображения)
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusRibbonTabAppearance : RibbonTabItem
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Основной менеджер встраиваемых окон и вкладок
			/// </summary>
			public static readonly DependencyProperty DockManagerProperty = DependencyProperty.Register(nameof(DockManager),
				typeof(DockingManager),
				typeof(LotusRibbonTabAppearance),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Основной менеджер встраиваемых окон и вкладок
			/// </summary>
			public DockingManager DockManager
			{
				get { return (DockingManager)GetValue(DockManagerProperty); }
				set { SetValue(DockManagerProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusRibbonTabAppearance()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(RibbonTabItem));
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка вкладки ленты
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRibbonTabView_Loaded(Object sender, RoutedEventArgs args)
			{
				ribbonDropAccentTheme.ItemsSource = ControlzEx.Theming.ThemeManager.Current.BaseColors;
				ribbonDropAppTheme.ItemsSource = ControlzEx.Theming.ThemeManager.Current.ColorSchemes;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Смена визуального стиля для ленты
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnViewStyleRibbon_Checked(Object sender, SelectionChangedEventArgs args)
			{
				var base_color = ribbonDropAccentTheme.SelectedItem as String;
				var color_schemes = ribbonDropAppTheme.SelectedItem as String;
				if (String.IsNullOrEmpty(base_color) == false && String.IsNullOrEmpty(color_schemes) == false)
				{
					ControlzEx.Theming.ThemeManager.Current.ChangeTheme(Application.Current, base_color, color_schemes);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Смена визуального стиля для элементов управления
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnViewStyleControl_Checked(Object sender, RoutedEventArgs args)
			{
				if (this.IsLoaded)
				{
					var style = ((Fluent.RadioButton)sender)!.Header!.ToString();
					var control = "";
					var toolkit = "";
					switch (style)
					{
						case "Infragistics":
							{
								control = "pack://application:,,,/Lotus.Windows;component/Themes/Infragistics/IG/IG.MSControls.Core.Implicit.xaml";
							}
							break;
						case "Office 2010":
							{
								control = "pack://application:,,,/Lotus.Windows;component/Themes/Infragistics/Office2010Blue/Office2010Blue.MSControls.Core.Implicit.xaml";
							}
							break;
						case "Office 2013":
							{
								control = "pack://application:,,,/Lotus.Windows;component/Themes/Infragistics/Office2013/Office2013.MSControls.Core.Implicit.xaml";
							}
							break;
						default:
							break;
					}

					if (!String.IsNullOrEmpty(control))
					{
						var resource_controls = new ResourceDictionary();
						resource_controls.Source = new Uri(control, UriKind.Absolute);
						Application.Current.Resources.MergedDictionaries.Add(resource_controls);
					}
					if (!String.IsNullOrEmpty(toolkit))
					{
						var resource_toolkit = new ResourceDictionary();
						resource_toolkit.Source = new Uri(toolkit, UriKind.Absolute);
						this.Resources.MergedDictionaries.Add(resource_toolkit);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Смена визуального стиля для таблиц
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnViewStyleTable_Checked(Object sender, RoutedEventArgs args)
			{
				// Method intentionally left empty.
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Смена визуального стиля для панелей
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnViewStyleDocking_Checked(Object sender, RoutedEventArgs args)
			{
				if (this.IsLoaded && DockManager != null)
				{
					var style = ((Fluent.RadioButton)sender).Tag.ToString();
					switch (style)
					{
						case "Aero":
							{
								DockManager.Theme = new AvalonDock.Themes.AeroTheme();
							}
							break;
						case "Metro":
							{
								DockManager.Theme = new AvalonDock.Themes.MetroTheme();
							}
							break;
						case "VS2010":
							{
								DockManager.Theme = new AvalonDock.Themes.VS2010Theme();
							}
							break;
						case "VS2013":
							{
								DockManager.Theme = new AvalonDock.Themes.Vs2013BlueTheme();
							}
							break;
						default:
							break;
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