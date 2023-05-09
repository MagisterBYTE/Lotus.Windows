//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Общая подсистема
// Подраздел: Подсистема запросов данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsQuery.cs
*		Определение основных типов и структур данных для формирования универсальных запросов к репозиторию.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/**
         * \defgroup WindowsCommonQueries Подсистема запросов данных
         * \ingroup WindowsCommon
         * \brief Подсистема запросов данных.
         * @{
         */
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Оператор сравнения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TComparisonOperator
		{
			/// <summary>
			/// Равно
			/// </summary>
			[LotusAbbreviation("=")]
			Equality,

			/// <summary>
			/// Не равно
			/// </summary>
			[LotusAbbreviation("!=")]
			Inequality,

			/// <summary>
			/// Меньше
			/// </summary>
			[LotusAbbreviation("<")]
			LessThan,

			/// <summary>
			/// Меньше или равно
			/// </summary>
			[LotusAbbreviation("<=")]
			LessThanOrEqual,

			/// <summary>
			/// Больше
			/// </summary>
			[LotusAbbreviation(">")]
			GreaterThan,

			/// <summary>
			/// Больше или равно
			/// </summary>
			[LotusAbbreviation(">=")]
			GreaterThanOrEqual
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Статический класс реализующий методы расширений для перечисления <see cref="TComparisonOperator"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XComparisonOperatorExtension
		{
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение текстового представления оператора сравнения
			/// </summary>
			/// <param name="comparison_operator">Оператор сравнения</param>
			/// <returns>Текстовое представление</returns>
			//---------------------------------------------------------------------------------------------------------
			public static String GetOperatorOfString(this TComparisonOperator comparison_operator)
			{
				String result = "";
				switch (comparison_operator)
				{
					case TComparisonOperator.Equality:
						result = " = ";
						break;
					case TComparisonOperator.Inequality:
						result = " != ";
						break;
					case TComparisonOperator.LessThan:
						result = " < ";
						break;
					case TComparisonOperator.LessThanOrEqual:
						result = " <= ";
						break;
					case TComparisonOperator.GreaterThan:
						result = " > ";
						break;
					case TComparisonOperator.GreaterThanOrEqual:
						result = " >= ";
						break;
				}

				return result;
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс представляющий запрос
		/// </summary>
		/// <remarks>
		/// <para>
		/// Запрос представляет собой набор определённых условий по которым извлекаются данные или фильтруются для отображения
		/// </para>
		/// <para>
		/// Он может применяться как к списку с объектам конкретных типов так для работы с сырыми данным
		/// </para>
		/// <para>
		/// Запрос при этом представлен в виде стандартного SQL запроса и предиката
		/// </para>
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public class CQuery : PropertyChangedBase
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			private static readonly PropertyChangedEventArgs PropertyArgsSQLQuery = new PropertyChangedEventArgs(nameof(SQLQuery));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected internal ListArray<CQueryItem> mItems;
			protected internal String mSQLQuery;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Элементы запроса
			/// </summary>
			public ListArray<CQueryItem> Items
			{
				get
				{
					return (mItems);
				}
			}

			/// <summary>
			/// Стандартный SQL запрос
			/// </summary>
			public String SQLQuery
			{
				get
				{
					ComputeSQLQuery();
					return (mSQLQuery);
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CQuery()
			{
				mItems = new ListArray<CQueryItem>();
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusNotify =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Информирование данного объекта о начале изменения данных указанного объекта
			/// </summary>
			/// <param name="source">Объект данные которого будут меняться</param>
			/// <param name="data_name">Имя данных</param>
			/// <returns>Статус разрешения/согласования изменения данных</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean OnNotifyUpdating(System.Object source, String data_name)
			{
				return (true);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Информирование данного объекта об окончании изменении данных указанного объекта
			/// </summary>
			/// <param name="source">Объект данные которого изменились</param>
			/// <param name="data_name">Имя данных</param>
			//---------------------------------------------------------------------------------------------------------
			public void OnNotifyUpdated(System.Object source, String data_name)
			{
				NotifyPropertyChanged(PropertyArgsSQLQuery);
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вычисление SQL запроса на основе элементов запроса
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void ComputeSQLQuery()
			{
				String sql_query = "";

				for (Int32 i = 0; i < mItems.Count; i++)
				{
					if (mItems[i].ComputeSQLQuery(ref sql_query))
					{
						if (i < mItems.Count - 1)
						{
							sql_query += " AND";
						}
					}
				}

				mSQLQuery = sql_query;
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс представляющий элемент запроса
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CQueryItem : PropertyChangedBase, ILotusNotCalculation
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			public static readonly PropertyChangedEventArgs PropertyArgsSQLQueryItem = new PropertyChangedEventArgs(nameof(SQLQueryItem));
			public static readonly PropertyChangedEventArgs PropertyArgsNotCalculation = new PropertyChangedEventArgs(nameof(NotCalculation));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected internal String mPropertyName;
			protected internal CQuery mQueryOwned;
			protected internal Boolean mNotCalculation;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Имя свойства/столбца
			/// </summary>
			public String PropertyName
			{
				get
				{
					return (mPropertyName);
				}
				set
				{
					mPropertyName = value;
				}
			}

			/// <summary>
			/// Запрос
			/// </summary>
			public CQuery QueryOwned
			{
				get
				{
					return (mQueryOwned);
				}
				set
				{
					mQueryOwned = value;
				}
			}

			/// <summary>
			/// Элемент стандартного SQL запроса
			/// </summary>
			public String SQLQueryItem
			{
				get
				{
					return (ToString());
				}
			}

			/// <summary>
			/// Элемент запроса не участвует в запросе
			/// </summary>
			public Boolean NotCalculation
			{
				get { return (mNotCalculation); }
				set
				{
					mNotCalculation = value;
					NotifyPropertyChanged(PropertyArgsNotCalculation);
					if (QueryOwned != null) QueryOwned.OnNotifyUpdated(this, nameof(NotCalculation));
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CQueryItem()
			{
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
			public virtual Boolean ComputeSQLQuery(ref String sql_query)
			{
				return (false);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================