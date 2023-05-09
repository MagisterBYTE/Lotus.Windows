//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы редактирования и выбора контента
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusVector3DEditor.xaml.cs
*		Элемент-редактор для редактирования свойства типа трехмерного вектора.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//---------------------------------------------------------------------------------------------------------------------
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFControlsEditor
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Универсальный конвертор типа Vector3D между различными типами представлений 
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class Vector3DToVector3DConverter : IValueConverter
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация объекта вектор в объект типа <see cref="Vector3D"/>
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Объект <see cref="Vector3D"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				if (value is Vector3D)
				{
					return (value);
				}

				if (value is System.Windows.Media.Media3D.Vector3D)
				{
					System.Windows.Media.Media3D.Vector3D v = (System.Windows.Media.Media3D.Vector3D)value;
					return (new Vector3D(v.X, v.Y, v.Z));
				}

				if (value is System.Windows.Media.Media3D.Point3D)
				{
					System.Windows.Media.Media3D.Point3D v = (System.Windows.Media.Media3D.Point3D)value;
					return (new Vector3D(v.X, v.Y, v.Z));
				}

#if USE_ASSIMP
				if (value is Assimp.Vector3D)
				{
					Assimp.Vector3D v = (Assimp.Vector3D)value;
					return (new Vector3D(v.X, v.Y, v.Z));
				}
#endif

#if USE_SHARPDX
				if (value is SharpDX.Vector3)
				{
					SharpDX.Vector3 v = (SharpDX.Vector3)value;
					return (new Vector3D(v.X, v.Y, v.Z));
				}
#endif
				return (Vector3D.Zero);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация объект типа <see cref="Vector3D"/> в объект вектора
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр(реальный тип для преобразования)</param>
			/// <param name="culture">Культура</param>
			/// <returns>Объект вектора </returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				Type real_type = (Type)parameter;
				if (real_type == null) real_type = target_type;

				if (real_type == typeof(Vector3D))
				{
					return (value);
				}

				if (real_type == typeof(System.Windows.Media.Media3D.Vector3D))
				{
					Vector3D v = (Vector3D)value;
					return (new System.Windows.Media.Media3D.Vector3D(v.X, v.Y, v.Z));
				}

				if (real_type == typeof(System.Windows.Media.Media3D.Point3D))
				{
					Vector3D v = (Vector3D)value;
					return (new System.Windows.Media.Media3D.Point3D(v.X, v.Y, v.Z));
				}

#if USE_ASSIMP
				if (real_type == typeof(Assimp.Vector3D))
				{
					Vector3D v = (Vector3D)value;
					return (new Assimp.Vector3D((Single)v.X, (Single)v.Y, (Single)v.Z));
				}
#endif

#if USE_SHARPDX
				if (real_type == typeof(SharpDX.Vector3))
				{
					Vector3D v = (Vector3D)value;
					return (new SharpDX.Vector3((Single)v.X, (Single)v.Y, (Single)v.Z));
				}
