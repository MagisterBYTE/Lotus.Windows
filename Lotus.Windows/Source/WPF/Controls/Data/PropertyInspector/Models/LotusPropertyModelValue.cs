//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для работы с данными
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusPropertyModelValue.cs
*		Модель отображения свойства объекта к конкретными типом значения свойства.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Core.Inspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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
		/// Модель отображения свойства объекта к конкретными типом значения свойства
		/// </summary>
		/// <typeparam name="TValue">Тип значения свойства</typeparam>
		//-------------------------------------------------------------------------------------------------------------
		public class PropertyModel<TValue> : CPropertyModelBase, IComparable<PropertyModel<TValue>>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static readonly PropertyChangedEventArgs PropertyArgsValue = new PropertyChangedEventArgs(nameof(Value));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected internal TValue _value;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Значение свойства
			/// </summary>
			public virtual TValue Value
			{
				get { return _value; }
				set
				{
					// Произошло изменение свойства со стороны инспектора свойств
					_value = value;
					if (_info != null && _info.CanWrite)
					{
						// Обновляем значение свойства у объекта
						_info.SetValue(_instance, _value, null);
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
			public PropertyModel()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public PropertyModel(PropertyInfo property_info)
				: base(property_info)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			/// <param name="property_type">Допустимый тип свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public PropertyModel(PropertyInfo property_info, TPropertyType property_type)
				: base(property_info, property_type)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			/// <param name="property_desc">Список описания свойства</param>
			/// <param name="property_type">Допустимый тип свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public PropertyModel(PropertyInfo property_info, List<CPropertyDesc> property_desc, TPropertyType property_type)
				: base(property_info, property_desc, property_type)
			{

			}
			#endregion

			#region System methods
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение объектов для упорядочивания
			/// </summary>
			/// <param name="other">Сравниваемый объект</param>
			/// <returns>Статус сравнения объектов</returns>
			//---------------------------------------------------------------------------------------------------------
			public int CompareTo(PropertyModel<TValue>? other)
			{
				return base.CompareTo(other);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование к текстовому представлению
			/// </summary>
			/// <returns>Краткое наименование финасового инструмента</returns>
			//---------------------------------------------------------------------------------------------------------
			public override string ToString()
			{
				return DisplayName;
			}
			#endregion

			#region Main methods
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка значения напрямую
			/// </summary>
			/// <remarks>
			/// В данном случае мы должны уведомить как инспектор свойств и сам объект 
			/// </remarks>
			/// <param name="value">Значение свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public override void SetValue(object value)
			{
				// Устанавливаем значение свойства объекта
				if (_info != null)
				{
					_info.SetValue(_instance, value, null);
				}

				// Уведомляем инспектор свойств
				_value = (TValue)value;
				OnPropertyChanged(PropertyArgsValue);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка нового объекта
			/// </summary>
			/// <remarks>
			/// В данном случае мы должны уведомить инспектор свойств
			/// </remarks>
			//---------------------------------------------------------------------------------------------------------
			protected override void SetInstance()
			{
				if (_info != null)
				{
					try
					{
						// Получаем актуальное значение с объекта
						_value = (TValue)_info.GetValue(_instance)!;
					}
					catch (InvalidCastException invalid_cast)
					{
						XLogger.LogException(invalid_cast);
					}

					// Информируем
					OnPropertyChanged(PropertyArgsValue);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на значение что оно из списка значений
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void CheckIsValueFromList()
			{
				_isValueFromList = false;

				if (IsListValues)
				{
					var enumerable = CPropertyDesc.GetValue(_listValues, _listValuesMemberName,
						_listValuesMemberType, _instance) as IEnumerable;
					if (enumerable != null)
					{
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
						foreach (var item in enumerable)
						{
							if (item.Equals(Value))
							{
								_isValueFromList = true;
								break;
							}
						}
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
					}
				}

				OnPropertyChanged(PropertyArgsIsValueFromList);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения свойства со стороны объекта
			/// </summary>
			/// <remarks>
			/// В данном случае мы должны уведомить инспектор свойств
			/// </remarks>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnPropertyChangedFromInstance(object? sender, PropertyChangedEventArgs args)
			{
				if (_info != null && _info.Name == args.PropertyName)
				{
					// Получаем актуальное значение с объекта
					try
					{
						_value = (TValue)_info.GetValue(_instance)!;
					}
					catch (InvalidCastException invalid_cast)
					{
						XLogger.LogException(invalid_cast);
					}

					// Информируем
					OnPropertyChanged(PropertyArgsValue);
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