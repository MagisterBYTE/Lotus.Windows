using System.Windows.Documents;

namespace Lotus.Windows
{
    /** \addtogroup WindowsWPFExtension
	*@{*/
    /// <summary>
    /// Статический класс реализующий методы расширения для типа <see cref="FlowDocument"/>.
    /// </summary>
    public static class XWindowsFlowDocumentExtension
    {
        /// <summary>
        /// Установить текст для указанного элемента Run.
        /// </summary>
        /// <param name="this">Документ.</param>
        /// <param name="run_name">Имя элемента Run.</param>
        /// <param name="text">Текст.</param>
        public static void SetTextForRunName(this FlowDocument @this, string run_name, string text)
        {
            var run = @this.FindName(run_name) as Run;
            if (run != null)
            {
                run.Text = text;
            }
        }
    }
    /**@}*/
}