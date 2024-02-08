using Semight.Fwm.Connection.ConnectionAssistantLib.CommunicateInterface;
using Semight.Fwm.Connection.ConnectionAssistantLib.ExtensionMethods;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Semight.Fwm.Connection.ConnectionAssistantLib
{
    /// <summary>
    /// 连接助手
    /// </summary>
    public class ConnectionContext
    {
        private readonly ICommunicate communicateImplement;

        private static readonly object connectionLock = new object();

        public ConnectionContext(ICommunicate communicate)
        {
            communicateImplement = communicate;
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="strIP">IP地址</param>
        /// <param name="Port">端口号</param>
        /// <returns></returns>
        public bool StartConnect(int timeout = 2000)
        {
            var cancellationSource = new CancellationTokenSource();
            try
            {
                var task = Task.Run(() =>
                {
                    lock (connectionLock)
                    {
                        return communicateImplement.Connect();
                    }
                });

                if (!task.JudgeTimeOut(timeout, cancellationSource))
                {
                    throw new TimeoutException("Connection Failed : Time Limit Exceeded! ");
                }
                else
                    return task.GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cancellationSource?.Dispose();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool CloseConnect()
        {
            try
            {
                lock (connectionLock)
                {
                    return communicateImplement.DisConnect();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <exception cref="Exception"></exception>
        public void SendMessage(string msg)
        {
            try
            {
                if (!communicateImplement.Connected)
                    throw new Exception("连接已断开");

                if (string.IsNullOrEmpty(msg))
                    throw new Exception("传入指令为空！");

                communicateImplement.Send(msg);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 发送消息并收到数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timeOut"></param>
        /// <param name="replyLength"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] SendMessageWithData(string msg, int replyLength = 1024, int timeOut = 6000)
        {
            if (!communicateImplement.Connected)
                throw new Exception("连接已断开");

            if (string.IsNullOrEmpty(msg))
                throw new Exception("传入指令为空！");

            var cancelTokenSource = new CancellationTokenSource();
            try
            {
                communicateImplement.Send(msg);

                var task = Task.Run(() => communicateImplement.ReceiveData(replyLength, cancelTokenSource.Token));

                if (!task.JudgeTimeOut(timeOut, cancelTokenSource))
                    throw new TimeoutException("Acquire Data Failed : Time Limit Exceeded!");

                if (task.Result.AsParallel().All(x => x == 0))
                    throw new Exception("Acquire Data Failed : Invalid Data");

                return task.Result;
            }
            catch
            {
                throw;
            }
            finally
            {
                cancelTokenSource?.Dispose();
            }
        }

        /// <summary>
        /// 发送消息并收到消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string SendMessageWithReply(string msg, int timeOut = 2000)
        {
            if (!communicateImplement.Connected)
                throw new Exception("连接已断开");

            if (string.IsNullOrEmpty(msg))
                throw new Exception("传入指令为空！");

            var cancelTokenSource = new CancellationTokenSource();
            try
            {
                communicateImplement.Send(msg);

                var stopwatch = Stopwatch.StartNew();
                var task = Task.Run(() => communicateImplement.ReceiveMessage(cancelTokenSource.Token));
                stopwatch.Stop();

                if (!task.JudgeTimeOut(timeOut, cancelTokenSource))
                    throw new TimeoutException("Acquire Message Failed : Time Limit Exceeded!");

                if (string.IsNullOrWhiteSpace(task.Result))
                    throw new TimeoutException("Acquire Message Failed : Invalid Message!");

                return task.Result;
            }
            catch
            {
                throw;
            }
            finally
            {
                cancelTokenSource?.Dispose();
            }
        }
    }
}