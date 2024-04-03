using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using FreeImageAPI;

using Lotus.Core;
using Lotus.Windows;

namespace Lotus.Windows
{
    /** \addtogroup WindowsViewerImageControls
	*@{*/
    /// <summary>
    /// Элемент для просмотра и редактирования файлов в формате изображения.
    /// </summary>
    public partial class LotusViewerImage : UserControl, ILotusViewerContentFile, INotifyPropertyChanged
    {
        #region Static fields
        /// <summary>
        /// Список поддерживаемых форматов файлов.
        /// </summary>
        public static readonly string[] SupportFormatFile = new string[]
        {
            ".bmp",
            ".jpeg",
            ".jpg",
            ".png",
            ".tiff",
            ".tif",
            ".psd",
            ".tga",
            ".targa",
            ".gif",
            ".hdr",
            ".dds"
        };

        //
        // Константы для информирования об изменении свойств
        //
        protected static readonly PropertyChangedEventArgs PropertyArgsImageWidth = new PropertyChangedEventArgs(nameof(ImageWidth));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageHeight = new PropertyChangedEventArgs(nameof(ImageHeight));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageResolutionX = new PropertyChangedEventArgs(nameof(ImageResolutionX));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageResolutionY = new PropertyChangedEventArgs(nameof(ImageResolutionY));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageFormat = new PropertyChangedEventArgs(nameof(ImageFormat));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageImageType = new PropertyChangedEventArgs(nameof(ImageType));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageColorType = new PropertyChangedEventArgs(nameof(ImageColorType));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageColorDepth = new PropertyChangedEventArgs(nameof(ImageColorDepth));
        protected static readonly PropertyChangedEventArgs PropertyArgsImagePixelFormat = new PropertyChangedEventArgs(nameof(ImagePixelFormat));
        protected static readonly PropertyChangedEventArgs PropertyArgsIsTransparentImage = new PropertyChangedEventArgs(nameof(IsTransparentImage));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageRedMask = new PropertyChangedEventArgs(nameof(ImageRedMask));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageGreenMask = new PropertyChangedEventArgs(nameof(ImageGreenMask));
        protected static readonly PropertyChangedEventArgs PropertyArgsImageBlueMask = new PropertyChangedEventArgs(nameof(ImageBlueMask));
        #endregion

        #region Static methods
        /// <summary>
        /// Проверка на поддерживаемый формат файла.
        /// </summary>
        /// <param name="extension">Расширение файла.</param>
        /// <returns>Статус поддержки.</returns>
        public static bool IsSupportFormatFile(string extension)
        {
            return SupportFormatFile.Contains(extension);
        }

        /// <summary>
        /// Загрузка изображения по полному пути.
        /// </summary>
        /// <param name="file_name">Имя файла.</param>
        /// <returns>Объект BitmapSource.</returns>
        public static BitmapSource? LoadFromFile(string file_name)
        {
            // Format is stored in 'format' on successfull load.
            FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;

            // Try loading the file
            FIBITMAP dib = FreeImage.LoadEx(file_name, ref format);

            try
            {
                // Error handling
                if (dib.IsNull)
                {
                    return null;
                }

                var image = FreeImage.GetBitmap(dib).ToBitmapSource();
                FreeImage.UnloadEx(ref dib);

                return image;
            }
            catch (Exception exc)
            {
                XLogger.LogExceptionModule(nameof(LotusViewerImage), exc);
                return null;
            }
        }

        /// <summary>
        /// Загрузка изображения по полному пути.
        /// </summary>
        /// <param name="file_name">Имя файла.</param>
        /// <param name="width">Требуемая ширина изображения.</param>
        /// <param name="height">Требуемая высота изображения.</param>
        /// <returns>Объект BitmapSource.</returns>
        public static BitmapSource? LoadFromFile(string file_name, int width, int height)
        {
            // Format is stored in 'format' on successfull load.
            FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;

            // Try loading the file
            FIBITMAP dib = FreeImage.LoadEx(file_name, ref format);

            try
            {
                // Error handling
                if (dib.IsNull)
                {
                    return null;
                }

                var image = FreeImage.GetBitmap(dib).ToBitmapSource(width, height);
                FreeImage.UnloadEx(ref dib);

                return image;
            }
            catch (Exception exc)
            {
                XLogger.LogExceptionModule(nameof(LotusViewerImage), exc);
                return null;
            }
        }
        #endregion

