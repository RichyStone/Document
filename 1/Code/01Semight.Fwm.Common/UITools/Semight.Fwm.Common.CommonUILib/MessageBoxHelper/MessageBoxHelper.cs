using System.Windows;

namespace Semight.Fwm.Common.CommonUILib.MessageBoxHelper
{
    public static class MessageBoxHelper
    {
        /// <summary>
        /// 成功消息盒
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        public static void SuccessBox(string message, string caption = "Success")
        {
            MessageBox.Show($"{message}", caption, MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        /// <summary>
        /// 错误消息盒
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        public static void ErrorBox(string message, string caption = "Error")
        {
            MessageBox.Show($"{message}", caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 信息消息盒
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        public static void InfoBox(string message, string caption = "Information")
        {
            MessageBox.Show($"{message}", caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 警告信息盒
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        public static void WarningBox(string message, string caption = "Warning")
        {
            MessageBox.Show($"{message}", caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// 疑问消息盒
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static MessageBoxResult QuestionBox(string message, string caption = "Question")
        {
            return MessageBox.Show($"{message}", caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}