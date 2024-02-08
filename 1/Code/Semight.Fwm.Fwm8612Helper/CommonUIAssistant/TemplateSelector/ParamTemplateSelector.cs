using System.Windows;
using System.Windows.Controls;

namespace Semight.Fwm.Fwm8612Helper.CommonUIAssistant.TemplateSelector
{
    public class ParamTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var template = new DataTemplate();
            if (container is not FrameworkElement element || item == null) return template;

            return template ?? new DataTemplate();
        }

        private DataTemplate GetCorrectTemplate(FrameworkElement element, string key)
        {
            if (element.FindResource(key) is DataTemplate template)
                return template;
            else
                return new DataTemplate();
        }
    }
}