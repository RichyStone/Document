using System;
using System.IO;
using System.Xml.Serialization;

namespace Semight.Fwm.Common.CommonTools.Serialize
{
    public static class XmlSerializeHelper
    {
        /// <summary>
        /// 把对象使用XML格式写入本地文件
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj">对象</param>
        /// <param name="filePath">文件地址</param>
        /// <param name="extension">文件后缀名，默认为.xml</param>
        /// <returns>序列化是否成功</returns>
        public static bool WriteFile<TObject>(TObject obj, string filePath, string extension = ".xml")
            where TObject : class
        {
            try
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (File.Exists(filePath + extension))
                    File.Delete(filePath + extension);

                var path = Path.ChangeExtension(filePath, extension);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());

                using (Stream stream = new FileStream(path, FileMode.CreateNew))
                    serializer.Serialize(stream, obj);

                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 读取文件并反序列化
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="filePath">文件地址</param>
        /// <param name="obj">反序列化得到的对象</param>
        /// <param name="extension">文件后缀名</param>
        /// <returns>反序列化是否成功</returns>
        public static bool ReadFile<TObject>(string filePath, out TObject obj, string extension = ".xml")
            where TObject : class
        {
            obj = null;

            try
            {
                var path = string.IsNullOrEmpty(extension) ? filePath : Path.ChangeExtension(filePath, extension);

                if (File.Exists(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TObject));

                    using (Stream stream = new FileStream(path, FileMode.Open))
                    {
                        var ret = serializer.Deserialize(stream);
                        if (ret is TObject)
                            obj = ret as TObject;
                        else
                            throw new Exception("反序列化失败");
                    }
                }
                else
                    throw new Exception($"文件路径不存在:{path}");

                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}