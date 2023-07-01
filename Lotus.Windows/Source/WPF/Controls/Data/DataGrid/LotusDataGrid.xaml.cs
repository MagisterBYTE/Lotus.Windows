//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для работы с данными
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusDataGrid.xaml.cs
*		Элемент управления отображением данных с расширенной функциональностью.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/**
         * \defgroup WindowsWPFControlsData Элементы для работы с данными
         * \ingroup WindowsWPFControls
         * \brief Элементы для работы с данными.
         * @{
         */
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент управления отображением данных с расширенной функциональностью
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusDataGrid : DataGrid
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			//
			// ФИЛЬТРАЦИЯ ДАННЫХ
			//
			/// <summary>
			/// Предикат фильтрации
			/// </summary>
			public static readonly DependencyProperty FilterPredicateProperty = DependencyProperty.Register(nameof(FilterPredicate),
				typeof(Predicate<System.Object>), typeof(LotusDataGrid), new FrameworkPropertyMetadata(FilterPredicateDefault, 
					FrameworkPropertyMetadataOptions.None));

			/// <summary>
			/// Статус отображения элементов фильтрования у столбцов
			/// </summary>
			public static readonly DependencyProperty IsShowFilterColumnProperty = DependencyProperty.Register(nameof(IsShowFilterColumn),
				typeof(Boolean), typeof(LotusDataGrid), new FrameworkPropertyMetadata(false,
					FrameworkPropertyMetadataOptions.AffectsRender|FrameworkPropertyMetadataOptions.AffectsMeasure,
					OnShowFilterColumnChanged));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			/// <summary>
			/// Предикат фильтрации по умолчанию
			/// </summary>
			private static Predicate<System.Object> FilterPredicateDefault = delegate { return true; };

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения статуса отображения элементов фильтрования у столбцов
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void OnShowFilterColumnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
			{
				var data_grid = (LotusDataGrid)sender;
				var new_value = (Boolean)args.NewValue;
				if (new_value)
				{
					data_grid.ShowFilterColunm();
				}
				else
				{
					data_grid.HideFilterColunm();
				}
			}
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ФИЛЬТРАЦИЯ ДАННЫХ
			//
			/// <summary>
			/// Статус отображения элементов фильтрования у столбцов
			/// </summary>
			[Browsable(false)]
			public Boolean IsShowFilterColumn
			{
				get { return (Boolean)GetValue(IsShowFilterColumnProperty); }
				set { SetValue(IsShowFilterColumnProperty, value); }
			}

			/// <summary>
			/// Предикат фильтрации
			/// </summary>
			[Browsable(false)]
			public Predicate<System.Object> FilterPredicate
			{
				get { return (Predicate<System.Object>)GetValue(FilterPredicateProperty); }
				set { SetValue(FilterPredicateProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusDataGrid()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(DataGrid));
			}
			#endregion

			#region ======================================= МЕТОДЫ ФИЛЬТРОВАНИЯ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Показ фильтров в столбцах
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void ShowFilterColunm()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Скрытие фильтров в столбцах
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void HideFilterColunm()
			{

			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Процесс генерирование столбцов
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDataRecords_AutoGeneratingColumn(Object sender, DataGridAutoGeneratingColumnEventArgs args)
			{
				args.Cancel = false;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание генерирование столбцов
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDataRecords_AutoGeneratedColumns(Object sender, EventArgs args)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Редактирование данных
			/// </summary>
			/// <remarks>
			/// Возникает перед выходом ячейки из режима редактирования
			/// </remarks>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnDataRecords_CellEditEnding(Object sender, DataGridCellEditEndingEventArgs args)
			{

			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================