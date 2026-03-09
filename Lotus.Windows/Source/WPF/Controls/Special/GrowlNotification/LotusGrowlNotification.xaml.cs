using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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
    /// Тип уведомления.
    /// </summary>
    public enum TNotificationType
    {
        /// <summary>
        /// Информация.
        /// </summary>
        Info,

        /// <summary>
        /// Предупреждение.
        /// </summary>
        Warning,

        /// <summary>
        /// Ошибка.
        /// </summary>
        Error
    }

    /// <summary>
    /// Данные уведомления.
    /// </summary>
    public class Notification : PropertyChangedBase
    {
        #region Static fields
        private static readonly PropertyChangedEventArgs PropertyArgsNoticeType = new(nameof(NoticeType));
        private static readonly PropertyChangedEventArgs PropertyArgsMessage = new(nameof(Message));
        private static readonly PropertyChangedEventArgs PropertyArgsID = new(nameof(ID));
        private static readonly PropertyChangedEventArgs PropertyArgsTitle = new(nameof(Title));
        #endregion

        #region Fields
        protected internal TNotificationType _noticeType;
        protected internal string _message;
        protected internal int _id;
        protected internal string _title;
        #endregion

        #region Properties
        /// <summary>
        /// Тип уведомления.
        /// </summary>
        public TNotificationType NoticeType
        {
            get => _noticeType;
            set
            {
                if (_noticeType != value)
                {
                    _noticeType = value;
                    OnPropertyChanged(PropertyArgsNoticeType);
                }
            }
        }

        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged(PropertyArgsMessage);
                }
            }
        }

        /// <summary>
        /// Уникальный идентификатор уведомления.
        /// </summary>
        public int ID
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(PropertyArgsID);
                }
            }
        }

        /// <summary>
        /// Заголовок уведомления.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(PropertyArgsTitle);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Наблюдаемая коллекция уведомлений.
    /// </summary>
    public class Notifications : ObservableCollection<Notification>
    {
    }

    /// <summary>
    /// Конвертер типа <see cref="TNotificationType"/> в соответствующую графическую пиктограмму.
    /// </summary>
    [ValueConversion(typeof(TNotificationType), typeof(BitmapSource))]
    public sealed class NotificationTypeToImageConverter : IValueConverter
    {
        #region Properties
        /// <summary>
        /// Пиктограмма информации.
        /// </summary>
        public BitmapSource Info { get; set; } = default!;

        /// <summary>
        /// Пиктограмма предупреждения.
        /// </summary>
        public BitmapSource Warning { get; set; } = default!;

        /// <summary>
        /// Пиктограмма ошибки.
        /// </summary>
        public BitmapSource Error { get; set; } = default!;
        #endregion

        #region Methods
        /// <summary>
        /// Конвертация типа уведомления в пиктограмму.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (TNotificationType)value switch
            {
                TNotificationType.Warning => Warning,
                TNotificationType.Error => Error,
                _ => Info
            };

        /// <inheritdoc />
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
        #endregion
    }

    /// <summary>
    /// Всплывающее окно уведомлений (Growl).
    /// </summary>
    public partial class LotusGrowlNotification : Window
    {
        #region Constants
        private const int _maxNotifications = 4;
        #endregion

        #region Fields
        private int _count;
        private Notifications _currentNotifications;
        private Notifications _bufferNotifications;
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusGrowlNotification()
        {
            InitializeComponent();
            _currentNotifications = [];
            _bufferNotifications = [];
            NotificationsControl.DataContext = _currentNotifications;
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Добавление уведомления.
        /// </summary>
        /// <param name="notification">Уведомление.</param>
        public void AddNotification(Notification notification)
        {
            notification.ID = _count++;
            if (_currentNotifications.Count + 1 > _maxNotifications)
                _bufferNotifications.Add(notification);
            else
                _currentNotifications.Add(notification);

            if (_currentNotifications.Count > 0 && !IsActive)
                Show();
        }

        /// <summary>
        /// Добавление уведомления по типу и тексту.
        /// </summary>
        /// <param name="noticeType">Тип уведомления.</param>
        /// <param name="message">Текст сообщения.</param>
        public void AddNotification(TNotificationType noticeType, string message)
        {
            var notification = new Notification
            {
                NoticeType = noticeType,
                Message = message,
                ID = _count++,
                Title = noticeType switch
                {
                    TNotificationType.Info => "Информация",
                    TNotificationType.Warning => "Предупреждение",
                    TNotificationType.Error => "Ошибка",
                    _ => string.Empty
                }
            };

            if (_currentNotifications.Count + 1 > _maxNotifications)
                _bufferNotifications.Add(notification);
            else
                _currentNotifications.Add(notification);

            if (_currentNotifications.Count > 0 && !IsActive)
                Show();
        }

        /// <summary>
        /// Удаление уведомления.
        /// </summary>
        /// <param name="notification">Уведомление.</param>
        public void RemoveNotification(Notification notification)
        {
            if (_currentNotifications.Contains(notification))
                _currentNotifications.Remove(notification);

            if (_bufferNotifications.Count > 0)
            {
                _currentNotifications.Add(_bufferNotifications[0]);
                _bufferNotifications.RemoveAt(0);
            }

            if (_currentNotifications.Count < 1)
                Hide();
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Обработка схлопывания уведомления по завершении анимации.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void NotificationWindow_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (args.NewSize.Height != 0.0)
                return;

            var element = (sender as Grid)!;
            RemoveNotification(_currentNotifications.First(n => n.ID == XNumberConverter.ParseInt(element.Tag.ToString()!)));
        }
        #endregion
    }
    /**@}*/
}
