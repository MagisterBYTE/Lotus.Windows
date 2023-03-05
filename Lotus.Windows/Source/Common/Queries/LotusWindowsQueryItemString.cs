//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Общая подсистема
// Подраздел: Подсистема запросов данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsQueryItemString.cs
*		Класс представляющий элемент запроса для строковых значений.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup WindowsCommonQueries
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс представляющий элемент запроса для строковых значений
		/// </summary>
		/// <remarks>
		/// Поддерживаются опции поиска представление типом <see cref="TStringSearchOption"/> и стандартная операция LIKE
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public class CQueryItemString : CQueryItem
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			private static readonly PropertyChangedEventArgs PropertyArgsSearchOption = new PropertyChangedEventArgs(nameof(SearchOption));
			private static readonly PropertyChangedEventArgs PropertyArgsSearchValue = new PropertyChangedEventArgs(nameof(SearchValue));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected TStringSearchOption mSearchOption;
			protected String mSearchValue;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Опции поиска в строке
			/// </summary>
			public TStringSearchOption SearchOption
			{
				get
				{
					return (mSearchOption);
				}
				set
				{
					if (mSearchOption != value)
					{
						mSearchOption = value;
						NotifyPropertyChanged(PropertyArgsSearchOption);
						NotifyPropertyChanged(PropertyArgsSQLQueryItem);
						if (QueryOwned != null) QueryOwned.OnNotifyUpdated(this, nameof(SearchOption));
					}
				}
			}

			/// <summary>
			/// Значение для сравнения
			/// </summary>
			public String SearchValue
			{
				get
				{
					return (mSearchValue);
				}
				set
				{
					if (mSearchValue != value)
					{
						mSearchValue = value;
						NotifyPropertyChanged(PropertyArgsSearchValue);
						NotifyPropertyChanged(PropertyArgsSQLQueryItem);
						if (QueryOwned != null) QueryOwned.OnNotifyUpdated(this, nameof(SearchValue));
					}
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CQueryItemString()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="search_option">Опции поиска в строке</param>
			/// <param name="search_value">Значение для сравнения</param>
			//---------------------------------------------------------------------------------------------------------
			public CQueryItemString(TStringSearchOption search_option, String search_value)
			{
				mSearchOption = search_option;
				mSearchValue = search_value;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование к текстовому представлению
			/// </summary>
			/// <returns>Наименование объекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public override String ToString()
			{
				String name = "";
				ComputeSQLQuery(ref name);
				return (name);
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Формирование SQL запроса
			/// </summary>
			/// <param name="sql_query">SQL запрос</param>
			/// <returns>Статус формирования элемента запроса</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean ComputeSQLQuery(ref String sql_query)
			{
				if ((mNotCalculation == false) && (String.IsNullOrEmpty(mSearchValue) == false))
				{
					switch (mSearchOption)
					{
						case TStringSearchOption.Start:
							{
								sql_query += " " + mPropertyName + " LIKE '" + mSearchValue + "%'";
							}
							break;
						case TStringSearchOption.End:
							{
								sql_query += " " + mPropertyName + " LIKE '%" + mSearchValue + "'";
							}
							break;
						case TStringSearchOption.Contains:
							{
								sql_query += " " + mPropertyName + " LIKE '%" + mSearchValue + "%'";
							}
							break;
						case TStringSearchOption.Equal:
							break;
						default:
							break;
					}

					return (true);
				}

				return (false);
			}
			#endregion

			#region ======================================= МЕТОДЫ ПРИВЯЗКИ ===========================================
#if USE_WINDOWS
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Привязка текстового поля к строке поиска
			/// </summary>
			/// <param name="text_box">Текстовое поле</param>
			//---------------------------------------------------------------------------------------------------------
			public void BindingTextBoxToSearchValue(in System.Windows.Controls.TextBox text_box)
			{
				if(text_box != null)
				{
					System.Windows.Data.Binding binding = new System.Windows.Data.Binding();
					binding.Source = this;
					binding.Path = new System.Windows.PropertyPath(nameof(SearchValue));

					System.Windows.Data.BindingOperations.SetBinding(text_box,
						System.Windows.Controls.TextBox.TextProperty, binding);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Привязка выпадающего списка к опциям поиска
			/// </summary>
			/// <param name="combo_box">Выпадающий список</param>
			//---------------------------------------------------------------------------------------------------------
			public void BindingComboBoxToSearchOption(in System.Windows.Controls.ComboBox combo_box)
			{
				if (combo_box != null)
				{
					System.Windows.Data.Binding binding = new System.Windows.Data.Binding();
					binding.Source = this;
					binding.Path = new System.Windows.PropertyPath(nameof(SearchOption));
					binding.Converter = EnumToStringConverter.Instance;

					combo_box.ItemsSource = XEnum.GetDescriptions(typeof(TStringSearchOption));
					System.Windows.Data.BindingOperations.SetBinding(combo_box,
						System.Windows.Controls.ComboBox.SelectedValueProperty, binding);
				}
			}
#endif
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================