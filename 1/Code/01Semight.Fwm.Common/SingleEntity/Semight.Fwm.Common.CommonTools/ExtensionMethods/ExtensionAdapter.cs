using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Semight.Fwm.Common.CommonTools.ExtensionMethods
{
    public static class ExtensionAdapter
    {
        #region TimeOut

        public static bool JudgeTimeOut(this Task task, int TimeOout)
        {
            var completed = Task.WhenAny(task, Task.Delay(TimeOout));
            return completed.Result == task;
        }

        public static bool JudgeTimeOut(this Task task, TimeSpan TimeOout)
        {
            var completed = Task.WhenAny(task, Task.Delay(TimeOout));
            return completed.Result == task;
        }

        public static bool JudgeTimeOut(this Task task, int TimeOout, CancellationTokenSource tokenSource)
        {
            var completed = Task.WhenAny(task, Task.Delay(TimeOout));
            if (completed.Result != task)
            {
                tokenSource.Cancel();
                return false;
            }
            else
                return true;
        }

        public static bool JudgeTimeOut(this Task task, TimeSpan TimeOout, CancellationTokenSource tokenSource)
        {
            var completed = Task.WhenAny(task, Task.Delay(TimeOout));
            if (completed.Result != task)
            {
                tokenSource.Cancel();
                return false;
            }
            else
                return true;
        }

        public static bool JudgeTimeOut(this Thread thread, int TimeOout)
        {
            if (!thread.Join(TimeOout))
            {
                thread.Interrupt();
                return false;
            }
            else
                return true;
        }

        public static bool JudgeTimeOut(this Thread thread, TimeSpan TimeOout)
        {
            if (!thread.Join(TimeOout))
            {
                thread.Interrupt();
                return false;
            }
            else
                return true;
        }

        #endregion TimeOut

        #region DeepClone

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static T DeepClone<T>(this T source) where T : class
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            binaryFormatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            var result = (T)binaryFormatter.Deserialize(stream);
            stream.Close();
            stream.Dispose();

            return result;
        }

        #endregion DeepClone

        #region Awaiter

        public static TaskAwaiter GetAwaiter(this TimeSpan span) => Task.Delay(span).GetAwaiter();

        public static TaskAwaiter GetAwaiter(this int milliSeconds) => Task.Delay(milliSeconds).GetAwaiter();

        #endregion Awaiter
    }
}