using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyModel;
using Synctool.InternalFunc.Office;
using Synctool.InternalFunc.Office.Common;
using Synctool.Linq;

namespace Synctool.Static
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
        /// 将小写的金钱转换成大写的金钱
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string ConvCHN(decimal money)
        {
            string[] numList = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[] unitList = { "分", "角", "元", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟" };
            if (money == 0) return "零元整";
            StringBuilder strMoney = new StringBuilder();
            string strNum = decimal.Truncate(money * 100).ToString();
            int len = strNum.Length;
            int zero = 0;
            for (int i = 0; i < len; i++)
            {
                int num = int.Parse(strNum.Substring(i, 1));
                int unitNum = len - i - 1;
                if (num == 0)
                {
                    zero++;
                    if (unitNum == 2 || unitNum == 6 || unitNum == 10)
                    {
                        if (unitNum == 2 || zero < 4)
                            strMoney.Append(unitList[unitNum]);
                        zero = 0;
                    }
                }
                else
                {
                    if (zero > 0)
                    {
                        strMoney.Append(numList[0]);
                        zero = 0;
                    }
                    strMoney.Append(numList[num]);
                    strMoney.Append(unitList[unitNum]);
                }
            }
            if (zero > 0)
                strMoney.Append("整");
            return strMoney.ToString();
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
            try
            {
                foreach (char ch in param)
                {
                    Result += Has[ch].ToString();
                    Result += "0";
                }
            }
            catch { return "not supported char!"; }
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
            TimeSpan Span = new TimeSpan(long.Parse(TimeStamp + "0000000"));
            return StartTime.Add(Span);
        }

        /// <summary>
        /// 时间转时间戳
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <returns></returns>
        public static string ConvertDateTime(DateTime TimeStamp)
        {
            DateTime StartTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc, TimeZoneInfo.Local);
            return ((int)((TimeStamp - StartTime).TotalMilliseconds / 1000)).ToString();
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
            string randomNum = "";
            int flag = -1;//记录上次随机数的数值，尽量避免产生几个相同的随机数
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                if (flag != -1)
                {
                    rand = new Random(i * flag * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(60);
                if (flag == t)
                {
                    return VerifyCode();
                }
                flag = t;
                randomNum += CharArray[t];
            }
            return randomNum;
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
        /// 导出EXCEL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data">需要被导出的数据</param>
        /// <param name="Types">Excel类型</param>
        /// <param name="SheetName">工作表名称</param>
        /// <param name="stream">流</param>
        /// <param name="action">自定义导出</param>
        /// <param name="DateFormat">事件格式</param>
        public static void ExportExcel<T>(IEnumerable<T> Data, ExcelType Types, string SheetName,
            Action<Stream> action, Stream stream = null, string DateFormat = "yyyy-MM-dd") where T : class, new()
        {
            ExcelFactory.ExportExcel(Data, Types, SheetName, action, stream, DateFormat);
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
        }
    }
}
