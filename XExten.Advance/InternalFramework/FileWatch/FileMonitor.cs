using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XExten.Advance.InternalFramework.FileWatch
{
    /// <summary>
    /// 文件监听
    /// </summary>
    internal class FileMonitor
    {
        private static FileSystemWatcher SystemWatcher = new FileSystemWatcher();

        /// <summary>
        /// 文件监听(读取)
        /// </summary>
        /// <param name="Module"></param>
        internal static void MonitorRead(FileModule Module)
        {
            IFileProvider FileProvider = new PhysicalFileProvider(Module.Path);
            FileReaderOnChaged(() => FileProvider.Watch(Module.Pattern), () => Module.Action.Invoke());
        }
        protected static IDisposable FileReaderOnChaged(Func<IChangeToken> ChangeTokenProducer, Action ChangeTokenConsumer)
        {
            return ChangeTokenProducer().RegisterChangeCallback(obj =>
            {
                ChangeTokenConsumer();
                FileReaderOnChaged(ChangeTokenProducer, ChangeTokenConsumer);
            }, null);
        }
        /// <summary>
        /// 文件监听(不读取)
        /// </summary>
        /// <param name="Module"></param>
        internal static void MonitorInit(FileModule Module)
        {
            SystemWatcher.BeginInit();
            SystemWatcher.Filter = Module.Pattern;
            SystemWatcher.IncludeSubdirectories = Module.IsInclude;
            SystemWatcher.EnableRaisingEvents = true;
            SystemWatcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;
            SystemWatcher.Path = Module.Path;
            SystemWatcher.Created += (sender, events) => Module.Events.Invoke(sender, events);
            SystemWatcher.Deleted += (sender, events) => Module.Events.Invoke(sender, events);
            SystemWatcher.Changed += (sender, events) => Module.Events.Invoke(sender, events);
            SystemWatcher.Renamed += (sender, events) => Module.Events.Invoke(sender, events);
            SystemWatcher.EndInit();
        }
    }
}
