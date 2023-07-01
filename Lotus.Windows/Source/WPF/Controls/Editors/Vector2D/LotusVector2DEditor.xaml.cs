//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы редактирования и выбора контента
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusVector2DEditor.xaml.cs
*		Элемент-редактор для редактирования свойства типа двухмерного вектора.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
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
		/// Элемент-редактор для редактирования свойства типа двухмерного вектора
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusVector2DEditor : UserControl, ITypeEditor
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			public static Vector2D VectorCache;
			public static Vector2DToVector2DConverter VectorConverter = new Vector2DToVector2DConverter();
			#endregion

			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Значение вектора
			/// </summary>
			public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), 
				typeof(Vector2D),
				typeof(LotusVector2DEditor), 
				new FrameworkPropertyMetadata(Vector2D.Zero, Value_Changed));

			/// <summary>
			/// Минимальное значение
			/// </summary>
			public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), 
				typeof(Vector2D),
				typeof(LotusVector2DEditor), 
				new FrameworkPropertyMetadata(Vector2D.Zero, FrameworkPropertyMetadataOptions.AffectsRender,
					MaxMinValue_Changed));

			/// <summary>
			/// Максимальное значение
			/// </summary>
			public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), 
				typeof(Vector2D),
				typeof(LotusVector2DEditor), 
				new FrameworkPropertyMetadata(Vector2D.Zero, FrameworkPropertyMetadataOptions.AffectsRender,
					MaxMinValue_Changed));

			/// <summary>
			/// Шаг приращения
			/// </summary>
			public static readonly DependencyProperty StepProperty = DependencyProperty.Register(nameof(Step), 
				typeof(Double),
				typeof(LotusVector2DEditor), 
				new FrameworkPropertyMetadata(1.0));

			/// <summary>
			/// Значение по умолчанию
			/// </summary>
			public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(nameof(DefaultValue), 
				typeof(Vector2D),
				typeof(LotusVector2DEditor), 
				new FrameworkPropertyMetadata(Vector2D.Zero, FrameworkPropertyMetadataOptions.AffectsRender,
					ValueDefault_Changed));

			/// <summary>
			/// Формат отображения значения
			/// </summary>
			public static readonly DependencyProperty FormatValueProperty = DependencyProperty.Register(nameof(FormatValue),
				typeof(String),
				typeof(LotusVector2DEditor), 
				new FrameworkPropertyMetadata("", Format_Changed));

			/// <summary>
			/// Формат отображения значения по умолчанию
			/// </summary>
			public static readonly DependencyProperty FormatValueDefaultProperty = DependencyProperty.Register(nameof(FormatValueDefault), 
				typeof(String),
				typeof(LotusVector2DEditor), 
				new FrameworkPropertyMetadata("{0:0}", FormatValueDefault_Changed));

			/// <summary>
			/// Режим только для чтения
			/// </summary>
			public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), 
				typeof(Boolean),
				typeof(LotusVector2DEditor), new FrameworkPropertyMetadata(false,
					FrameworkPropertyMetadataOptions.AffectsArrange |
					FrameworkPropertyMetadataOptions.AffectsRender, 
					ReadOnly_Changed));

			// Событие – изменения значения
			public static readonly RoutedEvent ValueChangedEvent =
				EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler),
					typeof(LotusVector2DEditor));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение свойства зависимости
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void Value_Changed(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				var vector_editor = (LotusVector2DEditor)obj;
				Vector2D? value = (Vector2D)args.NewValue;
				if (value.HasValue)
				{
					vector_editor.SetPresentValue();
				}

				vector_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения максимального/минимального значения величины
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void MaxMinValue_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
			{
				//LotusVector2DEditor vector_editor = (LotusVector2DEditor)sender;

				//if (args.Property == MinValueProperty)
				//{
				//	Vector2D min_value = (Vector2D)args.NewValue;
				//	if (vector_editor.Value < min_value)
				//	{
				//		vector_editor.Value = min_value;
				//		vector_editor.SetPresentValue();
				//		vector_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
				//	}
				//}
				//else
				//{
				//	Vector2D max_value = (Vector2D)args.NewValue;
				//	if (vector_editor.Value > max_value)
				//	{
				//		vector_editor.Value = max_value;
				//		vector_editor.SetPresentValue();
				//		vector_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
				//	}
				//}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения значения по умолчанию
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void ValueDefault_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
			{
				var vector_editor = (LotusVector2DEditor)sender;
				var new_value = (Vector2D)args.NewValue;

				vector_editor.Value = new_value;
				vector_editor.SetPresentValue();
				vector_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения формата отображения значения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void Format_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
			{
				var vector_editor = (LotusVector2DEditor)sender;
				vector_editor.SetPresentValue();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения формата отображения значения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void FormatValueDefault_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
			{
				var vector_editor = (LotusVector2DEditor)sender;
				vector_editor.SetPresentValue();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения значения только для чтения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void ReadOnly_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
			{
				var vector_editor = (LotusVector2DEditor)sender;
				var new_read_only = (Boolean)args.NewValue;
				if (new_read_only)
				{
					vector_editor.miClear.IsEnabled = false;
					vector_editor.miPaste.IsEnabled = false;
					vector_editor.miDefault.IsEnabled = false;
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
			public Vector2D Value
			{
				get { return (Vector2D)GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}

			/// <summary>
			/// Минимальное значение
			/// </summary>
			public Vector2D MinValue
			{
				get { return (Vector2D)GetValue(MinValueProperty); }
				set { SetValue(MinValueProperty, value); }
			}

			/// <summary>
			/// Максимальное значение
			/// </summary>
			public Vector2D MaxValue
			{
				get { return (Vector2D)GetValue(MaxValueProperty); }
				set { SetValue(MaxValueProperty, value); }
			}

			/// <summary>
			/// Шаг приращения
			/// </summary>
			public Double Step
			{
				get { return (Double)GetValue(StepProperty); }
				set { SetValue(StepProperty, value); }
			}

			/// <summary>
			/// Значение по умолчанию
			/// </summary>
			public Vector2D DefaultValue
			{
				get { return (Vector2D)GetValue(DefaultValueProperty); }
				set { SetValue(DefaultValueProperty, value); }
			}

			/// <summary>
			/// Формат отображения значения
			/// </summary>
			public String FormatValue
			{
				get { return (String)GetValue(FormatValueProperty); }
				set { SetValue(FormatValueProperty, value); }
			}

			/// <summary>
			/// Формат отображения значения по умолчанию
			/// </summary>
			public String FormatValueDefault
			{
				get { return (String)GetValue(FormatValueDefaultProperty); }
				set { SetValue(FormatValueDefaultProperty, value); }
			}

			/// <summary>
			/// Режим только для чтения
			/// </summary>
			public Boolean IsReadOnly
			{
				get { return (Boolean)GetValue(IsReadOnlyProperty); }
				set { SetValue(IsReadOnlyProperty, value); }
			}

			/// <summary>
			/// The ValueChanged event is called when the TextField of the control changes
			/// </summary>
			public event RoutedEventHandler ValueChanged
			{
				add { AddHandler(ValueChangedEvent, value); }
				remove { RemoveHandler(ValueChangedEvent, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusVector2DEditor()
			{
				InitializeComponent();
			}
			#endregion

			#region ======================================= ЭЛЕМЕНТ РЕДАКТОРА =========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Элемент редактор свойства типа Vector2D
			/// </summary>
			/// <param name="propertyItem">Параметры свойства</param>
			/// <returns>Редактор</returns>
			//---------------------------------------------------------------------------------------------------------
			public FrameworkElement ResolveEditor(PropertyItem propertyItem)
			{
				var binding = new Binding(nameof(Value));
				binding.Source = propertyItem;
				binding.ValidatesOnExceptions = true;
				binding.ValidatesOnDataErrors = true;
				binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
				binding.Converter = VectorConverter;
				binding.ConverterParameter = propertyItem.PropertyType;

				// Привязываемся к свойству
				BindingOperations.SetBinding(this, ValueProperty, binding);

				// Сохраняем объект
				mPropertyItem = propertyItem;

				return this;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Режим отображения величины
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void SetPresentValue()
			{
				spinnerX.IsEnabled = false;
				spinnerX.Value = Value.X;
				spinnerX.IsEnabled = true;

				spinnerY.IsEnabled = false;
				spinnerY.Value = Value.Y;
				spinnerY.IsEnabled = true;
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
					Value = new Vector2D(spinnerX.Value.Value, Value.Y);
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
					Value = new Vector2D(Value.X, spinnerY.Value.Value);
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
				if (VectorCache != Vector2D.Zero)
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
				VectorCache = new Vector2D(spinnerX.Value.Value, spinnerY.Value.Value);
				if (VectorCache != Vector2D.Zero)
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
				if (mPropertyItem != null)
				{
					for (var i = 0; i < mPropertyItem.PropertyDescriptor.Attributes.Count; i++)
					{
						Attribute attr = mPropertyItem.PropertyDescriptor.Attributes[i];
						if (attr is LotusDefaultValueAttribute)
						{
							var def_value = attr as LotusDefaultValueAttribute;

						//	// Сначала смотрим свойства
						//	Object v = mPropertyItem.Instance.GetObjectPropertyValue(def_value.NamePropertyDefaultValue);
						//	if (v == null)
						//	{
						//		// Смотрим поля
						//		v = mPropertyItem.Instance.GetObjectFieldValue(def_value.NamePropertyDefaultValue);
						//	}

						//	// Если все правильно
						//	if (v != null && v.GetType() == mPropertyItem.PropertyType)
						//	{
						//		// Конвертируем
						//		Value = (Vector2D)VectorConverter.Convert(v, mPropertyItem.PropertyType, mPropertyItem.PropertyType, null);
						//	}
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
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================