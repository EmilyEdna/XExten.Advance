using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace XExten.Advance.ThreadFramework
{
    internal class TaskModel
    {
        /// <summary>
        /// 线程
        /// </summary>
        public Task ThreadTask { get; set; }
        public CancellationTokenSource Token { get; set; } = new CancellationTokenSource();
        /// <summary>
        /// 需要执行的代码
        /// </summary>
        public Action RunTask { get; set; }
    }
}