        #region Declare DependencyProperty 
        /// <summary>
        /// Имя файла.
        /// </summary>
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName),
            typeof(string),
            typeof(LotusViewerImage),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
        #endregion

        #region Fields
        protected internal string _fileName;
        protected internal int _imageWidth;
        protected internal int _imageHeight;
        protected internal int _imageResolutionX;
        protected internal int _imageResolutionY;
        protected internal FREE_IMAGE_FORMAT _freeImageFormat;
        protected internal FREE_IMAGE_TYPE _freeImageType;
        protected internal FREE_IMAGE_COLOR_TYPE _freeImageColorType;
        protected internal int _colorDepth;
        protected internal PixelFormat _pixelFormat;
        protected internal bool _isTransparentImage;
        protected internal uint _imageRedMask;
        protected internal uint _imageGreenMask;
        protected internal uint _imageBlueMask;

        protected internal FIBITMAP _freeImageBitmap;
        protected internal Image _imagePresented;
        protected internal BitmapSource? _bitmapOriginal;
        protected internal BitmapSource? _bitmapAlpha;
        protected internal BitmapSource? _bitmapNoTransparent;
        protected internal string _currentMessage;
        #endregion

        #region Properties
        /// <summary>
        /// Имя файла.
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        //
        // ПАРАМЕТРЫ ИЗОБРАЖЕНИЯ
        //
        /// <summary>
        /// Ширина изображения.
        /// </summary>
        public int ImageWidth
        {
            get
            {
                return _imageWidth;
            }
        }

        /// <summary>
        /// Высота изображения.
        /// </summary>
        public int ImageHeight
        {
            get
            {
                return _imageHeight;
            }
        }

        /// <summary>
        /// Разрешение изображения по X.
        /// </summary>
        public int ImageResolutionX
        {
            get
            {
                return _imageResolutionX;
            }
        }

        /// <summary>
        /// Разрешение изображения по Y.
        /// </summary>
        public int ImageResolutionY
        {
            get
            {
                return _imageResolutionY;
            }
        }

        /// <summary>
        /// Формат изображения.
        /// </summary>
        public FREE_IMAGE_FORMAT ImageFormat
        {
            get
            {
                return _freeImageFormat;
            }
        }

        /// <summary>
        /// Тип изображения.
        /// </summary>
        public FREE_IMAGE_TYPE ImageType
        {
            get
            {
                return _freeImageType;
            }
        }

        /// <summary>
        /// Тип цвета изображения.
        /// </summary>
        public FREE_IMAGE_COLOR_TYPE ImageColorType
        {
            get
            {
                return _freeImageColorType;
            }
        }

        /// <summary>
        /// Глубина цвета.
        /// </summary>
        public int ImageColorDepth
        {
            get { return _colorDepth; }
        }

        /// <summary>
        /// Формат пикселя.
        /// </summary>
        public PixelFormat ImagePixelFormat
        {
            get { return _pixelFormat; }
        }

        /// <summary>
        /// Статус прозрачности изображения.
        /// </summary>
        public bool IsTransparentImage
        {
            get { return _isTransparentImage; }
        }

        /// <summary>
        /// Маска прозрачности для красного цвета.
        /// </summary>
        public uint ImageRedMask
        {
            get { return _imageRedMask; }
        }

        /// <summary>
        /// Маска прозрачности для зеленого цвета.
        /// </summary>
        public uint ImageGreenMask
        {
            get { return _imageGreenMask; }
        }

