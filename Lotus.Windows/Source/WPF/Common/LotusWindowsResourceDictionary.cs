//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsResourceDictionary.cs
*		Кэшированный словарь ресурсов.
*		Реализация кэшированного словаря ресурсов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsWPFCommon
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Кэшированный словарь ресурсов
		/// </summary>
		/// <remarks>
		/// Общий словарь ресурс является специализированным словарь ресурс, который загружает это содержимое 
		/// только один раз.Если второй экземпляр с тем же источником создан, он только объединяет ресурсы из кэша
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public class SharedResourceDictionary : ResourceDictionary
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			/// <summary>
			/// Внутренний кэш загруженных словарей
			/// </summary>
			private static Dictionary<Uri, ResourceDictionary> mSharedDictionaries = new Dictionary<Uri, ResourceDictionary>();

			/// <summary>
			/// Значение, указывающее, является ли приложение в режиме разработки
			/// </summary>
			private static Boolean mIsInDesignerMode;
			#endregion

			#region ======================================= СТАТИЧЕСКИЕ СВОЙСТВА ======================================
			/// <summary>
			/// Общий кэш загруженных словарей
			/// </summary>
			public static Dictionary<Uri, ResourceDictionary> SharedDictionaries
			{
				get { return mSharedDictionaries; }
			}
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			/// <summary>
			/// Локальные данные исходного URI
			/// </summary>
			private Uri mSourceUri;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Получает или задает идентификатор Uniform Resource (URI), для загрузки ресурсов
			/// </summary>
			public new Uri Source
			{
				get { return (mSourceUri); }
				set
				{
					mSourceUri = value;

					// Always load the dictionary by default in designer mode.
					if (!mSharedDictionaries.ContainsKey(value) || mIsInDesignerMode)
					{
						// If the dictionary is not yet loaded, load it by setting
						// the source of the base class
						base.Source = value;

						// add it to the cache if we're not in designer mode
						if (!mIsInDesignerMode)
						{
							mSharedDictionaries.Add(value, this);
						}
					}
					else
					{
						// If the dictionary is already loaded, get it from the cache
						MergedDictionaries.Add(mSharedDictionaries[value]);
					}
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Статический конструктор
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			static SharedResourceDictionary()
			{
				mIsInDesignerMode = (Boolean)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================