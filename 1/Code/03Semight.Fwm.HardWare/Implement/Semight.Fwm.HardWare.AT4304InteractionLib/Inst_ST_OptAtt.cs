using Semight.Fwm.HardWare.AT4304InteractionLib.Model;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Semight.Fwm.HardWare.AT4304InteractionLib
{
    public class Inst_ST_OptAtt
    {
        #region Fields

        private bool bsingleMode = true;

        #endregion Fields

        #region Import

        private const string DllPath = "Libs\\LX_VOA.dll";

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_OpenViaSerial(uint DeviceID, uint PortNumber);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_OpenViaEthernet(uint DeviceID, byte[] IPAddress, ushort ServerPort);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern void DLL_CloseDevice(uint DeviceID);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetProductName(uint DeviceID, [MarshalAs(UnmanagedType.LPStr)] StringBuilder ProductName);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetProductName(uint DeviceID, byte[] ProductName);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetSerialNumber(uint DeviceID, byte[] SerialNumber);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetVersion(uint DeviceID, byte[] Version);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetIPAddress(uint DeviceID, byte[] IPAddress);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_WriteIPAddress(uint DeviceID, byte[] IPAdress);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetServerPort(uint DeviceID, out UIntPtr ServerPort);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_WriteServerPort(uint DeviceID, ushort ServerPort);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetChannelCount(uint DeviceID, out UIntPtr ChannelCount);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetWavelengthCount(uint DeviceID, out UIntPtr WavelengthCount);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetWavelengthList(uint DeviceID, ushort[] WavelengthList);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetChannelWavelengthID(uint DeviceID, uint Channel, byte[] WavelengthID);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_SetChannelWavelengthID(uint DeviceID, uint Channel, byte[] WavelengthID);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetStepCount(uint DeviceID, out UIntPtr StepCount);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetStepList(uint DeviceID, float[] StepList);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetChannelStepID(uint DeviceID, uint Channel, byte[] StepID);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_SetChannelStepID(uint DeviceID, uint Channel, byte[] StepID);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetAttenuationRange(uint DeviceID, out UIntPtr MaxAttenuation);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetChannelAttenuation(uint DeviceID, uint Channel, float[] Attenuation);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_SetChannelAttenuation(uint DeviceID, uint Channel, float[] Attenuation);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetChannelShutter(uint DeviceID, uint Channel, byte[] Shutter);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_SetChannelShutter(uint DeviceID, uint Channel, byte[] Shutter);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetChannelPower(uint DeviceID, uint Channel, uint Detector, float[] Power);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetChannelPowerOffset(uint DeviceID, uint Channel, uint Detector, uint WavelengthID, float[] Offset);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_WriteChannelPowerOffset(uint DeviceID, uint Channel, uint Detector, uint WavelengthID, float[] Offset);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_GetChannelAttenuationOffset(uint DeviceID, uint Channel, uint WavelengthID, float[] Offset);

        [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
        private static extern bool DLL_WriteChannelAttenuationOffset(uint DeviceID, uint Channel, uint WavelengthID, float[] Offset);

        #endregion Import

        #region Function

        public bool OpenDevice(uint deviceID, string ipAddress, int port_id)
        {
            try
            {
                byte[] array = new byte[4];
                string[] array2 = ipAddress.Split('.');
                for (int i = 0; i < array2.Length; i++)
                {
                    array[i] = byte.Parse(array2[i]);
                }

                bool flag = DLL_OpenViaEthernet(deviceID, array, (ushort)port_id);
                ushort[] values = QueryWaveLengthList(deviceID);
                string text = string.Join(",", values);
                if (text.Contains("850"))
                {
                    bsingleMode = false;
                }
                return flag;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool OpenDevice(uint deviceID, uint port)
        {
            try
            {
                bool flag = DLL_OpenViaSerial(deviceID, port);
                ushort[] values = QueryWaveLengthList(deviceID);
                string text = string.Join(",", values);
                if (text.Contains("850"))
                {
                    bsingleMode = false;
                }
                return flag;
            }
            catch (Exception)
            {
                throw new Exception("Instrument not found");
            }
        }

        public bool OpenDevice(uint deviceID, string port)
        {
            try
            {
                uint portNumber = uint.Parse(port.Substring(3));
                bool flag = DLL_OpenViaSerial(deviceID, portNumber);
                ushort[] values = QueryWaveLengthList(deviceID);
                string text = string.Join(",", values);
                if (text.Contains("850"))
                {
                    bsingleMode = false;
                }

                return flag;
            }
            catch (Exception)
            {
                throw new Exception("Instrument not found");
            }
        }

        public void CloseDevice(uint deviceID)
        {
            DLL_CloseDevice(deviceID);
        }

        public string QueryProductName(uint deviceID)
        {
            byte[] array = new byte[6];
            StringBuilder stringBuilder = new StringBuilder(6);
            bool flag = DLL_GetProductName(deviceID, array);
            return Encoding.Default.GetString(array);
        }

        public string QuerySerialNumber(uint deviceID)
        {
            byte[] array = new byte[12];
            bool flag = DLL_GetSerialNumber(deviceID, array);
            return Encoding.Default.GetString(array);
        }

        public string QueryVersion(uint deviceID)
        {
            string text = "";
            byte[] array = new byte[4];
            bool flag = DLL_GetVersion(deviceID, array);
            for (int i = 0; i < array.Length; i++)
            {
                text = text + array[i] + ".";
            }

            return text;
        }

        public string HardwareIdentification(uint deviceID)
        {
            return QueryProductName(deviceID) + "," + QuerySerialNumber(deviceID) + "," + QueryVersion(deviceID);
        }

        public string QueryIPaddress(uint deviceID)
        {
            byte[] array = new byte[4];
            string text = "";
            bool flag = DLL_GetIPAddress(deviceID, array);
            for (int i = 0; i < array.Length; i++)
            {
                text = text + array[i] + ".";
            }

            return text;
        }

        public void SetIPaddress(uint deviceID, string ipaddress)
        {
            string[] array = ipaddress.Split('.');
            byte[] array2 = new byte[4];
            for (int i = 0; i < array.Length; i++)
            {
                array2[i] = byte.Parse(array[i]);
            }

            bool flag = DLL_WriteIPAddress(deviceID, array2);
        }

        public ushort QueryNetPort(uint deviceID)
        {
            UIntPtr ServerPort = UIntPtr.Zero;
            bool flag = DLL_GetServerPort(deviceID, out ServerPort);
            return (ushort)(uint)ServerPort;
        }

        public void SetNetPort(uint deviceID, ushort port)
        {
            bool flag = DLL_WriteServerPort(deviceID, port);
        }

        public int QueryChannelCount(uint deviceID)
        {
            UIntPtr ChannelCount = UIntPtr.Zero;
            bool flag = DLL_GetChannelCount(deviceID, out ChannelCount);
            return (int)(uint)ChannelCount;
        }

        public ushort[] QueryWaveLengthList(uint deviceID)
        {
            byte[] array = new byte[10];
            UIntPtr WavelengthCount = UIntPtr.Zero;
            bool flag = DLL_GetWavelengthCount(deviceID, out WavelengthCount);
            ushort[] array2 = new ushort[(uint)WavelengthCount];
            bool flag2 = DLL_GetWavelengthList(deviceID, array2);
            return array2;
        }

        public byte[] QueryWaveLengthID(uint deviceID)
        {
            byte[] array = new byte[4];
            uint channel = 0u;
            bool flag = DLL_GetChannelWavelengthID(deviceID, channel, array);
            return array;
        }

        public int QueryChanWaveLengthID(uint deviceID, uint channel)
        {
            lock (this)
            {
                byte[] array = new byte[4];
                bool flag = DLL_GetChannelWavelengthID(deviceID, channel, array);
                return array[0];
            }
        }

        public void SetWaveLength(uint deviceID, uint ichannel, int wavelengthID)
        {
            lock (this)
            {
                DLL_SetChannelWavelengthID(deviceID, ichannel, new byte[4]
                {
                    (byte)wavelengthID,
                    0,
                    0,
                    0
                });
            }
        }

        public int QueryStepCount(uint deviceID)
        {
            lock (this)
            {
                UIntPtr StepCount = UIntPtr.Zero;
                bool flag = DLL_GetStepCount(deviceID, out StepCount);
                return (int)(uint)StepCount;
            }
        }

        public float[] QueryStepList(uint deviceID)
        {
            lock (this)
            {
                float[] array = new float[QueryStepCount(deviceID)];
                bool flag = DLL_GetStepList(deviceID, array);
                return array;
            }
        }

        public int QueryCalibratedWavelengthCount(uint deviceID)
        {
            UIntPtr WavelengthCount = UIntPtr.Zero;
            bool flag = DLL_GetWavelengthCount(deviceID, out WavelengthCount);
            return (int)(uint)WavelengthCount;
        }

        public Attenuation QueryAttStep(uint deviceID, uint ichannel)
        {
            lock (this)
            {
                Attenuation result = Attenuation.att_0p1;
                byte[] array = new byte[QueryStepCount(deviceID)];
                bool flag = DLL_GetChannelStepID(deviceID, ichannel, array);
                switch (array[0])
                {
                    case 1:
                        result = Attenuation.att_0p1;
                        break;

                    case 2:
                        result = Attenuation.att_1;
                        break;

                    case 3:
                        result = Attenuation.att_10;
                        break;
                }

                return result;
            }
        }

        public void SetAttStep(uint deviceID, uint ichannel, Attenuation att)
        {
            lock (this)
            {
                int num = 0;

                switch (att)
                {
                    case Attenuation.att_0p1: num = 1; break;
                    case Attenuation.att_1: num = 2; break;
                    default: num = 3; break;
                }

                byte[] array = new byte[QueryStepCount(deviceID)];
                array[0] = (byte)num;
                bool flag = DLL_SetChannelStepID(deviceID, ichannel, array);
            }
        }

        public int QueryAttenuationRange(uint deviceID)
        {
            UIntPtr MaxAttenuation = UIntPtr.Zero;
            bool flag = DLL_GetAttenuationRange(deviceID, out MaxAttenuation);
            return (int)(uint)MaxAttenuation;
        }

        public void SetOutputPowerOffset(uint deviceID, uint ichannel, uint wavelengthID, float offset)
        {
            lock (this)
            {
                uint num = 2u;
                num = ((!bsingleMode) ? 1u : 2u);
                bool flag = DLL_WriteChannelPowerOffset(deviceID, ichannel, num, wavelengthID, new float[4] { offset, 0f, 0f, 0f });
                Thread.Sleep(100);
            }
        }

        public void SetOutputPowerOffset(uint deviceID, uint wavelengthID, float[] offset)
        {
            lock (this)
            {
                uint channel = 0u;
                uint num = 2u;
                num = ((!bsingleMode) ? 1u : 2u);
                bool flag = DLL_WriteChannelPowerOffset(deviceID, channel, num, wavelengthID, offset);
                Thread.Sleep(100);
            }
        }

        public float QueryOutputPowerOffset(uint deviceID, uint ichannel, uint wavelengthID)
        {
            lock (this)
            {
                uint num = 2u;
                num = ((!bsingleMode) ? 1u : 2u);
                float[] array = new float[4];
                bool flag = DLL_GetChannelPowerOffset(deviceID, ichannel, num, wavelengthID, array);
                Thread.Sleep(100);
                return array[0];
            }
        }

        public float[] QueryOutputPowerOffset(uint deviceID, uint wavelengthID)
        {
            lock (this)
            {
                uint channel = 0u;
                uint num = 2u;
                num = ((!bsingleMode) ? 1u : 2u);
                float[] array = new float[4];
                bool flag = DLL_GetChannelPowerOffset(deviceID, channel, num, wavelengthID, array);
                Thread.Sleep(100);
                return array;
            }
        }

        public void SetInputPowerOffset(uint deviceID, uint ichannel, uint wavelengthID, float offset)
        {
            lock (this)
            {
                uint num = 1u;
                num = (bsingleMode ? 1u : 2u);
                bool flag = DLL_WriteChannelPowerOffset(deviceID, ichannel, num, wavelengthID, new float[4] { offset, 0f, 0f, 0f });
                Thread.Sleep(100);
            }
        }

        public void SetInputPowerOffset(uint deviceID, uint wavelengthID, float[] offset)
        {
            lock (this)
            {
                uint channel = 0u;
                uint num = 1u;
                num = (bsingleMode ? 1u : 2u);
                bool flag = DLL_WriteChannelPowerOffset(deviceID, channel, num, wavelengthID, offset);
                Thread.Sleep(100);
            }
        }

        public float QueryInputPowerOffset(uint deviceID, uint ichannel, uint wavelengthID)
        {
            lock (this)
            {
                uint num = 0u;
                num = (bsingleMode ? 1u : 2u);
                float[] array = new float[4];
                bool flag = DLL_GetChannelPowerOffset(deviceID, ichannel, num, wavelengthID, array);
                Thread.Sleep(100);
                return array[0];
            }
        }

        public float[] QueryInputPowerOffset(uint deviceID, uint wavelengthID)
        {
            lock (this)
            {
                uint channel = 0u;
                uint num = 0u;
                num = (bsingleMode ? 1u : 2u);
                float[] array = new float[4];
                bool flag = DLL_GetChannelPowerOffset(deviceID, channel, num, wavelengthID, array);
                Thread.Sleep(100);
                return array;
            }
        }

        public float QueryAttenuationOffset(uint deviceID, uint ichannel, uint wavelengthID)
        {
            lock (this)
            {
                float[] array = new float[4];
                bool flag = DLL_GetChannelAttenuationOffset(deviceID, ichannel, wavelengthID, array);
                return array[0];
            }
        }

        public float[] QueryAttenuationOffset(uint deviceID, uint wavelengthID)
        {
            lock (this)
            {
                uint channel = 0u;
                float[] array = new float[4];
                bool flag = DLL_GetChannelAttenuationOffset(deviceID, channel, wavelengthID, array);
                return array;
            }
        }

        public void SetAttenuationOffset(uint deviceID, uint ichannel, uint wavelengthID, float offset)
        {
            lock (this)
            {
                if (ichannel != 0)
                {
                    bool flag = DLL_WriteChannelAttenuationOffset(deviceID, ichannel, wavelengthID, new float[4] { offset, 0f, 0f, 0f });
                }
            }
        }

        public float[] SetAttenuationOffset(uint deviceID, uint wavelengthID, float[] offset)
        {
            lock (this)
            {
                uint channel = 0u;
                bool flag = DLL_WriteChannelAttenuationOffset(deviceID, channel, wavelengthID, offset);
                return offset;
            }
        }

        public float QueryAttenuation(uint deviceID, uint ichannel)
        {
            lock (this)
            {
                float[] array = new float[4];
                bool flag = false;
                if (ichannel != 0)
                {
                    int num = 0;
                    do
                    {
                        num++;
                        try
                        {
                            flag = DLL_GetChannelAttenuation(deviceID, ichannel, array);
                        }
                        catch (Exception)
                        {
                        }

                        Thread.Sleep(100);
                    }
                    while (num <= 5 && !flag);
                }

                return array[0];
            }
        }

        public float[] QueryAttenuation(uint deviceID)
        {
            lock (this)
            {
                uint channel = 0u;
                float[] result = new float[4];
                bool flag = false;
                int num = 0;
                do
                {
                    num++;
                    try
                    {
                        flag = DLL_GetChannelAttenuation(deviceID, channel, result);
                    }
                    catch (Exception)
                    {
                    }
                }
                while (num <= 5 && !flag);
                return result;
            }
        }

        public void SetAttenuation(uint deviceID, uint ichannel, float att)
        {
            lock (this)
            {
                bool flag = false;
                int num = 0;
                do
                {
                    num++;
                    float[] array = new float[4] { att, 0f, 0f, 0f };
                    float num2 = QueryAttenuation(deviceID, ichannel);
                    float num3 = ((num2 > 30f) ? (num2 - 18f) : num2);
                    float num4 = ((att > 30f) ? (att - 18f) : att);
                    int millisecondsTimeout = (int)(100f + Math.Abs(num3 - num4) * 60f);
                    try
                    {
                        flag = DLL_SetChannelAttenuation(deviceID, ichannel, array);
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(millisecondsTimeout);
                }
                while (num <= 5 && !flag);
            }
        }

        public void SetAttenuation(uint deviceID, float[] att)
        {
            lock (this)
            {
                uint channel = 0u;
                bool flag = false;
                float[] array = new float[4];
                float[] array2 = new float[4];
                int num = 0;
                do
                {
                    num++;
                    for (uint num2 = 1u; num2 < 5; num2++)
                    {
                        array[num2 - 1] = QueryAttenuation(deviceID, num2);
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        array2[i] = Math.Abs(array[i] - att[i]);
                    }

                    float num3 = array2[0];
                    float[] array3 = array;
                    foreach (float num4 in array3)
                    {
                        if (num4 > num3)
                        {
                            num3 = num4;
                        }
                    }

                    int millisecondsTimeout = (int)(100f + num3 * 60f);
                    try
                    {
                        flag = DLL_SetChannelAttenuation(deviceID, channel, att);
                        Thread.Sleep(millisecondsTimeout);
                    }
                    catch (Exception)
                    {
                    }
                }
                while (num <= 5 && !flag);
            }
        }

        public void SetoutputPower(uint deviceID, uint ichannel, float power)
        {
            float num = float.Epsilon;
            float num2 = float.Epsilon;
            float num3 = float.Epsilon;
            float num4 = 1f;
            int num5 = 0;
            if (bsingleMode)
            {
                float num6 = QueryInputPower(deviceID, ichannel);
                do
                {
                    num = QueryOutputPower(deviceID, ichannel);
                    num2 = Math.Abs(power - num);
                    if (num2 > 40f)
                    {
                        num2 = 40f;
                    }

                    num3 = QueryAttenuation(deviceID, ichannel);
                    num5++;
                    if (!(power <= num6))
                    {
                        continue;
                    }

                    if (power >= num)
                    {
                        if (num3 - num2 <= 0f)
                        {
                            SetAttenuation(deviceID, ichannel, 0f);
                        }
                        else
                        {
                            SetAttenuation(deviceID, ichannel, Math.Abs(Math.Abs(num3) - Math.Abs(num2)));
                        }
                    }
                    else
                    {
                        SetAttenuation(deviceID, ichannel, Math.Abs(Math.Abs(num3) + Math.Abs(num2)));
                    }

                    num4 = Math.Abs(power - QueryOutputPower(deviceID, ichannel));
                }
                while (num5 <= 3 && (double)num4 > 0.05);
                return;
            }

            do
            {
                num = QueryOutputPower(deviceID, ichannel);
                num2 = Math.Abs(power - num);
                if (num2 > 40f)
                {
                    num2 = 40f;
                }

                num3 = QueryAttenuation(deviceID, ichannel);
                num5++;
                if (power >= num)
                {
                    if (num3 - num2 <= 0f)
                    {
                        SetAttenuation(deviceID, ichannel, 0f);
                    }
                    else
                    {
                        SetAttenuation(deviceID, ichannel, Math.Abs(Math.Abs(num3) - Math.Abs(num2)));
                    }
                }
                else
                {
                    SetAttenuation(deviceID, ichannel, Math.Abs(Math.Abs(num3) + Math.Abs(num2)));
                }

                num4 = Math.Abs(power - QueryOutputPower(deviceID, ichannel));
            }
            while (num5 <= 3 && (double)num4 > 0.05);
        }

        public void SetoutputPower(uint deviceID, float[] power)
        {
            float num = 1f;
            for (uint num2 = 0u; num2 < 4; num2++)
            {
                int num3 = 0;
                do
                {
                    float num4 = QueryOutputPower(deviceID, num2 + 1);
                    float num5 = Math.Abs(power[num2] - num4);
                    float num6 = QueryAttenuation(deviceID, num2 + 1);
                    num3++;
                    if (power[num2] >= num4)
                    {
                        if (num6 - num5 <= 0f)
                        {
                            SetAttenuation(deviceID, num2 + 1, 0f);
                        }
                        else
                        {
                            SetAttenuation(deviceID, num2 + 1, Math.Abs(num6 - num5));
                        }
                    }
                    else
                    {
                        SetAttenuation(deviceID, num2 + 1, Math.Abs(num6 + num5));
                    }

                    num = (float)Math.Round(Math.Abs(power[num2] - QueryOutputPower(deviceID, num2 + 1)), 1);
                }
                while (num3 <= 0 && (double)num > 0.05);
            }
        }

        public void OutputControl(uint deviceID, uint channel, bool state)
        {
            lock (this)
            {
                byte[] array = new byte[4];
                if (state)
                {
                    array[0] = 1;
                }

                bool flag = DLL_SetChannelShutter(deviceID, channel, array);
                Thread.Sleep(200);
            }
        }

        public void OutputControl(uint deviceID, bool state)
        {
            lock (this)
            {
                uint channel = 0u;
                byte[] array = new byte[4];
                if (state)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = 1;
                    }
                }

                bool flag = DLL_SetChannelShutter(deviceID, channel, array);
                Thread.Sleep(1000);
            }
        }

        public byte QueryOutputStatus(uint deviceID, uint channel)
        {
            lock (this)
            {
                byte[] array = new byte[4];
                bool flag = DLL_GetChannelShutter(deviceID, channel, array);
                return array[channel - 1];
            }
        }

        public byte[] QueryOutputStatus(uint deviceID)
        {
            lock (this)
            {
                uint channel = 0u;
                byte[] array = new byte[4];
                bool flag = DLL_GetChannelShutter(deviceID, channel, array);
                return array;
            }
        }

        public float QueryInputPower(uint deviceID, uint channel)
        {
            lock (this)
            {
                uint num = 0u;
                num = (bsingleMode ? 1u : 2u);
                float[] array = new float[4];
                bool flag = false;
                int num2 = 0;
                do
                {
                    num2++;
                    try
                    {
                        flag = DLL_GetChannelPower(deviceID, channel, num, array);
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(100);
                }
                while (num2 <= 5 && !flag);
                return array[0];
            }
        }

        public float[] QueryInputPower(uint deviceID)
        {
            lock (this)
            {
                uint channel = 0u;
                uint num = 0u;
                num = (bsingleMode ? 1u : 2u);
                float[] array = new float[4];
                bool flag = false;
                int num2 = 0;
                do
                {
                    num2++;
                    try
                    {
                        flag = DLL_GetChannelPower(deviceID, channel, num, array);
                    }
                    catch (Exception)
                    {
                    }
                }
                while (num2 <= 5 && !flag);
                return array;
            }
        }

        public float QueryOutputPower(uint deviceID, uint channel)
        {
            lock (this)
            {
                bool flag = false;
                float[] array = new float[4];
                uint num = 0u;
                int num2 = 0;
                if (channel != 0)
                {
                    num = ((!bsingleMode) ? 1u : 2u);
                    do
                    {
                        num2++;
                        try
                        {
                            flag = DLL_GetChannelPower(deviceID, channel, num, array);
                            Thread.Sleep(100);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    while (num2 <= 5 && !flag);
                }

                return array[0];
            }
        }

        public float[] QueryOutputPower(uint deviceID)
        {
            lock (this)
            {
                uint channel = 0u;
                uint num = 0u;
                float[] array = new float[4];
                num = ((!bsingleMode) ? 1u : 2u);
                int num2 = 0;
                bool flag = false;
                do
                {
                    num2++;
                    try
                    {
                        flag = DLL_GetChannelPower(deviceID, channel, num, array);
                    }
                    catch (Exception)
                    {
                    }
                }
                while (num2 <= 5 && !flag);
                return array;
            }
        }

        #endregion Function
    }
}