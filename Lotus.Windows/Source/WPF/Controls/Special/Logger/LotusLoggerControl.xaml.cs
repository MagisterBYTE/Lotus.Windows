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
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Графическая пиктограмма</returns>
			//---------------------------------------------------------------------------------------------------------
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				var val = (TLogType)value;
				BitmapSource? bitmap;
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
						bitmap = Info;
						break;
				}

				return bitmap;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация графической пиктограммы в тип TLogType
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="targetType">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Объект TLogType</returns>
			//---------------------------------------------------------------------------------------------------------
			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return null!;
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

			#region Main methods
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор шаблона привязки данных
			/// </summary>
			/// <param name="item">Объект</param>
			/// <param name="container">Контейнер</param>
			/// <returns>Нужный шаблон</returns>
			//---------------------------------------------------------------------------------------------------------
			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				var message = (LogMessage)item;

				if (string.IsNullOrEmpty(message.Module))
				{
					if (string.IsNullOrEmpty(message.MemberName))
					{
						return Simple;
					}
					else
					{
						return Trace;
					}
				}
				else
				{
					if (string.IsNullOrEmpty(message.MemberName))
					{
						return SimpleModule;
					}
					else
					{
						return TraceModule;
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
		public partial class LotusLoggerControl : UserControl, ILotusLoggerView, INotifyPropertyChanged
		{
			#region ======================================= ДАННЫЕ ====================================================
			private ListArray<LogMessage> _messages;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Все сообщения
			/// </summary>
			public ListArray<LogMessage> Messages
			{
				get { return _messages; }
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
				_messages = new ListArray<LogMessage>();
				_messages.IsNotify = true;
				outputData.ItemsSource = _messages;
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
			public void Log(string text, TLogType type)
			{
				_messages.Add(new LogMessage(text, type));
				outputData.ScrollIntoView(_messages[_messages.Count - 1]);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление сообщения
			/// </summary>
			/// <param name="message">Сообщение</param>
			//---------------------------------------------------------------------------------------------------------
			public void Log(LogMessage message)
			{
				_messages.Add(message);
				outputData.ScrollIntoView(_messages[_messages.Count - 1]);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление сообщения
			/// </summary>
			/// <param name="moduleName">Имя модуля</param>
			/// <param name="text">Имя сообщения</param>
			/// <param name="type">Тип сообщения</param>
			//---------------------------------------------------------------------------------------------------------
			public void LogModule(string moduleName, string text, TLogType type)
			{
				_messages.Add(new LogMessage(moduleName, text, type));
				outputData.ScrollIntoView(_messages[_messages.Count - 1]);
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
			private void OnButtonMessageClear_Click(object sender, RoutedEventArgs args)
			{
				_messages.Clear();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Очистка списка сообщений
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnButtonSave_Click(object sender, RoutedEventArgs args)
			{
				XLogger.SaveToText("Log.txt");
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler? PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(string property_name = "")
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