using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Semight.Fwm.Common.CommonUILib.CustomBehavior
{
    public class ClearTxtBehavior : Behavior<Button>
    {
        public TextBox Target
        {
            get { return (TextBox)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(TextBox), typeof(ClearTxtBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += ClearFuction;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Click -= ClearFuction;
        }

        private void ClearFuction(object sender, RoutedEventArgs e)
        {
            Target?.Clear();
        }
    }
}