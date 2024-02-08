using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Media;

namespace Semight.Fwm.Common.CommonUILib.CustomBehavior
{
    public class BorderBehavior : Behavior<Border>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseEnter += (d, e) => { AssociatedObject.Background = Brushes.Blue; };
            AssociatedObject.MouseLeave += (d, e) => { AssociatedObject.Background = Brushes.Red; };
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseEnter -= (d, e) => { AssociatedObject.Background = Brushes.Blue; };
            AssociatedObject.MouseLeave -= (d, e) => { AssociatedObject.Background = Brushes.Red; };
        }
    }
}