using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace Semight.Fwm.Common.CommonUILib.CustomUserControl.LogView
{
    /// <summary>
    /// UcLogViewer.xaml 的交互逻辑
    /// </summary>
    public partial class UCLogViewer : UserControl
    {
        public UCLogViewer()
        {
            InitializeComponent();
            RTB_Logger.Document.Blocks.Clear();
        }

        public void AddLog(string msg, LoggerType type)
        {
            try
            {
                _ = Dispatcher.Invoke(DispatcherPriority.Normal, () =>
                {
                    string t = string.Format("{0:G}", DateTime.Now);
                    var run = new Run();
                    switch (type)
                    {
                        case LoggerType.Info:
                            run.Text = t + " 信息：" + msg;
                            run.Foreground = Brushes.Black;
                            break;

                        case LoggerType.Success:
                            run.Text = t + " 成功：" + msg;
                            run.Foreground = Brushes.Green;
                            break;

                        case LoggerType.Warning:
                            run.Text = t + " 警告：" + msg;
                            run.Foreground = Brushes.BlueViolet;
                            break;

                        case LoggerType.Error:
                            run.Text = t + " 错误：" + msg;
                            run.Foreground = Brushes.OrangeRed;
                            break;

                        default:
                            break;
                    }

                    var paragraph = new Paragraph(run) { LineHeight = 2 };
                    RTB_Logger.Document.Blocks.Add(paragraph);

                    // 滚动至最后行
                    RTB_Logger.ScrollToEnd();

                    // 删除
                    if (RTB_Logger.Document.Blocks.Count > 1000)
                    {
                        _ = RTB_Logger.Document.Blocks.Remove(RTB_Logger.Document.Blocks.FirstBlock);
                    }
                });
            }
            catch
            {
                throw;
            }
        }
    }
}