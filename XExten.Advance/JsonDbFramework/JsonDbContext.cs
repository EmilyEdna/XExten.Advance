﻿using System;
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
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="DbFile"></param>
        public JsonDbContext(string DbFile)
        {
            this.DbFile = DbFile;
            LoadDbFile();
        }

        private void LoadDbFile()
        {
            var path = Path.GetDirectoryName(DbFile);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(DbFile))
                File.Create(DbFile).Dispose();
        }
        /// <summary>
        /// 加载数据到内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public JsonDbHandle<T> LoadInMemory<T>()
        {
            string Key = string.Empty;
            FileStream fileStream = new FileStream(DbFile, FileMode.Open, FileAccess.Read);
            byte[] array = new byte[fileStream.Length];
            fileStream.Read(array, 0, array.Length);
            fileStream.Close();
            List<T> Res= new List<T>();
            if (array.Length != 0)
                Res = Encoding.UTF8.GetString(array).ToModel<List<T>>();
            return new JsonDbHandle<T>(Res, this);
        }

        internal void SetString<T>(List<T> JsonData)
        {
            WaitWrite = JsonData.ToJson();
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> SaveChange<T>()
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
                return WaitWrite.ToModel<List<T>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
