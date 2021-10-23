﻿using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using XExten.Advance.LinqFramework;

namespace XExten.Advance.CacheFramework.MongoDbCache
{
    /// <summary>
    ///
    /// </summary>
    public class MongoDbCaches
    {
        private static IMongoDatabase instance;

        /// <summary>
        /// 链接字符串
        /// </summary>
        public static string MongoDBConnectionString { get; set; }

        /// <summary>
        /// 库名
        /// </summary>
        public static string MongoDBName { get; set; }

        /// <summary>
        /// 获取实例
        /// </summary>
        public static IMongoDatabase Instance
        {
            get
            {
                if (instance == null)
                    return instance = new MongoClient(MongoDBConnectionString).GetDatabase(MongoDBName);
                else
                    return instance;
            }
        }

        /// <summary>
        /// 插入单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void Insert<T>(T t)
        {
            Instance.GetCollection<T>(typeof(T).Name).InsertOne(t);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void InsertMany<T>(IList<T> t)
        {
            Instance.GetCollection<T>(typeof(T).Name).InsertMany(t);
        }

        /// <summary>
        /// 查询单个记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public static T Search<T>(Expression<Func<T, bool>> Filter)
        {
            return Instance.GetCollection<T>(typeof(T).Name).Find(Filter).FirstOrDefault();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public static IList<T> SearchMany<T>(Expression<Func<T, bool>> Filter)
        {
            return Instance.GetCollection<T>(typeof(T).Name).Find(Filter).ToList();
        }

       /// <summary>
       /// 更新单个
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <typeparam name="TField"></typeparam>
       /// <param name="Filter"></param>
       /// <param name="Exp"></param>
       /// <param name="value"></param>
       /// <returns></returns>
        public static int Update<T,TField>(Expression<Func<T, bool>> Filter, Expression<Func<T, TField>> Exp, TField value)
        {
            return (int)Instance.GetCollection<T>(typeof(T).Name).UpdateOne(Filter, Builders<T>.Update.Set(Exp, value)).ModifiedCount;
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public static int UpdateMany<T>(Expression<Func<T, bool>> filter, T Entity)
        {
            var define = new List<UpdateDefinition<T>>();
            Entity.GetType().GetProperties().ForEnumerEach(t =>
            {
                define.Add(Builders<T>.Update.Set(t.Name, t.GetValue(Entity)));
            });
            return (int)Instance.GetCollection<T>(Entity.GetType().Name).UpdateMany(filter, Builders<T>.Update.Combine(define)).ModifiedCount;
        }

        /// <summary>
        /// 删除单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="filter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Delete<T,TField>(Expression<Func<T, TField>> filter, TField value)
        {
            return (int)Instance.GetCollection<T>(typeof(T).Name).DeleteOne(Builders<T>.Filter.Eq(filter, value)).DeletedCount;
        }

        /// <summary>
        /// 批量删除记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="filter"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int DeleteMany<T, TField>(Expression<Func<T, TField>> filter, params TField[] args)
        {
            return (int)Instance.GetCollection<T>(typeof(T).Name).DeleteMany(Builders<T>.Filter.In(filter, args)).DeletedCount;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static IMongoQueryable<T> GetPage<T>(int PageIndex, int PageSize)
        {
            return Instance.GetCollection<T>(typeof(T).Name).AsQueryable().Skip((PageIndex - 1) * PageSize).Take(PageSize);
        }

        /// <summary>
        /// 返回查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IMongoQueryable<T> Query<T>()
        {
            return Instance.GetCollection<T>(typeof(T).Name).AsQueryable();
        }

    }
}