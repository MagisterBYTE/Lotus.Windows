//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Общая подсистема
// Подраздел: Подсистема запросов данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsQueryItemDateTime.cs
*		Класс представляющий элемент запроса для значений даты-времени.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
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
		/** \addtogroup WindowsCommonQueries
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс представляющий элемент запроса для значений даты-времени
		/// </summary>
		/// <remarks>
		/// Поддерживаются стандартные операторы сравнения и стандартная операция BETWEEN
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public class CQueryItemDateTime : CQueryItem
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			private static readonly PropertyChangedEventArgs PropertyArgsComparisonOperator = new PropertyChangedEventArgs(nameof(ComparisonOperator));
			private static readonly PropertyChangedEventArgs PropertyArgsComparisonValueLeft = new PropertyChangedEventArgs(nameof(ComparisonValueLeft));
			private static readonly PropertyChangedEventArgs PropertyArgsComparisonValueRight = new PropertyChangedEventArgs(nameof(ComparisonValueRight));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected internal TComparisonOperator _comparisonOperator;
			protected internal DateTime _comparisonValueLeft;
			protected internal DateTime _comparisonValueRight;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Оператор сравнения
			/// </summary>
			public TComparisonOperator ComparisonOperator
			{
				get
				{
					return _comparisonOperator;
				}
				set
				{
					if (_comparisonOperator != value)
					{
						_comparisonOperator = value;
						OnPropertyChanged(PropertyArgsComparisonOperator);
						OnPropertyChanged(PropertyArgsSQLQueryItem);
						if (QueryOwned != null) QueryOwned.OnNotifyUpdated(this, nameof(ComparisonOperator));
					}
				}
			}

			/// <summary>
			/// Значение для сравнения слева
			/// </summary>
			public DateTime ComparisonValueLeft
			{
				get
				{
					return _comparisonValueLeft;
				}
				set
				{
					if (ComparisonValueLeft != value)
					{
						_comparisonValueLeft = value;
						OnPropertyChanged(PropertyArgsComparisonValueLeft);
						OnPropertyChanged(PropertyArgsSQLQueryItem);
						if (QueryOwned != null) QueryOwned.OnNotifyUpdated(this, nameof(ComparisonValueLeft));
					}
				}
			}

			/// <summary>
			/// Значение для сравнения справа
			/// </summary>
			public DateTime ComparisonValueRight
			{
				get
				{
					return _comparisonValueRight;
				}
				set
				{
					if (_comparisonValueRight != value)
					{
						_comparisonValueRight = value;
						OnPropertyChanged(PropertyArgsComparisonValueRight);
						OnPropertyChanged(PropertyArgsSQLQueryItem);
						if (QueryOwned != null) QueryOwned.OnNotifyUpdated(this, nameof(_comparisonValueRight));
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
			public CQueryItemDateTime()
			{
				_comparisonValueLeft = DateTime.Now;
				_comparisonValueRight = DateTime.Now;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="comparison_operator">Оператор сравнения</param>
			/// <param name="comparison_value">Значение для сравнения</param>
			//---------------------------------------------------------------------------------------------------------
			public CQueryItemDateTime(TComparisonOperator comparison_operator, DateTime comparison_value)
			{
				_comparisonOperator = comparison_operator;
				_comparisonValueLeft = comparison_value;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="comparison_value_left">Значение для сравнения слева</param>
			/// <param name="comparison_value_right">Значение для сравнения справа</param>
			//---------------------------------------------------------------------------------------------------------
			public CQueryItemDateTime(DateTime comparison_value_left, DateTime comparison_value_right)
			{
				_comparisonOperator = TComparisonOperator.Equality;
				_comparisonValueLeft = comparison_value_left;
				_comparisonValueRight = comparison_value_right;
			}
			#endregion

#region System methods
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование к текстовому представлению
			/// </summary>
			/// <returns>Наименование объекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public override string ToString()
			{
				var name = "";
				ComputeSQLQuery(ref name);
				return name;
			}
			#endregion

			#region Main methods
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Формирование SQL запроса
			/// </summary>
			/// <param name="sql_query">SQL запрос</param>
			/// <returns>Статус формирования элемента запроса</returns>
			//---------------------------------------------------------------------------------------------------------
			public override bool ComputeSQLQuery(ref string sql_query)
			{
				if (_notCalculation == false)
				{
					if (_comparisonOperator == TComparisonOperator.Equality)
					{
						if (_comparisonValueRight > _comparisonValueLeft)
						{
							sql_query += " " + _propertyName + " BETWEEN " + _comparisonValueLeft.ToString()
								+ " AND " + _comparisonValueRight.ToString();
							return true;
						}
					}
					else
					{
						sql_query += " " + _propertyName + _comparisonOperator.GetOperatorOfString() + _comparisonValueLeft.ToString();
						return true;
					}
				}

				return false;
			}
			#endregion

			#region ======================================= МЕТОДЫ ПРИВЯЗКИ ===========================================
#if USE_WINDOWS
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Привязка выпадающего списка к оператору сравнения
			/// </summary>
			/// <param name="combo_box">Выпадающий список</param>
			//---------------------------------------------------------------------------------------------------------
			public void BindingComboBoxToComparisonOperator(in System.Windows.Controls.ComboBox combo_box)
			{
				if (combo_box != null)
				{
					var binding = new System.Windows.Data.Binding();
					binding.Source = this;
					binding.Path = new System.Windows.PropertyPath(path: nameof(ComparisonOperator));
					binding.Converter = EnumToStringConverter.Instance;

					combo_box.ItemsSource = XEnum.GetDescriptions(typeof(TComparisonOperator));
					System.Windows.Data.BindingOperations.SetBinding(combo_box,
						System.Windows.Controls.ComboBox.SelectedValueProperty, binding);
				}
			}
#endif
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================