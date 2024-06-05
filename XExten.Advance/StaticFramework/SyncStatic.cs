using Chinese;
using Microsoft.Extensions.DependencyModel;
using Polly;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using XExten.Advance.AopFramework;
using XExten.Advance.InternalFramework.Express;
using XExten.Advance.InternalFramework.Express.Common;
using XExten.Advance.InternalFramework.FileHandle;
using XExten.Advance.InternalFramework.FileWatch;
using XExten.Advance.InternalFramework.Securities;
using XExten.Advance.InternalFramework.Securities.Common;
using XExten.Advance.InternalFramework.Translate;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;


namespace XExten.Advance.StaticFramework
{
    /// <summary>
    /// StaticUtile
    /// </summary>
    public static class SyncStatic
    {
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="action"></param>
        /// <param name="ection"></param>
        /// <param name="final"></param>
        public static void TryCatch(Action action, Action<Exception> ection, Action final = null)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                ection.Invoke(ex);
            }
            finally
            {
                final?.Invoke();
            }
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="ection"></param>
        /// <param name="final"></param>
        /// <returns></returns>
        public static T TryCatch<T>(Func<T> action, Func<Exception, T> ection, Action final = null)
        {
            try
            {
                return action.Invoke();
            }
            catch (Exception ex)
            {
                return ection.Invoke(ex);
            }
            finally
            {
                final?.Invoke();
            }
        }

        /// <summary>
        /// 获取中文拼音
        /// </summary>
        /// <param name="Chinese"></param>
        /// <param name="format"></param>
        /// <param name="chineseType"></param>
        /// <returns></returns>
        public static string CHNPinYin(string Chinese, PinyinFormat format, ChineseTypes chineseType = ChineseTypes.Simplified)
        {
            return Pinyin.GetString(chineseType, Chinese, format);
        }

        /// <summary>
        /// 读取Xml
        /// </summary>
        /// <param name="NodeItem"></param>
        /// <param name="NodeKey"></param>
        /// <param name="NodeValue"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ReadXml(string NodeItem, string NodeKey, string NodeValue)
        {
            Dictionary<String, String> XmlMap = new Dictionary<String, String>();
            string XmlPath = Directory.GetDirectories(Directory.GetCurrentDirectory()).Where(t => t.ToLower().Contains("xml")).FirstOrDefault();
            string[] XmlFilePath = Directory.GetFiles(XmlPath, "*.xml");
            XmlFilePath.ForArrayEach<string>(item =>
            {
                XElement XNodes = XElement.Load(item);
                var elements = XNodes.Elements(NodeItem.IsNullOrEmpty() ? "Item" : NodeItem);
                if (elements.Count() > 0)
                {
                    elements.ForEnumerEach<XElement>(element =>
                    {
                        string Key = element.Element(NodeKey.IsNullOrEmpty() ? "Key" : NodeKey).Value;
                        string Value = element.Element(NodeValue.IsNullOrEmpty() ? "Value" : NodeValue).Value;
                        if (!XmlMap.ContainsKey(Key))
                            XmlMap.Add(Key, Value);
                    });
                }
            });
            return XmlMap;
        }

