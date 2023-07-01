//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Элементы редактирования и выбора контента
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusMeasurementEditor.xaml.cs
*		Элемент-редактор для редактирования свойства числового типа с соответствующей единицей измерения.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.UnitMeasurement;
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
		/// Элемент-редактор для редактирования свойства числового типа с соответствующей единицей измерения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusMeasurementEditor : UserControl
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			/// <summary>
			/// Значение
			/// </summary>
			public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(TMeasurementValue),
				typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(TMeasurementValue.Empty, 
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange, 
					Value_Changed));

			/// <summary>
			/// Минимальное значение
			/// </summary>
			public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(Double),
				typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender,
					MaxMinValue_Changed));

			/// <summary>
			/// Максимальное значение
			/// </summary>
			public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(Double),
				typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender,
					MaxMinValue_Changed));

			/// <summary>
			/// Шаг приращения
			/// </summary>
			public static readonly DependencyProperty StepProperty = DependencyProperty.Register(nameof(Step), typeof(Double),
				typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(1.0));

			/// <summary>
			/// Значение по умолчанию
			/// </summary>
			public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(nameof(DefaultValue), typeof(Double),
				typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender,
					ValueDefault_Changed));

			/// <summary>
			/// Формат отображения значения
			/// </summary>
			public static readonly DependencyProperty FormatValueProperty = DependencyProperty.Register(nameof(FormatValue), typeof(String),
				typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender,
					Format_Changed));

			/// <summary>
			/// Формат отображения значения по умолчанию
			/// </summary>
			public static readonly DependencyProperty FormatValueDefaultProperty = DependencyProperty.Register(nameof(FormatValueDefault), typeof(String),
				typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata("{0:0}", FrameworkPropertyMetadataOptions.AffectsRender,
					FormatValueDefault_Changed));

			/// <summary>
			/// Режим только для чтения
			/// </summary>
			public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(Boolean),
				typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(false, 
					FrameworkPropertyMetadataOptions.AffectsArrange|
					FrameworkPropertyMetadataOptions.AffectsRender, ReadOnly_Changed));

			// Событие – изменения значения
			public static readonly RoutedEvent ValueChangedEvent =
				EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), 
					typeof(LotusMeasurementEditor));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения значения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void Value_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
			{
				var spin_editor = (LotusMeasurementEditor)sender;
				
				spin_editor.SetPresentValue();

				spin_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
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
				var spin_editor = (LotusMeasurementEditor)sender;

				if (args.Property == MinValueProperty)
				{
					var min_value = (Double)args.NewValue;
					if (spin_editor.Value.Value < min_value)
					{
						spin_editor.Value = spin_editor.Value.Clone(min_value);
						spin_editor.SetPresentValue();
						spin_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
					}
				}
				else
				{
					var max_value = (Double)args.NewValue;
					if (spin_editor.Value.Value > max_value)
					{
						spin_editor.Value = spin_editor.Value.Clone(max_value);
						spin_editor.SetPresentValue();
						spin_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
					}
				}
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
				var spin_editor = (LotusMeasurementEditor)sender;
				var new_value = (Double)args.NewValue;

				spin_editor.Value = spin_editor.Value.Clone(new_value);
				spin_editor.SetPresentValue();
				spin_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
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
				var spin_editor = (LotusMeasurementEditor)sender;
				spin_editor.SetPresentValue();
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
				var spin_editor = (LotusMeasurementEditor)sender;
				spin_editor.SetPresentValue();
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
				var spin_editor = (LotusMeasurementEditor)sender;
				var new_read_only = (Boolean)args.NewValue;
				if (new_read_only)
				{
					spin_editor.miClear.IsEnabled = false;
					spin_editor.miPaste.IsEnabled = false;
					spin_editor.miDefault.IsEnabled = false;
				}
			}
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			protected Boolean mIsDirectText;
			protected static TMeasurementValue mCopyValue;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Значение
			/// </summary>
			public TMeasurementValue Value
			{
				get { return (TMeasurementValue)GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}

			/// <summary>
			/// Минимальное значение
			/// </summary>
			public Double MinValue
			{
				get { return (Double)GetValue(MinValueProperty); }
				set { SetValue(MinValueProperty, value); }
			}

			/// <summary>
			/// Максимальное значение
			/// </summary>
			public Double MaxValue
			{
				get { return (Double)GetValue(MaxValueProperty); }
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
			public Double DefaultValue
			{
				get { return (Double)GetValue(DefaultValueProperty); }
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
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusMeasurementEditor()
			{
				InitializeComponent();
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
				mIsDirectText = true;
				if (String.IsNullOrEmpty(FormatValue))
				{
					if (TextField.IsFocused == false)
					{
						TextField.Text = String.Format(FormatValueDefault, Value.Value);
					}
				}
				else
				{
					if (TextField.IsFocused == false)
					{
						TextField.Text = String.Format(FormatValue, Value.Value);
					}
				}

				buttonMenu.Content = Value.GetAbbreviationUnit();

				mIsDirectText = false;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Переустановка текста
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void ResetText()
			{
				mIsDirectText = true;
				TextField.Text = 0 < MinValue ? MinValue.ToString() : "0";
				mIsDirectText = false;
				TextField.SelectAll();
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события предварительного ввода текста
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTextField_PreviewTextInput(Object sender, TextCompositionEventArgs args)
			{
				//Double result = 0;
				//args.Handled = !XNumbers.ParseDoubleFormat(args.Text, out result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события изменения текста
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTextField_TextChanged(Object sender, TextChangedEventArgs args)
			{
				if (mIsDirectText == false)
				{
					Double result = 0;
					if (XNumbers.ParseDoubleFormat(TextField.Text, out result))
					{
						Value = new TMeasurementValue(result, Value.QuantityType, Value.UnitType);
						if (Value.Value < MinValue) Value = Value.Clone(MinValue);
						if (Value.Value > MaxValue) Value = Value.Clone(MaxValue);
					}
					else
					{
						ResetText();
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Потеря фокуса текстового поля
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnTextField_LostFocus(Object sender, RoutedEventArgs args)
			{
				// 1) Пробуем преобразовать текст в число
				Double result = 0;
				if (XNumbers.ParseDoubleFormat(TextField.Text, out result))
				{
					Value = new TMeasurementValue(result, Value.QuantityType, Value.UnitType);
					if (Value.Value < MinValue) Value = Value.Clone(MinValue);
					if (Value.Value > MaxValue) Value = Value.Clone(MaxValue);

					// 2) Форматируем поле
					mIsDirectText = true;
					if (String.IsNullOrEmpty(FormatValue))
					{
						TextField.Text = String.Format(FormatValueDefault, Value);
					}
					else
					{
						TextField.Text = String.Format(FormatValue, Value);
					}
					mIsDirectText = false;
				}
				else
				{
					ResetText();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события увеличения значения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonUp_Click(Object sender, RoutedEventArgs args)
			{
				var result = Value.Value + Step;
				if (result > MaxValue)
				{
					Value = Value.Clone(MaxValue);
				}
				else
				{
					Value = Value.Clone(result);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события уменьшения значения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonDown_Click(Object sender, RoutedEventArgs args)
			{
				var result = Value.Value - Step;
				if (result < MinValue)
				{
					Value = Value.Clone(MinValue);
				}
				else
				{
					Value = Value.Clone(result);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработчик события открытия контекстного меню
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonMenu_Click(Object sender, RoutedEventArgs args)
			{
				contextMenu.IsOpen = true;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка разрядности - ноль цифр после запятой
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioRadixZero_Checked(Object sender, RoutedEventArgs args)
			{
				if(String.IsNullOrEmpty(FormatValue))
				{
					FormatValueDefault = "{0}";
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка разрядности - одна цифра после запятой
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioRadixOne_Checked(Object sender, RoutedEventArgs args)
			{
				if (String.IsNullOrEmpty(FormatValue))
				{
					FormatValueDefault = "{0:F1}";
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка разрядности - две цифры после запятой
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioRadixTwo_Checked(Object sender, RoutedEventArgs args)
			{
				if (String.IsNullOrEmpty(FormatValue))
				{
					FormatValueDefault = "{0:F2}";
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка разрядности - три цифры после запятой
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRadioRadixThree_Checked(Object sender, RoutedEventArgs args)
			{
				if (String.IsNullOrEmpty(FormatValue))
				{
					FormatValueDefault = "{0:F3}";
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование значения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemCopyValue_Click(Object sender, RoutedEventArgs args)
			{
				mCopyValue = Value;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка значения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemPasteValue_Click(Object sender, RoutedEventArgs args)
			{
				Value = mCopyValue;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка значения по умолчанию
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemSetDefaultValue_Click(Object sender, RoutedEventArgs args)
			{
				if (IsReadOnly == false)
				{
					Value = Value.Clone(DefaultValue);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Очистка значения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemClearValue_Click(Object sender, RoutedEventArgs args)
			{
				Value = Value.Clone(0);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Очистка вектора
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnMenuItemSetUnit_Click(Object sender, RoutedEventArgs args)
			{
				var unit_type = (Enum)((MenuItem)sender).Tag;
				Value = new TMeasurementValue(Value.Value, unit_type);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================