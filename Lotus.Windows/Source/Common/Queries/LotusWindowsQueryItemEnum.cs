//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Общая подсистема
// Подраздел: Подсистема запросов данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsQueryItemEnum.cs
*		Класс представляющий элемент запроса для перечисляемых значений.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsCommonQueries
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс представляющий элемент запроса для перечисляемых значений
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CQueryItemEnum : CQueryItem
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			private static readonly PropertyChangedEventArgs PropertyArgsSourceItems = new PropertyChangedEventArgs(nameof(SourceItems));
			private static readonly PropertyChangedEventArgs PropertyArgsFiltredItems = new PropertyChangedEventArgs(nameof(FiltredItems));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected internal List<System.Object> _sourceItems;
			protected internal List<System.Object> _filtredItems;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Коллекция которая является источником данных
			/// </summary>
			public List<System.Object> SourceItems
			{
				get
				{
					return _sourceItems;
				}
				set
				{
					_sourceItems = value;
					NotifyPropertyChanged(PropertyArgsSourceItems);
				}
			}

			/// <summary>
			/// Список элементов которые выбраны
			/// </summary>
			public List<System.Object> FiltredItems
			{
				get
				{
					return _filtredItems;
				}
				set
				{
					_filtredItems = value;
					NotifyPropertyChanged(PropertyArgsFiltredItems);
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CQueryItemEnum()
			{
				_filtredItems = new List<System.Object>();
				_sourceItems = new List<System.Object>();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="enum_type">Тип перечисления</param>
			//---------------------------------------------------------------------------------------------------------
			public CQueryItemEnum(Type enum_type)
			{
				_filtredItems = new List<System.Object>();
				_sourceItems = new List<System.Object>();
				_sourceItems.AddRange(XEnum.GetDescriptions(enum_type));
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
				return JoinFiltredItems();
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
				if (_notCalculation == false)
				{
					if (_filtredItems.Count > 0)
					{
						var included = new StringBuilder(_filtredItems.Count * 10);
						for (var i = 0; i < _filtredItems.Count; i++)
						{
							if (i != 0)
							{
								included.Append(", ");
							}

							included.Append("'" + _filtredItems[i].ToString() + "'");
						}

						sql_query += " " + _propertyName + " IN (" + included.ToString() + ")";
						return true;
					}
				}

				return false;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Соединение выбранных элементов одну строку
			/// </summary>
			/// <returns>Строка с выбранными элементами</returns>
			//---------------------------------------------------------------------------------------------------------
			public String JoinFiltredItems()
			{
				if (_filtredItems.Count > 0)
				{
					var included = new StringBuilder(_filtredItems.Count * 10);
					for (var i = 0; i < _filtredItems.Count; i++)
					{
						if (i != 0)
						{
							included.Append(", ");
						}

						included.Append(_filtredItems[i].ToString());
					}

					return included.ToString();
				}

				return String.Empty;
			}
			#endregion

			#region ======================================= МЕТОДЫ ПРИВЯЗКИ ===========================================
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================