#endif
				return (value);
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент-редактор для редактирования свойства типа трехмерного вектора
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusVector3DEditor : UserControl, ITypeEditor
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			public static Vector3D VectorCache;
			public static Vector3DToVector3DConverter VectorConverter = new Vector3DToVector3DConverter();
			#endregion

			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Значение вектора
			/// </summary>
			public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(Vector3D),
				typeof(LotusVector3DEditor), new FrameworkPropertyMetadata(Vector3D.Zero, OnValuePropertyChanged));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение свойства зависимости
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void OnValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				LotusVector3DEditor editor_vector = (LotusVector3DEditor)obj;
				Vector3D? value = ((Vector3D)(args.NewValue));
				if (value.HasValue)
				{
					editor_vector.IsEnabled = false;
					editor_vector.spinnerX.Value = value.Value.X;
					editor_vector.spinnerY.Value = value.Value.Y;
					editor_vector.spinnerZ.Value = value.Value.Z;
					editor_vector.IsEnabled = true;
				}
			}
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			private PropertyItem mPropertyItem;
			private String mFormatRadix;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Значение вектора
			/// </summary>
			public Vector3D Value
			{
				get { return (Vector3D)GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusVector3DEditor()
			{
				InitializeComponent();
			}
			#endregion

			#region ======================================= ЭЛЕМЕНТ РЕДАКТОРА =========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Элемент редактор свойства типа Vector3D
			/// </summary>
			/// <param name="property_item">Параметры свойства</param>
			/// <returns>Редактор</returns>
			//---------------------------------------------------------------------------------------------------------
			public FrameworkElement ResolveEditor(PropertyItem property_item)
			{
				var binding = new Binding(nameof(Value));
				binding.Source = property_item;
				binding.ValidatesOnExceptions = true;
				binding.ValidatesOnDataErrors = true;
				binding.Mode = property_item.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
				binding.Converter = VectorConverter;
				binding.ConverterParameter = property_item.PropertyType;
				
				// Привязываемся к свойству
				BindingOperations.SetBinding(this, ValueProperty, binding);

				// Сохраняем объект
				mPropertyItem = property_item;

				return (this);
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения координат X
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnSpinnerX_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Object> args)
			{
				if (spinnerX.Value != null)
				{
					Value = new Vector3D(spinnerX.Value.Value, Value.Y, Value.Z);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения координат Y
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnSpinnerY_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Object> args)
			{
				if (spinnerY.Value != null)
				{
					Value = new Vector3D(Value.X, spinnerY.Value.Value, Value.Z);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения координат Z
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnSpinnerZ_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Object> args)
			{
				if (spinnerZ.Value != null)
				{
					Value = new Vector3D(Value.X, Value.Y, spinnerZ.Value.Value);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие контекстного меню
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonMenu_Click(Object sender, RoutedEventArgs args)
			{
				ButtonMenu.ContextMenu.IsOpen = true;
				if (VectorCache != Vector3D.Zero)
				{
					miPaste.Header = "Вставить (" + VectorCache.ToString("F1") + ")";
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка разрядности - ноль цифр после запятой
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadixZero_Checked(Object sender, RoutedEventArgs args)
			{
				mFormatRadix = "F0";
				spinnerX.FormatString = mFormatRadix;
				spinnerY.FormatString = mFormatRadix;
				spinnerZ.FormatString = mFormatRadix;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка разрядности - одна цифра после запятой
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadixOne_Checked(Object sender, RoutedEventArgs args)
			{
				mFormatRadix = "F1";
				spinnerX.FormatString = mFormatRadix;
				spinnerY.FormatString = mFormatRadix;
				spinnerZ.FormatString = mFormatRadix;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка разрядности - две цифры после запятой
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadixTwo_Checked(Object sender, RoutedEventArgs args)
			{
				mFormatRadix = "F2";
				spinnerX.FormatString = mFormatRadix;
				spinnerY.FormatString = mFormatRadix;
				spinnerZ.FormatString = mFormatRadix;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование вектора
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemCopyVector_Click(Object sender, RoutedEventArgs args)
			{
				VectorCache = new Vector3D(spinnerX.Value.Value, spinnerY.Value.Value, spinnerZ.Value.Value);
				if (VectorCache != Vector3D.Zero)
				{
					miPaste.Header = "Вставить (" + VectorCache.ToStringValue(mFormatRadix) + ")";
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка вектора
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemPasteVector_Click(Object sender, RoutedEventArgs args)
			{
				spinnerX.Value = VectorCache.X;
				spinnerY.Value = VectorCache.Y;
				spinnerZ.Value = VectorCache.Y;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка значения по умолчанию вектора
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemSetDefaultVector_Click(Object sender, RoutedEventArgs args)
			{
				if(mPropertyItem != null)
				{
					for (Int32 i = 0; i < mPropertyItem.PropertyDescriptor.Attributes.Count; i++)
					{
						Attribute attr = mPropertyItem.PropertyDescriptor.Attributes[i];
						if (attr is LotusDefaultValueAttribute)
						{
							LotusDefaultValueAttribute def_value = attr as LotusDefaultValueAttribute;

							//// Сначала смотрим свойства
							//Object v = mPropertyItem.Instance.GetObjectPropertyValue(def_value.NamePropertyDefaultValue);
							//if(v == null)
							//{
							//	// Смотрим поля
							//	v = mPropertyItem.Instance.GetObjectFieldValue(def_value.NamePropertyDefaultValue);
							//}

							//// Если все правильно
							//if(v != null && v.GetType() == mPropertyItem.PropertyType)
							//{
							//	// Конвертируем
							//	Value = (Vector3D)VectorConverter.Convert(v, mPropertyItem.PropertyType, mPropertyItem.PropertyType, null);
							//}
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Очистка вектора
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemClearVector_Click(Object sender, RoutedEventArgs args)
			{
				spinnerX.Value = 0;
				spinnerY.Value = 0;
				spinnerZ.Value = 0;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================