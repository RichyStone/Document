using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace Semight.Fwm.Common.CommonUILib.CustomBehavior
{
    public class MouseWheelBehavior : Behavior<TextBox>
    {
        public double MaxValue { get; set; }

        public double MinValue { get; set; }

        public double Scale { get; set; } = 0.1;

        public int DecimalPlace { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseWheel += MouseWheelEvent;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseWheel -= MouseWheelEvent;
        }

        private void MouseWheelEvent(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (!double.TryParse(AssociatedObject.Text, out var curValue)) return;

            if (e.Delta > 0)
                curValue += Scale;
            else
                curValue -= Scale;

            if (curValue > MaxValue) curValue = MaxValue;

            if (curValue < MinValue) curValue = MinValue;

            AssociatedObject.Text = curValue.ToString($"F{DecimalPlace}");
        }
    }
}