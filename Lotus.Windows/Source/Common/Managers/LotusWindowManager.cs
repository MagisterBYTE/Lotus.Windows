﻿//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Общая подсистема
// Подраздел: Подсистема центральных менеджеров
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowManager.cs
*		Центральный менеджер основной формы приложения.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Core.Inspector;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsCommonManagers
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Центральный менеджер основной формы приложения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XWindowManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			/// <summary>
			/// Инспектор свойств
			/// </summary>
#pragma warning disable S2223 // Non-constant static fields should not be visible
			public static ILotusPropertyInspector PropertyInspector;
#pragma warning restore S2223 // Non-constant static fields should not be visible
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================