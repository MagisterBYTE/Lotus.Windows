//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Общие элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusGrowlNotification.xaml.cs
*		Элемент для уведомления о сообщениях, предупреждениях и ошибках.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFControlsCommon
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Тип сообщения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TNotificationType
		{
			/// <summary>
			/// Информация
			/// </summary>
			Info,

			/// <summary>
			/// Предупреждение
			/// </summary>
			Warning,

			/// <summary>
			/// Ошибка
			/// </summary>
			Error
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сообщение
		/// </summary>
		/// <remarks>
		/// Класс представляющий собой структуру сообщения
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public class CNotification : PropertyChangedBase
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			private static readonly PropertyChangedEventArgs PropertyArgsNoticeType = new PropertyChangedEventArgs(nameof(NoticeType));
			private static readonly PropertyChangedEventArgs PropertyArgsMessage = new PropertyChangedEventArgs(nameof(Message));
			private static readonly PropertyChangedEventArgs PropertyArgsID = new PropertyChangedEventArgs(nameof(ID));
			private static readonly PropertyChangedEventArgs PropertyArgsTitle = new PropertyChangedEventArgs(nameof(Title));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			private TNotificationType mNoticeType;
			private String mMessage;
			private Int32 mID;
			private String mTitle;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Тип сообщения
			/// </summary>
			public TNotificationType NoticeType
			{
				get { return (mNoticeType); }
				set
				{
					if (mNoticeType != value)
					{
						mNoticeType = value;
						NotifyPropertyChanged(PropertyArgsNoticeType);
					}
				}
			}

			/// <summary>
			/// Текст сообщения
			/// </summary>
			public String Message
			{
				get { return (mMessage); }
				set
				{
					if (mMessage != value)
					{
						mMessage = value;
						NotifyPropertyChanged(PropertyArgsMessage);
					}
				}
			}

			/// <summary>
			/// Уникальный идентификатор сообщения
			/// </summary>
			public Int32 ID
			{
				get { return (mID); }
				set
				{
					if (mID != value)
					{
						mID = value;
						NotifyPropertyChanged(PropertyArgsID);
					}
				}
			}

			/// <summary>
			/// Заголовок сообщения
			/// </summary>
			public String Title
			{
				get { return (mTitle); }
				set
				{
					if (mTitle != value)
					{
						mTitle = value;
						NotifyPropertyChanged(PropertyArgsTitle);
					}
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Наблюдаемая коллекция для сообщений
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CNotifications : ObservableCollection<CNotification>
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер типа <see cref="TNotificationType"/> в соответствующую графическую пиктограмму
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(TNotificationType), typeof(BitmapSource))]
		public sealed class CNotificationTypeToImageConverter : IValueConverter
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Пиктограмма сообщения
			/// </summary>
			public BitmapSource Info { get; set; }

			/// <summary>
			/// Пиктограмма предупреждения
			/// </summary>
			public BitmapSource Warning { get; set; }

			/// <summary>
			/// Пиктограмма ошибки
			/// </summary>
			public BitmapSource Error { get; set; }
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертер типа NotificationType в соответствующую графическую пиктограмму.
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Графическая пиктограмма</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				TNotificationType val = (TNotificationType)value;
				switch (val)
				{
					case TNotificationType.Info:
						{
							return (Info);
						}
					case TNotificationType.Warning:
						{
							return (Warning);
						}
					case TNotificationType.Error:
						{
							return (Error);
						}
					default:
						break;
				}

				return (null);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация графической пиктограммы в тип NotificationType.
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Тип NotificationType</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				return (null);
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент для уведомления о сообщениях, предупреждениях и ошибках
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusGrowlNotification : Window
		{
			#region ======================================= КОНСТАНТНЫЕ ДАННЫЕ ========================================
			/// <summary>
			/// Максимальное количество видимых оповещений
			/// </summary>
			private const Int32 MaxNotifications = 4;
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			private Int32 mCount;
			private CNotifications mCurrentNotifications;
			private CNotifications mBufferNotifications;
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusGrowlNotification()
			{
				InitializeComponent();
				mCurrentNotifications = new CNotifications();
				mBufferNotifications = new CNotifications();
				NotificationsControl.DataContext = mCurrentNotifications;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление сообщения
			/// </summary>
			/// <param name="notification">Сообщение</param>
			//---------------------------------------------------------------------------------------------------------
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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление сообщения
			/// </summary>
			/// <param name="notice_type">Тип сообщения</param>
			/// <param name="message">Текст сообщения</param>
			//---------------------------------------------------------------------------------------------------------
			public void AddNotification(TNotificationType notice_type, String message)
			{
				CNotification notification = new CNotification();
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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаление сообщения
			/// </summary>
			/// <param name="notification">Сообщение</param>
			//---------------------------------------------------------------------------------------------------------
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

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение размеров окна
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void NotificationWindow_SizeChanged(Object sender, SizeChangedEventArgs args)
			{
				if (args.NewSize.Height != 0.0)
				{
					return;
				}
				var element = sender as Grid;
				RemoveNotification(mCurrentNotifications.First(n => n.ID == Int32.Parse(element.Tag.ToString())));
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================