using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Lotus.Windows
{

    /**
     * \defgroup WindowsWPF Подсистема работы с WPF
     * \ingroup Windows
     * \brief Подсистема работы с WPF содержит код с WPF и сопутствующей инфраструктуры.
     * \defgroup WindowsWPFCommon Общая подсистема
     * \ingroup WindowsWPF
     * \brief Общая подсистема.
     * @{
     */
    /// <summary>
    /// Статический класс для упрощения работы с анимацией.
    /// </summary>
    public static class XAnimationHelper
    {
        /// <summary>
        /// Запускает анимацию вещественного типа определенного значения на свойства зависимостей.
        /// </summary>
        /// <param name="animatableElement">Элемент.</param>
        /// <param name="dependencyProperty">Свойство зависимости.</param>
        /// <param name="toValue">Целевое значение.</param>
        /// <param name="animationDuration">Продолжительность в секундах.</param>
        public static void StartAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, double toValue,
            double animationDuration)
        {
            StartAnimation(animatableElement, dependencyProperty, toValue, animationDuration, null);
        }

        /// <summary>
        /// Запускает анимацию вещественного типа определенного значения на свойства зависимостей.
        /// Вы можете передать в обработчик событий для вызова, когда анимация завершена.
        /// </summary>
        /// <param name="animatableElement">Элемент.</param>
        /// <param name="dependencyProperty">Свойство зависимости.</param>
        /// <param name="toValue">Целевое значение.</param>
        /// <param name="animationDuration">Продолжительность в секундах.</param>
        /// <param name="completedHandler">Обработчик события окончания анимации.</param>
        public static void StartAnimation(UIElement animatableElement, DependencyProperty dependencyProperty, double toValue,
            double animationDuration, EventHandler? completedHandler)
        {
            var fromValue = (double)animatableElement.GetValue(dependencyProperty);

            var animation = new DoubleAnimation
            {
                From = fromValue,
                To = toValue,
                Duration = TimeSpan.FromSeconds(animationDuration)
            };

            animation.Completed += delegate (object? sender, EventArgs args)
            {
                //
                // When the animation has completed bake final value of the animation
                // into the property.
                //
                animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));
                CancelAnimation(animatableElement, dependencyProperty);

                completedHandler?.Invoke(sender, args);
            };

            animation.Freeze();
            animatableElement.BeginAnimation(dependencyProperty, animation);
        }

        /// <summary>
        /// Отмена любых анимации, которые работают на свойства зависимостей.
        /// </summary>
        /// <param name="animatableElement">Элемент.</param>
        /// <param name="dependencyProperty">Свойство зависимости.</param>
        public static void CancelAnimation(UIElement animatableElement, DependencyProperty dependencyProperty)
        {
            animatableElement.BeginAnimation(dependencyProperty, null);
        }
    }
    /**@}*/
}