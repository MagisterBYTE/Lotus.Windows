using System.Windows;
using System.Windows.Media.Media3D;

using Fluent;

namespace Lotus.Windows
{
    /**
     * \defgroup WindowsViewerContent3DControls Элементы управления
     * \ingroup WindowsViewerContent3D
     * \brief Элементы управления.
     * @{
     */
    /// <summary>
    /// Контекстная вкладка ленты для просмотра свойств и редактирования 3D контента.
    /// </summary>
    public partial class LotusRibbonTabContent3DEditor : RibbonTabItem
    {
        #region Declare DependencyProperty 
        /// <summary>
        /// Основной редактор 3D контента.
        /// </summary>
        public static readonly DependencyProperty Content3DViewEditorProperty = DependencyProperty.Register(nameof(Content3DViewEditor),
            typeof(LotusViewerContent3D),
            typeof(LotusRibbonTabContent3DEditor),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
        #endregion

        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// Основной редактор 3D контента.
        /// </summary>
        public LotusViewerContent3D Content3DViewEditor
        {
            get { return (LotusViewerContent3D)GetValue(Content3DViewEditorProperty); }
            set { SetValue(Content3DViewEditorProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию инициализирует объект класса предустановленными значениями.
        /// </summary>
        public LotusRibbonTabContent3DEditor()
        {
            InitializeComponent();
            SetResourceReference(StyleProperty, typeof(RibbonTabItem));
        }
        #endregion

        #region Main methods
        #endregion

        #region Event handlers 
        /// <summary>
        /// Загрузка вкладки ленты.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRibbonTabContent3DEditor_Loaded(object sender, RoutedEventArgs args)
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Открытие файла.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonOpen_Click(object sender, RoutedEventArgs args)
        {
            if (Content3DViewEditor != null)
            {
                Content3DViewEditor.OpenFile(null!, null);
            }
        }

        /// <summary>
        /// Открытие файла в программе Notepad.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonOpenNotepad_Click(object sender, RoutedEventArgs args)
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Сохранение файла.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonSave_Click(object sender, RoutedEventArgs args)
        {
            if (Content3DViewEditor != null)
            {
                Content3DViewEditor.SaveFile();
            }
        }

        /// <summary>
        /// Сохранение файла под новым именем.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnButtonSaveAs_Click(object sender, RoutedEventArgs args)
        {
            if (Content3DViewEditor != null)
            {
                Content3DViewEditor.SaveAsFile(null!, null);
            }
        }

        /// <summary>
        /// Установка вверх оси Y.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioSetYUp_Checked(object sender, RoutedEventArgs args)
        {
            if (Content3DViewEditor != null)
            {
                Content3DViewEditor.Helix3DViewer.ModelUpDirection = new Vector3D(0, 1, 0);
            }
        }

        /// <summary>
        /// Установка вверх оси Z.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnRadioSetZUp_Checked(object sender, RoutedEventArgs args)
        {
            if (Content3DViewEditor != null)
            {
                Content3DViewEditor.Helix3DViewer.ModelUpDirection = new Vector3D(0, 0, 1);
            }
        }

        /// <summary>
        /// Инверсия по оси X.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void OnCheckBoxInverseX_Checked(object sender, RoutedEventArgs args)
        {
            if (Content3DViewEditor != null)
            {
                //Content3DViewEditor.Helix3DViewer.Inver = new Vector3D(0, 0, 1);
            }
        }
        #endregion
    }
    /**@}*/
}