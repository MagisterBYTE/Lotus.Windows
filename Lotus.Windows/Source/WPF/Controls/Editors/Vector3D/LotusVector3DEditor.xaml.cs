using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using Lotus.Core;
using Lotus.Maths;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsEditor
	*@{*/
    /// <summary>
    /// Элемент-редактор для редактирования свойства типа трёхмерного вектора.
    /// </summary>
    public partial class LotusVector3DEditor : UserControl
    {
        #region Declare DependencyProperty
        /// <summary>
        /// Значение вектора.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Vector3D), typeof(LotusVector3DEditor),
                new FrameworkPropertyMetadata(Vector3D.Zero,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    Value_Changed));

        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(Vector3D), typeof(LotusVector3DEditor),
                new FrameworkPropertyMetadata(Vector3D.Zero));

        /// <summary>
        /// Шаг приращения.
        /// </summary>
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(double), typeof(LotusVector3DEditor),
                new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// Режим только для чтения.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(LotusVector3DEditor),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Событие изменения значения.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(LotusVector3DEditor));
        #endregion

        #region DependencyProperty callbacks
        /// <summary>
        /// Обработчик изменения значения вектора.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private static void Value_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var editor = (LotusVector3DEditor)sender;
            editor.SetPresentValue();
            editor.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Значение вектора.
        /// </summary>
        [TypeConverter(typeof(LotusVector3DTypeConverter))]
        public Vector3D Value
        {
            get => (Vector3D)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        [TypeConverter(typeof(LotusVector3DTypeConverter))]
        public Vector3D DefaultValue
        {
            get => (Vector3D)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
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
        public LotusVector3DEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Обновляет отображение компонентов вектора в редакторах.
        /// WPF подавляет повторное срабатывание события DependencyProperty при одинаковом значении,
        /// поэтому дополнительная защита от рекурсии не требуется.
        /// </summary>
        private void SetPresentValue()
        {
            EditorX.Value = Value.X;
            EditorY.Value = Value.Y;
            EditorZ.Value = Value.Z;
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Обработчик изменения компонента X.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnEditorX_ValueChanged(object sender, RoutedEventArgs args)
        {
            Value = new Vector3D(EditorX.Value, Value.Y, Value.Z);
        }

        /// <summary>
        /// Обработчик изменения компонента Y.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnEditorY_ValueChanged(object sender, RoutedEventArgs args)
        {
            Value = new Vector3D(Value.X, EditorY.Value, Value.Z);
        }

        /// <summary>
        /// Обработчик изменения компонента Z.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnEditorZ_ValueChanged(object sender, RoutedEventArgs args)
        {
            Value = new Vector3D(Value.X, Value.Y, EditorZ.Value);
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
        /// Строка формата берётся из свойства Tag радиокнопки.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioRadix_Checked(object sender, RoutedEventArgs args)
        {
            if (sender is not RadioButton { Tag: string tag })
            {
                return;
            }

            EditorX.FormatValueDefault = tag;
            EditorY.FormatValueDefault = tag;
            EditorZ.FormatValueDefault = tag;
        }

        /// <summary>
        /// Обработчик копирования вектора в буфер обмена.
        /// Формат: "X,Y,Z" с инвариантной культурой.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemCopyVector_Click(object sender, RoutedEventArgs args)
        {
            var x = Value.X.ToString(CultureInfo.InvariantCulture);
            var y = Value.Y.ToString(CultureInfo.InvariantCulture);
            var z = Value.Z.ToString(CultureInfo.InvariantCulture);
            Clipboard.SetText($"{x},{y},{z}");
        }

        /// <summary>
        /// Обработчик вставки вектора из буфера обмена.
        /// Ожидаемый формат: "X,Y,Z".
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemPasteVector_Click(object sender, RoutedEventArgs args)
        {
            if (!Clipboard.ContainsText())
            {
                return;
            }

            var parts = Clipboard.GetText().Split(',');
            if (parts.Length == 3 &&
                XNumberConverter.TryParseDouble(parts[0].Trim(), out var x) &&
                XNumberConverter.TryParseDouble(parts[1].Trim(), out var y) &&
                XNumberConverter.TryParseDouble(parts[2].Trim(), out var z))
            {
                Value = new Vector3D(x, y, z);
            }
        }

        /// <summary>
        /// Обработчик очистки вектора (сброс в ноль).
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemClearVector_Click(object sender, RoutedEventArgs args)
        {
            Value = Vector3D.Zero;
        }

        /// <summary>
        /// Обработчик восстановления значения по умолчанию.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnMenuItemSetDefaultVector_Click(object sender, RoutedEventArgs args)
        {
            Value = DefaultValue;
        }
        #endregion
    }
    /**@}*/
}