        /// <summary>
        /// 时间戳转时间
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static DateTime ConvertStamptime(string TimeStamp)
        {
            DateTime StartTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc, TimeZoneInfo.Local);
            if (TimeStamp.Length == 10)
            {
                TimeSpan Span = new TimeSpan(long.Parse(TimeStamp + "0000000"));
                return StartTime.Add(Span);
            }
            else
            {
                TimeSpan Span = new TimeSpan(long.Parse(TimeStamp + "0000"));
                return StartTime.Add(Span);
            }
        }

        /// <summary>
        /// 时间转时间戳
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <param name="RtSecond">True 返回秒 False 返回毫秒</param>
        /// <returns></returns>
        public static string ConvertDateTime(DateTime TimeStamp, bool RtSecond = true)
        {
            DateTime StartTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc, TimeZoneInfo.Local);
            if (RtSecond)
                return ((long)((TimeStamp - StartTime).TotalMilliseconds / 1000)).ToString();
            else
                return ((long)(TimeStamp - StartTime).TotalMilliseconds).ToString();
        }

        /// <summary>
        /// 将对象序列化为XML(XmlSerializer)
        /// 说明：此方法序列化复杂类，如果没有声明XmlInclude等特性，可能会引发“使用 XmlInclude 或 SoapInclude 特性静态指定非已知的类型。”的错误。
        /// (Description: This method serializes complex classes. If you do not declare features such as XmlInclude, you may get an error "Use the XmlInclude or SoapInclude feature to statically specify a non-known type.")
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public static string XmlSerializer<T>(T Param)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(typeof(T));
            try
            {
                //序列化对象
                xml.Serialize(Stream, Param);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader reader = new StreamReader(Stream);
            string str = reader.ReadToEnd();
            reader.Dispose();
            Stream.Dispose();
            return str;
        }

        /// <summary>
        /// 反系列化XML(XmlDeserialize)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string Xml)
        {
            using (StringReader reader = new StringReader(Xml))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }

        /// <summary>
        /// 获取指定的程序集
        /// </summary>
        /// <param name="AssemblyName"></param>
        /// <returns></returns>
        public static List<Assembly> Assembly(string AssemblyName)
        {
            List<Assembly> Assemblies = new List<Assembly>();
            var lib = DependencyContext.Default;
            var libs = lib.CompileLibraries.Where(t => t.Name.Contains(AssemblyName)).ToList();
            foreach (var item in libs)
            {
                Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(item.Name));
                Assemblies.Add(assembly);
            }
            return Assemblies;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonValue"></param>
        /// <param name="Param"></param>
        public static void SetPropertiesValue<T>(Dictionary<string, object> JsonValue, T Param) where T : class, new() => Expsion.SetProptertiesValue(JsonValue, Param);

        /// <summary>
        /// 文件监听
        /// </summary>
        /// <param name="Module"></param>
        public static void FileMonitors(FileModule Module)
        {
            if (!Module.Module)
                FileMonitor.MonitorInit(Module);
            else
                FileMonitor.MonitorRead(Module);
        }

        /// <summary>
        ///  返回一个new表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> GetExpression<T>(params string[] PropertyName) where T : class, new() => Expsion.GetExpression<T>(PropertyName);


        /// <summary>
        ///  返回一个bool表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Property"></param>
        /// <param name="Data"></param>
        /// <param name="QueryType"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetExpression<T>(string Property, object Data, QType QueryType) => Expsion.GetExpression<T>(Property, Data, QueryType);

        /// <summary>
        /// 等待重试无返回
        /// </summary>
        /// <param name="action"></param>
        /// <param name="exhandle"></param>
        /// <param name="Times"></param>
        /// <param name="WaitSpan"></param>
        public static void DoRetryWait(Action action, Action<Exception, int, int> exhandle = null, int Times = 3, int WaitSpan = 10)
        {
            Policy.Handle<Exception>().WaitAndRetry(Times, (span) => TimeSpan.FromSeconds(WaitSpan), (ex, span, count, context) => exhandle?.Invoke(ex, count, Times)).Execute(action);
        }

        /// <summary>
        /// 等待重试有返回
        /// </summary>
        /// <param name="action"></param>
        /// <param name="exhandle"></param>
        /// <param name="Times"></param>
        /// <param name="WaitSpan"></param>
        public static T DoRetryWait<T>(Func<T> action, Action<Exception, int, int> exhandle = null, int Times = 3, int WaitSpan = 10)
        {
            return Policy.Handle<Exception>().WaitAndRetry(Times, (span) => TimeSpan.FromSeconds(WaitSpan), (ex, span, count, context) => exhandle?.Invoke(ex, count, Times)).Execute(action);
        }

        /// <summary>
        /// 无返回重试
        /// </summary>
        /// <param name="action"></param>
        /// <param name="exhandle"></param>
        /// <param name="Times"></param>
        public static void DoRetry(Action action, Action<Exception, int, int> exhandle = null, int Times = 3)
        {
            Policy.Handle<Exception>().Retry(Times, (ex, count, context) => exhandle?.Invoke(ex, count, Times)).Execute(action);
        }

        /// <summary>
        /// 有返回重试
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="exhandle"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public static T DoRetry<T>(Func<T> action, Action<Exception, int, int> exhandle = null, int Times = 3)
        {
            return Policy.Handle<Exception>().Retry(Times, (ex, count, context) => exhandle?.Invoke(ex, count, Times)).Execute(action);
        }

        /// <summary>
        /// 短路由无返回
        /// </summary>
        /// <param name="action"></param>
        /// <param name="exhandle"></param>
        /// <param name="Times"></param>
        /// <param name="Seconds"></param>
        public static void DoRetryBreak(Action action, Action<Exception> exhandle = null, int Times = 3, int Seconds = 60)
        {
            Policy.Handle<Exception>().CircuitBreaker(Times, TimeSpan.FromSeconds(Seconds), (ex, time) => exhandle?.Invoke(ex), null).Execute(action);
        }

        /// <summary>
        /// 短路由有返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="exhandle"></param>
        /// <param name="Times"></param>
        /// <param name="Seconds"></param>
        /// <returns></returns>
        public static T DoRetryBreak<T>(Func<T> action, Action<Exception> exhandle = null, int Times = 3, int Seconds = 60)
        {
            return Policy.Handle<Exception>().CircuitBreaker(Times, TimeSpan.FromSeconds(Seconds), (ex, time) => exhandle?.Invoke(ex), null).Execute(action);
        }

        /// <summary>
        /// lz加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Compress(string input, SecurityType type = SecurityType.Normal)
        {
            return type switch
            {
                SecurityType.Base64 => LzString.CompressToBase64(input),
                SecurityType.UTF16 => LzString.CompressToUTF16(input),
                SecurityType.EncodedURI => LzString.CompressToEncodedURIComponent(input),
                SecurityType.Uint8 => Encoding.UTF8.GetString(LzString.CompressToUint8Array(input)),
                SecurityType.Normal => LzString.Compress(input),
                _ => LzString.Compress(input),
            };
        }

        /// <summary>
        /// lz解密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Decompress(string input, SecurityType type = SecurityType.Normal)
        {
            return type switch
            {
                SecurityType.Base64 => LzString.DecompressFromBase64(input),
                SecurityType.UTF16 => LzString.DecompressFromUTF16(input),
                SecurityType.EncodedURI => LzString.DecompressFromEncodedURIComponent(input),
                SecurityType.Uint8 => LzString.DecompressFromUint8Array(Encoding.UTF8.GetBytes(input)),
                SecurityType.Normal => LzString.Decompress(input),
                _ => LzString.Decompress(input),
            };
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CreateFile(string path) => FileManager.CreateFile(path);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CreateDir(string path) => FileManager.CreateDir(path);

        /// <summary>
        /// 创建文件夹下文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CreateFull(string path) => FileManager.CreateFull(path);

        /// <summary>
        /// 删除所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void DeleteFolder(string path) => FileManager.DeleteFolder(path);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void DeleteFile(string path)=> FileManager.DeleteFile(path);

        /// <summary>
        /// 删除目录下所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="condition">保留文件排除条件</param>
        public static void DeleteDirFile(string path, params string[] condition) => FileManager.DeleteDirFile(path, condition);

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string WriteFile(byte[] bytes, string path) => FileManager.WriteFile(bytes, path);

        /// <summary>
        /// 写入目录文件
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string WriteDirFile(byte[] bytes, string path) => FileManager.WriteDirFile(bytes, path);

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadFile(string path) => FileManager.ReadFile(path);

        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="query"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string Translate(string query, string from = "auto", string to = "zh-CN")=> Translation.Translatate(query, from, to);

        /// <summary>
        /// 注册Aop代理
        /// </summary>
        /// <returns></returns>
        public static void RegistAop<T>()
        {
            var Class = typeof(T);
            Class.Assembly.GetTypes()
               .Where(t => t.IsClass)
               .Where(t => t.GetInterfaces().Contains(Class))
               .ForEnumerEach(item =>
               {
                   IocDependency.Register(Class, AopProxy.CreateProxyOfRealize(Class, item).GetType());
               });
        }

        /// <summary>
        /// 创建RSAKey
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="multiple"></param>
        public static void GenerateRSAKey(string savePath, int multiple = 2)=> RSAGenerate.GenerateKey(savePath, multiple);

        /// <summary>
        /// RSA加解密
        /// </summary>
        /// <param name="input">输入的数据</param>
        /// <param name="type">0 表示解密 1表示加密</param>
        /// <returns></returns>
        public static string RSA(string input, bool type) => type ? RSAGenerate.RSAEncrypt(input) : RSAGenerate.RSADecrypt(input);

        /// <summary>
        ///程序多开检测
        /// </summary>
        /// <returns></returns>
        public static bool MultiOpenCheck() =>
            Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.ModuleName)).Length > 1;

    }
}
