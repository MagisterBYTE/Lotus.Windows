using System;
using System.Globalization;
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
        #region Declare DependencyProperty
        /// <summary>
        /// Значение.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(TMeasurementValue), typeof(LotusMeasurementEditor),
                new FrameworkPropertyMetadata(TMeasurementValue.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    Value_Changed,
                    CoerceValue));

        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(LotusMeasurementEditor),
                new FrameworkPropertyMetadata(0.0, MinMaxValue_Changed));

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(LotusMeasurementEditor),
                new FrameworkPropertyMetadata(100.0, MinMaxValue_Changed));

        /// <summary>
        /// Шаг приращения.
        /// </summary>
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(double), typeof(LotusMeasurementEditor),
                new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(double), typeof(LotusMeasurementEditor),
                new FrameworkPropertyMetadata(0.0, DefaultValue_Changed));

        /// <summary>
        /// Формат отображения значения (приоритетный, задаётся извне).
        /// </summary>
        public static readonly DependencyProperty FormatValueProperty =
            DependencyProperty.Register(nameof(FormatValue), typeof(string), typeof(LotusMeasurementEditor),
                new FrameworkPropertyMetadata(string.Empty, AnyFormat_Changed));

        /// <summary>
        /// Формат отображения значения по умолчанию (используется если <see cref="FormatValue"/> не задан).
        /// </summary>
        public static readonly DependencyProperty FormatValueDefaultProperty =
            DependencyProperty.Register(nameof(FormatValueDefault), typeof(string), typeof(LotusMeasurementEditor),
                new FrameworkPropertyMetadata("{0:0}", AnyFormat_Changed));

        /// <summary>
        /// Режим только для чтения.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(LotusMeasurementEditor),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Событие изменения значения.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(LotusMeasurementEditor));
        #endregion

        #region DependencyProperty callbacks
        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var editor = (LotusMeasurementEditor)d;
            var value = (TMeasurementValue)baseValue;
            if (value.Value < editor.MinValue) return value.Clone(editor.MinValue);
            if (value.Value > editor.MaxValue) return value.Clone(editor.MaxValue);
            return value;
        }

        /// <summary>
        /// Обработчик изменения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void Value_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var editor = (LotusMeasurementEditor)sender;
            editor.SetPresentValue();
            editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        /// <summary>
        /// Обработчик изменения минимального или максимального значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void MinMaxValue_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            // Повторная коррекция Value с учётом новых границ. Если Value изменится —
            // Value_Changed обновит отображение и вызовет событие автоматически.
            ((LotusMeasurementEditor)sender).CoerceValue(ValueProperty);
        }

        /// <summary>
        /// Обработчик изменения значения по умолчанию.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void DefaultValue_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            // Инициализируем текущее значение из DefaultValue, сохраняя текущий тип единицы.
            // Value_Changed обновит отображение и вызовет событие.
            var editor = (LotusMeasurementEditor)sender;
            editor.Value = editor.Value.Clone((double)args.NewValue);
        }

        /// <summary>
        /// Обработчик изменения формата отображения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void AnyFormat_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((LotusMeasurementEditor)sender).SetPresentValue();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Значение.
        /// </summary>
        public TMeasurementValue Value
        {
            get => (TMeasurementValue)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        /// <summary>
        /// Шаг приращения.
        /// </summary>
        public double Step
        {
            get => (double)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        public double DefaultValue
        {
            get => (double)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }

        /// <summary>
        /// Формат отображения значения (приоритетный, задаётся извне).
        /// </summary>
        public string FormatValue
        {
            get => (string)GetValue(FormatValueProperty);
            set => SetValue(FormatValueProperty, value);
        }

        /// <summary>
        /// Формат отображения значения по умолчанию.
        /// </summary>
        public string FormatValueDefault
        {
            get => (string)GetValue(FormatValueDefaultProperty);
            set => SetValue(FormatValueDefaultProperty, value);
        }

        /// <summary>
        /// Режим только для чтения.
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>
        /// Событие изменения значения.
        /// </summary>
        public event RoutedEventHandler ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
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
        /// Обновляет отображение форматированного значения в текстовом поле и аббревиатуру единицы в кнопке меню.
        /// Не обновляет текстовое поле, если оно находится в фокусе (пользователь редактирует).
        /// </summary>
        private void SetPresentValue()
        {
            if (!TextField.IsFocused)
            {
                var format = string.IsNullOrEmpty(FormatValue) ? FormatValueDefault : FormatValue;
                TextField.Text = string.Format(format, Value.Value);
            }

            ButtonMenu.Content = Value.GetAbbreviationUnit();
        }

        /// <summary>
        /// Применяет введённый текст: парсит, корректирует и обновляет отображение.
        /// Вызывается при потере фокуса или нажатии Enter.
        /// </summary>
        private void ApplyTextValue()
        {
            if (XNumberConverter.TryParseDouble(TextField.Text, out var result))
            {
                Value = Value.Clone(result); // CoerceValue выполнит корректировку по Min/Max.
            }
            else
            {
                Value = Value.Clone(MinValue > 0 ? MinValue : 0);
            }

            var format = string.IsNullOrEmpty(FormatValue) ? FormatValueDefault : FormatValue;
            TextField.Text = string.Format(format, Value.Value);
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Обработчик нажатия клавиши в текстовом поле.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnTextField_KeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                ApplyTextValue();
                args.Handled = true;
            }
        }

        /// <summary>
        /// Обработчик потери фокуса текстовым полем.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnTextField_LostFocus(object sender, RoutedEventArgs args)
        {
            ApplyTextValue();
        }

        /// <summary>
        /// Обработчик увеличения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonUp_Click(object sender, RoutedEventArgs args)
        {
            Value = Value.Clone(Value.Value + Step);
        }

        /// <summary>
        /// Обработчик уменьшения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonDown_Click(object sender, RoutedEventArgs args)
        {
            Value = Value.Clone(Value.Value - Step);
        }

        /// <summary>
        /// Обработчик открытия контекстного меню кнопки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonMenu_Click(object sender, RoutedEventArgs args)
        {
            if (sender is Button { ContextMenu: { } menu })
            {
                menu.IsOpen = true;
            }
        }

        /// <summary>
        /// Обработчик выбора разрядности отображения.
        /// Формат берётся из свойства Tag радиокнопки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioRadix_Checked(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrEmpty(FormatValue) || sender is not RadioButton { Tag: string tag })
            {
                return;
            }

            FormatValueDefault = tag;
        }

        /// <summary>
        /// Обработчик копирования значения в буфер обмена.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemCopyValue_Click(object sender, RoutedEventArgs args)
        {
            Clipboard.SetText(Value.Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Обработчик вставки значения из буфера обмена.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemPasteValue_Click(object sender, RoutedEventArgs args)
        {
            if (Clipboard.ContainsText() &&
                XNumberConverter.TryParseDouble(Clipboard.GetText(), out var result))
            {
                Value = Value.Clone(result);
            }
        }

        /// <summary>
        /// Обработчик восстановления значения по умолчанию.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemSetDefaultValue_Click(object sender, RoutedEventArgs args)
        {
            Value = Value.Clone(DefaultValue);
        }

        /// <summary>
        /// Обработчик очистки значения (сброс в ноль).
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemClearValue_Click(object sender, RoutedEventArgs args)
        {
            Value = Value.Clone(0);
        }

        /// <summary>
        /// Обработчик выбора единицы измерения из меню.
        /// Тип единицы берётся из свойства Tag пункта меню.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemSetUnit_Click(object sender, RoutedEventArgs args)
        {
            if (sender is MenuItem { Tag: Enum unitType })
            {
                Value = Value.Clone(unitType);
            }
        }
        #endregion
    }
    /**@}*/
}
