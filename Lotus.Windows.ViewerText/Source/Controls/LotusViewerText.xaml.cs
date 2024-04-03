using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Utils;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsViewerTextControls
	*@{*/
    /// <summary>
    /// Allows producing foldings from a document based on braces.
    /// </summary>
    public class BraceFoldingStrategy
    {
        /// <summary>
        /// Gets/Sets the opening brace. The default value is '{'.
        /// </summary>
        public char OpeningBrace { get; set; }

        /// <summary>
        /// Gets/Sets the closing brace. The default value is '}'.
        /// </summary>
        public char ClosingBrace { get; set; }

        /// <summary>
        /// Creates a new BraceFoldingStrategy.
        /// </summary>
        public BraceFoldingStrategy()
        {
            this.OpeningBrace = '{';
            this.ClosingBrace = '}';
        }

        /// <summary>
        ///.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="document"></param>
        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="firstErrorOffset"></param>
        /// <returns></returns>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            var newFoldings = new List<NewFolding>();

            var startOffsets = new Stack<int>();
            var lastNewLineOffset = 0;
            var openingBrace = this.OpeningBrace;
            var closingBrace = this.ClosingBrace;
            for (var i = 0; i < document.TextLength; i++)
            {
                var c = document.GetCharAt(i);
                if (c == openingBrace)
                {
                    startOffsets.Push(i);
                }
                else if (c == closingBrace && startOffsets.Count > 0)
                {
                    var startOffset = startOffsets.Pop();
                    // don't fold if opening and closing brace are on the same line
                    if (startOffset < lastNewLineOffset)
                    {
                        newFoldings.Add(new NewFolding(startOffset, i + 1));
                    }
                }
                else if (c == '\n' || c == '\r')
                {
                    lastNewLineOffset = i + 1;
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }

    /// <summary>
    /// Элемент для просмотра и редактирования файлов в текстовом формате.
    /// </summary>
    public partial class LotusViewerText : UserControl, ILotusViewerContentFile, INotifyPropertyChanged
    {
        #region Static fields
        /// <summary>
        /// Список поддерживаемых форматов файлов.
        /// </summary>
        public static readonly string[] SupportFormatFile = new string[]
        {
            ".txt",
            ".md",
            ".cs",
            ".xml",
            ".php",
            ".java",
            ".info",
            ".json",
            ".js",
            ".css"
        };
        #endregion

        #region Static methods
        /// <summary>
        /// Проверка на поддерживаемый формат файла.
        /// </summary>
        /// <param name="extension">Расширение файла.</param>
        /// <returns>Статус поддержки.</returns>
        public static bool IsSupportFormatFile(string extension)
        {
            return SupportFormatFile.ContainsElement(extension);
        }
        #endregion

        #region Declare DependencyProperty 
        /// <summary>
        /// Имя файла.
        /// </summary>
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName),
            typeof(string),
            typeof(LotusViewerText),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
        #endregion

        #region Fields
        protected internal FoldingManager? _foldingManager;
        protected internal object? _foldingStrategy;

        // Reasonable max and min font size values
        private const double FONT_MAX_SIZE = 60d;
        private const double FONT_MIN_SIZE = 5d;
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

        /// <summary>
        /// Основной текстовый редактор.
        /// </summary>
        public ICSharpCode.AvalonEdit.TextEditor AvalonTextEditor
        {
            get { return avalonTextEditor; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusViewerText()
        {
            InitializeComponent();
        }
        #endregion

        #region ILotusViewerContentFile methods
        /// <summary>
        /// Создание нового файла с указанным именем и параметрами.
        /// </summary>
        /// <param name="file_name">Имя файла.</param>
        /// <param name="parameters_create">Параметры создания файла.</param>
        public void NewFile(string? file_name, CParameters? parameters_create)
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
                file_name = XFileDialog.Open("Открыть документ", string.Empty);
                if (file_name != null && file_name.IsExists())
                {
                    // Загружаем файл
                    AvalonTextEditor.Load(file_name);

                    FileName = file_name;
                    XLogger.LogInfoModule(nameof(LotusViewerText), $"Открыт файл с именем: [{FileName}]");
                }
            }
            else
            {
                // Загружаем файл
                AvalonTextEditor.Load(file_name);
                FileName = file_name;
                XLogger.LogInfoModule(nameof(LotusViewerText), $"Открыт файл с именем: [{FileName}]");
            }
        }

        /// <summary>
        /// Сохранения файла.
        /// </summary>
        public void SaveFile()
        {
            // Если имя файла есть
            if (string.IsNullOrEmpty(FileName) == false)
            {
                AvalonTextEditor.Save(FileName);
                XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
            }
            else
            {
                var file_name = XFileDialog.Save("Сохранить документ", string.Empty);
                if (file_name != null && XFilePath.CheckCorrectFileName(file_name))
                {
                    AvalonTextEditor.Save(file_name);
                    FileName = file_name;
                    XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
                }
            }
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
                    var dir = Path.GetDirectoryName(FileName);
                    var file = Path.GetFileNameWithoutExtension(FileName);
                    var ext = Path.GetExtension(FileName).Remove(0, 1);

                    file_name = XFileDialog.Save("Сохранить документ как", dir ?? string.Empty, file, ext);
                    if (file_name != null && XFilePath.CheckCorrectFileName(file_name))
                    {
                        AvalonTextEditor.Save(file_name);
                        FileName = file_name;
                        XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
                    }
                }
                else
                {
                    file_name = XFileDialog.Save("Сохранить документ как", string.Empty);
                    if (file_name != null && XFilePath.CheckCorrectFileName(file_name))
                    {
                        AvalonTextEditor.Save(file_name);
                        FileName = file_name;
                        XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
                    }
                }
            }
            else
            {
                if (XFilePath.CheckCorrectFileName(file_name))
                {
                    AvalonTextEditor.Save(file_name);
                    FileName = file_name;
                    XLogger.LogInfoModule(nameof(LotusViewerText), $"Файл с именем: [{FileName}] сохранен");
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
            AvalonTextEditor.Clear();
            FileName = "";
        }
        #endregion

        #region Main methods
        /// <summary>
        /// Обновление статус сворачивания.
        /// </summary>
        private void UpdateFoldings()
        {
            if (_foldingManager == null) return;

            if (_foldingStrategy is BraceFoldingStrategy braceFoldingStrategy)
            {
                braceFoldingStrategy.UpdateFoldings(_foldingManager, AvalonTextEditor.Document);
            }
            if (_foldingStrategy is XmlFoldingStrategy xmlFoldingStrategy)
            {
                xmlFoldingStrategy.UpdateFoldings(_foldingManager, AvalonTextEditor.Document);
            }
        }

        /// <summary>
        /// Смена подсветки синтаксиса.
        /// </summary>
        /// <param name="syntax">Язык подсветки синтаксиса.</param>
        public void ChangedSyntaxHighlighting(string syntax)
        {
            if (AvalonTextEditor.SyntaxHighlighting == null)
            {
                _foldingStrategy = null;
            }
            else
            {
                switch (AvalonTextEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        _foldingStrategy = new XmlFoldingStrategy();
                        AvalonTextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        AvalonTextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(AvalonTextEditor.Options);
                        _foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        AvalonTextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        _foldingStrategy = null;
                        break;
                }
            }
            if (_foldingStrategy != null)
            {
                if (_foldingManager == null)
                {
                    _foldingManager = FoldingManager.Install(AvalonTextEditor.TextArea);
                }
                UpdateFoldings();
            }
            else
            {
                if (_foldingManager != null)
                {
                    FoldingManager.Uninstall(_foldingManager);
                    _foldingManager = null;
                }
            }
        }

        /// <summary>
        /// Смена кодировки.
        /// </summary>
        /// <param name="encoding">Кодировка.</param>
        public void ChangedEncoding(Encoding encoding)
        {
            AvalonTextEditor.Text = FileReader.ReadFileContent(FileName, encoding);
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Загрузка пользовательского элемента.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnUserControl_Loaded(object sender, RoutedEventArgs args)
        {
            SearchPanel.Install(AvalonTextEditor);
        }

        /// <summary>
        /// Прокрутка колеса мыши.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnAvalonTextEditor_PreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            var ctrl = Keyboard.Modifiers == ModifierKeys.Control;
            if (ctrl)
            {
                var increase = args.Delta > 0;
                var currentSize = avalonTextEditor.FontSize;

                if (increase)
                {
                    if (currentSize < FONT_MAX_SIZE)
                    {
                        var newSize = Math.Min(FONT_MAX_SIZE, currentSize + 1);
                        avalonTextEditor.FontSize = newSize;
                    }
                }
                else
                {
                    if (currentSize > FONT_MIN_SIZE)
                    {
                        var newSize = Math.Max(FONT_MIN_SIZE, currentSize - 1);
                        avalonTextEditor.FontSize = newSize;
                    }
                }

                args.Handled = true;
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