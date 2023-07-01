//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для работы с данными
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusPropertyModelVector2D.cs
*		Модель отображения свойства объекта c типом Vector2D
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
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
		/// Модель отображения свойства объекта c типом Vector2D
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CPropertyModelVector2D : PropertyModel<Vector2D>
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Значение свойства
			/// </summary>
			public override Vector2D Value
			{
				get { return mValue; }
				set
				{
					// Произошло изменение свойства со стороны инспектора свойств
					mValue = value;
					if (mInfo != null && mInfo.CanWrite)
					{


						// Обновляем значение свойства у объекта
						mInfo.SetValue(mInstance, ConvertToRealType(), null);
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
			public CPropertyModelVector2D()
			{
				mPropertyType = TPropertyType.Enum;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public CPropertyModelVector2D(PropertyInfo property_info)
				: base(property_info, TPropertyType.Vector2D)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			/// <param name="property_desc">Список описания свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public CPropertyModelVector2D(PropertyInfo property_info, List<CPropertyDesc> property_desc)
				: base(property_info, property_desc, TPropertyType.Vector2D)
			{
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертирование значение Vector2D в реальный тип свойства
			/// </summary>
			/// <returns></returns>
			//---------------------------------------------------------------------------------------------------------
			public System.Object ConvertToRealType()
			{
				if(mInfo.PropertyType == typeof(Vector2Df))
				{
					return new Vector2Df((Single)mValue.X, (Single)mValue.Y);
				}
				if (mInfo.PropertyType == typeof(Vector2Di))
				{
					return new Vector2Di((Int32)mValue.X, (Int32)mValue.Y);
				}
				if (mInfo.PropertyType == typeof(Point))
				{
					return new Point(mValue.X, mValue.Y);
				}
				if (mInfo.PropertyType == typeof(Vector))
				{
					return new Vector(mValue.X, mValue.Y);
				}

				return mValue;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертирование в значение Vector2D из реальный тип свойства
			/// </summary>
			/// <returns></returns>
			//---------------------------------------------------------------------------------------------------------
			public Vector2D ConvertFromRealType()
			{
				if (mInfo.PropertyType == typeof(Vector2Df))
				{
					var vector = (Vector2Df)mInfo.GetValue(mInstance);
					return new Vector2D(vector.X, vector.Y);
				}
				if (mInfo.PropertyType == typeof(Vector2Di))
				{
					var vector = (Vector2Di)mInfo.GetValue(mInstance);
					return new Vector2D(vector.X, vector.Y);
				}
				if (mInfo.PropertyType == typeof(Point))
				{
					var vector = (Point)mInfo.GetValue(mInstance);
					return new Vector2D(vector.X, vector.Y);
				}
				if (mInfo.PropertyType == typeof(Vector))
				{
					var vector = (Vector)mInfo.GetValue(mInstance);
					return new Vector2D(vector.X, vector.Y);
				}

				return (Vector2D)mInfo.GetValue(mInstance);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка значения напрямую
			/// </summary>
			/// <remarks>
			/// В данном случае мы должны уведомить как инспектор свойств и сам объект 
			/// </remarks>
			/// <param name="value">Значение свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public override void SetValue(System.Object value)
			{
				// Устанавливаем значение свойства объекта
				if (mInfo != null)
				{
					mInfo.SetValue(mInstance, value, null);
				}

				// Уведомляем инспектор свойств
				mValue = (Vector2D)value;
				NotifyPropertyChanged(PropertyArgsValue);
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
				if (mInfo != null)
				{
					// Получаем актуальное значение с объекта
					mValue = ConvertFromRealType();

					// Информируем
					NotifyPropertyChanged(PropertyArgsValue);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на значение что оно из списка значений
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void CheckIsValueFromList()
			{
				mIsValueFromList = false;

				if (IsListValues)
				{
					var enumerable = CPropertyDesc.GetValue(mListValues, mListValuesMemberName,
						mListValuesMemberType, mInstance) as IEnumerable;
					foreach (var item in enumerable)
					{
						if(item.Equals(Value))
						{
							mIsValueFromList = true;
							break;
						}
					}
				}

				NotifyPropertyChanged(PropertyArgsIsValueFromList);
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
			protected override void OnPropertyChangedFromInstance(Object sender, PropertyChangedEventArgs args)
			{
				if (mInfo != null && mInfo.Name == args.PropertyName)
				{
					// Получаем актуальное значение с объекта
					mValue = ConvertFromRealType();

					// Информируем
					NotifyPropertyChanged(PropertyArgsValue);
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