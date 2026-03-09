using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsEditor
	*@{*/
    /// <summary>
    /// Элемент-редактор для редактирования свойства числового типа.
    /// </summary>
    public partial class LotusNumericEditor : UserControl
    {
        #region Declare DependencyProperty
        /// <summary>
        /// Значение.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(LotusNumericEditor),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    Value_Changed,
                    CoerceValue));

        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(LotusNumericEditor),
                new FrameworkPropertyMetadata(0.0, MinMaxValue_Changed));

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(LotusNumericEditor),
                new FrameworkPropertyMetadata(100.0, MinMaxValue_Changed));

        /// <summary>
        /// Шаг приращения.
        /// </summary>
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(double), typeof(LotusNumericEditor),
                new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(double), typeof(LotusNumericEditor),
                new FrameworkPropertyMetadata(0.0, DefaultValue_Changed));

        /// <summary>
        /// Формат отображения значения (приоритетный, задаётся извне).
        /// </summary>
        public static readonly DependencyProperty FormatValueProperty =
            DependencyProperty.Register(nameof(FormatValue), typeof(string), typeof(LotusNumericEditor),
                new FrameworkPropertyMetadata(string.Empty, AnyFormat_Changed));

        /// <summary>
        /// Формат отображения значения по умолчанию (используется если <see cref="FormatValue"/> не задан).
        /// </summary>
        public static readonly DependencyProperty FormatValueDefaultProperty =
            DependencyProperty.Register(nameof(FormatValueDefault), typeof(string), typeof(LotusNumericEditor),
                new FrameworkPropertyMetadata("{0:0}", AnyFormat_Changed));

        /// <summary>
        /// Режим только для чтения.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(LotusNumericEditor),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Событие изменения значения.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(LotusNumericEditor));
        #endregion

        #region DependencyProperty callbacks
        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var editor = (LotusNumericEditor)d;
            var value = (double)baseValue;
            if (value < editor.MinValue) return editor.MinValue;
            if (value > editor.MaxValue) return editor.MaxValue;
            return value;
        }

        /// <summary>
        /// Обработчик изменения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void Value_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var editor = (LotusNumericEditor)sender;
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
            ((LotusNumericEditor)sender).CoerceValue(ValueProperty);
        }

        /// <summary>
        /// Обработчик изменения значения по умолчанию.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void DefaultValue_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            // Инициализируем текущее значение из DefaultValue.
            // Value_Changed обновит отображение и вызовет событие.
            ((LotusNumericEditor)sender).Value = (double)args.NewValue;
        }

        /// <summary>
        /// Обработчик изменения формата отображения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void AnyFormat_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((LotusNumericEditor)sender).SetPresentValue();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Значение.
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
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
        public LotusNumericEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Обновляет отображение форматированного значения в текстовом поле.
        /// Не обновляет, если поле находится в фокусе (пользователь редактирует).
        /// </summary>
        private void SetPresentValue()
        {
            if (TextField.IsFocused)
            {
                return;
            }

            var format = string.IsNullOrEmpty(FormatValue) ? FormatValueDefault : FormatValue;
            TextField.Text = string.Format(format, Value);
        }

        /// <summary>
        /// Применяет введённый текст: парсит, корректирует и обновляет отображение.
        /// Вызывается при потере фокуса или нажатии Enter.
        /// </summary>
        private void ApplyTextValue()
        {
            if (XNumberConverter.TryParseDouble(TextField.Text, out var result))
            {
                Value = result; // CoerceValue выполнит корректировку по Min/Max.
            }
            else
            {
                Value = MinValue > 0 ? MinValue : 0;
            }

            var format = string.IsNullOrEmpty(FormatValue) ? FormatValueDefault : FormatValue;
            TextField.Text = string.Format(format, Value);
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
            Value += Step;
        }

        /// <summary>
        /// Обработчик уменьшения значения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonDown_Click(object sender, RoutedEventArgs args)
        {
            Value -= Step;
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
            Clipboard.SetText(Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
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
                Value = result;
            }
        }

        /// <summary>
        /// Обработчик восстановления значения по умолчанию.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemSetDefaultValue_Click(object sender, RoutedEventArgs args)
        {
            Value = DefaultValue;
        }

        /// <summary>
        /// Обработчик очистки значения (сброс в ноль).
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemClearValue_Click(object sender, RoutedEventArgs args)
        {
            Value = 0;
        }
        #endregion
    }
    /**@}*/
}
