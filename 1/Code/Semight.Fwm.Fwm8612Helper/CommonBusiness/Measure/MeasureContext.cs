#define  SelfTest

using Semight.Fwm.Common.CommonModels.Classes.WaveLength;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.WaveLengthCalc;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Semight.Fwm.Fwm8612Helper.CommonBusiness.Measure
{
    public static class MeasureContext
    {
        #region Fields

        /// <summary>
        /// 下位机交互接口
        /// </summary>
        public static readonly FWM8612Context WaveContext = FWM8612Context.GetInstance();

        /// <summary>
        /// 连接状态
        /// </summary>
        public static bool Connected => WaveContext.Connected;

        /// <summary>
        /// 测量状态
        /// </summary>
        public static bool IsMesuring => measureTokenSource != null && !measureTokenSource.IsCancellationRequested;

        /// <summary>
        /// 测量暂停标志位
        /// </summary>
        private static bool measurePause = false;

        #region CallBack

        /// <summary>
        /// 失败回调
        /// </summary>
        public static Action<string>? ErrorAction { get; set; }

        /// <summary>
        /// 波长显示回调
        /// </summary>
        public static Action<WaveLengthValue>? WaveLengthVisualAction { get; set; }

        /// <summary>
        /// 原始数据折线图回调
        /// </summary>
        public static Action<List<int[]>>? CCDDataVisualAction { get; set; }

        #endregion CallBack

        #endregion Fields

        #region Methods

        #region 连接

        public static bool Connect() => WaveContext.Connect();

        /// <summary>
        /// 初始化下位机
        /// </summary>
        /// <param name="api"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool Connect(string ip, int port) => WaveContext.Connect(ip, port);

        /// <summary>
        /// 断连下位机
        /// </summary>
        /// <returns></returns>
        public static bool DisConnect() => WaveContext.DisConnect();

        #endregion 连接

        #region 测量

        /// <summary>
        /// 测量波长令牌
        /// </summary>
        private static CancellationTokenSource? measureTokenSource;

        /// <summary>
        /// 开始测量波长
        /// </summary>
        public static bool StartMeasure()
        {
            try
            {
#if SelfTest
                goto SelfTestFlag;
#endif

                if (!Connected)
                {
                    App.Current.Dispatcher.Invoke(() => ErrorAction?.Invoke("未连接下位机或连接失败！"));
                    return false;
                }

                if (!WaveContext.GetParamValidity())
                {
                    App.Current.Dispatcher.Invoke(() => ErrorAction?.Invoke($"校准数据有效性验证失败！"));
                    return false;
                }

                WaveContext.FPGA_SetExpoTimeAuto();
                Thread.Sleep(1000);

            SelfTestFlag:
                measureTokenSource?.Dispose();
                measureTokenSource = new CancellationTokenSource();

                new Thread(() =>
                {
                    while (!measureTokenSource.IsCancellationRequested)
                    {
                        try
                        {
#if !SelfTest

                            if (!Connected)
                            {
                                Thread.Sleep(300);
                                continue;
                            }
#endif

                            if (measurePause)
                            {
                                Thread.Sleep(300);
                                continue;
                            }

                            var waveLengthValue = new WaveLengthValue();
                            if (GlobalConfig.SoftwareSetting.LowerComputerCalculate)
                            {
                                if (measurePause) continue;

                                var res = WaveContext.GetWaveLengthFromLower();
                                waveLengthValue.Final = res.Item2;
                                waveLengthValue.Power = res.Item1;
                            }
                            else
                            {
                                if (measurePause) continue;
#if SelfTest
                                waveLengthValue = GetSimulateWaveLength(GlobalConfig.Average);
#else
                                waveLengthValue = GetCurWaveLengthAndPower(GlobalConfig.Average);
#endif
                            }

                            WaveLengthVisualAction?.Invoke(waveLengthValue);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError("测量波长错误", ex);
                            if (!measurePause)
                            {
                                StopMeasure();
                                App.Current.Dispatcher.Invoke(() => ErrorAction?.Invoke($"测量出现错误：{ex.Message}！"));
                            }
                        }
                        finally
                        {
                            Thread.Sleep(1);
                        }
                    }
                })
                { IsBackground = true }.Start();

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("开始测量失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 暂停测量
        /// </summary>
        public static void PauseMeasure()
        {
            measurePause = true;
        }

        /// <summary>
        /// 重启测量
        /// </summary>
        public static void RestartMeasure()
        {
            measurePause = false;
        }

        /// <summary>
        /// 停止测量
        /// </summary>
        public static bool StopMeasure()
        {
            try
            {
                measureTokenSource?.Cancel();

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("停止测量失败", ex);
                return false;
            }
        }

        #endregion 测量

        #region 波长

        private static List<int[]> GetCavityDataFromLower() => FWM8612Context.GetInstance().GetCavityDataFromLower();

        /// <summary>
        /// 获取模拟波长
        /// </summary>
        /// <param name="compensate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static WaveLengthValue GetSimulateWaveLength(int average = 20)
        {
            try
            {
                var random = new Random();

                var waveList = new List<WaveLengthValue>();

                for (int i = 0; i < average; i++)
                {
                    List<int[]> dataList = new List<int[]>();

                    var cavity1 = new int[512];
                    var cavity2 = new int[512];
                    for (int j = 0; j < cavity1.Length; j++)
                    {
                        cavity1[j] = random.Next(1000, 65535);
                        cavity2[j] = random.Next(1000, 65535);
                    }
                    dataList.Add(cavity1);
                    dataList.Add(cavity2);
                    dataList.Add(new int[] { random.Next(3290, 3310), random.Next(5000, 8000), random.Next(5000, 8000), 0 });
                    //CCDDataVisualAction?.Invoke(dataList);

                    WaveLengthValue waveLengthValue = new WaveLengthValue
                    {
                        Power = -11 + random.NextDouble(),
                        Step1 = 1550 + random.NextDouble(),
                        Step2 = 1550 + random.NextDouble(),
                        Final = 1550 + random.NextDouble(),
                        Interference = 1000 + random.NextDouble(),
                        Ref = 1225 + random.NextDouble(),
                    };

                    if (i == 18)
                        return new WaveLengthValue { OverExposed = true, Power = -11 + random.NextDouble(), };

                    waveList.Add(waveLengthValue);

                    Thread.Sleep(4);
                }

                return new WaveLengthValue
                {
                    Step1 = waveList.Select(w => w.Step1).Average(),
                    Step2 = waveList.Select(w => w.Step2).Average(),
                    Final = waveList.Select(w => w.Final).Average(),
                    Interference = waveList.Select(w => w.Interference).Average(),
                    Ref = waveList.Select(w => w.Ref).Average(),
                    Power = waveList.Select(w => w.Power).Average(),
                };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取当前功率和波长
        /// </summary>
        /// <param name="compensate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static WaveLengthValue GetCurWaveLengthAndPower(int average = 20)
        {
            try
            {
                var waveList = new List<WaveLengthValue>();
                for (int i = 0; i < average; i++)
                {
                    var dataList = GetCavityDataFromLower();

                    if (dataList == null || dataList.Count < 3)
                        throw new Exception("CCD Data was Wrong!");

                    CCDDataVisualAction?.Invoke(dataList);

                    var waveLengthValue = WaveLengthCalculator.CalcWaveLengthAndPower(dataList[0], dataList[1], dataList[2], (true, true));
                    waveList.Add(waveLengthValue);

                    if (waveLengthValue.OverExposed)
                        break;
                }

                return new WaveLengthValue
                {
                    Step1 = waveList.Select(w => w.Step1).Average(),
                    Step2 = waveList.Select(w => w.Step2).Average(),
                    Final = waveList.Select(w => w.Final).Average(),
                    Interference = waveList.Select(w => w.Interference).Average(),
                    Ref = waveList.Select(w => w.Ref).Average(),
                    Diff = waveList.Select(w => w.Diff).Average(),
                    Power = waveList.Select(w => w.Power).Average(),
                };
            }
            catch
            {
                throw;
            }
        }

        #endregion 波长

        #endregion Methods
    }
}