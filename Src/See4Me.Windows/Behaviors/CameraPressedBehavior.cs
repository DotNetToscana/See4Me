using Microsoft.Xaml.Interactivity;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace See4Me.Behaviors
{
    /// <summary>
    /// A behavior that listens for a specified event on its source and executes its actions when that event is fired.
    /// </summary>
    [ContentProperty(Name = "Actions")]
    public sealed class CameraPressedBehavior : Behavior
    {
        /// <summary>
        /// Identifies the <seealso cref="Actions"/> dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
            "Actions",
            typeof(ActionCollection),
            typeof(CameraPressedBehavior),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets the collection of actions associated with the behavior. This is a dependency property.
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                var actionCollection = (ActionCollection)this.GetValue(ActionsProperty);
                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();
                    this.SetValue(ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        [PlatformSpecific]
        private readonly bool isTypePresent;

        public CameraPressedBehavior()
        {
            isTypePresent = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
        }

        /// <summary>
        /// Called after the behavior is attached to the <see cref="Microsoft.Xaml.Interactivity.Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (isTypePresent)
                HardwareButtons.CameraPressed += HardwareButtons_CameraPressed;
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="Microsoft.Xaml.Interactivity.Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (isTypePresent)
                HardwareButtons.CameraPressed -= HardwareButtons_CameraPressed;
        }

        private void HardwareButtons_CameraPressed(object sender, CameraEventArgs e)
        {
            this.OnEvent(this.AssociatedObject, null);
        }

        private void OnEvent(object sender, object eventArgs)
        {
            Interaction.ExecuteActions(this, this.Actions, eventArgs);
        }
    }
}
