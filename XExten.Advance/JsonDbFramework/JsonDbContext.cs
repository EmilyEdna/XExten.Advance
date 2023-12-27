using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.JsonDbFramework
{
    /// <summary>
    /// JsonDb上下文
    /// </summary>
    public class JsonDbContext
    {
        private string DbFile;
        private string WaitWrite;
        private string Directorys;
        private string CurrentJson;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="DbFile"></param>
        /// <param name="LoadOnce"></param>
        public JsonDbContext(string DbFile, bool LoadOnce = true)
        {
            this.DbFile = DbFile;
            if (LoadOnce && LoadDbFile())
                LoadInMonery();
        }

        private bool LoadDbFile()
        {
            try
            {
                Directorys = Path.GetDirectoryName(DbFile);
                if (!Directory.Exists(Directorys))
                    Directory.CreateDirectory(Directorys);
                if (!File.Exists(DbFile))
                    File.Create(DbFile).Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void LoadInMonery()
        {
            FileStream fileStream = new FileStream(DbFile, FileMode.Open, FileAccess.Read);
            byte[] array = new byte[fileStream.Length];
            fileStream.Read(array, 0, array.Length);
            fileStream.Close();
            if (array.Length != 0)
                CurrentJson = Encoding.UTF8.GetString(array);
        }

        /// <summary>
        /// 实时数据到内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public JsonDbHandle<T> RuntimeLoad<T>()
        {
            FileStream fileStream = new FileStream(DbFile, FileMode.Open, FileAccess.Read);
            byte[] array = new byte[fileStream.Length];
            fileStream.Read(array, 0, array.Length);
            fileStream.Close();
            List<T> Res = new List<T>();
            if (array.Length != 0)
                Res = Encoding.UTF8.GetString(array).ToModel<List<T>>();
            return new JsonDbHandle<T>(Res, this);
        }

        /// <summary>
        /// 加载数据到内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public JsonDbHandle<T> LoadInMonery<T>() 
        {
            List<T> Res = new List<T>();
            if (!CurrentJson.IsNullOrEmpty())
                Res= CurrentJson.ToModel<List<T>>();
            return new JsonDbHandle<T>(Res, this);
        }

        internal void SetString<T>(List<T> JsonData)
        {
            CurrentJson= WaitWrite = JsonData.ToJson();
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> SaveChange<T>()
        {
            if (WaitWrite.IsNullOrEmpty()) return null;
            try
            {
                var bytes = Encoding.UTF8.GetBytes(WaitWrite);
                Stream stream = new MemoryStream(bytes);
                FileStream fs = new FileStream(DbFile, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(fs);
                writer.Write(bytes);
                writer.Close();
                fs.Close();
                stream.Close();
                stream.Dispose();
                return WaitWrite.ToModel<List<T>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        public bool SaveChange()
        {
            if (WaitWrite.IsNullOrEmpty()) return false;
            try
            {
                var bytes = Encoding.UTF8.GetBytes(WaitWrite);
                Stream stream = new MemoryStream(bytes);
                FileStream fs = new FileStream(DbFile, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(fs);
                writer.Write(bytes);
                writer.Close();
                fs.Close();
                stream.Close();
                stream.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="action"></param>
        public void SaveChange(Action<string, string> action)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(WaitWrite);
                Stream stream = new MemoryStream(bytes);
                FileStream fs = new FileStream(DbFile, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(fs);
                writer.Write(bytes);
                writer.Close();
                fs.Close();
                stream.Close();
                stream.Dispose();
                action.Invoke(Directorys, Path.GetFileName(DbFile));
            }
            catch (Exception)
            {

            }
        }
    }
}
