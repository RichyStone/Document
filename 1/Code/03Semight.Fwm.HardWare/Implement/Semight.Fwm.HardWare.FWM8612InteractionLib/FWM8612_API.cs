using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.Common.CommonModels.Classes.Param;
using Semight.Fwm.Connection.ConnectionAssistantLib;
using Semight.Fwm.Connection.ConnectionAssistantLib.Net;
using Semight.Fwm.Connection.ConnectionAssistantLib.USB;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Command;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Function;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Semight.Fwm.Common.CommonModels.Classes.HardWare;

namespace Semight.Fwm.HardWare.FWM8612InteractionLib
{
    public class FWM8612_API
    {
        #region Connection

        public bool Connected { get; private set; }

        public int Vid => Convert.ToInt32("0483", 16);

        public int Pid => Convert.ToInt32("5740", 16);

        /// <summary>
        /// 通讯下下文
        /// </summary>
        private ConnectionContext connectionContext;

        /// <summary>
        /// 开启网口连接
        /// </summary>
        /// <param name="strIP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port)
        {
            try
            {
                var communicateImplement = new NetCommunicator(ip, port);
                connectionContext = new ConnectionContext(communicateImplement);

                return Connected = connectionContext.StartConnect();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 开启USB连接
        /// </summary>
        /// <param name="strIP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        public bool Connect(string serial = "")
        {
            try
            {
                var communicateImplement = new USBCommunicator(Vid, Pid, serial);
                connectionContext = new ConnectionContext(communicateImplement);

                return Connected = connectionContext.StartConnect();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                Connected = !connectionContext.CloseConnect();

                return !Connected;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Connection

        #region TransitionTools

        private static readonly object transLock = new object();

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="commandMsg"></param>
        /// <returns></returns>
        private string AcquireMessage(string commandMsg, int timeout = 2000)
        {
            try
            {
                if (!Connected) throw new Exception("Connection was Broke!");
#if SelfTest
                timeout *= 1000;
#endif

                lock (transLock)
                {
                    var command = commandMsg + "\n";
                    var reply = connectionContext.SendMessageWithReply(command, timeout).Trim();
                    return reply;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="commandMsg"></param>
        /// <param name="replyLength"></param>
        /// <returns></returns>
        private byte[] AcquireData(string commandMsg, int replyLength, int timeout = 2000)
        {
            try
            {
                if (!Connected) throw new Exception("Connection was Broke!");

#if SelfTest
                timeout *= 1000;
#endif
                lock (transLock)
                {
                    var command = commandMsg + "\n";
                    var reply = connectionContext.SendMessageWithData(command, replyLength, timeout);
                    return reply;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="commandMsg"></param>
        private void SendCommand(string commandMsg)
        {
            try
            {
                if (!Connected) throw new Exception("Connection was Broke!");

                lock (transLock)
                {
                    var command = commandMsg + "\n";
                    connectionContext.SendMessage(command);
                    Thread.Sleep(3);
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion TransitionTools

        #region OPC

        /// <summary>
        /// 获取上条指令是否成功
        /// </summary>
        /// <returns></returns>
        private bool QueryOPC()
        {
            string command = $"{CommandConst.QuerySuccess}?";
            var reply = AcquireMessage(command);

            return !string.IsNullOrEmpty(reply) && reply.Trim().Equals("1");
        }

        #endregion OPC

        #region CCDData

        /// <summary>
        /// 读取CCD原始数据
        /// </summary>
        /// <returns></returns>
        public List<int[]> ReadCCD_SingleN()
        {
            try
            {
                var lstData = new List<int[]>();
                string command = $"{CommandConst.CCDSample}?";
                byte[] readbuffer = AcquireData(command, 2800);

                if (readbuffer.Length >= 2800)
                {
                    lstData.Add(ResolveHelper.ResolveCCDData(readbuffer.Take(1024).ToArray()));
                    lstData.Add(ResolveHelper.ResolveCCDData(readbuffer.Skip(1024).Take(1024).ToArray()));

                    int m = 2050;
                    int[] TRHAP = new int[4];
                    ////Temperature
                    TRHAP[0] = ResolveHelper.ResolveHardWareData(readbuffer.Skip(m).Take(4).ToArray());
                    ////RH
                    TRHAP[1] = ResolveHelper.ResolveHardWareData(readbuffer.Skip(m + 4).Take(4).ToArray());
                    ////AP
                    TRHAP[2] = ResolveHelper.ResolveHardWareData(readbuffer.Skip(m + 8).Take(4).ToArray());
                    ////Power
                    TRHAP[3] = ResolveHelper.ResolveHardWareData(readbuffer.Skip(2796).Take(4).ToArray());

                    lstData.Add(TRHAP);
                }

                readbuffer = null;
                return lstData;
            }
            catch
            {
                throw;
            }
        }

        #endregion CCDData

        #region HardWareSettings

        #region 出厂设置

        /// <summary>
        /// 保存出厂设置
        /// </summary>
        public void FactorySave()
        {
            SendCommand(CommandConst.FactorySave);
        }

        /// <summary>
        /// 恢复出厂设置
        /// </summary>
        public void RST()
        {
            SendCommand(CommandConst.Rst);
        }

        /// <summary>
        /// 获取机器信息
        /// </summary>
        /// <returns></returns>
        public IDNInfo QueryIDN()
        {
            var command = $"{CommandConst.QueryIDN}?";
            var reply = AcquireMessage(command);
            var idnStrs = reply.Split(',');
            if (idnStrs.Length >= 5)
                return new IDNInfo() { Company = idnStrs[0], InstrumentModel = idnStrs[1], InstrumentSN = idnStrs[2], FirmWare_Version = idnStrs[3], FPGA_Version = idnStrs[4] };
            else
                return null;
        }

        #endregion 出厂设置

        #region IPAddress

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <param name="ipAddressType"></param>
        /// <returns></returns>
        public string GetIPSetting(IpAddressType ipAddressType)
        {
            var str = GetIpCommandStr(ipAddressType);
            var reply = AcquireMessage(str + "?");
            return reply.Replace('\"', ' ').Trim();
        }

        /// <summary>
        /// 设置IP地址
        /// </summary>
        /// <param name="ipAddressType"></param>
        /// <param name="value"></param>
        public void SetIpSetting(IpAddressType ipAddressType, string value)
        {
            var str = $"{GetIpCommandStr(ipAddressType)} +\"+{value}+\"";
            SendCommand(str);
        }

        /// <summary>
        /// 获取IP设定Command
        /// </summary>
        /// <param name="ipAddressType"></param>
        /// <returns></returns>
        private string GetIpCommandStr(IpAddressType ipAddressType)
        {
            string str = string.Empty;
            switch (ipAddressType)
            {
                case IpAddressType.Ip:
                    str = CommandConst.FlashIp;
                    break;

                case IpAddressType.Port:
                    str = CommandConst.FlashPort;
                    break;

                case IpAddressType.Mask:
                    str = CommandConst.FlashMask;
                    break;

                case IpAddressType.Gate:
                    str = CommandConst.FlashGate;
                    break;
            }

            return str;
        }

        #endregion IPAddress

        #region 自动校准

        /// <summary>
        /// 设置自动校准
        /// </summary>
        /// <param ></param>
        public void SetAutoCalibration(AutoCalibration autoCalibration)
        {
            var temp = string.Empty;
            switch (autoCalibration)
            {
                case AutoCalibration.ON:
                    temp = "1";
                    break;

                case AutoCalibration.OFF:
                    temp = "0";
                    break;

                case AutoCalibration.ONCE:
                    temp = "M";
                    break;
            }

            string command = $"{CommandConst.AutoCalibration} {temp}";
            SendCommand(command);
        }

        /// <summary>
        /// 查询自动校准
        /// </summary>
        /// <param ></param>
        public bool QueryAutoCalibration()
        {
            string command = $"{CommandConst.AutoCalibration}?";
            var reply = AcquireMessage(command);

            return !string.IsNullOrEmpty(reply) && reply.Trim().Equals("1");
        }

        /// <summary>
        /// 设置自动校准周期
        /// </summary>
        /// <param ></param>
        public void SetAutoCalibrationCycle(int cycle)
        {
            string command = $"{CommandConst.AutoCalibrationCycle} {cycle}";
            SendCommand(command);
        }

        /// <summary>
        /// 查询自动校准周期
        /// </summary>
        /// <param ></param>
        public int QueryAutoCalibrationCycle()
        {
            string command = $"{CommandConst.AutoCalibrationCycle}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToInt(reply);
        }

        /// <summary>
        /// 查询上次校准时间
        /// </summary>
        /// <param ></param>
        public string QueryLastCalibrationTime()
        {
            string command = $"{CommandConst.LastCalibrationTime}?";
            return AcquireMessage(command, 80);
        }

        #endregion 自动校准

        #region 切换光源

        /// <summary>
        /// 切换到内部光源
        /// </summary>
        public void SetLaserIn()
        {
            SendCommand(CommandConst.SetLaserIn);
        }

        /// <summary>
        /// 切换到外部光源
        /// </summary>
        public void SetLaserOut()
        {
            SendCommand(CommandConst.SetLaserOut);
        }

        #endregion 切换光源

        #region 开关/初始化

        /// <summary>
        /// 控制电源板 0/1  打开FPGA业务板电源，没有回复信息。
        /// </summary>
        /// <param name="value"></param>
        public void OpenFPGAPower(bool onOff)
        {
            var value = onOff ? 1 : 0;
            string command = $"{CommandConst.OpenFPGAPower} {value}";
            SendCommand(command);
        }

        /// <summary>
        ///  打开或关闭 CCD ADC采样功能，在发送读取ADC数据命令之前需要先发送该命令，没有回复消息。 0/1
        /// </summary>
        /// <param name="value"></param>
        public void OpenCCDADC(bool onOff)
        {
            var value = onOff ? 1 : 0;

            string command = $"{CommandConst.OpenCCDADC} {value}";
            SendCommand(command);
        }

        public bool MacDefault()
        {
            var reply = AcquireMessage(CommandConst.MacDefault);
            return !string.IsNullOrEmpty(reply) && reply.Equals("1");
        }

        #endregion 开关/初始化

        #region 增益

        /// <summary>
        /// 增益补偿
        /// </summary>
        public void SwitchGain(int channel, GainState state)
        {
            var str = state == GainState.High ? CommandConst.GainHighState : CommandConst.GainLowState;
            string command = $"{CommandConst.Gain}{channel} {str}";
            SendCommand(command);
        }

        /// <summary>
        /// 查询增益状态
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public GainState QueryGAIN(int channel)
        {
            string command = $"{CommandConst.Gain}{channel}?";
            var reply = AcquireMessage(command);
            if (!string.IsNullOrEmpty(reply) && reply.ToUpper().Equals(CommandConst.GainHighState.ToUpper()))
                return GainState.High;
            else return GainState.Low;
        }

        #endregion 增益

        #region 采样参数

        /// <summary>
        /// 设置采样设定
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="strType"></param>
        /// <param name="value"></param>
        public void SetSamplingSetting(WaveChannel channel, SampleParamType paramType, ADCType type, int value)
        {
            var sampleParam = paramType == SampleParamType.PGA ? CommandConst.ADCPGA : CommandConst.ADCOffset;
            var command = $"{CommandConst.ADCSetting}{(int)channel}:{sampleParam}:{type} {value}";
            SendCommand(command);
        }

        #endregion 采样参数

        #region 触发模式

        /// <summary>
        /// 获取触发模式
        /// </summary>
        /// <returns></returns>
        public FWMTriggerMode GetTrigMode()
        {
            string command = $"{CommandConst.TriggerMode}?";
            var reply = AcquireMessage(command);

            var value = FWMTriggerMode.None;
            switch (reply.ToUpper())
            {
                case "SINGLE": value = FWMTriggerMode.Singleton; break;
                case "INTERNAL": value = FWMTriggerMode.SoftWare; break;
                case "EXTERNAL": value = FWMTriggerMode.External; break;
            }

            return value;
        }

        /// <summary>
        /// 设置触发模式
        /// </summary>
        /// <param name="triggerMode"></param>
        public void SetTriggerMode(FWMTriggerMode triggerMode)
        {
            var value = string.Empty;
            switch (triggerMode)
            {
                case FWMTriggerMode.Singleton: value = "SINgle"; break;
                case FWMTriggerMode.SoftWare: value = "INTernal"; break;
                case FWMTriggerMode.External: value = "EXTernal"; break;
            }
            var command = $"{CommandConst.TriggerMode} {value}";
            SendCommand(command);
        }

        /// <summary>
        /// 获取内触发频率
        /// </summary>
        public int GetTrigFrequency()
        {
            var command = $"{CommandConst.TriggerFrequency}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToInt(reply);
        }

        /// <summary>
        /// 设置内触发频率
        /// </summary>
        /// <param name="numValue"></param>
        public void SetTrigFrequency(int numValue)
        {
            var command = $"{CommandConst.TriggerFrequency} {numValue}";
            SendCommand(command);
        }

        #endregion 触发模式

        #region 曝光时间

        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="value"></param>
        public void FPGA_SetIntergrationTime(double value)
        {
            var command = $"{CommandConst.FPGA_IntergrationTime} {value}";
            SendCommand(command);
        }

        /// <summary>
        /// 查询曝光时间
        /// </summary>
        /// <returns></returns>
        public int GetFPGA_IntergrationTime()
        {
            var command = $"{CommandConst.FPGA_IntergrationTime}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToInt(reply);
        }

        /// <summary>
        /// 进行一次自动曝光
        /// </summary>
        public void FPGA_SetExpoTimeAuto()
        {
            var command = $"{CommandConst.FPGA_ExpoTimeAuto}";
            SendCommand(command);

            OpenCCDADC(true);
        }

        /// <summary>
        /// 设置自适应曝光
        /// </summary>
        public void FPGA_SetExpoTimeAdaptive(bool adaptive)
        {
            var value = adaptive ? 1 : 0;
            var command = $"{CommandConst.FPGA_ExpoTimeAdaptive} {value}";
            SendCommand(command);

            OpenCCDADC(true);
        }

        /// <summary>
        /// 查询自适应曝光
        /// </summary>
        public bool GetExpoTimeAdaptive()
        {
            var command = $"{CommandConst.FPGA_ExpoTimeAdaptive}?";
            var reply = AcquireMessage(command);
            return !string.IsNullOrEmpty(reply) && reply.Equals("1");
        }

        #endregion 曝光时间

        #region 波长和功率

        /// <summary>
        /// 获取波长+功率
        /// </summary>
        /// <returns></returns>
        public string[] MeasureWLAndPower()
        {
            var command = $"{CommandConst.MeasureWLAndPower}?";
            var reply = AcquireMessage(command);
            return reply.Split(',');
        }

        /// <summary>
        /// 获取机器信息
        /// </summary>
        /// <returns></returns>
        public double MeasureWL()
        {
            var command = $"{CommandConst.MeasureWaveLength}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToDouble(reply);
        }

        /// <summary>
        /// 获取功率数据
        /// </summary>
        /// <returns></returns>
        public double MeasurePower()
        {
            var command = $"{CommandConst.MeasurePower}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToDouble(reply);
        }

        /// <summary>
        /// 获取功率单位
        /// </summary>
        /// <returns></returns>
        public PowerUnit GetPowerUnit()
        {
            var command = $"{CommandConst.PowerUnit}?";
            var reply = AcquireMessage(command);

            var unit = PowerUnit.mW;
            switch (reply.ToLower())
            {
                case "dbm":
                    unit = PowerUnit.dBm;
                    break;

                case "mw":
                    unit = PowerUnit.mW;
                    break;
            }

            return unit;
        }

        /// <summary>
        /// 设置功率单位
        /// </summary>
        /// <param name="powerUnit"></param>
        public void SetPowerUnit(PowerUnit powerUnit)
        {
            var unit = string.Empty;
            switch (powerUnit)
            {
                case PowerUnit.dBm:
                    unit = "DBM";
                    break;

                case PowerUnit.mW:
                    unit = "MW";
                    break;
            }

            var command = $"{CommandConst.PowerUnit} {unit}";
            SendCommand(command);
        }

        /// <summary>
        /// 获取波长单位
        /// </summary>
        /// <returns></returns>
        public WaveLengthUnit GetWaveLengthUnit()
        {
            var command = $"{CommandConst.WaveLengthUnit}?";
            var reply = AcquireMessage(command);

            var unit = WaveLengthUnit.Cm_1;
            switch (reply.ToLower())
            {
                case "nm":
                    unit = WaveLengthUnit.nm;
                    break;

                case "thz":
                    unit = WaveLengthUnit.THz;
                    break;

                case "icm":
                    unit = WaveLengthUnit.Cm_1;
                    break;
            }

            return unit;
        }

        /// <summary>
        /// 设置波长单位
        /// </summary>
        /// <param name="waveLengthUnit"></param>
        public void SetWavelengthUnit(WaveLengthUnit waveLengthUnit)
        {
            var unit = string.Empty;
            switch (waveLengthUnit)
            {
                case WaveLengthUnit.nm:
                    unit = "NM";
                    break;

                case WaveLengthUnit.THz:
                    unit = "THZ";
                    break;

                case WaveLengthUnit.Cm_1:
                    unit = "ICM";
                    break;
            }

            var command = $"{CommandConst.WaveLengthUnit} {unit}";
            SendCommand(command);
        }

        #endregion 波长和功率

        #region 带宽

        /// <summary>
        /// 获取带宽类型（宽带/窄带）
        /// </summary>
        /// <returns></returns>
        public CavityType GetWaveThickness()
        {
            var command = $"{CommandConst.WaveBand}?";
            var reply = AcquireMessage(command);

            var cavity = CavityType.None;
            switch (reply.ToLower())
            {
                case "narrow":
                    cavity = CavityType.Narrow;
                    break;

                case "broad":
                    cavity = CavityType.Broad;
                    break;
            }

            return cavity;
        }

        /// <summary>
        /// 设置带宽类型（宽带/窄带）
        /// </summary>
        /// <returns></returns>
        public void SetWaveThickness(CavityType cavityType)
        {
            var cavity = string.Empty;
            switch (cavityType)
            {
                case CavityType.Narrow:
                    cavity = "Narrow";
                    break;

                case CavityType.Broad:
                    cavity = "Broad";
                    break;
            }

            var command = $"{CommandConst.WaveBand} {cavity}";
            SendCommand(command);
        }

        #endregion 带宽

        #region 平均数

        /// <summary>
        /// 获取平均数
        /// </summary>
        /// <returns></returns>
        public int GetCalculatingAverage()
        {
            var command = $"{CommandConst.AverageCount}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToInt(reply);
        }

        /// <summary>
        /// 设置平均数
        /// </summary>
        /// <returns></returns>
        public void SetCalculatingAverage(int value)
        {
            var command = $"{CommandConst.AverageCount} {value}";
            SendCommand(command);
        }

        #endregion 平均数

        #region Temperature

        /// <summary>
        /// 获取温度
        /// </summary>
        /// <returns></returns>
        public string GetTCM_Temperature(int num)
        {
            var command = $"{CommandConst.GetTCM_Temperature}{num}:{CommandConst.TemperatureFix}?";
            return AcquireMessage(command);
        }

        /// <summary>
        /// 获取加热是否完成
        /// </summary>
        /// <returns></returns>
        public bool GetTCM_HeaterState()
        {
            var command = $"{CommandConst.GetTCM_HeaterState}?";
            var reply = AcquireMessage(command);
            return !string.IsNullOrEmpty(reply) && reply.Equals("1");
        }

        #endregion Temperature

        #endregion HardWareSettings

        #region Parameters

        /// <summary>
        /// 验证校准参数有效性
        /// </summary>
        /// <returns>返回0表示当前校准数据无效，可能没有校准过或CRC有问题，返回1表示校准数据有效</returns>
        public bool GetCalibrateValidity()
        {
            string command = $"{CommandConst.GetParamValidity}?";
            var reply = AcquireMessage(command);
            return !string.IsNullOrEmpty(reply) && reply.Equals("1");
        }

        /// <summary>
        /// 保存校准参数
        /// </summary>
        public void CalibrateSAVE()
        {
            SendCommand(CommandConst.SaveParam);
        }

        /// <summary>
        /// 获取下位机参数
        /// </summary>
        /// <returns></returns>
        public LowerComputerParam ReadLowerComputerParam()
        {
            var multiParam = GetMultiPeaksParam();
            var powerParam = GetPowerCompensate();
            var dispersionParam = GetDispersionCompensate();
            var fizeauParam = GetFizeau();
            var freqParam = GetFreqStableParam();

            return new LowerComputerParam
            {
                MultiPeaksParam = multiParam,
                PowerParam = powerParam,
                DispersionCompensation = dispersionParam,
                FizeauParameters = fizeauParam,
                FreqStableWLParam = freqParam,
            };
        }

        /// <summary>
        /// 写入下位机参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool WriteLowerComputerParam(LowerComputerParam param)
        {
            try
            {
                var result = new List<bool>
            {
                SetFizeauParam(param.FizeauParameters),
                SetDispersionCompensate(param.DispersionCompensation),
                SetPowerCompensate(param.PowerParam),
                SetMultiPeaksParam(param.MultiPeaksParam),
                SetFreqStableParam(param.FreqStableWLParam)
            };

                return result.All(res => res);
            }
            catch
            {
                throw;
            }
        }

        #region 菲索参数

        #region Get

        /// <summary>
        /// 查询斐索参数
        /// </summary>
        /// <returns></returns>
        public List<FizeauParam> GetFizeau()
        {
            try
            {
                var paramList = new List<FizeauParam>();

                foreach (var objBand in Enum.GetValues(typeof(WaveBand)))
                {
                    var waveBand = (WaveBand)objBand;
                    if (waveBand == WaveBand.None) continue;

                    foreach (var objThickness in Enum.GetValues(typeof(ThickNessType)))
                    {
                        var thickness = (ThickNessType)objThickness;
                        if (thickness == ThickNessType.None) continue;

                        var param = GetFizeau(waveBand, thickness);
                        paramList.Add(param);
                    }
                }

                return paramList;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 查询斐索参数
        /// </summary>
        /// <param name="Thickness"><THIN | THICk></param>
        /// <param name="waveBand">x取值O、E、S、C、L、U，</param>
        /// <returns></returns>
        public FizeauParam GetFizeau(WaveBand waveBand, ThickNessType Thickness)
        {
            try
            {
                var thickness = Thickness == ThickNessType.Thick ? CommandConst.ThickFix : CommandConst.ThinFix;
                string command = $"{CommandConst.Fizeau}:{waveBand}{CommandConst.WaveBandFix}:{thickness}?";
                var reply = AcquireMessage(command);
                string[] arrValue = reply.Split(',');

                var fizeauParam = new FizeauParam()
                {
                    WaveBand = waveBand,
                    Thickness = Thickness,
                    TotalPixels = Fwm8612CommonMethod.ConvertToDouble(arrValue[0]),
                    PixelWidth = Fwm8612CommonMethod.ConvertToDouble(arrValue[1]),
                    RefCavity = Fwm8612CommonMethod.ConvertToDouble(arrValue[2]),
                    ThicknessCavity = Fwm8612CommonMethod.ConvertToDouble(arrValue[3]),
                    AngleCavity = Fwm8612CommonMethod.ConvertToDouble(arrValue[4])
                };

                return fizeauParam;
            }
            catch
            {
                throw;
            }
        }

        #endregion Get

        #region Set

        /// <summary>
        /// 设置斐索参数
        /// </summary>
        public bool SetFizeauParam(List<FizeauParam> param)
        {
            try
            {
                var result = true;

                for (int i = 0; i < param.Count; i++)
                    if (!SetFizeauParam(param[i]))
                        result = false;

                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置斐索参数
        /// </summary>
        public bool SetFizeauParam(FizeauParam param)
        {
            try
            {
                SetFizeauParam(param.WaveBand, param.Thickness, param.TotalPixels,
                            param.PixelWidth, param.RefCavity, param.ThicknessCavity, param.AngleCavity);

                Thread.Sleep(10);
                return QueryOPC();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置斐索参数
        /// </summary>
        /// <param name="Thickness"><THIN | THICk></param>
        /// <param name="waveBand">x取值O、E、S、C、L、U，</param>
        /// <param name="elements"></param>
        /// <param name="pixelsize"></param>
        /// <param name="zeropoint"></param>
        /// <param name="wedgespacing"></param>
        /// <param name="wedgeangle"></param>
        private void SetFizeauParam(WaveBand waveBand, ThickNessType Thickness, double elements, double pixelsize, double zeropoint, double wedgespacing, double wedgeangle)
        {
            var thickness = Thickness == ThickNessType.Thick ? CommandConst.ThickFix : CommandConst.ThinFix;
            string command = $"{CommandConst.Fizeau}:{waveBand}{CommandConst.WaveBandFix}:{thickness} {elements},{pixelsize},{zeropoint},{wedgespacing},{wedgeangle}";
            SendCommand(command);
        }

        #endregion Set

        #endregion 菲索参数

        #region 色散补偿参数

        #region Get

        /// <summary>
        /// 获取色散补偿参数
        /// </summary>
        public DispersionCompensateParam GetDispersionCompensate()
        {
            try
            {
                var compensateParam = new DispersionCompensateParam();

                compensateParam.FitStatus = GetDispersionCompensateStatus();
                foreach (var objBand in Enum.GetValues(typeof(WaveBand)))
                {
                    var waveBand = (WaveBand)objBand;
                    if (waveBand == WaveBand.None) continue;

                    foreach (var objCavityType in Enum.GetValues(typeof(CavityType)))
                    {
                        var cavityType = (CavityType)objCavityType;
                        if (cavityType == CavityType.None) continue;

                        var param = GetDispersionCompensate(waveBand, cavityType);
                        compensateParam.CompensationParam.Add(param);
                    }
                }

                return compensateParam;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取色散补偿参数
        /// </summary>
        /// <returns></returns>
        public CompensationParam GetDispersionCompensate(WaveBand waveBand, CavityType cavity)
        {
            try
            {
                var cavityFix = cavity == CavityType.Broad ? "BROad" : "NARRow";
                string command = $"{CommandConst.Compensation}:{waveBand}{CommandConst.WaveBandFix}:{cavityFix}?";
                var reply = AcquireMessage(command);
                string[] arrValue = reply.Split(',');

                var param = new CompensationParam
                {
                    WaveBand = waveBand,
                    CavityType = cavity,
                    K1 = Fwm8612CommonMethod.ConvertToDouble(arrValue[0]),
                    K2 = Fwm8612CommonMethod.ConvertToDouble(arrValue[1]),
                    B = Fwm8612CommonMethod.ConvertToDouble(arrValue[2])
                };

                return param;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 查询色散补偿状态
        /// </summary>
        /// <returns></returns>
        public bool GetDispersionCompensateStatus()
        {
            string command = $"{CommandConst.CompensationStatus}?";
            var reply = AcquireMessage(command);

            var result = !string.IsNullOrEmpty(reply) && reply.Equals("1");
            return result;
        }

        #endregion Get

        #region Set

        /// <summary>
        /// 写入色散补偿参数
        /// </summary>
        /// <param name="compensationParam"></param>
        /// <returns></returns>
        public bool SetDispersionCompensate(DispersionCompensateParam compensationParam)
        {
            try
            {
                var result = true;

                if (!SetDispersionCompensateStatus(compensationParam.FitStatus))
                    result = false;

                var list = compensationParam.CompensationParam;
                for (int i = 0; i < list.Count; i++)
                    if (!SetDispersionCompensate(list[i]))
                        result = false;

                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 写入色散补偿参数
        /// </summary>
        /// <param name="compensationParam"></param>
        /// <returns></returns>
        public bool SetDispersionCompensate(CompensationParam compensationParam)
        {
            try
            {
                SetDispersionCompensate(compensationParam.WaveBand, compensationParam.CavityType, compensationParam.K1, compensationParam.K2, compensationParam.B);

                Thread.Sleep(10);
                return QueryOPC();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置色散补偿参数
        /// </summary>
        /// <param name="band"><BROad | NARRow></param>
        /// <param name="waveBand"> x取值O、E、S、C、L、U</param>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <param name="b"></param>
        private void SetDispersionCompensate(WaveBand waveBand, CavityType cavity, double k1, double k2, double b)
        {
            var cavityFix = cavity == CavityType.Broad ? "BROad" : "NARRow";
            string command = $"{CommandConst.Compensation}:{waveBand}{CommandConst.WaveBandFix}:{cavityFix} {k1},{k2},{b}";
            SendCommand(command);
        }

        /// <summary>
        /// 设置色散补偿状态
        /// </summary>
        /// <param name="status">0/1</param>
        public bool SetDispersionCompensateStatus(bool status)
        {
            var value = status ? "1" : "0";
            string command = $"{CommandConst.CompensationStatus} {value}";
            SendCommand(command);

            Thread.Sleep(10);
            return QueryOPC();
        }

        #endregion Set

        #endregion 色散补偿参数

        #region 功率补偿参数

        #region Get

        /// <summary>
        /// 查询功率补偿参数
        /// </summary>
        /// <returns></returns>
        public PowerParam GetPowerCompensate()
        {
            try
            {
                string command = $"{CommandConst.PowerCompensation}?";
                var reply = AcquireMessage(command);
                string[] arrValue = reply.Split(',');
                var powerParam = new PowerParam()
                {
                    Power_K = Fwm8612CommonMethod.ConvertToDouble(arrValue[0]),
                    Power_B = Fwm8612CommonMethod.ConvertToDouble(arrValue[1]),
                };

                return powerParam;
            }
            catch
            {
                throw;
            }
        }

        #endregion Get

        #region Set

        /// <summary>
        /// 设置功率补偿参数
        /// </summary>
        public bool SetPowerCompensate(PowerParam powerParam)
        {
            SetPowerCompensate(powerParam.Power_K, powerParam.Power_B);

            Thread.Sleep(10);
            return QueryOPC();
        }

        /// <summary>
        /// 设置功率补偿参数
        /// </summary>
        /// <param name="k"></param>
        /// <param name="b"></param>
        private void SetPowerCompensate(double k, double b)
        {
            string command = $"{CommandConst.PowerCompensation} {k},{b}";
            SendCommand(command);
        }

        #endregion Set

        #endregion 功率补偿参数

        #region 多峰参数

        #region Get

        /// <summary>
        /// 获取多峰参数
        /// </summary>
        /// <returns></returns>
        public MultiPeaksParam GetMultiPeaksParam()
        {
            try
            {
                var multiPeaksFlag_Temp = GetMultiPeakStatus();
                var SIDmultiPeaksFlag_Temp = GetMultiPeakSIDStatus();

                var multiPeaksParam = new MultiPeaksParam()

                {
                    MultiPeaksFlag = multiPeaksFlag_Temp,
                    SIDMultiPeaksFlag = SIDmultiPeaksFlag_Temp,
                };

                foreach (var item in Enum.GetValues(typeof(WaveChannel)))
                {
                    var channel = (WaveChannel)item;
                    multiPeaksParam.Peaks.Add(GetMultiPeaksParam(channel));
                }

                return multiPeaksParam;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取指定通道的多峰参数
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public PeakParam GetMultiPeaksParam(WaveChannel channel)
        {
            try
            {
                var peak = new double[4];
                peak[0] = GetMultiPeakNode((int)channel);
                peak[1] = GetMultiPeakNDTH((int)channel);
                peak[2] = GetMultiPeakPVTH((int)channel);
                peak[3] = GetMultiPeakNDSTH((int)channel);

                var peakParam = new PeakParam()
                {
                    ChannelIndex = channel,
                    Node = peak[0],
                    Nd_th = peak[1],
                    Pv_th = peak[2],
                    Nd_sth = peak[3],
                };

                return peakParam;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 查询多峰识别状态
        /// </summary>
        /// <returns></returns>
        private bool GetMultiPeakStatus()
        {
            string command = $"{CommandConst.MultiPeakStatus}?";
            var reply = AcquireMessage(command);

            var status = !string.IsNullOrEmpty(reply) && reply.Equals("1");
            return status;
        }

        /// <summary>
        /// 查询ND_STH阈值使能
        /// </summary>
        /// <returns></returns>
        private bool GetMultiPeakSIDStatus()
        {
            string command = $"{CommandConst.MultiPeakSIDStatus}?";
            var reply = AcquireMessage(command);

            var status = !string.IsNullOrEmpty(reply) && reply.Equals("1");
            return status;
        }

        /// <summary>
        /// 查询多峰设置基准波峰个数
        /// </summary>
        /// <returns></returns>
        private int GetMultiPeakNode(int channel)
        {
            string command = $"{CommandConst.MultiPeakNode}{channel}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToInt(reply);
        }

        /// <summary>
        /// 查询ND_TH
        /// </summary>
        /// <returns></returns>
        private double GetMultiPeakNDTH(int channel)
        {
            string command = $"{CommandConst.MultiPeakNDTH}{channel}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToDouble(reply);
        }

        /// <summary>
        /// 查询PV_TH
        /// </summary>
        /// <returns></returns>
        private double GetMultiPeakPVTH(int channel)
        {
            string command = $"{CommandConst.MultiPeakPVTH}{channel}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToDouble(reply);
        }

        /// <summary>
        /// 查询ND_STH
        /// </summary>
        /// <returns></returns>
        private double GetMultiPeakNDSTH(int channel)
        {
            string command = $"{CommandConst.MultiPeakNDSTH}{channel}?";
            var reply = AcquireMessage(command);
            return Fwm8612CommonMethod.ConvertToDouble(reply);
        }

        #endregion Get

        #region Set

        /// <summary>
        /// 设置多峰参数
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool SetMultiPeaksParam(MultiPeaksParam multiPeaksParam)
        {
            try
            {
                var result = true;
                if (!SetMultiPeakStatus(multiPeaksParam.MultiPeaksFlag))
                    result = false;

                if (!SetMultiPeakSIDStatus(multiPeaksParam.SIDMultiPeaksFlag))
                    result = false;

                var list = multiPeaksParam.Peaks;
                for (int i = 0; i < list.Count; i++)
                    if (!SetMultiPeaksParam(list[i]))
                        result = false;

                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置指定通道的多峰参数
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool SetMultiPeaksParam(PeakParam peakParam)
        {
            try
            {
                var channel = (int)peakParam.ChannelIndex;
                SetMultiPeakNDSTH(channel, peakParam.Nd_sth);
                SetMultiPeakNode(channel, (int)peakParam.Node);
                SetMultiPeakNDTH(channel, peakParam.Nd_th);
                SetMultiPeakPVTH(channel, peakParam.Pv_th);

                Thread.Sleep(10);
                return QueryOPC();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置多峰识别状态
        /// </summary>
        /// <param name="status"></param>
        private bool SetMultiPeakStatus(bool status)
        {
            var value = status ? "1" : "0";
            string command = $"{CommandConst.MultiPeakStatus} {value}";
            SendCommand(command);

            Thread.Sleep(10);
            return QueryOPC();
        }

        /// <summary>
        /// 设置ND_STH阈值使能
        /// </summary>
        /// <param name="status"></param>
        private bool SetMultiPeakSIDStatus(bool status)
        {
            var value = status ? "1" : "0";
            string command = $"{CommandConst.MultiPeakSIDStatus} {value}";
            SendCommand(command);

            Thread.Sleep(10);
            return QueryOPC();
        }

        /// <summary>
        /// 设置ND_STH
        /// </summary>
        ///
        /// <param name="status"></param>
        private void SetMultiPeakNDSTH(int channel, double ndsth)
        {
            string command = $"{CommandConst.MultiPeakNDSTH}{channel} {ndsth}";
            SendCommand(command);
        }

        /// <summary>
        /// 设置多峰设置基准波峰个数
        /// </summary>
        /// <param name="status"></param>
        private void SetMultiPeakNode(int channel, int node)
        {
            string command = $"{CommandConst.MultiPeakNode}{channel} {node}";
            SendCommand(command);
        }

        /// <summary>
        /// 设置ND_TH
        /// </summary>
        /// <param name="status"></param>
        private void SetMultiPeakNDTH(int channel, double ndth)
        {
            string command = $"{CommandConst.MultiPeakNDTH}{channel} {ndth}";
            SendCommand(command);
        }

        /// <summary>
        /// 设置PV_TH
        /// </summary>
        /// <param name="status"></param>
        private void SetMultiPeakPVTH(int channel, double pvth)
        {
            string command = $"{CommandConst.MultiPeakPVTH}{channel} {pvth}";
            SendCommand(command);
        }

        #endregion Set

        #endregion 多峰参数

        #region 稳频光源参数

        #region Get

        public List<FreqStableParam> GetFreqStableParam()
        {
            try
            {
                var paramList = new List<FreqStableParam>();
                foreach (var objBand in Enum.GetValues(typeof(WaveBand)))
                {
                    var waveBand = (WaveBand)objBand;
                    if (waveBand == WaveBand.None) continue;

                    var param = GetFreqStableParam(waveBand);
                    paramList.Add(param);
                }

                return paramList;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 查询稳频光源参考波长
        /// </summary>
        /// <returns>x取值O、E、S、C、L、U，params为step1,step2,final</returns>
        public FreqStableParam GetFreqStableParam(WaveBand waveBand)
        {
            try
            {
                string command = $"{CommandConst.FSWL}:{waveBand}{CommandConst.WaveBandFix}?";
                var reply = AcquireMessage(command);
                string[] arrValue = reply.Split(',');
                var param = new FreqStableParam()
                {
                    WaveBand = waveBand,
                    WL_Step1 = Fwm8612CommonMethod.ConvertToDouble(arrValue[0]),
                    WL_Step2 = Fwm8612CommonMethod.ConvertToDouble(arrValue[1]),
                    WL_Final = Fwm8612CommonMethod.ConvertToDouble(arrValue[2])
                };

                return param;
            }
            catch
            {
                throw;
            }
        }

        #endregion Get

        #region Set

        /// <summary>
        /// 设置稳频光源参考波长
        /// </summary>
        public bool SetFreqStableParam(List<FreqStableParam> paramList)
        {
            try
            {
                var result = true;
                for (int i = 0; i < paramList.Count; i++)
                    if (!SetFreqStableParam(paramList[i]))
                        result = false;

                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置稳频光源参考波长
        /// </summary>
        public bool SetFreqStableParam(FreqStableParam param)
        {
            try
            {
                SetFreqStableParam(param.WaveBand, param.WL_Step1, param.WL_Step2, param.WL_Final);

                Thread.Sleep(10);
                return QueryOPC();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置稳频光源参考波长
        /// </summary>
        /// <param name="status"></param>
        private void SetFreqStableParam(WaveBand waveBand, double step1, double step2, double final)
        {
            string command = $"{CommandConst.FSWL}:{waveBand}{CommandConst.WaveBandFix} {step1},{step2},{final}";
            SendCommand(command);
        }

        #endregion Set

        #endregion 稳频光源参数

        #endregion Parameters
    }
}