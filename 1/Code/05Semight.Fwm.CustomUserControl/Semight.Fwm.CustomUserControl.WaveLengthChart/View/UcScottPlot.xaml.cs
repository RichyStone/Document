using ScottPlot;
using ScottPlot.TickGenerators;
using Semight.Fwm.Common.CommonFileOperationLib.Excel;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.CustomUserControl.WaveLengthChart.Model;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace Semight.Fwm.CustomUserControl.WaveLengthChart.View
{
    /// <summary>
    /// UcLineChart.xaml 的交互逻辑
    /// </summary>
    public partial class UcScottPlot : UserControl
    {
        #region Fields

        /// <summary>
        /// 当前数据索引
        /// </summary>
        private int curDataIndex = 0;

        /// <summary>
        /// 当前数据索引值
        /// </summary>
        public int CurDataIndex => curDataIndex;

        /// <summary>
        /// 以时间做横坐标标志位
        /// </summary>
        private bool dateTimeXAxis = false;

        /// <summary>
        ///功率线条显示标志位
        /// </summary>
        private bool powerVisible = true;

        /// <summary>
        /// 波长线条显示标志位
        /// </summary>
        private bool waveLengthVisible = true;

        /// <summary>
        /// 秒表
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        private ConcurrentQueue<VisualData> visualDatas = new ConcurrentQueue<VisualData>();

        private readonly ScottPlot.Color powerColor = Color.FromHex("#C82423");
        private readonly ScottPlot.Color waveLengthColor = Color.FromHex("#2F7FC1");

        private ScottPlot.Plottables.Scatter powerScatter;
        private ScottPlot.Plottables.Scatter wavelengthScatter;

        private int viewRange = 10_000;

        #endregion Fields

        #region Constructor

        public UcScottPlot()
        {
            InitializeComponent();
            RegisterEvent();
        }

        #endregion Constructor

        #region Event

        private void RegisterEvent()
        {
            WaveChart.MouseMove -= WaveChart_MouseMove;
            WaveChart.MouseMove += WaveChart_MouseMove;
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaveChart_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var point = e.GetPosition(WaveChart);
            var pos = WaveChart.Plot.GetCoordinates(new Pixel(point.X, point.Y));

            if (!dateTimeXAxis)
            {
                var xIndex = (int)Math.Round(pos.X, 0);
                var countLimit = xIndex + 1;

                CurX.Text = $"X: {xIndex}";
                if (xIndex >= 0 && visualDatas.Count >= countLimit && visualDatas.Any(da => da.Xvalue == xIndex))
                {
                    var data = visualDatas.First(da => da.Xvalue == xIndex);
                    CurWavelength.Text = $"Y-L: {data.LeftYvalue:f7}";
                    CurPower.Text = $"Y-R: {data.RightYvalue:f5}";
                }
            }
            else
            {
                var time = Math.Round(pos.X, 3);
                if (time >= 0 && visualDatas.Any(da => da.TimeValue == time))
                {
                    var data = visualDatas.First(da => da.TimeValue == time);
                    CurX.Text = $"X: {time:f3}";
                    CurWavelength.Text = $"Y-L: {data.LeftYvalue:f7}";
                    CurPower.Text = $"Y-R: {data.RightYvalue:f5}";
                }
            }
        }

        #endregion Event

        #region Method

        /// <summary>
        /// 初始化图表
        /// </summary>
        public void InitialChartView(string title = "WaveLengthChart", string xLbale = "Point", string leftLable = "WaveLength", string rightLable = "Power", int leftDigits = 7, int rightDigits = 5)
        {
            WaveChart.Plot.Title(title);
            WaveChart.Plot.XLabel(xLbale);

            WaveChart.Plot.Axes.Left.Label.Text = leftLable;
            WaveChart.Plot.Axes.Left.Label.ForeColor = waveLengthColor;
            WaveChart.Plot.Axes.Left.TickLabelStyle.ForeColor = waveLengthColor;

            WaveChart.Plot.Axes.Right.Label.Text = rightLable;
            WaveChart.Plot.Axes.Right.Label.ForeColor = powerColor;
            WaveChart.Plot.Axes.Right.TickLabelStyle.ForeColor = powerColor;

            WaveChart.Plot.Axes.Left.TickGenerator = new NumericAutomatic()
            {
                LabelFormatter = x => x.ToString($"f{leftDigits}"),
            };

            WaveChart.Plot.Axes.Right.TickGenerator = new NumericAutomatic()
            {
                LabelFormatter = x => x.ToString($"f{rightDigits}"),
            };

            WaveChart.Plot.Legend.IsVisible = true;
            WaveChart.Refresh();

            stopwatch.Start();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitialData()
        {
            visualDatas.Clear();

            Interlocked.Exchange(ref curDataIndex, 0);
            stopwatch.Restart();
        }

        /// <summary>
        /// 得到新数据处理
        /// </summary>
        /// <param name="leftY"></param>
        public void NewDataHandler(double leftY, bool refresh = false)
        {
            try
            {
                Interlocked.Increment(ref curDataIndex);

                visualDatas.Enqueue(new VisualData
                {
                    Xvalue = CurDataIndex,
                    TimeValue = stopwatch.ElapsedMilliseconds / 1000.0,
                    LeftYvalue = leftY,
                });

                if (refresh)
                    RefreshChartView();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 得到新数据处理
        /// </summary>
        /// <param name="leftY"></param>
        ///       /// <param name="rightY"></param>
        public void NewDataHandler(double leftY, double rightY, bool refresh = false)
        {
            try
            {
                Interlocked.Increment(ref curDataIndex);

                visualDatas.Enqueue(new VisualData
                {
                    Xvalue = CurDataIndex,
                    TimeValue = stopwatch.ElapsedMilliseconds / 1000.0,
                    LeftYvalue = leftY,
                    RightYvalue = rightY,
                });

                if (refresh)
                    RefreshChartView();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 得到新数据处理
        /// </summary>
        /// <param name="collection"></param>
        public void NewDataHandler(IEnumerable<double> collection, bool refresh = false)
        {
            try
            {
                foreach (var item in collection)
                {
                    Interlocked.Increment(ref curDataIndex);

                    visualDatas.Enqueue(new VisualData
                    {
                        Xvalue = CurDataIndex,
                        TimeValue = stopwatch.ElapsedMilliseconds / 1000.0,
                        LeftYvalue = item,
                    });
                }

                if (refresh)
                    RefreshChartView();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 得到新数据处理
        /// </summary>
        /// <param name="collection"></param>
        public void NewDataHandler(IEnumerable<(double, double)> collection, bool refresh = false)
        {
            try
            {
                foreach (var item in collection)
                {
                    Interlocked.Increment(ref curDataIndex);

                    visualDatas.Enqueue(new VisualData
                    {
                        Xvalue = CurDataIndex,
                        TimeValue = stopwatch.ElapsedMilliseconds / 1000.0,
                        LeftYvalue = item.Item1,
                        RightYvalue = item.Item2,
                    });
                }

                if (refresh)
                    RefreshChartView();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 刷新图表
        /// </summary>
        private void RefreshChartView()
        {
            if (visualDatas.Count > 0)
                RefreshChartView(visualDatas);
        }

        /// <summary>
        /// 刷新图表
        /// </summary>
        private void RefreshChartView(IEnumerable<VisualData> visualDatas)
        {
            if (visualDatas.Count() == 0) return;
            WaveChart.Plot.Clear();

            Func<VisualData, double> expression = dateTimeXAxis ? da => da.TimeValue : da => da.Xvalue;
            var xlist = visualDatas.Select(expression).ToList();

            var leftList = visualDatas.Select(da => da.LeftYvalue).ToList();
            var rightList = visualDatas.Select(da => da.RightYvalue).ToList();

            wavelengthScatter = WaveChart.Plot.Add.Scatter(xlist, leftList, color: waveLengthColor);
            powerScatter = WaveChart.Plot.Add.Scatter(xlist, rightList, color: powerColor);

            var xminIndex = xlist.Count > viewRange && viewRange > 0 ? xlist.Count - viewRange : 0;
            WaveChart.Plot.Axes.SetLimitsX(xlist[xminIndex], xlist.Max());

            var minLeft = leftList.Min();
            var maxLeft = leftList.Max();
            WaveChart.Plot.Axes.Left.Min = minLeft - 0.0000002;
            WaveChart.Plot.Axes.Left.Max = maxLeft + 0.0000002;

            var minRight = rightList.Min();
            var maxRight = rightList.Max();
            WaveChart.Plot.Axes.Right.Min = minRight - 0.0002;
            WaveChart.Plot.Axes.Right.Max = maxRight + 0.0002;

            if (wavelengthScatter != null)
            {
                wavelengthScatter.Smooth = true;
                wavelengthScatter.IsVisible = waveLengthVisible;
                wavelengthScatter.Axes.YAxis = WaveChart.Plot.Axes.Left;
            }

            if (powerScatter != null)
            {
                powerScatter.Smooth = true;
                powerScatter.IsVisible = powerVisible;
                powerScatter.Axes.YAxis = WaveChart.Plot.Axes.Right;
            }

            WaveChart.Refresh();
        }

        /// <summary>
        /// 设置线条可见性
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="lineIndex"></param>
        public void SetLineSeriesVisible(bool visible, int lineIndex)
        {
            if (lineIndex == 0)
                waveLengthVisible = visible;
            else if (lineIndex == 1)
                powerVisible = visible;

            RefreshChartView();
        }

        /// <summary>
        /// 更换X轴
        /// </summary>
        /// <param name="dateTimeAxis"></param>
        public void ChangeXAxis(bool dateTimeAxis)
        {
            dateTimeXAxis = dateTimeAxis;
            if (dateTimeAxis)
            {
                WaveChart.Plot.XLabel("Time/sec");
                WaveChart.Plot.Axes.Bottom.TickGenerator = new NumericAutomatic()
                {
                    LabelFormatter = x => x.ToString("f3"),
                };
            }
            else
            {
                WaveChart.Plot.XLabel("Point");
                WaveChart.Plot.Axes.Bottom.TickGenerator = new NumericAutomatic()
                {
                    LabelFormatter = x => x.ToString("N0"),
                };
            }

            RefreshChartView();
        }

        /// <summary>
        /// 更换波长单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messagerTransData"></param>
        public void ChangeWavelengthUnit(string unit)
        {
            WaveChart.Plot.Axes.Left.Label.Text = $"WaveLength/{unit}";
            WaveChart.Refresh();
        }

        /// <summary>
        /// 更换功率单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messagerTransData"></param>
        public void ChangePowerUnit(string unit)
        {
            WaveChart.Plot.Axes.Right.Label.Text = $"Power/{unit}";
            WaveChart.Refresh();
        }

        public void SetViewRange(int range)
        {
            viewRange = range;
            RefreshChartView();
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void ExportData(string path)
        {
            try
            {
                await Task.Run(() =>
                {
                    SaveToLocal(path, visualDatas);
                });
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ErrorBox($"导出失败！");
                LogHelper.LogError("导出数据失败", ex);
            }
        }

        /// <summary>
        /// 保存到本地
        /// </summary>
        private void SaveToLocal(string savePath, IEnumerable<VisualData> visualDatas)
        {
            if (visualDatas.Count() == 0) return;

            try
            {
                var dir = Path.GetDirectoryName(savePath);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var dataTable = new DataTable();
                dataTable.Columns.Add("Number", typeof(int));
                dataTable.Columns.Add("Time", typeof(double));
                dataTable.Columns.Add("WaveLength", typeof(double));
                dataTable.Columns.Add("Power", typeof(double));

                var i = 1;
                foreach (var item in visualDatas)
                {
                    var dataRow = dataTable.Rows.Add();
                    dataRow[0] = i;
                    dataRow[1] = item.TimeValue;
                    dataRow[2] = item.LeftYvalue;
                    dataRow[3] = item.RightYvalue;

                    i++;
                }

                ExcelHelper.Export(dataTable, "Number,ElapsedTime/ms,WaveLength/nm,Power/dBm", savePath, "WaveLengthData");
                dataTable.Dispose();

                MessageBoxHelper.SuccessBox("导出数据成功！");
            }
            catch
            {
                throw;
            }
        }

        #endregion Method
    }
}