//=====================================================================================================================
// Проект: Модуль для отображения 3D контента
// Раздел: Представление 3D контента
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusObject3DDataSelector.cs
*		Селекторы для выбора модели отображения данных.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.04.2023
//=====================================================================================================================
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
//---------------------------------------------------------------------------------------------------------------------
using Helix = HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Animations;
using HelixToolkit.SharpDX.Core.Assimp;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Object3D;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/**
         * \defgroup WindowsViewerContent3DView Представление 3D контента
         * \ingroup WindowsViewerContent3D
         * \brief Элементы и конструкции для отображения 3D контента.
         * @{
         */
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Селектор шаблона данных для отображения иерархии элементов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CHelixToolkitDataSelector : DataTemplateSelector
		{
			#region ======================================= ДАННЫЕ ====================================================
			/// <summary>
			/// Шаблон для представления сцены
			/// </summary>
			public DataTemplate Scene { get; set; }

			/// <summary>
			/// Шаблон для представления узла
			/// </summary>
			public DataTemplate Node { get; set; }

			/// <summary>
			/// Шаблон для представления модели
			/// </summary>
			public DataTemplate Model { get; set; }

			/// <summary>
			/// Шаблон для представления неизвестного узла
			/// </summary>
			public DataTemplate Unknow { get; set; }
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
				var node = item as GroupNode;
				if (node != null)
				{
					return Node;
				}

				var model = item as MeshNode;
				if (model != null)
				{
					return Model;
				}

				var scene = item as SceneNode;
				if (scene != null)
				{
					return Scene;
				}

				return Unknow;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================