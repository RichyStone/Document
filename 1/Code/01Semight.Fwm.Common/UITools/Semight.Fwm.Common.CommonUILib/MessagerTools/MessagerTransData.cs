namespace Semight.Fwm.Common.CommonUILib.MessagerTools
{
    /// <summary>
    /// Messager消息传输对象
    /// </summary>
    public class MessagerTransData
    {
        public object? Data { get; set; }

        public bool BoolValue { get; set; }

        public int IntValue { get; set; }

        public string? StrValue { get; set; }
    }

    /// <summary>
    /// Messager消息传输对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessagerTransData<T>
    {
        public T? Value { get; set; }
    }
}