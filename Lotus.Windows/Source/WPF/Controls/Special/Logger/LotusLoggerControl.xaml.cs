//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Элементы интерфейса
// Группа: Специальные элементы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusLoggerControl.xaml.cs
*		Панель для ведения лога и вывода вспомогательной информации.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Globalization;
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
		/**
         * \defgroup WindowsWPFControlsSpecial Специальные элементы
         * \ingroup WindowsWPFControls
         * \brief Специальные элементы.
         * @{
         */
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер типа <see cref="TLogType"/> в соответствующую графическую пиктограмму
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[ValueConversion(typeof(TLogType), typeof(BitmapSource))]
		public sealed class CLogTypeToImageConverter : IValueConverter
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Пиктограмма для Info
			/// </summary>
			public BitmapSource Info { get; set; }

			/// <summary>
			/// Пиктограмма для Warning
			/// </summary>
			public BitmapSource Warning { get; set; }

			/// <summary>
			/// Пиктограмма для Error
			/// </summary>
			public BitmapSource Error { get; set; }

			/// <summary>
			/// Пиктограмма для Succeed
			/// </summary>
			public BitmapSource Succeed { get; set; }

			/// <summary>
			/// Пиктограмма для Failed
			/// </summary>
			public BitmapSource Failed { get; set; }
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация объекта TLogType в соответствующую графическую пиктограмму
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Графическая пиктограмма</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				TLogType val = (TLogType)value;
				BitmapSource bitmap = null;
				switch (val)
				{
					case TLogType.Info:
						{
							bitmap = Info;
						}
						break;
					case TLogType.Warning:
						{
							bitmap = Warning;
						}
						break;
					case TLogType.Error:
						{
							bitmap = Error;
						}
						break;
					case TLogType.Succeed:
						{
							bitmap = Succeed;
						}
						break;
					case TLogType.Failed:
						{
							bitmap = Failed;
						}
						break;
					default:
						break;
				}

				return (bitmap);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация графической пиктограммы в тип TLogType
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Объект TLogType</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				return (null);
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Селектор шаблона данных для отображения сообщения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CLogViewItemDataSelector : DataTemplateSelector
		{
			#region ======================================= ДАННЫЕ ====================================================
			/// <summary>
			/// Шаблон для представления простого сообщения
			/// </summary>
			public DataTemplate Simple { get; set; }

			/// <summary>
			/// Шаблон для представления сообщения с трассировкой
			/// </summary>
			public DataTemplate Trace { get; set; }

			/// <summary>
			/// Шаблон для представления простого сообщения с указанием модуля
			/// </summary>
			public DataTemplate SimpleModule { get; set; }

			/// <summary>
			/// Шаблон для представления сообщения с трассировкой с указанием модуля
			/// </summary>
			public DataTemplate TraceModule { get; set; }
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор шаблона привязки данных
			/// </summary>
			/// <param name="item">Объект</param>
			/// <param name="container">Контейнер</param>
			/// <returns>Нужный шаблон</returns>
			//---------------------------------------------------------------------------------------------------------
			public override DataTemplate SelectTemplate(Object item, DependencyObject container)
			{
				TLogMessage message = (TLogMessage)item;

				if (String.IsNullOrEmpty(message.Module))
				{
					if (String.IsNullOrEmpty(message.MemberName))
					{
						return (Simple);
					}
					else
					{
						return (Trace);
					}
				}
				else
				{
					if (String.IsNullOrEmpty(message.MemberName))
					{
						return (SimpleModule);
					}
					else
					{
						return (TraceModule);
					}
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Панель для ведения лога и вывода вспомогательной информации
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusLoggerControl : UserControl, ILoggerView, INotifyPropertyChanged
		{
			#region ======================================= ДАННЫЕ ====================================================
			private ListArray<TLogMessage> mMessages;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Все сообщения
			/// </summary>
			public ListArray<TLogMessage> Messages
			{
				get { return (mMessages); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusLoggerControl()
			{
				InitializeComponent();
				mMessages = new ListArray<TLogMessage>();
				mMessages.IsNotify = true;
				outputData.ItemsSource = mMessages;
			}
			#endregion

			#region ======================================= МЕТОДЫ ILoggerView ========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление сообщения
			/// </summary>
			/// <param name="text">Имя сообщения</param>
			/// <param name="type">Тип сообщения</param>
			//---------------------------------------------------------------------------------------------------------
			public void Log(String text, TLogType type)
			{
				mMessages.Add(new TLogMessage(text, type));
				outputData.ScrollIntoView(mMessages[mMessages.Count - 1]);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление сообщения
			/// </summary>
			/// <param name="message">Сообщение</param>
			//---------------------------------------------------------------------------------------------------------
			public void Log(TLogMessage message)
			{
				mMessages.Add(message);
				outputData.ScrollIntoView(mMessages[mMessages.Count - 1]);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление сообщения
			/// </summary>
			/// <param name="module">Имя модуля</param>
			/// <param name="text">Имя сообщения</param>
			/// <param name="type">Тип сообщения</param>
			//---------------------------------------------------------------------------------------------------------
			public void LogModule(String module, String text, TLogType type)
			{
				mMessages.Add(new TLogMessage(module, text, type));
				outputData.ScrollIntoView(mMessages[mMessages.Count - 1]);
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Очистка списка сообщений
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonMessageClear_Click(Object sender, RoutedEventArgs args)
			{
				mMessages.Clear();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Очистка списка сообщений
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonSave_Click(Object sender, RoutedEventArgs args)
			{
				XLogger.SaveToText("Log.txt");
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(String property_name = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property_name));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(PropertyChangedEventArgs args)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, args);
				}
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================