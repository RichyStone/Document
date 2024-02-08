using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using ScottPlot.AxisPanels;
using Semight.Fwm.Common.CommonFileOperationLib.Excel;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Semight.Fwm.CustomUserControl.WaveLengthChart.View
{
    /// <summary>
    /// UcOxyPlot.xaml 的交互逻辑
    /// </summary>
    public partial class UcOxyPlot : UserControl
    {
        #region Constructor

        public UcOxyPlot()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Fields

        private readonly LineSeries pointSeries1 = new();

        private readonly LineSeries pointSeries2 = new();

        private readonly LineSeries timeSeries1 = new() { IsVisible = false };

        private readonly LineSeries timeSeries2 = new() { IsVisible = false };

        private readonly LinearAxis leftAxis = new() { Position = AxisPosition.Left, Key = "WaveLength" };

        private readonly LinearAxis rightAxis = new() { Position = AxisPosition.Right, Key = "Power" };

        private readonly LinearAxis bottomAxis = new() { Position = AxisPosition.Bottom, Key = "Point" };

        private readonly LinearAxis timeAxis = new()
        {
            Position = AxisPosition.Bottom,
            IsAxisVisible = false,
            Key = "Time",
            Title = "Time/sec",
        };

        private readonly OxyColor waveLengthColor = OxyColor.Parse("#2F7FC1");
        private readonly OxyColor powerColor = OxyColor.Parse("#C82423");

        /// <summary>
        /// 当前数据索引
        /// </summary>
        private int curDataIndex = 0;

        /// <summary>
        /// 当前数据索引值
        /// </summary>
        public int CurDataIndex => curDataIndex;

        /// <summary>
        /// 最大容量
        /// </summary>
        private const int maxLength = 300_000;

        private int viewRange = 10_000;

        /// <summary>
        /// 以时间做横坐标标志位
        /// </summary>
        private bool dateTimeXAxis = false;

        /// <summary>
        /// 秒表
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        #endregion Fields

        #region Method

        /// <summary>
        /// 初始化图表
        /// </summary>
        public void InitialChartView(string title = "WaveLengthChart", string xLbale = "Point", string leftLable = "WaveLength", string rightLable = "Power")
        {
            var plotModel = new PlotModel();

            leftAxis.Title = leftLable;
            leftAxis.TitleColor = waveLengthColor;
            leftAxis.TextColor = waveLengthColor;

            rightAxis.Title = rightLable;
            rightAxis.TitleColor = powerColor;
            rightAxis.TextColor = powerColor;

            bottomAxis.Title = xLbale;

            plotModel.Axes.Add(leftAxis);
            plotModel.Axes.Add(rightAxis);
            plotModel.Axes.Add(bottomAxis);
            plotModel.Axes.Add(timeAxis);

            plotModel.Title = title;

            var legend = new Legend
            {
                LegendPosition = LegendPosition.RightTop,
                LegendPlacement = LegendPlacement.Inside,
                ShowInvisibleSeries = false
            };

            plotModel.Legends.Add(legend);

            pointSeries1.Title = timeSeries1.Title = leftLable;
            pointSeries2.Title = timeSeries2.Title = rightLable;

            pointSeries1.YAxisKey = timeSeries1.YAxisKey = leftAxis.Key;
            pointSeries1.XAxisKey = bottomAxis.Key;
            timeSeries1.XAxisKey = timeAxis.Key;
            pointSeries1.Color = timeSeries1.Color = waveLengthColor;
            pointSeries1.TextColor = timeSeries1.TextColor = waveLengthColor;

            pointSeries2.YAxisKey = timeSeries2.YAxisKey = rightAxis.Key;
            pointSeries2.XAxisKey = bottomAxis.Key;
            timeSeries2.XAxisKey = timeAxis.Key;
            pointSeries2.Color = timeSeries2.Color = powerColor;
            pointSeries2.TextColor = timeSeries2.TextColor = powerColor;

            plotModel.Series.Clear();
            plotModel.Series.Add(pointSeries1);
            plotModel.Series.Add(pointSeries2);
            plotModel.Series.Add(timeSeries1);
            plotModel.Series.Add(timeSeries2);

            plotModel.PlotType = PlotType.XY;
            ChartView.Model = plotModel;

            stopwatch.Start();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitialData()
        {
            pointSeries1.Points.Clear();
            pointSeries2.Points.Clear();
            timeSeries1.Points.Clear();
            timeSeries2.Points.Clear();
            Interlocked.Exchange(ref curDataIndex, 0);
            stopwatch.Restart();
        }

        /// <summary>
        /// 得到新数据处理
        /// </summary>
        /// <param name="power"></param>
        /// <param name="wavelength"></param>
        public void NewDataHandler(double power, double wavelength, bool refresh = false)
        {
            try
            {
                if (CurDataIndex >= maxLength - 1)
                    throw new IndexOutOfRangeException("测量数量超出界限！");
                Interlocked.Increment(ref curDataIndex);
                pointSeries1.Points.Add(new DataPoint(curDataIndex, wavelength));
                pointSeries2.Points.Add(new DataPoint(curDataIndex, power));

                var time = stopwatch.Elapsed.TotalSeconds;
                timeSeries1.Points.Add(new DataPoint(time, wavelength));
                timeSeries2.Points.Add(new DataPoint(time, power));

                if (refresh && pointSeries1.Points.Count > 0 && timeSeries1.Points.Count > 0)
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
            var timeMinIndex = timeSeries1.Points.Count > viewRange && viewRange > 0 ? timeSeries1.Points.Count - viewRange : 0;
            var minTime = timeSeries1.Points[timeMinIndex].X;
            var maxTime = timeSeries1.Points.Last().X;

            var minIndex = pointSeries1.Points.Count > viewRange && viewRange > 0 ? pointSeries1.Points.Count - viewRange : 0;
            var minX = pointSeries1.Points[minIndex].X;
            var maxX = pointSeries1.Points.Last().X;

            var series1 = dateTimeXAxis ? timeSeries1 : pointSeries1;
            var series2 = dateTimeXAxis ? timeSeries2 : pointSeries2;

            var minWavelength = series1.Points.Min(p => p.Y);
            var maxWavelength = series1.Points.Max(p => p.Y);

            var minPower = series2.Points.Min(p => p.Y);
            var maxPower = series2.Points.Max(p => p.Y);

            bottomAxis.Minimum = minX;
            bottomAxis.Maximum = maxX;

            timeAxis.Minimum = minTime;
            timeAxis.Maximum = maxTime;

            leftAxis.Minimum = minWavelength - 0.0000002;
            leftAxis.Maximum = maxWavelength + 0.0000002;

            rightAxis.Minimum = minPower - 0.0002;
            rightAxis.Maximum = maxPower + 0.0002;

            bottomAxis.IsAxisVisible = !dateTimeXAxis;
            timeAxis.IsAxisVisible = dateTimeXAxis;

            ChartView.Model.ResetAllAxes();
            ChartView.Model.InvalidatePlot(true);
        }

        /// <summary>
        /// 设置线条可见性
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="lineIndex"></param>
        public void SetLineSeriesVisible(bool visible, int lineIndex)
        {
            if (dateTimeXAxis)
            {
                if (lineIndex == 0)
                    timeSeries1.IsVisible = visible;
                else if (lineIndex == 1)
                    timeSeries2.IsVisible = visible;
            }
            else
            {
                if (lineIndex == 0)
                    pointSeries1.IsVisible = visible;
                else if (lineIndex == 1)
                    pointSeries2.IsVisible = visible;
            }
            ChartView.InvalidatePlot();
        }

        /// <summary>
        /// 更换X轴
        /// </summary>
        /// <param name="dateTimeAxis"></param>
        public void ChangeXAxis(bool dateTimeAxis)
        {
            dateTimeXAxis = dateTimeAxis;

            timeAxis.IsAxisVisible = dateTimeAxis;
            bottomAxis.IsAxisVisible = !dateTimeAxis;

            ChartView.InvalidatePlot();
        }

        /// <summary>
        /// 更换波长单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messagerTransData"></param>
        public void ChangeWavelengthUnit(string unit)
        {
            leftAxis.Title = $"WaveLength/{unit}";
            ChartView.InvalidatePlot();
        }

        /// <summary>
        /// 更换功率单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messagerTransData"></param>
        public void ChangePowerUnit(string unit)
        {
            rightAxis.Title = $"Power/{unit}";
            ChartView.InvalidatePlot();
        }

        public void SetViewRange(int range)
        {
            viewRange = range;

            if (pointSeries1.Points.Count > 0 && timeSeries1.Points.Count > 0)
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
                var waveArray = pointSeries1.Points.Select(p => p.Y).ToArray();
                var powerArray = pointSeries2.Points.Select(p => p.Y).ToArray();
                var timeArray = timeSeries1.Points.Select(p => p.X).ToArray();
                await Task.Run(() =>
                {
                    SaveToLocal(path, waveArray, powerArray, timeArray);
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
        private void SaveToLocal(string savePath, double[] waveLengtData, double[] powerData, double[] timeData)
        {
            if (waveLengtData.Length == 0 || powerData.Length == 0 || timeData.Length == 0) return;

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

                for (int i = 0; i < timeData.Length; i++)
                {
                    var dataRow = dataTable.Rows.Add();
                    dataRow[0] = i;
                    dataRow[1] = timeData[i];
                    dataRow[2] = waveLengtData[i];
                    dataRow[3] = powerData[i];
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