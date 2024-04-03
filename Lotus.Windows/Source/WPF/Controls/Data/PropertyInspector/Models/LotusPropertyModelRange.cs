//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для работы с данными
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusPropertyModelRange.cs
*		Модель отображения свойства объекта который имеет диапазон изменений.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Reflection;
using System.Collections.Generic;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Core.Inspector;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFControlsData
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Модель отображения свойства объекта который имеет диапазон изменений
		/// </summary>
		/// <typeparam name="TNumeric">Тип значения свойства</typeparam>
		//-------------------------------------------------------------------------------------------------------------
		public class PropertyModelRange<TNumeric> : PropertyModel<TNumeric>
		{
			#region ======================================= ДАННЫЕ ====================================================
			protected internal TNumeric _minValue;
			protected internal TNumeric _maxValue;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Минимальное значение
			/// </summary>
			public TNumeric MinValue
			{
				get { return _minValue; }
			}

			/// <summary>
			/// Максимальное значение
			/// </summary>
			public TNumeric MaxValue
			{
				get { return _maxValue; }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public PropertyModelRange()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public PropertyModelRange(PropertyInfo property_info)
				: base(property_info, TPropertyType.Numeric)
			{
				GetInfoFromAttributesRange();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			/// <param name="property_desc">Список описания свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public PropertyModelRange(PropertyInfo property_info, List<CPropertyDesc> property_desc)
				: base(property_info, property_desc, TPropertyType.Numeric)
			{
				GetInfoFromAttributesRange();
			}
			#endregion

			#region Main methods
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение данных описание свойства с его атрибутов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void GetInfoFromAttributesRange()
			{
				if (_info != null)
				{
					LotusMinValueAttribute? min_value = _info.GetAttribute<LotusMinValueAttribute>();
					if (min_value != null)
					{
						_minValue = (TNumeric)min_value.MinValue;
					}
					else
					{
						FieldInfo? field_info = typeof(TNumeric).GetField(nameof(MinValue), BindingFlags.Static | BindingFlags.Public);
						if (field_info != null)
						{
							_minValue = (TNumeric)field_info.GetValue(null)!;
						}
					}

					LotusMaxValueAttribute? max_value = _info.GetAttribute<LotusMaxValueAttribute>();
					if (max_value != null)
					{
						_maxValue = (TNumeric)max_value.MaxValue;
					}
					else
					{
						FieldInfo? field_info = typeof(TNumeric).GetField(nameof(MaxValue), BindingFlags.Static | BindingFlags.Public);
						if(field_info != null)
						{
							_maxValue = (TNumeric)field_info.GetValue(null)!;
						}
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