﻿//=====================================================================================================================
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
using Lotus.Object3D;
using Material = Lotus.Object3D.Material;
using Model3D = Lotus.Object3D.Model3D;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		/** \addtogroup WindowsViewerContent3DView
		*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Селектор шаблона данных для отображения иерархии элементов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CEntity3DDataSelector : DataTemplateSelector
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
			/// Шаблон для представления меша
			/// </summary>
			public DataTemplate Mesh { get; set; }

			/// <summary>
			/// Шаблон для набора мешей
			/// </summary>
			public DataTemplate MeshSet { get; set; }

			/// <summary>
			/// Шаблон для представления материала
			/// </summary>
			public DataTemplate Material { get; set; }

			/// <summary>
			/// Шаблон для представления текстурного слота
			/// </summary>
			public DataTemplate TextureSlot { get; set; }

			/// <summary>
			/// Шаблон для представления набора материалов
			/// </summary>
			public DataTemplate MaterialSet { get; set; }
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
				var scene = item as Scene3D;
				if (scene != null)
				{
					return Scene;
				}

				var node = item as Node3D;
				if (node != null)
				{
					return Node;
				}

				var model = item as Model3D;
				if (model != null)
				{
					return Model;
				}

				var mesh = item as Mesh3Df;
				if (mesh != null)
				{
					return Mesh;
				}

				var mesh_set = item as MeshSet;
				if (mesh_set != null)
				{
					return MeshSet;
				}

				var material = item as Material;
				if (material != null)
				{
					return Material;
				}

				var texture_slot = item as TextureSlot;
				if (texture_slot != null)
				{
					return TextureSlot;
				}

				var material_set = item as MaterialSet;
				if (material_set != null)
				{
					return MaterialSet;
				}

				return Scene;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/**@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================