        /// <summary>
        /// Маска прозрачности для синего цвета.
        /// </summary>
        public uint ImageBlueMask
        {
            get { return _imageBlueMask; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusViewerImage()
        {
            InitializeComponent();
            FreeImageEngine.Message += new OutputMessageFunction(OnFreeImageMessage);
        }

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~LotusViewerImage()
        {
            FreeImageEngine.Message -= new OutputMessageFunction(OnFreeImageMessage);
        }
        #endregion

        #region ILotusViewerContentFile methods
        /// <summary>
        /// Создание нового файла с указанным именем и параметрами.
        /// </summary>
        /// <param name="file_name">Имя файла.</param>
        /// <param name="parameters_create">Параметры создания файла.</param>
        public void NewFile(string file_name, CParameters? parameters_create)
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Открытие указанного файла.
        /// </summary>
        /// <param name="file_name">Полное имя файла.</param>
        /// <param name="parameters_open">Параметры открытия файла.</param>
        public void OpenFile(string? file_name, CParameters? parameters_open)
        {
            // Если файл пустой то используем диалог
            if (string.IsNullOrEmpty(file_name))
            {
                file_name = XFileDialog.Open("Открыть изображение", "");
                if (file_name != null && file_name.IsExists())
                {
                    // Загружаем файл
                    Load(file_name);

                    FileName = file_name;
                    XLogger.LogInfoModule(nameof(LotusViewerImage), $"Открыт файл с именем: [{FileName}]");
                }
            }
            else
            {
                // Загружаем файл
                Load(file_name);
                FileName = file_name;
                XLogger.LogInfoModule(nameof(LotusViewerImage), $"Открыт файл с именем: [{FileName}]");
            }
        }

        /// <summary>
        /// Сохранения файла.
        /// </summary>
        public void SaveFile()
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Сохранение файла под новым именем и параметрами.
        /// </summary>
        /// <param name="file_name">Полное имя файла.</param>
        /// <param name="parameters_save">Параметры сохранения файла.</param>
        public void SaveAsFile(string? file_name, CParameters? parameters_save)
        {
            if (string.IsNullOrEmpty(file_name))
            {
                if (string.IsNullOrEmpty(FileName) == false)
                {

                }
                else
                {

                }
            }
            else
            {
                if (XFilePath.CheckCorrectFileName(file_name))
                {

                }
            }
        }

        /// <summary>
        /// Печать файла.
        /// </summary>
        /// <param name="parameters_print">Параметры печати файла.</param>
        public void PrintFile(CParameters? parameters_print)
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Экспорт файла под указанным именем и параметрами.
        /// </summary>
        /// <param name="file_name">Полное имя файла.</param>
        /// <param name="parameters_export">Параметры для экспорта файла.</param>
        public void ExportFile(string? file_name, CParameters? parameters_export)
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Закрытие файла.
        /// </summary>
        public void CloseFile()
        {
            imagePresent.Source = null;
            _bitmapOriginal = null;
            _bitmapAlpha = null;
            _bitmapNoTransparent = null;
            FileName = "";
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Загрузка изображения по полному пути.
        /// </summary>
        /// <param name="file_name">Имя файла.</param>
        public void Load(string file_name)
        {
            if (!_freeImageBitmap.IsNull)
            {
                _freeImageBitmap.SetNull();
            }

            // Try loading the file
            _freeImageFormat = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
            _freeImageBitmap = FreeImage.LoadEx(file_name, ref _freeImageFormat);

            try
            {
                // Error handling
                if (_freeImageBitmap.IsNull)
                {
                    // Chech whether FreeImage generated an error messe
                    if (_currentMessage != null)
                    {
                        XLogger.LogErrorFormatModule(nameof(LotusViewerImage), "File could not be loaded!\nError:{0}", _currentMessage);
                    }
                    else
                    {
                        XLogger.LogErrorModule(nameof(LotusViewerImage), "File could not be loaded!");
                    }
                    return;
                }


                _fileName = file_name;

                //
                // РАЗМЕР ИЗОБРАЖЕНИЯ
                //
                _imageWidth = (int)FreeImage.GetWidth(_freeImageBitmap);
                _imageHeight = (int)FreeImage.GetHeight(_freeImageBitmap);
                _imageResolutionX = (int)FreeImage.GetResolutionX(_freeImageBitmap);
                _imageResolutionY = (int)FreeImage.GetResolutionY(_freeImageBitmap);

                //
                // ПАРАМЕТРЫ ИЗОБРАЖЕНИЯ
                //
                _freeImageType = FreeImage.GetImageType(_freeImageBitmap);
                _freeImageColorType = FreeImage.GetColorType(_freeImageBitmap);

                //
                // ПАРАМЕТРЫ ЦВЕТА
                //
                _colorDepth = (int)FreeImage.GetBPP(_freeImageBitmap);
                _pixelFormat = FreeImage.GetPixelFormat(_freeImageBitmap);
                _isTransparentImage = FreeImage.IsTransparent(_freeImageBitmap);

                //
                // ПАРАМЕТРЫ МАСКИ
                //
                _imageRedMask = FreeImage.GetRedMask(_freeImageBitmap);
                _imageGreenMask = FreeImage.GetGreenMask(_freeImageBitmap);
                _imageBlueMask = FreeImage.GetBlueMask(_freeImageBitmap);

                // Получаем презентатор
                if (_imagePresented == null) _imagePresented = (contentViewer.Content as Image)!;

                // Основной режим
                _bitmapOriginal = FreeImage.GetBitmap(_freeImageBitmap).ToBitmapSource();

                // Если есть прозрачность
                if (FreeImage.IsTransparent(_freeImageBitmap) && FreeImage.GetBPP(_freeImageBitmap) > 24)
                {
                    // Получаем альфа-канал
                    FIBITMAP bitmap_alpha = FreeImage.GetChannel(_freeImageBitmap, FREE_IMAGE_COLOR_CHANNEL.FICC_ALPHA);
                    if (!bitmap_alpha.IsNull)
                    {
                        _bitmapAlpha = FreeImage.GetBitmap(bitmap_alpha).ToBitmapSource();
                        FreeImage.UnloadEx(ref bitmap_alpha);
                    }

                    // Преобразуем
                    FIBITMAP bitmap_no_transparent = FreeImage.ConvertTo24Bits(_freeImageBitmap);
                    if (!bitmap_no_transparent.IsNull)
                    {
                        _bitmapNoTransparent = FreeImage.GetBitmap(bitmap_no_transparent).ToBitmapSource();
                        FreeImage.UnloadEx(ref bitmap_no_transparent);
                    }
                }

                _imagePresented.Source = _bitmapOriginal;
                _imagePresented.Width = _imageWidth;
                _imagePresented.Height = _imageHeight;
            }
            catch (Exception exc)
            {
                XLogger.LogExceptionModule(nameof(LotusViewerImage), exc);
            }

            // Always unload bitmap
            FreeImage.UnloadEx(ref _freeImageBitmap);

            NotifyPropertyChanged(PropertyArgsImageWidth);
            NotifyPropertyChanged(PropertyArgsImageHeight);
            NotifyPropertyChanged(PropertyArgsImageResolutionX);
            NotifyPropertyChanged(PropertyArgsImageResolutionY);
            NotifyPropertyChanged(PropertyArgsImageFormat);
            NotifyPropertyChanged(PropertyArgsImageImageType);
            NotifyPropertyChanged(PropertyArgsImageColorType);
            NotifyPropertyChanged(PropertyArgsImageColorDepth);
            NotifyPropertyChanged(PropertyArgsImagePixelFormat);
            NotifyPropertyChanged(PropertyArgsIsTransparentImage);
            NotifyPropertyChanged(PropertyArgsImageRedMask);
            NotifyPropertyChanged(PropertyArgsImageGreenMask);
            NotifyPropertyChanged(PropertyArgsImageBlueMask);
        }

        /// <summary>
        /// Отобразить оригинальное изображение.
        /// </summary>
        public void SetViewOriginal()
        {
            if (_bitmapOriginal != null)
            {
                _imagePresented.Source = _bitmapOriginal;
            }
        }

        /// <summary>
        /// Отобразить альфа-канал изображения.
        /// </summary>
        public void SetViewAlpha()
        {
            if (_bitmapAlpha != null)
            {
                _imagePresented.Source = _bitmapAlpha;
            }
        }

        /// <summary>
        /// Отобразить изображение без учета альфа канала.
        /// </summary>
        public void SetViewNoTransparent()
        {
            if (_bitmapNoTransparent != null)
            {
                _imagePresented.Source = _bitmapNoTransparent;
            }
        }
        #endregion

        #region Event handlers 
        /// <summary>
        /// Загрузка элемента.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnUserControl_Loaded(object sender, RoutedEventArgs args)
        {
            double min_width = 1;

            if (_imagePresented.Width > contentViewer.ViewportWidth - 20)
            {
                min_width = (contentViewer.ViewportWidth - 20) / _imagePresented.Width;
            }

            double min_height = 1;
            if (_imagePresented.Height > contentViewer.ViewportHeight - 20)
            {
                min_height = (contentViewer.ViewportHeight - 20) / _imagePresented.Height;
            }

            contentViewer.ContentScale = Math.Min(min_width, min_height);
        }

        /// <summary>
        /// Обработка сообщений библиотеки FreeImage.
        /// </summary>
        /// <param name="format_image">Формат изображения.</param>
        /// <param name="message">Строка сообщения.</param>
        private void OnFreeImageMessage(FREE_IMAGE_FORMAT format_image, string message)
        {
            if (this._currentMessage == null)
            {
                this._currentMessage = message;
            }
            else
            {
                this._currentMessage += "\n" + message;
            }
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