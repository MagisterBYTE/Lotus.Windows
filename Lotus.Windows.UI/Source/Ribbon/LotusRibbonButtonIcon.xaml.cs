//=====================================================================================================================
// Проект: Модуль UI платформы Windows
// Раздел: Элементы интерфейса
// Подраздел: Элементы для ленты
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusRibbonButtonIcon.xaml.cs
*		Кнопка ленты с поддержкой иконки из связанной команды.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Windows;
using System.Windows.Data;
//---------------------------------------------------------------------------------------------------------------------
using Fluent;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \defgroup WindowsWPFControlsRibbon Элементы ленты
		//! \ingroup WindowsWPFControls
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Кнопка ленты с поддержкой иконки из связанной команды
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusRibbonButtonIcon : Fluent.Button
		{
			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusRibbonButtonIcon()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(Fluent.Button));
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка кнопки
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRibbonButton_Loaded(Object sender, RoutedEventArgs args)
			{
				if(Command != null)
				{
					RoutedIconUICommand command_ui = Command as RoutedIconUICommand;
					if(command_ui != null)
					{
						Binding header_binding = new Binding(nameof(RoutedIconUICommand.Text));
						header_binding.Source = command_ui;
						Binding middle_icon_binding = new Binding(nameof(RoutedIconUICommand.MiddleIcon));
						middle_icon_binding.Source = command_ui;
						Binding large_icon_binding = new Binding(nameof(RoutedIconUICommand.LargeIcon));
						large_icon_binding.Source = command_ui;
						BindingOperations.SetBinding(this, Fluent.Button.HeaderProperty, header_binding);
						BindingOperations.SetBinding(this, Fluent.Button.IconProperty, middle_icon_binding);
						BindingOperations.SetBinding(this, Fluent.Button.LargeIconProperty, large_icon_binding);
					}
				}
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================