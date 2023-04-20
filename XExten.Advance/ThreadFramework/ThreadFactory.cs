using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XExten.Advance.ThreadFramework
{
    /// <summary>
    /// 线程工厂
    /// </summary>
    public class ThreadFactory
    {
        private static ThreadFactory _Instance;
        /// <summary>
        /// 实例
        /// </summary>
        public static ThreadFactory Instance
        {
            get
            {
                if (_Instance != null) return _Instance;
                else
                {
                    _Instance = new ThreadFactory();
                    return _Instance;
                }
            }
        }
        /// <summary>
        /// 等待时间
        /// </summary>
        public int RestartInterval { get; set; } = 2000;
        ConcurrentDictionary<string, TaskModel> Threads = new ConcurrentDictionary<string, TaskModel>();
        ConcurrentDictionary<string, Action> Actions = new ConcurrentDictionary<string, Action>();

        /// <summary>
        /// 带重启的线程循环任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        /// <param name="isRestart"></param>
        /// <param name="RunComplete"></param>
        public void StartWithRestart(Action action, string key, bool isRestart = false, Action RunComplete = null)
        {
            if (!Threads.ContainsKey(key))
            {
                Threads.TryAdd(key, new TaskModel());
                Threads[key].RunTask = action;
                Threads[key].ThreadTask = Task.Factory.StartNew(() =>
                {
                    Thread.CurrentThread.Name = key;
                    while (!Threads[key].Token.IsCancellationRequested)
                    {
                        if (isRestart)
                        {
                            try
                            {
                                Threads[key].RunTask?.Invoke();
                            }
                            catch
                            {
                                if (isRestart) Thread.Sleep(RestartInterval);
                            }
                        }
                        else Threads[key].RunTask?.Invoke();
                    }
                }, Threads[key].Token.Token).ContinueWith((task, obj) =>
                {
                    ThreadStatus(task, obj.ToString());
                    if (RunComplete != null) RunComplete();
                }, key);
            }
        }

        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        public void Start(Action action, string key)
        {
            if (!Threads.ContainsKey(key))
            {
                Threads.TryAdd(key, new TaskModel());
                Threads[key].RunTask = action;
                Threads[key].ThreadTask = Task.Factory.StartNew(() =>
                {
                    Thread.CurrentThread.Name = key;
                    while (!Threads[key].Token.IsCancellationRequested)
                    {
                        Threads[key].RunTask?.Invoke();
                        break;
                    }
                }, Threads[key].Token.Token).ContinueWith((task, obj) =>
                {
                    ThreadStatus(task, obj.ToString());
                }, key);
            }
        }

        /// <summary>
        /// 停止指定任务
        /// </summary>
        /// <param name="key">任务名</param>
        /// <param name="ExitCallback">任务结束的回调</param>
        public void StopTask(string key, Action ExitCallback = null)
        {
            if (Threads.ContainsKey(key))
            {
                Actions.TryAdd(key, ExitCallback);
                Threads[key].Token?.Cancel();
            }
        }

        /// <summary>
        /// 释放所有线程资源
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < Threads.Count; i++)
            {
                Threads.ElementAt(i).Value.Token.Cancel();
                Threads.ElementAt(i).Value.RunTask = null;
            }
        }

        /// <summary>
        /// 判断指定线程是否完成
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsComplete(string key)
        {
            if (Threads.ContainsKey(key)) return Threads[key].ThreadTask.IsCompleted;
            return false;
        }

        private void ThreadStatus(Task task, string key)
        {
            bool IsRemove = false;
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    IsRemove = true;
                    break;
                case TaskStatus.Faulted:
                    IsRemove = true;
                    break;
                case TaskStatus.Canceled:
                    IsRemove = true;
                    break;
                default:
                    break;
            }

            if (IsRemove)
            {
                if (Threads.ContainsKey(key)) Threads.TryRemove(key, out _);
                if (Actions.ContainsKey(key))
                {
                    Actions[key]?.Invoke();
                    Actions.TryRemove(key, out _);
                }

            }
        }
    }
}
