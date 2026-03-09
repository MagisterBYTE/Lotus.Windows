using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFControlsSpecial
	*@{*/
    /// <summary>
    /// Конвертер типа <see cref="TLogType"/> в соответствующую графическую пиктограмму.
    /// </summary>
    [ValueConversion(typeof(TLogType), typeof(BitmapSource))]
    public sealed class LogTypeToImageConverter : IValueConverter
    {
        #region Properties
        /// <summary>
        /// Пиктограмма для Info.
        /// </summary>
        public BitmapSource Info { get; set; }

        /// <summary>
        /// Пиктограмма для Warning.
        /// </summary>
        public BitmapSource Warning { get; set; }

        /// <summary>
        /// Пиктограмма для Error.
        /// </summary>
        public BitmapSource Error { get; set; }

        /// <summary>
        /// Пиктограмма для Succeed.
        /// </summary>
        public BitmapSource Succeed { get; set; }

        /// <summary>
        /// Пиктограмма для Failed.
        /// </summary>
        public BitmapSource Failed { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Конвертация объекта TLogType в соответствующую графическую пиктограмму.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Графическая пиктограмма.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (TLogType)value;
            BitmapSource? bitmap;
            switch (val)
            {
                case TLogType.Info: bitmap = Info; break;
                case TLogType.Warning: bitmap = Warning; break;
                case TLogType.Error: bitmap = Error; break;
                case TLogType.Succeed: bitmap = Succeed; break;
                case TLogType.Failed: bitmap = Failed; break;
                default: bitmap = Info; break;
            }
            return bitmap;
        }

        /// <summary>
        /// Конвертация графической пиктограммы в тип TLogType.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Объект TLogType.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null!;
        #endregion
    }

    /// <summary>
    /// Селектор шаблона данных для отображения сообщения.
    /// </summary>
    public class LogViewItemDataSelector : DataTemplateSelector
    {
        #region Properties
        /// <summary>
        /// Шаблон для представления простого сообщения.
        /// </summary>
        public DataTemplate Simple { get; set; }

        /// <summary>
        /// Шаблон для представления сообщения с трассировкой.
        /// </summary>
        public DataTemplate Trace { get; set; }

        /// <summary>
        /// Шаблон для представления простого сообщения с указанием модуля.
        /// </summary>
        public DataTemplate SimpleModule { get; set; }

        /// <summary>
        /// Шаблон для представления сообщения с трассировкой с указанием модуля.
        /// </summary>
        public DataTemplate TraceModule { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Выбор шаблона привязки данных.
        /// </summary>
        /// <param name="item">Объект.</param>
        /// <param name="container">Контейнер.</param>
        /// <returns>Нужный шаблон.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var message = (LogMessage)item;

            if (string.IsNullOrEmpty(message.Module))
                return string.IsNullOrEmpty(message.MemberName) ? Simple : Trace;
            else
                return string.IsNullOrEmpty(message.MemberName) ? SimpleModule : TraceModule;
        }
        #endregion
    }

    /// <summary>
    /// Панель для ведения лога и вывода вспомогательной информации.
    /// </summary>
    public class LotusLoggerControl : UserControl, ILotusLoggerView
    {
        #region Fields
        private ListArray<LogMessage> _messages;
        private ListBox? _listBox;
        #endregion

        #region Properties
        /// <summary>
        /// Все сообщения.
        /// </summary>
        public ListArray<LogMessage> Messages => _messages;
        #endregion

        #region Constructors
        static LotusLoggerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LotusLoggerControl),
                new FrameworkPropertyMetadata(typeof(LotusLoggerControl)));
        }

        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusLoggerControl()
        {
            _messages = [];
            _messages.IsNotify = true;
        }
        #endregion

        #region System methods
        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("buttonClear") is Button buttonClear)
                buttonClear.Click += OnButtonMessageClear_Click;

            if (GetTemplateChild("buttonSave") is Button buttonSave)
                buttonSave.Click += OnButtonSave_Click;

            if (GetTemplateChild("outputData") is ListBox listBox)
            {
                _listBox = listBox;
                _listBox.ItemsSource = _messages;
            }
        }
        #endregion

        #region ILotusLoggerView methods
        /// <summary>
        /// Добавление сообщения.
        /// </summary>
        /// <param name="text">Текст сообщения.</param>
        /// <param name="type">Тип сообщения.</param>
        public void Log(string text, TLogType type)
        {
            _messages.Add(new LogMessage(text, type));
            _listBox?.ScrollIntoView(_messages[_messages.Count - 1]);
        }

        /// <summary>
        /// Добавление сообщения.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public void Log(LogMessage message)
        {
            _messages.Add(message);
            _listBox?.ScrollIntoView(_messages[_messages.Count - 1]);
        }

        /// <summary>
        /// Добавление сообщения с указанием модуля.
        /// </summary>
        /// <param name="moduleName">Имя модуля.</param>
        /// <param name="text">Текст сообщения.</param>
        /// <param name="type">Тип сообщения.</param>
        public void LogModule(string moduleName, string text, TLogType type)
        {
            _messages.Add(new LogMessage(moduleName, text, type));
            _listBox?.ScrollIntoView(_messages[_messages.Count - 1]);
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Очистка списка сообщений.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonMessageClear_Click(object sender, RoutedEventArgs args)
        {
            _messages.Clear();
        }

        /// <summary>
        /// Сохранение лога в файл.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonSave_Click(object sender, RoutedEventArgs args)
        {
            XLogger.SaveToText("Log.txt");
        }
        #endregion
    }
    /**@}*/
}
