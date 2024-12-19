using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFCommon
	*@{*/
    /// <summary>
    /// Определение стандартной команды WPF c дополнительной иконкой.
    /// </summary>
    public class RoutedIconUICommand : RoutedUICommand, INotifyPropertyChanged
    {
        #region Static fields 
        private static readonly PropertyChangedEventArgs PropertyArgsMiddleIcon = new(nameof(MiddleIcon));
        private static readonly PropertyChangedEventArgs PropertyArgsLargeIcon = new(nameof(LargeIcon));
        #endregion

        #region Fields 
        private BitmapSource? _middleIcon;
        private BitmapSource? _largeIcon;
        #endregion

        #region Properties 
        /// <summary>
        /// Источник изображения средней иконки.
        /// </summary>
        public BitmapSource? MiddleIcon
        {
            get { return _middleIcon; }
            set
            {
                _middleIcon = value;
                NotifyPropertyChanged(PropertyArgsMiddleIcon);
            }
        }

        /// <summary>
        /// Источник изображения большой иконки.
        /// </summary>
        public BitmapSource? LargeIcon
        {
            get { return _largeIcon; }
            set
            {
                _largeIcon = value;
                NotifyPropertyChanged(PropertyArgsLargeIcon);
            }
        }
        #endregion

        #region Constructors 
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public RoutedIconUICommand()
        {
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="text">Описательный текст для команды.</param>
        /// <param name="middleIcon">Иконка команды.</param>
        public RoutedIconUICommand(string text, BitmapSource middleIcon)
        {
            Text = text;
            _middleIcon = middleIcon;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="text">Описательный текст для команды.</param>
        /// <param name="middleIcon">Иконка команды.</param>
        /// <param name="largeIcon">Иконка команды.</param>
        public RoutedIconUICommand(string text, BitmapSource middleIcon, BitmapSource largeIcon)
        {
            Text = text;
            _middleIcon = middleIcon;
            _largeIcon = largeIcon;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="text">Описательный текст для команды.</param>
        /// <param name="name">Объявленное имя команды для сериализации.</param>
        /// <param name="middleIcon">Иконка команды.</param>
        public RoutedIconUICommand(string text, string name, BitmapSource middleIcon)
            : base(text, name, typeof(Window))
        {
            _middleIcon = middleIcon;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="text">Описательный текст для команды.</param>
        /// <param name="name">Объявленное имя команды для сериализации.</param>
        /// <param name="middleIcon">Иконка команды.</param>
        /// <param name="largeIcon">Иконка команды.</param>
        public RoutedIconUICommand(string text, string name, BitmapSource middleIcon, BitmapSource largeIcon)
            : base(text, name, typeof(Window))
        {
            _middleIcon = middleIcon;
            _largeIcon = largeIcon;
        }

        /// <summary>
        /// Конструктор инициализирует объект класса указанными параметрами.
        /// </summary>
        /// <param name="text">Описательный текст для команды.</param>
        /// <param name="name">Объявленное имя команды для сериализации.</param>
        /// <param name="ownerType">Тип, регистрирующий команду.</param>
        public RoutedIconUICommand(string text, string name, Type ownerType)
            : base(text, name, ownerType)

        {

        }
        #endregion

        #region Interface INotifyPropertyChanged 
        /// <summary>
        /// Событие срабатывает ПОСЛЕ изменения свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вспомогательный метод для нотификации изменений свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        public void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Вспомогательный метод для нотификации изменений свойства.
        /// </summary>
        /// <param name="args">Аргументы события.</param>
        public void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, args);
            }
        }
        #endregion
    }
    /**@}*/
}