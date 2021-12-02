using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using Chinese;
using Microsoft.Extensions.DependencyModel;
using Polly;
using XExten.Advance.InternalFramework.Express;
using XExten.Advance.InternalFramework.Express.Common;
using XExten.Advance.InternalFramework.FileWatch;
/*using XExten.Advance.InternalFramework.Office;
using XExten.Advance.InternalFramework.Office.Common;*/
using XExten.Advance.LinqFramework;

namespace XExten.Advance.StaticFramework
{
    /// <summary>
    /// StaticUtil
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
        /// 随机手机
        /// </summary>
        /// <returns></returns>
        public static string RandomPho()
        {
            string[] phos = "134,135,136,137,138,139,150,151,152,157,158,159,130,131,132,155,156,133,153,180,181,182,183,185,186,176,187,188,189,177,178".Split(',');
            Random random = new Random();
            int index = random.Next(0, phos.Length - 1);
            return phos[index] + (random.Next(100, 888) + 10000).ToString().Substring(1) + (random.Next(1, 9100) + 10000).ToString().Substring(1);
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
        /// 返回条形码
        /// </summary>
        /// <param name="param"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string BarHtml(string param, int width, int height)
        {
            Hashtable Has = new Hashtable();

            #region 39码 12位

            Has.Add('A', "110101001011");
            Has.Add('B', "101101001011");
            Has.Add('C', "110110100101");
            Has.Add('D', "101011001011");
            Has.Add('E', "110101100101");
            Has.Add('F', "101101100101");
            Has.Add('G', "101010011011");
            Has.Add('H', "110101001101");
            Has.Add('I', "101101001101");
            Has.Add('J', "101011001101");
            Has.Add('K', "110101010011");
            Has.Add('L', "101101010011");
            Has.Add('M', "110110101001");
            Has.Add('N', "101011010011");
            Has.Add('O', "110101101001");
            Has.Add('P', "101101101001");
            Has.Add('Q', "101010110011");
            Has.Add('R', "110101011001");
            Has.Add('S', "101101011001");
            Has.Add('T', "101011011001");
            Has.Add('U', "110010101011");
            Has.Add('V', "100110101011");
            Has.Add('W', "110011010101");
            Has.Add('X', "100101101011");
            Has.Add('Y', "110010110101");
            Has.Add('Z', "100110110101");
            Has.Add('0', "101001101101");
            Has.Add('1', "110100101011");
            Has.Add('2', "101100101011");
            Has.Add('3', "110110010101");
            Has.Add('4', "101001101011");
            Has.Add('5', "110100110101");
            Has.Add('6', "101100110101");
            Has.Add('7', "101001011011");
            Has.Add('8', "110100101101");
            Has.Add('9', "101100101101");
            Has.Add('+', "100101001001");
            Has.Add('-', "100101011011");
            Has.Add('*', "100101101101");
            Has.Add('/', "100100101001");
            Has.Add('%', "101001001001");
            Has.Add('$', "100100100101");
            Has.Add('.', "110010101101");
            Has.Add(' ', "100110101101");

            #endregion 39码 12位

            param = "*" + param.ToUpper() + "*";
            string Result = "";
            TryCatch(() =>
            {
                foreach (char ch in param)
                {
                    Result += Has[ch].ToString();
                    Result += "0";
                }
            }, ex => throw new Exception("not supported char!"));
            string Html = "";
            string Color;
            foreach (char res in Result)
            {
                Color = res == '0' ? "#FFFFFF" : "#000000";
                Html += $"<div style=\"width:{width}px;height:{height}px;float:left;background:{Color}\"></div>";
            }
            Html += @"<div style='clear:both'></div>";
            int Len = Has['*'].ToString().Length;
            foreach (char item in param)
            {
                Html += $"<div style=\"width:{(width * (Len + 1))}px;float:left;color:#000000;text-align:center;\">{item}</div>";
            }
            Html += @"<div style=clear:both></div>";
            return $"<div style=\"background:#FFFFFF;padding:5px;font-size:{(width * 5)}px;font-family:楷体;\">{Html}</div>";
        }

        /// <summary>
        /// 把网页内容转为文本
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HText(string html)
        {
            string[] aryReg ={
            @"<script[^>]*?>.*?</script>",
            @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
            @"([\r\n])[\s]+",
            @"&(quot|#34);",
            @"&(amp|#38);",
            @"&(lt|#60);",
            @"&(gt|#62);",
            @"&(nbsp|#160);",
            @"&(iexcl|#161);",
            @"&(cent|#162);",
            @"&(pound|#163);",
            @"&(copy|#169);",
            @"&#(\d+);",
            @"-->",
            @"<!--.*\n"
            };

            string strOutput = html;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, string.Empty);
            }

            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");


