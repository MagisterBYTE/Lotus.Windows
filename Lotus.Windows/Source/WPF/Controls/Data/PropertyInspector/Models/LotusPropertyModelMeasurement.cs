//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для работы с данными
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusPropertyModelMeasurement.cs
*		Модель отображения свойства объекта который имеет тип единицы измерения.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.UnitMeasurement;
using Lotus.Windows;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup WindowsWPFControlsData
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Модель отображения свойства объекта типа <see cref="TMeasurementValue"/>
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class PropertyModelMeasurementValue : PropertyModel<TMeasurementValue>
		{
			#region ======================================= ДАННЫЕ ====================================================
			internal Double mMinValue;
			internal Double mMaxValue;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Минимальное значение
			/// </summary>
			public Double MinValue
			{
				get { return (mMinValue); }
			}

			/// <summary>
			/// Максимальное значение
			/// </summary>
			public Double MaxValue
			{
				get { return (mMaxValue); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public PropertyModelMeasurementValue()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public PropertyModelMeasurementValue(PropertyInfo property_info)
				: base(property_info, TPropertyType.Measurement)
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
			public PropertyModelMeasurementValue(PropertyInfo property_info, List<CPropertyDesc> property_desc)
				: base(property_info, property_desc, TPropertyType.Measurement)
			{
				GetInfoFromAttributesRange();
			}

			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение данных описание свойства с его атрибутов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void GetInfoFromAttributesRange()
			{
				if (mInfo != null)
				{
					LotusMinValueAttribute min_value = mInfo.GetAttribute<LotusMinValueAttribute>();
					if (min_value != null)
					{
						mMinValue = (Double)(Object)min_value.MinValue;
					}
					else
					{
						mMinValue = Double.MinValue;
					}

					LotusMaxValueAttribute max_value = mInfo.GetAttribute<LotusMaxValueAttribute>();
					if (max_value != null)
					{
						mMaxValue = (Double)(Object)max_value.MaxValue;
					}
					else
					{
						mMaxValue = Double.MaxValue;
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