using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
                    _Instance = new Lazy<ThreadFactory>().Value;
                    return _Instance;
                }
            }
        }

        /// <summary>
        /// 是否重启
        /// </summary>
        private bool IsRestar;

        private ConcurrentDictionary<string, TaskModel> Threads = new ConcurrentDictionary<string, TaskModel>();

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
            }
        }


        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        /// <param name="err"></param>
        /// <param name="restart"></param>
        public async void StartWithRestart(Func<Task> action, string key, Action<Exception> err = null, bool restart = true)
        {
            if (!Threads.ContainsKey(key))
            {
                IsRestar = true;
                Threads.TryAdd(key, new TaskModel());
                Threads[key].RunAsync = action;
                Threads[key].ThreadTask = (await Task.Factory.StartNew(async () =>
                {
                    while (!Threads[key].Token.IsCancellationRequested)
                    {
                        try
                        {
                            await action();
                        }
                        catch (Exception ex)
                        {
                            err?.Invoke(ex);
                        }
                    }
                }, Threads[key].Token.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current))
                .ContinueWith((task, obj) =>
                {
                    ThreadStatus(task, obj.ToString());
                    if (IsRestar)
                    {
                        if (restart) StartWithRestart(action, key, err, restart);
                    }
                }, key);
            }
        }

        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        /// <param name="err"></param>
        /// <param name="restart"></param>
        public void StartWithRestart(Action action, string key, Action<Exception> err = null, bool restart = true)
        {
            if (!Threads.ContainsKey(key))
            {
                IsRestar = true;
                Threads.TryAdd(key, new TaskModel());
                Threads[key].Run = action;
                Threads[key].ThreadTask = Task.Factory.StartNew(() =>
                {
                    while (!Threads[key].Token.IsCancellationRequested)
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            err?.Invoke(ex);
                        }
                    }
                }, Threads[key].Token.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current)
                    .ContinueWith((task, obj) =>
                    {
                        ThreadStatus(task, obj.ToString());
                        if (IsRestar)
                        {
                            if (restart) StartWithRestart(action, key, err, restart);
                        }
                    }, key);
            }
        }

        /// <summary>
        /// 获取所有的线程Key
        /// </summary>
        /// <returns></returns>
        public List<string> GetTaskKey() => Threads.Keys.ToList();

        /// <summary>
        /// 停止指定任务
        /// </summary>
        /// <param name="key">任务名</param>
        public void StopTask(string key)
        {
            if (Threads.ContainsKey(key))
            {
                Threads[key].Token?.Cancel();
            }
        }

        /// <summary>
        /// 释放所有线程资源
        /// </summary>
        public void Dispose()
        {
            IsRestar = false;
            for (int i = 0; i < Threads.Count; i++)
            {
                Threads.ElementAt(i).Value.Token.Cancel();
                Threads.ElementAt(i).Value.Run = null;
                Threads.ElementAt(i).Value.RunAsync = null;
            }
            IsRestar = true;
        }

        /// <summary>
        /// 释放所有线程资源排除特定的线程
        /// </summary>
        /// <param name="excludeKey"></param>
        public void ExcludeDispose(string excludeKey)
        {
            IsRestar = false;
            for (int i = 0; i < Threads.Count; i++)
            {
                var data = Threads.ElementAt(i);
                if (!data.Key.Equals(excludeKey))
                {
                    Threads.ElementAt(i).Value.Token.Cancel();
                    Threads.ElementAt(i).Value.Run = null;
                    Threads.ElementAt(i).Value.RunAsync = null;
                }
            }
            IsRestar = true;
        }

        /// <summary>
        /// 释放指定线程
        /// </summary>
        /// <param name="includeKey"></param>
        public void IncludeDispose(string includeKey)
        {
            IsRestar = false;
            Threads[includeKey].Token.Cancel();
            Threads[includeKey].Run = null;
            Threads[includeKey].RunAsync = null;
            Threads.Remove(includeKey, out _);
            IsRestar = true;
        }

    }
}
