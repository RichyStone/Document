using System;
using System.Threading;
using System.Threading.Tasks;

namespace Semight.Fwm.Connection.ConnectionAssistantLib.ExtensionMethods
{
    public static class ExtensionAdapter
    {
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
    }
}