using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.LogFramework
{
    /// <summary>
    /// 日志
    /// </summary>
    public static class XLog
    {

        /// <summary>
        /// 注册日志框架
        /// </summary>
        public static ILogger RegisetLog()
        {
            string Template = "[{Timestamp:yyyy年MM月dd日 HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}";
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            //日志
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: Template, theme: SystemConsoleTheme.Colored)
                .WriteTo.File(Path.Combine(dir, "Log_.log"), outputTemplate: Template, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Task.Run(() =>
            {
                while (true)
                {
                    Task.Delay(3600 * 1000).Wait();
                    if (!Directory.Exists(dir)) return;

                    Directory.GetFiles(dir)
                    .Where(t => DateTime.Now.ToString("yyyyMMdd").AsInt() - Path.GetFileName(t).Split("_").LastOrDefault().AsInt() > 3)
                    .ForEnumerEach(item => File.Delete(item));
                }
            });

            return Log.Logger;
        }



        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="info"></param>
        /// <param name="method"></param>
        /// <param name="classes"></param>
        /// <param name="line"></param>
        public static void Debug(this string info, [CallerMemberName] string method = "", [CallerFilePath] string classes = "", [CallerLineNumber] int line = -1)
        {
            var nc = classes.Split(Path.DirectorySeparatorChar.ToString()).Reverse();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\n");
            builder.AppendLine($"[{DateTime.Now:yyyy年MM月dd日 HH:mm:ss}] [Debug]");
            builder.AppendLine("\r");
            builder.AppendLine($"[NameSpace]:{nc.ElementAtOrDefault(1)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Class]:{nc.ElementAtOrDefault(0)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Mthod]:{method}");
            builder.AppendLine("\r");
            builder.AppendLine($"[LineNumber]:{line}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Debug]:{info}");
            builder.AppendLine("\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(builder.ToString());
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="method"></param>
        /// <param name="classes"></param>
        /// <param name="line"></param>
        public static void Info(this string info, [CallerMemberName] string method = "", [CallerFilePath] string classes = "", [CallerLineNumber] int line = -1)
        {
            var nc = classes.Split(Path.DirectorySeparatorChar.ToString()).Reverse();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\n");
            builder.AppendLine($"[NameSpace]:{nc.ElementAtOrDefault(1)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Class]:{nc.ElementAtOrDefault(0)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Mthod]:{method}");
            builder.AppendLine("\r");
            builder.AppendLine($"[LineNumber]:{line}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Info]:{info}");
            builder.AppendLine("\n");
            Log.Logger.Information(builder.ToString());
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="info"></param>
        /// <param name="method"></param>
        /// <param name="classes"></param>
        /// <param name="line"></param>
        public static void Warn(this string info, [CallerMemberName] string method = "", [CallerFilePath] string classes = "", [CallerLineNumber] int line = -1)
        {
            var nc = classes.Split(Path.DirectorySeparatorChar.ToString()).Reverse();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\n");
            builder.AppendLine($"[NameSpace]:{nc.ElementAtOrDefault(1)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Class]:{nc.ElementAtOrDefault(0)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Mthod]:{method}");
            builder.AppendLine("\r");
            builder.AppendLine($"[LineNumber]:{line}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Warn]:{info}");
            builder.AppendLine("\n");
            Log.Logger.Warning(builder.ToString());
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="info"></param>
        /// <param name="method"></param>
        /// <param name="classes"></param>
        /// <param name="line"></param>
        public static void Error(this string info, [CallerMemberName] string method = "", [CallerFilePath] string classes = "", [CallerLineNumber] int line = -1)
        {
            var nc = classes.Split(Path.DirectorySeparatorChar.ToString()).Reverse();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\n");
            builder.AppendLine($"[NameSpace]:{nc.ElementAtOrDefault(1)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Class]:{nc.ElementAtOrDefault(0)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Mthod]:{method}");
            builder.AppendLine("\r");
            builder.AppendLine($"[LineNumber]:{line}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Error]:{info}");
            builder.AppendLine("\n");
            Log.Logger.Error(builder.ToString());
        }

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="info"></param>
        /// <param name="method"></param>
        /// <param name="classes"></param>
        /// <param name="line"></param>
        public static void Fatal(this Exception ex, string info, [CallerMemberName] string method = "", [CallerFilePath] string classes = "", [CallerLineNumber] int line = -1)
        {
            var nc = classes.Split(Path.DirectorySeparatorChar.ToString()).Reverse();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\n");
            builder.AppendLine($"[NameSpace]:{nc.ElementAtOrDefault(1)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Class]:{nc.ElementAtOrDefault(0)}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Mthod]:{method}");
            builder.AppendLine("\r");
            builder.AppendLine($"[LineNumber]:{line}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Fatal]:{info}");
            builder.AppendLine("\r");
            builder.AppendLine($"[Stack]:{ex.StackTrace}");
            builder.AppendLine("\n");
            Log.Logger.Fatal(builder.ToString());
        }
    }
}
