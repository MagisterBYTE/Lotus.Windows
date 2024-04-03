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

    /** \addtogroup WindowsWPFControlsCommon
	*@{*/
    /// <summary>
    /// Тип сообщения.
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
    /// Сообщение.
    /// </summary>
    /// <remarks>
    /// Класс представляющий собой структуру сообщения.
    /// </remarks>
    public class CNotification : PropertyChangedBase
    {
        #region Static fields
        private static readonly PropertyChangedEventArgs PropertyArgsNoticeType = new PropertyChangedEventArgs(nameof(NoticeType));
        private static readonly PropertyChangedEventArgs PropertyArgsMessage = new PropertyChangedEventArgs(nameof(Message));
        private static readonly PropertyChangedEventArgs PropertyArgsID = new PropertyChangedEventArgs(nameof(ID));
        private static readonly PropertyChangedEventArgs PropertyArgsTitle = new PropertyChangedEventArgs(nameof(Title));
        #endregion

        #region Fields
        protected internal TNotificationType _noticeType;
        protected internal string _message;
        protected internal int _id;
        protected internal string _title;
        #endregion

        #region Properties
        /// <summary>
        /// Тип сообщения.
        /// </summary>
        public TNotificationType NoticeType
        {
            get { return _noticeType; }
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
            get { return _message; }
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
        /// Уникальный идентификатор сообщения.
        /// </summary>
        public int ID
        {
            get { return _id; }
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
        /// Заголовок сообщения.
        /// </summary>
        public string Title
        {
            get { return _title; }
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
    /// Наблюдаемая коллекция для сообщений.
    /// </summary>
    public class CNotifications : ObservableCollection<CNotification>
    {
    }

    /// <summary>
    /// Конвертер типа <see cref="TNotificationType"/> в соответствующую графическую пиктограмму.
    /// </summary>
    [ValueConversion(typeof(TNotificationType), typeof(BitmapSource))]
    public sealed class CNotificationTypeToImageConverter : IValueConverter
    {
        #region Properties
        /// <summary>
        /// Пиктограмма сообщения.
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
        /// Конвертер типа NotificationType в соответствующую графическую пиктограмму.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Графическая пиктограмма.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (TNotificationType)value;
            switch (val)
            {
                case TNotificationType.Info:
                    {
                        return Info;
                    }
                case TNotificationType.Warning:
                    {
                        return Warning;
                    }
                case TNotificationType.Error:
                    {
                        return Error;
                    }
                default:
                    return Info;
            }
        }

        /// <summary>
        /// Конвертация графической пиктограммы в тип NotificationType.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр.</param>
        /// <param name="culture">Культура.</param>
        /// <returns>Тип NotificationType.</returns>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }

    /// <summary>
    /// Элемент для уведомления о сообщениях, предупреждениях и ошибках.
    /// </summary>
    public partial class LotusGrowlNotification : Window
    {
        #region Const
        /// <summary>
        /// Максимальное количество видимых оповещений.
        /// </summary>
        private const int MaxNotifications = 4;
        #endregion

        #region Fields
        private int mCount;
        private CNotifications mCurrentNotifications;
        private CNotifications mBufferNotifications;
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusGrowlNotification()
        {
            InitializeComponent();
            mCurrentNotifications = new CNotifications();
            mBufferNotifications = new CNotifications();
            NotificationsControl.DataContext = mCurrentNotifications;
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Добавление сообщения.
        /// </summary>
        /// <param name="notification">Сообщение.</param>
        public void AddNotification(CNotification notification)
        {
            notification.ID = mCount++;
            if (mCurrentNotifications.Count + 1 > MaxNotifications)
            {
                mBufferNotifications.Add(notification);
            }
            else
            {
                mCurrentNotifications.Add(notification);
            }

            //Show window if there're notifications
            if (mCurrentNotifications.Count > 0 && !IsActive)
            {
                Show();
            }
        }

        /// <summary>
        /// Добавление сообщения.
        /// </summary>
        /// <param name="notice_type">Тип сообщения.</param>
        /// <param name="message">Текст сообщения.</param>
        public void AddNotification(TNotificationType notice_type, string message)
        {
            var notification = new CNotification();
            notification.NoticeType = notice_type;
            notification.Message = message;
            notification.ID = mCount++;

            switch (notice_type)
            {
                case TNotificationType.Info:
                    {
                        notification.Title = "Информация";
                    }
                    break;
                case TNotificationType.Warning:
                    {
                        notification.Title = "Предупреждение";
                    }
                    break;
                case TNotificationType.Error:
                    {
                        notification.Title = "Ошибка";
                    }
                    break;
                default:
                    break;
            }

            if (mCurrentNotifications.Count + 1 > MaxNotifications)
            {
                mBufferNotifications.Add(notification);
            }
            else
            {
                mCurrentNotifications.Add(notification);
            }

            //Show window if there're notifications
            if (mCurrentNotifications.Count > 0 && !IsActive)
            {
                Show();
            }
        }

        /// <summary>
        /// Удаление сообщения.
        /// </summary>
        /// <param name="notification">Сообщение.</param>
        public void RemoveNotification(CNotification notification)
        {
            if (mCurrentNotifications.Contains(notification))
            {
                mCurrentNotifications.Remove(notification);
            }

            if (mBufferNotifications.Count > 0)
            {
                mCurrentNotifications.Add(mBufferNotifications[0]);
                mBufferNotifications.RemoveAt(0);
            }

            //Close window if there's nothing to show
            if (mCurrentNotifications.Count < 1)
            {
                Hide();
            }
        }
        #endregion

        #region Event handlers 
        /// <summary>
        /// Изменение размеров окна.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void NotificationWindow_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (args.NewSize.Height != 0.0)
            {
                return;
            }
            var element = (sender as Grid)!;
            RemoveNotification(mCurrentNotifications.First(n => n.ID == int.Parse(element.Tag.ToString()!)));
        }
        #endregion
    }
    /**@}*/
}