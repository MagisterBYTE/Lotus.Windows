using Lotus.Core.Inspector;

namespace Lotus.Windows
{

    /** \addtogroup WindowsCommonManagers
	*@{*/
    /// <summary>
    /// Центральный менеджер основной формы приложения.
    /// </summary>
    public static class XWindowManager
    {
        #region Fields
        /// <summary>
        /// Инспектор свойств.
        /// </summary>
#pragma warning disable S2223 // Non-constant static fields should not be visible
        public static ILotusPropertyInspector PropertyInspector;
#pragma warning restore S2223 // Non-constant static fields should not be visible
        #endregion
    }
    /**@}*/
}