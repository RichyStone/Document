using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.Common.CommonModels.Classes.Param;
using System.Collections.Generic;
using System.Linq;

namespace Semight.Fwm.HardWare.FWM8612InteractionLib.Function
{
    /// <summary>
    /// 下位机参数建造者
    /// </summary>
    public class LCParamBulider
    {
        private LowerComputerParam _currentParam;

        /// <summary>
        /// 当前下位机参数
        /// </summary>
        public LowerComputerParam CurrentParam
        {
            get
            {
                if (_currentParam == null)
                    _currentParam = new LowerComputerParam();

                return _currentParam;
            }
        }

        #region Method

        #region Set

        public void SetLowerComputerParam(LowerComputerParam param)
        {
            _currentParam = param;
        }

        #region 功率校准参数

        /// <summary>
        /// 设置功率校准参数
        /// </summary>
        public void SetPowerParam(PowerParam powerParam)
        {
            CurrentParam.PowerParam = powerParam;
        }

        #endregion 功率校准参数

        #region 多峰参数

        /// <summary>
        /// 设置多峰参数
        /// </summary>
        public void SetMultiPeaksParam(MultiPeaksParam multiPeaksParam)
        {
            CurrentParam.MultiPeaksParam = multiPeaksParam;
        }

        /// <summary>
        /// 设置多峰参数
        /// </summary>
        public void SetMultiPeaksParam(PeakParam peakParam)
        {
            CurrentParam.MultiPeaksParam.Peaks.RemoveAll(pe => pe.ChannelIndex == peakParam.ChannelIndex);
            CurrentParam.MultiPeaksParam.Peaks.Add(peakParam);
        }

        public void SetMulitPeaksFlag(bool flag)
        {
            CurrentParam.MultiPeaksParam.MultiPeaksFlag = flag;
        }

        public void SetMultiPeaksSIDFlag(bool flag)
        {
            CurrentParam.MultiPeaksParam.SIDMultiPeaksFlag = flag;
        }

        #endregion 多峰参数

        #region 菲索参数

        /// <summary>
        /// 设置菲索参数
        /// </summary>
        public void SetFizeauParam(List<FizeauParam> fizeauParams)
        {
            foreach (var item in fizeauParams)
                SetFizeauParam(item);
        }

        /// <summary>
        /// 设置菲索参数
        /// </summary>
        public void SetFizeauParam(FizeauParam fizeauParam)
        {
            CurrentParam.FizeauParameters.RemoveAll(fi => fi.WaveBand == fizeauParam.WaveBand && fi.Thickness == fizeauParam.Thickness);
            CurrentParam.FizeauParameters.Add(fizeauParam);
        }

        #endregion 菲索参数

        #region 色散补偿参数

        /// <summary>
        /// 设置补偿状态
        /// </summary>
        /// <param name="status"></param>
        public void SetDispersionCompensationStatus(bool status)
        {
            CurrentParam.DispersionCompensation.FitStatus = status;
        }

        /// <summary>
        /// 设置补偿参数
        /// </summary>
        public void SetDispersionCompensation(CompensationParam compensationParam)
        {
            CurrentParam.DispersionCompensation.CompensationParam.RemoveAll(com => com.WaveBand == compensationParam.WaveBand && com.CavityType == compensationParam.CavityType);
            CurrentParam.DispersionCompensation.CompensationParam.Add(compensationParam);
        }

        /// <summary>
        /// 设置补偿参数
        /// </summary>
        public void SetDispersionCompensation(DispersionCompensateParam param)
        {
            CurrentParam.DispersionCompensation = param;
        }

        #endregion 色散补偿参数

        #region 参考光源参数

        /// <summary>
        /// 设置参考光源参数
        /// </summary>
        public void SetFreqStableWLParam(FreqStableParam param)
        {
            CurrentParam.FreqStableWLParam.RemoveAll(pa => pa.WaveBand == param.WaveBand);
            CurrentParam.FreqStableWLParam.Add(param);
        }

        /// <summary>
        /// 设置参考光源参数
        /// </summary>
        public void SetFreqStableWLParam(List<FreqStableParam> param)
        {
            foreach (var item in param)
                SetFreqStableWLParam(item);
        }

        #endregion 参考光源参数

        #endregion Set

        #region Get

        /// <summary>
        /// 获取功率校准参数
        /// </summary>
        /// <returns></returns>
        public PowerParam GetPowerParam()
        {
            return CurrentParam.PowerParam;
        }

        /// <summary>
        /// 获取多峰参数
        /// </summary>
        /// <returns></returns>
        public MultiPeaksParam GetMultiPeaksParam()
        {
            return CurrentParam.MultiPeaksParam;
        }

        /// <summary>
        /// 获取菲索参数
        /// </summary>
        /// <returns></returns>
        public List<FizeauParam> GetFizeauParams()
        {
            return CurrentParam.FizeauParameters;
        }

        /// <summary>
        /// 获取菲索参数
        /// </summary>
        /// <param name="waveBand"></param>
        /// <returns></returns>
        public List<FizeauParam> GetFizeauParams(WaveBand waveBand)
        {
            return CurrentParam.FizeauParameters.Where(fi => fi.WaveBand == waveBand).ToList();
        }

        /// <summary>
        /// 获取菲索参数
        /// </summary>
        /// <param name="wLWidthType"></param>
        /// <returns></returns>
        public List<FizeauParam> GetFizeauParams(ThickNessType wLWidthType)
        {
            return CurrentParam.FizeauParameters.Where(fi => fi.Thickness == wLWidthType).ToList();
        }

        /// <summary>
        /// 获取菲索参数
        /// </summary>
        /// <param name="waveBand"></param>
        /// <param name="wLWidthType"></param>
        /// <returns></returns>
        public FizeauParam GetFizeauParams(WaveBand waveBand, ThickNessType wLWidthType)
        {
            return CurrentParam.FizeauParameters.FirstOrDefault(fi => fi.WaveBand == waveBand && wLWidthType == fi.Thickness);
        }

        /// <summary>
        /// 获取补偿参数
        /// </summary>
        /// <returns></returns>
        public DispersionCompensateParam GetDispersionCompensation()
        {
            return CurrentParam.DispersionCompensation;
        }

        public List<CompensationParam> GetDispersionCompensation(WaveBand waveBand)
        {
            return CurrentParam.DispersionCompensation.CompensationParam.Where(co => co.WaveBand == waveBand).ToList();
        }

        /// <summary>
        /// 获取补偿参数
        /// </summary>
        /// <returns></returns>
        public CompensationParam GetDispersionCompensation(WaveBand waveBand, CavityType cavityType)
        {
            return CurrentParam.DispersionCompensation.CompensationParam.FirstOrDefault(co => co.WaveBand == waveBand && co.CavityType == cavityType);
        }

        /// <summary>
        /// 获取参考光源参数
        /// </summary>
        /// <returns></returns>
        public List<FreqStableParam> GetFreqStableWLParam()
        {
            return CurrentParam.FreqStableWLParam;
        }

        /// <summary>
        /// 获取参考光源参数
        /// </summary>
        /// <returns></returns>
        public FreqStableParam GetFreqStableWLParam(WaveBand waveBand)
        {
            return CurrentParam.FreqStableWLParam.FirstOrDefault(p=>p.WaveBand==waveBand);
        }

        #endregion Get

        #endregion Method
    }
}