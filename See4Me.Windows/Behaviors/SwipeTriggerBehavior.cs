using System;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using See4Me.Common;

namespace See4Me.Behaviors
{
    [ContentProperty(Name = "Actions")]
    public class SwipeTriggerBehavior : Behavior<UIElement>
    {
        /// <summary>
        /// Get/Sets the direction of the Swipe gesture 
        /// </summary>
        public SwipeDirection Direction { get; set; }

        #region Actions Dependency Property

        /// <summary> 
        /// Actions collection 
        /// </summary> 
        public ActionCollection Actions
        {
            get
            {
                var actions = (ActionCollection)base.GetValue(ActionsProperty);
                if (actions == null)
                {
                    actions = new ActionCollection();
                    base.SetValue(ActionsProperty, actions);
                }
                return actions;
            }
        }

        /// <summary> 
        /// Backing storage for Actions collection 
        /// </summary> 
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register("Actions",
                                        typeof(ActionCollection),
                                        typeof(SwipeTriggerBehavior),
                                        new PropertyMetadata(null));

        #endregion Actions Dependency Property

        protected void Execute(object sender, object parameter)
        {
            Interaction.ExecuteActions(sender, this.Actions, parameter);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.ManipulationMode = this.AssociatedObject.ManipulationMode | ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            this.AssociatedObject.ManipulationCompleted += OnManipulationCompleted;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.ManipulationCompleted -= OnManipulationCompleted;
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var isRight = e.Velocities.Linear.X.Between(0.3, 100);
            var isLeft = e.Velocities.Linear.X.Between(-100, -0.3);

            var isUp = e.Velocities.Linear.Y.Between(-100, -0.3);
            var isDown = e.Velocities.Linear.Y.Between(0.3, 100);

            switch (this.Direction)
            {
                case SwipeDirection.Left:
                    if (isLeft && !(isUp || isDown))
                        this.Execute(this.AssociatedObject, null);

                    break;

                case SwipeDirection.Right:
                    if (isRight && !(isUp || isDown))
                        this.Execute(this.AssociatedObject, null);

                    break;
                case SwipeDirection.Up:
                    if (isUp && !(isRight || isLeft))
                        this.Execute(this.AssociatedObject, null);

                    break;
                case SwipeDirection.Down:
                    if (isDown && !(isRight || isLeft))
                        this.Execute(this.AssociatedObject, null);

                    break;
                case SwipeDirection.LeftDown:
                    if (isLeft && isDown)
                        this.Execute(this.AssociatedObject, null);

                    break;
                case SwipeDirection.LeftUp:
                    if (isLeft && isUp)
                        this.Execute(this.AssociatedObject, null);

                    break;
                case SwipeDirection.RightDown:
                    if (isRight && isDown)
                        this.Execute(this.AssociatedObject, null);

                    break;
                case SwipeDirection.RightUp:
                    if (isRight && isUp)
                        this.Execute(this.AssociatedObject, null);

                    break;
            }
        }
    }

    public static class RangeExtensions
    {
        public static bool Between<T>(this T value, T from, T to) where T : IComparable<T>
            => value.CompareTo(from) >= 0 && value.CompareTo(to) <= 0;
    }
}
