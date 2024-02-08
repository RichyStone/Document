using Semight.Fwm.Common.CommonModels.Classes.HardWare;
using Semight.Fwm.Common.CommonModels.Classes.Param;
using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.Common.CommonModels.Interface.Connect.ConnectionWay;
using Semight.Fwm.HardWare.FWM8612InteractionLib;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Semight.Fwm.HardWare.HardwarePlatform.FWM8612
{
    public sealed class FWM8612Context : HardWareBase, INetConnect, IUSBConnect
    {
        #region Fields

        /// <summary>
        /// 下位机交互接口
        /// </summary>
        public readonly FWM8612_API WaveAPI = new FWM8612_API();

        /// <summary>
        /// 连接状态
        /// </summary>
        public override bool Connected => WaveAPI.Connected;

        public bool HeatCondition { get; set; }

        public override string InstrumentName => "FWM8612";

        public string HardwareSN { get; private set; }

        public IDNInfo IDNInfo { get; private set; }

        #endregion Fields

        #region Instance

        private static readonly Lazy<FWM8612Context> _instance = new Lazy<FWM8612Context>(() => new FWM8612Context());

        public static FWM8612Context GetInstance() => _instance.Value;

        private FWM8612Context()
        {
        }

        #endregion Instance

        #region Methods

        #region 连接

        public bool Connect()
        {
            try
            {
                if (!WaveAPI.Connect())
                    return false;

                return Initial();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 初始化下位机
        /// </summary>
        /// <param name="api"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port)
        {
            try
            {
                if (!WaveAPI.Connect(ip, port))
                    return false;

                return Initial();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initial()
        {
            try
            {
                if (!Connected)
                    return false;

                var hardwareOk = WaveAPI.MacDefault();
                IDNInfo = QueryIDN();
                HardwareSN = IDNInfo.InstrumentSN;
                return hardwareOk;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 断连下位机
        /// </summary>
        /// <returns></returns>
        public bool DisConnect()
        {
            try
            {
                if (Connected)
                    WaveAPI?.Close();

                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <returns></returns>
        public override bool HeartBeat()
        {
            try
            {
                return Task.Run(async () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GetAverageCount();
                        await Task.Delay(300);
                    }

                    return true;
                }).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        #endregion 连接

        #region 设定

        #region 仪器IDN

        public IDNInfo QueryIDN() => WaveAPI.QueryIDN();

        #endregion 仪器IDN

        #region 平均数

        public int GetAverageCount() => WaveAPI.GetCalculatingAverage();

        public void SetAverageCount(int average) => WaveAPI.SetCalculatingAverage(average);

        #endregion 平均数

        #region 获取TCM温度

        public string GetTCM_Temperature(int index) => WaveAPI.GetTCM_Temperature(index);

        public bool GetTCM_HeaterState() => WaveAPI.GetTCM_HeaterState();

        #endregion 获取TCM温度

        #region 波长和功率单位

        public WaveLengthUnit GetWaveLengthUnit() => WaveAPI.GetWaveLengthUnit();

        public void SetWavelengthUnit(WaveLengthUnit waveLengthUnit) => WaveAPI.SetWavelengthUnit(waveLengthUnit);

        public PowerUnit GetPowerUnit() => WaveAPI.GetPowerUnit();

        public void SetPowerUnit(PowerUnit powerUnit) => WaveAPI.SetPowerUnit(powerUnit);

        #endregion 波长和功率单位

        #region 薄厚腔

        public CavityType GetWaveBand() => WaveAPI.GetWaveThickness();

        public void SetWaveBand(CavityType waveBand) => WaveAPI.SetWaveThickness(waveBand);

        #endregion 薄厚腔

        #region 触发模式

        public int GetTrigFrequency() => WaveAPI.GetTrigFrequency();

        public void SetTrigFrequency(int frequency) => WaveAPI.SetTrigFrequency(frequency);

        public FWMTriggerMode GetTrigMode() => WaveAPI.GetTrigMode();

        public void SetTriggerMode(FWMTriggerMode trigger) => WaveAPI.SetTriggerMode(trigger);

        #endregion 触发模式

        #region 内外光源

        public void SetLaserIn() => WaveAPI.SetLaserIn();

        public void SetLaserOut() => WaveAPI.SetLaserOut();

        #endregion 内外光源

        #region 曝光时间

        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="value"></param>
        public void FPGA_SetIntergrationTime(double value) => WaveAPI.FPGA_SetIntergrationTime(value);

        /// <summary>
        /// 查询曝光时间
        /// </summary>
        /// <returns></returns>
        public int GetFPGA_IntergrationTime() => WaveAPI.GetFPGA_IntergrationTime();

        /// <summary>
        /// 进行一次自动曝光
        /// </summary>
        public void FPGA_SetExpoTimeAuto() => WaveAPI.FPGA_SetExpoTimeAuto();

        /// <summary>
        /// 设置自适应曝光
        /// </summary>
        public void FPGA_SetExpoTimeAdaptive(bool value) => WaveAPI.FPGA_SetExpoTimeAdaptive(value);

        /// <summary>
        /// 查询自适应曝光
        /// </summary>
        public bool GetExpoTimeAdaptive() => WaveAPI.GetExpoTimeAdaptive();

        #endregion 曝光时间

        #region IPAddress

        public string GetIPSetting(IpAddressType addressType) => WaveAPI.GetIPSetting(addressType);

        public void SetIPSetting(IpAddressType ipAddressType, string value) => WaveAPI.SetIpSetting(ipAddressType, value);

        #endregion IPAddress

        #region 自动校准

        /// <summary>
        /// 设置自动校准
        /// </summary>
        /// <param ></param>
        public void SetAutoCalibration(AutoCalibration autoCalibration) => WaveAPI.SetAutoCalibration(autoCalibration);

        /// <summary>
        /// 查询自动校准
        /// </summary>
        /// <param ></param>
        public bool QueryAutoCalibration() => WaveAPI.QueryAutoCalibration();

        /// <summary>
        /// 设置自动校准周期
        /// </summary>
        /// <param ></param>
        public void SetAutoCalibrationCycle(int cycle) => WaveAPI.SetAutoCalibrationCycle(cycle);

        /// <summary>
        /// 查询自动校准周期
        /// </summary>
        /// <param ></param>
        public int QueryAutoCalibrationCycle() => WaveAPI.QueryAutoCalibrationCycle();

        /// <summary>
        /// 查询上次校准时间
        /// </summary>
        /// <param ></param>
        public string QueryLastCalibrationTime() => WaveAPI.QueryLastCalibrationTime();

        #endregion 自动校准

        #region 增益

        public GainState GetGainState(int channel) => WaveAPI.QueryGAIN(channel);

        public void SetGainState(int channel, GainState state) => WaveAPI.SwitchGain(channel, state);

        #endregion 增益

        #region 采样

        public void SetSamplingSetting(WaveChannel channel, SampleParamType paramType, ADCType type, int value) => WaveAPI.SetSamplingSetting(channel, paramType, type, value);

        #endregion 采样

        #endregion 设定

        #region 下位机参数

        /// <summary>
        /// 获取校准参数有效性
        /// </summary>
        /// <param name="waveAPI"></param>
        /// <returns></returns>
        public bool GetParamValidity()
        {
            try
            {
                if (!Connected)
                    return false;

                return WaveAPI.GetCalibrateValidity();
            }
            catch
            {
                throw;
            }
        }

        #region Write

        /// <summary>
        /// 写入功率参数
        /// </summary>
        /// <returns></returns>
        public void WritePowerParam(PowerParam powerParam) => WaveAPI.SetPowerCompensate(powerParam);

        public bool WriteLowerComputerParam(LowerComputerParam lowerComputerParam) => WaveAPI.WriteLowerComputerParam(lowerComputerParam);

        /// <summary>
        /// 通知下位机保存参数
        /// </summary>
        public void SaveParamToLower()
        {
            try
            {
                if (Connected)
                    WaveAPI.CalibrateSAVE();
            }
            catch
            {
                throw;
            }
        }

        #endregion Write

        #endregion 下位机参数

        #region 波长

        public void OpenCCD(bool onOff) => WaveAPI.OpenCCDADC(onOff);

        /// <summary>
        /// 从下位机读取CCD数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<int[]> GetCavityDataFromLower() => WaveAPI.ReadCCD_SingleN();

        /// <summary>
        /// 从下位机获取波长
        /// </summary>
        public (double, double) GetWaveLengthFromLower()
        {
            try
            {
                var array = WaveAPI.MeasureWLAndPower();

                if (array.Length >= 2 && double.TryParse(array[0], out double power) && double.TryParse(array[1], out double waveLength))
                    return (power, waveLength);
                else
                    throw new Exception("There are errors in the obtained data!");
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