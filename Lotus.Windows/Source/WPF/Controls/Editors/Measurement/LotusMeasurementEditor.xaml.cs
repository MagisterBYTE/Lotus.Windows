using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Lotus.Core;
using Lotus.UnitMeasurement;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsEditor
	*@{*/
    /// <summary>
    /// Элемент-редактор для редактирования свойства числового типа с соответствующей единицей измерения.
    /// </summary>
    public partial class LotusMeasurementEditor : UserControl
    {
        #region Static fields 
        /// <summary>
        /// Текущие скопированное значение.
        /// </summary>
        public static TMeasurementValue CopyValue
        {
            get { return _copyValue; }
        }

        private static TMeasurementValue _copyValue = new TMeasurementValue();
        #endregion

        #region Declare DependencyProperty 
        /// <summary>
        /// Значение.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(TMeasurementValue),
            typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(TMeasurementValue.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange,
                Value_Changed));

        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(double),
            typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender,
                MaxMinValue_Changed));

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(double),
            typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender,
                MaxMinValue_Changed));

        /// <summary>
        /// Шаг приращения.
        /// </summary>
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(nameof(Step), typeof(double),
            typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(nameof(DefaultValue), typeof(double),
            typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender,
                ValueDefault_Changed));

        /// <summary>
        /// Формат отображения значения.
        /// </summary>
        public static readonly DependencyProperty FormatValueProperty = DependencyProperty.Register(nameof(FormatValue), typeof(string),
            typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender,
                Format_Changed));

        /// <summary>
        /// Формат отображения значения по умолчанию.
        /// </summary>
        public static readonly DependencyProperty FormatValueDefaultProperty = DependencyProperty.Register(nameof(FormatValueDefault), typeof(string),
            typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata("{0:0}", FrameworkPropertyMetadataOptions.AffectsRender,
                FormatValueDefault_Changed));

        /// <summary>
        /// Режим только для чтения.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool),
            typeof(LotusMeasurementEditor), new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender, ReadOnly_Changed));

        // Событие – изменения значения
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler),
                typeof(LotusMeasurementEditor));
        #endregion

        #region DependencyProperty methods
        /// <summary>
        /// Обработчик события изменения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void Value_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var spin_editor = (LotusMeasurementEditor)sender;

            spin_editor.SetPresentValue();

            spin_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        /// <summary>
        /// Обработчик события изменения максимального/минимального значения величины.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void MaxMinValue_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var spin_editor = (LotusMeasurementEditor)sender;

            if (args.Property == MinValueProperty)
            {
                var min_value = (double)args.NewValue;
                if (spin_editor.Value.Value < min_value)
                {
                    spin_editor.Value = spin_editor.Value.Clone(min_value);
                    spin_editor.SetPresentValue();
                    spin_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
                }
            }
            else
            {
                var max_value = (double)args.NewValue;
                if (spin_editor.Value.Value > max_value)
                {
                    spin_editor.Value = spin_editor.Value.Clone(max_value);
                    spin_editor.SetPresentValue();
                    spin_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
                }
            }
        }

        /// <summary>
        /// Обработчик события изменения значения по умолчанию.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void ValueDefault_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var spin_editor = (LotusMeasurementEditor)sender;
            var new_value = (double)args.NewValue;

            spin_editor.Value = spin_editor.Value.Clone(new_value);
            spin_editor.SetPresentValue();
            spin_editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        /// <summary>
        /// Обработчик события изменения формата отображения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void Format_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var spin_editor = (LotusMeasurementEditor)sender;
            spin_editor.SetPresentValue();
        }

        /// <summary>
        /// Обработчик события изменения формата отображения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "<Pending>")]
        private static void FormatValueDefault_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var spin_editor = (LotusMeasurementEditor)sender;
            spin_editor.SetPresentValue();
        }

        /// <summary>
        /// Обработчик события изменения значения только для чтения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void ReadOnly_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var spin_editor = (LotusMeasurementEditor)sender;
            var new_read_only = (bool)args.NewValue;
            if (new_read_only)
            {
                spin_editor.miClear.IsEnabled = false;
                spin_editor.miPaste.IsEnabled = false;
                spin_editor.miDefault.IsEnabled = false;
            }
        }
        #endregion

        #region Fields
        protected internal bool _isDirectText;
        #endregion

        #region Properties
        /// <summary>
        /// Значение.
        /// </summary>
        public TMeasurementValue Value
        {
            get { return (TMeasurementValue)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        /// <summary>
        /// Шаг приращения.
        /// </summary>
        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        public double DefaultValue
        {
            get { return (double)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        /// <summary>
        /// Формат отображения значения.
        /// </summary>
        public string FormatValue
        {
            get { return (string)GetValue(FormatValueProperty); }
            set { SetValue(FormatValueProperty, value); }
        }

        /// <summary>
        /// Формат отображения значения по умолчанию.
        /// </summary>
        public string FormatValueDefault
        {
            get { return (string)GetValue(FormatValueDefaultProperty); }
            set { SetValue(FormatValueDefaultProperty, value); }
        }

        /// <summary>
        /// Режим только для чтения.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// The ValueChanged event is called when the TextField of the control changes.
        /// </summary>
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusMeasurementEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Режим отображения величины.
        /// </summary>
        private void SetPresentValue()
        {
            _isDirectText = true;
            if (string.IsNullOrEmpty(FormatValue))
            {
                if (TextField.IsFocused == false)
                {
                    TextField.Text = string.Format(FormatValueDefault, Value.Value);
                }
            }
            else
            {
                if (TextField.IsFocused == false)
                {
                    TextField.Text = string.Format(FormatValue, Value.Value);
                }
            }

            buttonMenu.Content = Value.GetAbbreviationUnit();

            _isDirectText = false;
        }

        /// <summary>
        /// Переустановка текста.
        /// </summary>
        private void ResetText()
        {
            _isDirectText = true;
            TextField.Text = 0 < MinValue ? MinValue.ToString() : "0";
            _isDirectText = false;
            TextField.SelectAll();
        }
        #endregion

        #region Event handlers 
        /// <summary>
        /// Обработчик события предварительного ввода текста.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnTextField_PreviewTextInput(object sender, TextCompositionEventArgs args)
        {
            // Double result = 0
            // args.Handled = !XNumbers.ParseDoubleFormat(args.Text, out result)
        }

        /// <summary>
        /// Обработчик события изменения текста.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnTextField_TextChanged(object sender, TextChangedEventArgs args)
        {
            if (_isDirectText == false)
            {
                if (XNumberHelper.TryParseDouble(TextField.Text, out var result))
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

        /// <summary>
        /// Потеря фокуса текстового поля.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnTextField_LostFocus(object sender, RoutedEventArgs args)
        {
            // 1) Пробуем преобразовать текст в число
            if (XNumberHelper.TryParseDouble(TextField.Text, out var result))
            {
                Value = new TMeasurementValue(result, Value.QuantityType, Value.UnitType);
                if (Value.Value < MinValue) Value = Value.Clone(MinValue);
                if (Value.Value > MaxValue) Value = Value.Clone(MaxValue);

                // 2) Форматируем поле
                _isDirectText = true;
                if (string.IsNullOrEmpty(FormatValue))
                {
                    TextField.Text = string.Format(FormatValueDefault, Value);
                }
                else
                {
                    TextField.Text = string.Format(FormatValue, Value);
                }
                _isDirectText = false;
            }
            else
            {
                ResetText();
            }
        }

        /// <summary>
        /// Обработчик события увеличения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonUp_Click(object sender, RoutedEventArgs args)
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

        /// <summary>
        /// Обработчик события уменьшения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonDown_Click(object sender, RoutedEventArgs args)
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

        /// <summary>
        /// Обработчик события открытия контекстного меню.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonMenu_Click(object sender, RoutedEventArgs args)
        {
            contextMenu.IsOpen = true;
        }

        /// <summary>
        /// Установка разрядности - ноль цифр после запятой.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioRadixZero_Checked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(FormatValue))
            {
                FormatValueDefault = "{0}";
            }
        }

        /// <summary>
        /// Установка разрядности - одна цифра после запятой.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioRadixOne_Checked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(FormatValue))
            {
                FormatValueDefault = "{0:F1}";
            }
        }

        /// <summary>
        /// Установка разрядности - две цифры после запятой.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioRadixTwo_Checked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(FormatValue))
            {
                FormatValueDefault = "{0:F2}";
            }
        }

        /// <summary>
        /// Установка разрядности - три цифры после запятой.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioRadixThree_Checked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(FormatValue))
            {
                FormatValueDefault = "{0:F3}";
            }
        }

        /// <summary>
        /// Копирование значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemCopyValue_Click(object sender, RoutedEventArgs args)
        {
            _copyValue = Value;
        }

        /// <summary>
        /// Вставка значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemPasteValue_Click(object sender, RoutedEventArgs args)
        {
            Value = _copyValue;
        }

        /// <summary>
        /// Установка значения по умолчанию.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemSetDefaultValue_Click(object sender, RoutedEventArgs args)
        {
            if (IsReadOnly == false)
            {
                Value = Value.Clone(DefaultValue);
            }
        }

        /// <summary>
        /// Очистка значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemClearValue_Click(object sender, RoutedEventArgs args)
        {
            Value = Value.Clone(0);
        }

        /// <summary>
        /// Очистка вектора.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemSetUnit_Click(object sender, RoutedEventArgs args)
        {
            var unit_type = (Enum)((MenuItem)sender).Tag;
            Value = new TMeasurementValue(Value.Value, unit_type);
        }
        #endregion
    }
    /**@}*/
}