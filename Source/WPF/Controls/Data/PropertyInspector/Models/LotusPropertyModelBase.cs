//=====================================================================================================================
// Проект: LotusWindows
// Раздел: Модуль работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы для работы с данными
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusPropertyModelBase.cs
*		Базовая модель отображения свойства объекта.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows.Controls;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
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
		/// Допустимый тип свойства
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TPropertyType
		{
			/// <summary>
			/// Логический тип
			/// </summary>
			Boolean,

			/// <summary>
			/// Числовой тип
			/// </summary>
			Numeric,

			/// <summary>
			/// Тип единицы измерения
			/// </summary>
			Measurement,

			/// <summary>
			/// Перечисление
			/// </summary>
			Enum,

			/// <summary>
			/// Строковый тип
			/// </summary>
			String,

			/// <summary>
			/// Тип даты-времени
			/// </summary>
			DateTime,

			/// <summary>
			/// Двухмерный вектор
			/// </summary>
			Vector2D,

			/// <summary>
			/// Базовый тип
			/// </summary>
			Object,

			/// <summary>
			/// Неизвестный тип
			/// </summary>
			Unknow
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Базовая модель отображения свойства объекта
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CPropertyModelBase : PropertyChangedBase, IComparable<CPropertyModelBase>, IDisposable
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsIsValueFromList = new PropertyChangedEventArgs(nameof(IsValueFromList));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			protected internal PropertyInfo mInfo;
			protected internal TPropertyType mPropertyType;
			protected internal System.Object mInstance;

			// Параметры описания
			protected internal String mDisplayName;
			protected internal String mDescription;
			protected internal Int32 mPropertyOrder = -1;
			protected internal String mCategory;
			protected internal Int32 mCategoryOrder = -1;

			// Параметры управления
			protected internal Boolean mIsReadOnly;
			protected internal Object mDefaultValue;
			protected internal String mFormatValue;

			// Список значений величины
			protected internal Object mListValues;
			protected internal String mListValuesMemberName;
			protected internal TInspectorMemberType mListValuesMemberType;
			protected internal Boolean mIsValueFromList;

			// Управление кнопкой
			protected internal String mButtonCaption;
			protected internal String mButtonMethodName;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Метаданные свойства объекта
			/// </summary>
			public PropertyInfo Info
			{
				get { return (mInfo); }
				set
				{
					mInfo = value;
				}
			}

			/// <summary>
			/// Допустимый тип свойства
			/// </summary>
			public TPropertyType PropertyType
			{
				get { return (mPropertyType); }
				set
				{
					mPropertyType = value;
				}
			}

			/// <summary>
			/// Экземпляр объекта
			/// </summary>
			public System.Object Instance
			{
				get { return (mInstance); }
				set
				{
					if (mInstance != null)
					{
						if (mInstance != value)
						{
							// Если объект поддерживает стандартную нотификацию
							INotifyPropertyChanged property_changed_prev = mInstance as INotifyPropertyChanged;
							if (property_changed_prev != null)
							{
								property_changed_prev.PropertyChanged -= OnPropertyChangedFromInstance;
							}
						}
					}

					mInstance = value;
					if (mInstance != null)
					{
						// Если объект поддерживает стандартную нотификацию
						INotifyPropertyChanged property_changed = mInstance as INotifyPropertyChanged;
						if (property_changed != null)
						{
							property_changed.PropertyChanged += OnPropertyChangedFromInstance;
						}

						SetInstance();
					}
				}
			}

			//
			// ПАРАМЕТРЫ ОПИСАНИЯ
			//
			/// <summary>
			/// Отображаемое имя свойства
			/// </summary>
			public String DisplayName
			{
				get
				{
					if (String.IsNullOrEmpty(mDisplayName))
					{
						if (mInfo != null)
						{
							return (mInfo.Name);
						}
						else
						{
							return ("Noname");
						}
					}
					else
					{
						return (mDisplayName);
					}
				}
				set
				{
					mDisplayName = value;
				}
			}

			/// <summary>
			/// Описание свойства
			/// </summary>
			public String Description
			{
				get { return (mDescription); }
				set
				{
					mDescription = value;
				}
			}

			/// <summary>
			/// Порядковый номер отображения свойства внутри категории
			/// </summary>
			public Int32 PropertyOrder
			{
				get { return (mPropertyOrder); }
				set
				{
					mPropertyOrder = value;
				}
			}

			/// <summary>
			/// Категория свойства
			/// </summary>
			public String Category
			{
				get { return (mCategory); }
				set
				{
					mCategory = value;
				}
			}

			/// <summary>
			/// Порядковый номер отображения категории
			/// </summary>
			public Int32 CategoryOrder
			{
				get { return (mCategoryOrder); }
				set
				{
					mCategoryOrder = value;
				}
			}

			//
			// ПАРАМЕТРЫ УПРАВЛЕНИЯ
			//
			/// <summary>
			/// Свойство только для чтения
			/// </summary>
			public Boolean IsReadOnly
			{
				get { return (mIsReadOnly); }
				set
				{
					mIsReadOnly = value;
				}
			}

			/// <summary>
			/// Значение свойства по умолчанию
			/// </summary>
			public Object DefaultValue
			{
				get { return (mDefaultValue); }
				set
				{
					mDefaultValue = value;
				}
			}

			/// <summary>
			/// Статус наличия значения по умолчанию
			/// </summary>
			public Boolean IsDefaultValue
			{
				get { return (mDefaultValue != null); }
			}

			/// <summary>
			/// Формат отображения значения свойства
			/// </summary>
			public String FormatValue
			{
				get { return (mFormatValue); }
				set
				{
					mFormatValue = value;
				}
			}

			/// <summary>
			/// Статус наличия формата отображения значения свойства
			/// </summary>
			public Boolean IsFormatValue
			{
				get { return (String.IsNullOrEmpty(FormatValue) == false); }
			}

			//
			// СПИСОК ЗНАЧЕНИЙ ВЕЛИЧИНЫ
			//
			/// <summary>
			/// Список допустимых значений свойств
			/// </summary>
			public Object ListValues
			{
				get { return (mListValues); }
				set
				{
					mListValues = value;
				}
			}

			/// <summary>
			/// Имя члена данных списка допустимых значений свойств
			/// </summary>
			public String ListValuesMemberName
			{
				get { return (mListValuesMemberName); }
				set
				{
					mListValuesMemberName = value;
				}
			}

			/// <summary>
			/// Тип члена данных списка допустимых значений свойств
			/// </summary>
			public TInspectorMemberType ListValuesMemberType
			{
				get { return (mListValuesMemberType); }
				set
				{
					mListValuesMemberType = value;
				}
			}

			/// <summary>
			/// Статус наличия списка значений
			/// </summary>
			public Boolean IsListValues
			{
				get { return (mListValues != null || mListValuesMemberName.IsExists()); }
			}

			/// <summary>
			/// Статус значения свойства из списка значений
			/// </summary>
			public Boolean IsValueFromList
			{
				get { return (mIsValueFromList); }
				set
				{
					mIsValueFromList = value;
					NotifyPropertyChanged(PropertyArgsIsValueFromList);
				}
			}

			//
			// УПРАВЛЕНИЕ КНОПКОЙ
			//
			/// <summary>
			/// Надпись над кнопкой
			/// </summary>
			public String ButtonCaption
			{
				get { return (mButtonCaption); }
			}

			/// <summary>
			/// Имя метода который вызывается при нажатии на кнопку
			/// </summary>
			public String ButtonMethodName
			{
				get { return (mButtonMethodName); }
			}

			/// <summary>
			/// Статус наличия атрибута для управления свойством через кнопку
			/// </summary>
			public Boolean IsButtonMethod
			{
				get { return (mButtonMethodName.IsExists()); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CPropertyModelBase()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public CPropertyModelBase(PropertyInfo property_info)
				:this(property_info, TPropertyType.Unknow)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			/// <param name="property_type">Допустимый тип свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public CPropertyModelBase(PropertyInfo property_info, TPropertyType property_type)
			{
				mInfo = property_info;
				mPropertyType = property_type;
				ApplyInfoFromAttributes();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="property_info">Метаданные свойства</param>
			/// <param name="property_desc">Список описания свойства</param>
			/// <param name="property_type">Допустимый тип свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public CPropertyModelBase(PropertyInfo property_info, List<CPropertyDesc> property_desc, TPropertyType property_type)
			{
				mInfo = property_info;
				mPropertyType = property_type;
				ApplyInfoFromDecs(property_desc);	// Имеет преимущество
				ApplyInfoFromAttributes();
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение объектов для упорядочивания
			/// </summary>
			/// <param name="other">Сравниваемый объект</param>
			/// <returns>Статус сравнения объектов</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CPropertyModelBase other)
			{
				Int32 category_order = mCategoryOrder.CompareTo(other.CategoryOrder);
				if(category_order == 0)
				{
					if (mCategory.IsExists())
					{
						Int32 category = mCategory.CompareTo(other.Category);
						if (category == 0)
						{
							return (mPropertyOrder.CompareTo(other.PropertyOrder));
						}
						else
						{
							return (category);
						}
					}
					else
					{
						return (1);
					}
				}
				else
				{
					return (category_order);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование к текстовому представлению
			/// </summary>
			/// <returns>Краткое наименование финасового инструмента</returns>
			//---------------------------------------------------------------------------------------------------------
			public override String ToString()
			{
				return (DisplayName);
			}
			#endregion

			#region ======================================= МЕТОДЫ IDisposable ========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Освобождение управляемых ресурсов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Освобождение управляемых ресурсов
			/// </summary>
			/// <param name="disposing">Статус освобождения</param>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void Dispose(Boolean disposing)
			{
				// Освобождаем только управляемые ресурсы
				if (disposing)
				{
					Instance = null;
				}

				// Освобождаем неуправляемые ресурсы
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение данных описание свойства с его атрибутов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void ApplyInfoFromAttributes()
			{
				if (mInfo != null)
				{
					DisplayNameAttribute display_name = mInfo.GetAttribute<DisplayNameAttribute>();
					if (display_name != null && String.IsNullOrEmpty(mDisplayName))
					{
						mDisplayName = display_name.DisplayName;
					}

					DescriptionAttribute description = mInfo.GetAttribute<DescriptionAttribute>();
					if (description != null && String.IsNullOrEmpty(mDescription))
					{
						mDescription = description.Description;
					}

					LotusPropertyOrderAttribute property_order = mInfo.GetAttribute<LotusPropertyOrderAttribute>();
					if (property_order != null)
					{
						mPropertyOrder = property_order.Order;
					}

					LotusAutoOrderAttribute auto_order = mInfo.GetAttribute<LotusAutoOrderAttribute>();
					if (auto_order != null)
					{
						mPropertyOrder = auto_order.Order;
					}

					CategoryAttribute category = mInfo.GetAttribute<CategoryAttribute>();
					if (category != null && String.IsNullOrEmpty(mCategory))
					{
						mCategory = category.Category;
					}

					LotusCategoryOrderAttribute category_order = mInfo.GetAttribute<LotusCategoryOrderAttribute>();
					if (category_order != null)
					{
						mCategoryOrder = category_order.Order;
					}

					ReadOnlyAttribute read_only = mInfo.GetAttribute<ReadOnlyAttribute>();
					if (read_only != null)
					{
						mIsReadOnly = read_only.IsReadOnly;
					}
					if (mInfo.CanWrite == false)
					{
						mIsReadOnly = true;
					}

					DefaultValueAttribute default_value = mInfo.GetAttribute<DefaultValueAttribute>();
					if (default_value != null)
					{
						mDefaultValue = default_value.Value;
					}

					LotusListValuesAttribute list_values = mInfo.GetAttribute<LotusListValuesAttribute>();
					if (list_values != null)
					{
						mListValues = list_values.ListValues;
						mListValuesMemberName = list_values.MemberName;
						mListValuesMemberType = list_values.MemberType;
					}

					LotusNumberFormatAttribute format_value = mInfo.GetAttribute<LotusNumberFormatAttribute>();
					if (format_value != null && String.IsNullOrEmpty(mFormatValue))
					{
						mFormatValue = format_value.FormatValue;
					}

					LotusButtonAttribute button_method = mInfo.GetAttribute<LotusButtonAttribute>();
					if (button_method != null && button_method.MethodName.IsExists())
					{
						mButtonCaption = button_method.Label;
						mButtonMethodName = button_method.MethodName;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение данных описание свойства с помощью внешнего описания свойства
			/// </summary>
			/// <param name="descs">Список описания свойства</param>
			//---------------------------------------------------------------------------------------------------------
			protected void ApplyInfoFromDecs(List<CPropertyDesc> descs)
			{
				if (descs != null && descs.Count > 0)
				{
					for (Int32 i = 0; i < descs.Count; i++)
					{
						CPropertyDesc desc = descs[i];
						if (desc != null)
						{
							if (String.IsNullOrEmpty(desc.DisplayName) == false)
							{
								mDisplayName = desc.DisplayName;
							}

							if (String.IsNullOrEmpty(desc.Description) == false)
							{
								mDescription = desc.Description;
							}

							if (desc.PropertyOrder != -1)
							{
								mPropertyOrder = desc.PropertyOrder;
							}

							if (String.IsNullOrEmpty(desc.Category) == false)
							{
								mCategory = desc.Category;
							}

							if (desc.CategoryOrder != -1)
							{
								mCategoryOrder = desc.CategoryOrder;
							}

							if (desc.IsReadOnly)
							{
								mIsReadOnly = true;
							}

							if (desc.DefaultValue != null)
							{
								mDefaultValue = desc.DefaultValue;
							}

							if (desc.ListValues != null)
							{
								mListValues = desc.ListValues;
							}
						}
					}
				}
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
			public virtual void SetValue(System.Object value)
			{
				// Устанавливаем значение свойства объекта
				if (mInfo != null)
				{
					mInfo.SetValue(mInstance, value, null);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка нового объекта
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void SetInstance()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Заполнить контекстное меню списком допустимых значений
			/// </summary>
			/// <param name="context_menu">Контекстное меню</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void AssingContenxMenuListValues(ContextMenu context_menu)
			{
				if (IsListValues)
				{
					IEnumerable enumerable = CPropertyDesc.GetValue(mListValues, mListValuesMemberName, 
						mListValuesMemberType, mInstance) as IEnumerable;
					if (context_menu != null && enumerable != null)
					{
						context_menu.Items.Clear();
						foreach (var item in enumerable)
						{
							context_menu.Items.Add(new MenuItem() { Header = item, Tag = this });
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на значение что оно из списка значений
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void CheckIsValueFromList()
			{
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
			protected virtual void OnPropertyChangedFromInstance(Object sender, PropertyChangedEventArgs args)
			{
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================