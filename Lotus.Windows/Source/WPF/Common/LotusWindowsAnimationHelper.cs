//=====================================================================================================================
// Проект: Модуль платформы Windows
// Раздел: Подсистема работы с WPF
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsAnimationHelper.cs
*		Статический класс для упрощения работы с анимацией.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Windows;
using System.Windows.Media.Animation;
//=====================================================================================================================
namespace Lotus
{
	namespace Windows
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \defgroup WindowsWPF Подсистема работы с WPF
		//! Подсистема работы с WPF содержит код с WPF и сопутствующей инфраструктуры
		//! \ingroup Windows
		//-------------------------------------------------------------------------------------------------------------
		//! \defgroup WindowsWPFCommon Общая подсистема
		//! \ingroup WindowsWPF
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Статический класс для упрощения работы с анимацией
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XAnimationHelper
		{
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запускает анимацию вещественного типа определенного значения на свойства зависимостей
			/// </summary>
			/// <param name="animatable_element">Элемент</param>
			/// <param name="dependency_property">Свойство зависимости</param>
			/// <param name="to_value">Целевое значение</param>
			/// <param name="animation_duration">Продолжительность в секундах</param>
			//---------------------------------------------------------------------------------------------------------
			public static void StartAnimation(UIElement animatable_element, DependencyProperty dependency_property, Double to_value, 
				Double animation_duration)
			{
				StartAnimation(animatable_element, dependency_property, to_value, animation_duration, null);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запускает анимацию вещественного типа определенного значения на свойства зависимостей.
			/// Вы можете передать в обработчик событий для вызова, когда анимация завершена
			/// </summary>
			/// <param name="animatable_element">Элемент</param>
			/// <param name="dependency_property">Свойство зависимости</param>
			/// <param name="to_value">Целевое значение</param>
			/// <param name="animation_duration">Продолжительность в секундах</param>
			/// <param name="completed_handler">Обработчик события окончания анимации</param>
			//---------------------------------------------------------------------------------------------------------
			public static void StartAnimation(UIElement animatable_element, DependencyProperty dependency_property, Double to_value, 
				Double animation_duration, EventHandler completed_handler)
			{
				Double fromValue = (Double)animatable_element.GetValue(dependency_property);

				DoubleAnimation animation = new DoubleAnimation();
				animation.From = fromValue;
				animation.To = to_value;
				animation.Duration = TimeSpan.FromSeconds(animation_duration);

				animation.Completed += delegate(Object sender, EventArgs args)
				{
					//
					// When the animation has completed bake final value of the animation
					// into the property.
					//
					animatable_element.SetValue(dependency_property, animatable_element.GetValue(dependency_property));
					CancelAnimation(animatable_element, dependency_property);

					if (completed_handler != null)
					{
						completed_handler(sender, args);
					}
				};

				animation.Freeze();
				animatable_element.BeginAnimation(dependency_property, animation);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отмена любых анимации, которые работают на свойства зависимостей
			/// </summary>
			/// <param name="animatable_element">Элемент</param>
			/// <param name="dependency_property">Свойство зависимости</param>
			//---------------------------------------------------------------------------------------------------------
			public static void CancelAnimation(UIElement animatable_element, DependencyProperty dependency_property)
			{
				animatable_element.BeginAnimation(dependency_property, null);
			}
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================