            return strOutput;
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
        /// 创建一个验证吗
        /// </summary>
        /// <returns></returns>
        public static string VerifyCode()
        {
            char[] CharArray ={
                '1','2','3','4','5','6','7','8','9',
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            };
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                var index = rand.Next(60);
                sb.Append(CharArray[index]);
            }
            return sb.ToString();
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

        /*/// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data">需要被导出的数据</param>
        /// <param name="Types">Excel类型</param>
        /// <param name="SheetName">工作表名称</param>
        /// <param name="stream">流</param>
        /// <param name="action">自定义导出</param>
        /// <param name="footer">页脚内容</param>
        /// <param name="DateFormat">时间格式</param>
        public static void ExportExcel<T>(IEnumerable<T> Data, ExcelType Types, string SheetName,
            Action<Stream> action, dynamic footer = null, Stream stream = null, string DateFormat = "yyyy-MM-dd") where T : class, new()
        {
            ExcelFactory.ExportExcel(Data, Types, SheetName, action, footer, stream, DateFormat);
        }

        /// <summary>
        /// 导入EXCEL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fs">流</param>
        /// <param name="Types">类型</param>
        /// <param name="HasPageFooter">文档是否有页脚</param>
        /// <param name="SheetIndex">数据表索引</param>
        /// <returns></returns>
        public static List<T> ImportExcel<T>(Stream fs, ExcelType Types,
            bool HasPageFooter = false, int SheetIndex = 0) where T : new()
        {
            return ExcelFactory.ImportExcel<T>(fs, Types, HasPageFooter, SheetIndex);
        }*/

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonValue"></param>
        /// <param name="Param"></param>
        public static void SetProptertiesValue<T>(Dictionary<string, object> JsonValue, T Param) where T : class, new()
        {
            Expsion.SetProptertiesValue(JsonValue, Param);
        }

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
        public static Expression<Func<T, object>> GetExpression<T>(params string[] PropertyName) where T : class, new()
        {
            return Expsion.GetExpression<T>(PropertyName);
        }

        /// <summary>
        ///  返回一个bool表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Property"></param>
        /// <param name="Data"></param>
        /// <param name="QueryType"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetExpression<T>(string Property, object Data, QType QueryType)
        {
            return Expsion.GetExpression<T>(Property, Data, QueryType);
        }

        /// <summary>
        /// 无返回重试
        /// </summary>
        /// <param name="action"></param>
        /// <param name="handle"></param>
        /// <param name="Times"></param>
        public static void DoRetry(Action action, Action handle = null, int Times = 3)
        {
            Policy.Handle<Exception>().Retry(Times, (Ex, Count, Context) =>
            {
                handle?.Invoke();
            }).Execute(action);
        }
        /// <summary>
        /// 有返回重试
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="handle"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public static T DoRetry<T>(Func<T> action,Action handle=null, int Times = 3)
        {
            return Policy.Handle<Exception>().Retry(Times, (Ex, Count, Context) =>
            {
                handle?.Invoke();
            }).Execute(action);
        }
        /// <summary>
        /// 短路由无返回
        /// </summary>
        /// <param name="action"></param>
        /// <param name="Times"></param>
        /// <param name="Seconds"></param>
        public static void DoRetryBreak(Action action, int Times = 3, int Seconds = 60)
        {
            Policy.Handle<Exception>().CircuitBreaker(Times, TimeSpan.FromSeconds(Seconds)).Execute(action);
        }
        /// <summary>
        /// 短路由有返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="Times"></param>
        /// <param name="Seconds"></param>
        /// <returns></returns>
        public static T DoRetryBreak<T>(Func<T> action, int Times = 3, int Seconds = 60)
        {
            return Policy.Handle<Exception>().CircuitBreaker(Times, TimeSpan.FromSeconds(Seconds)).Execute(action);
        }
    }
}
