using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.CacheFramework;
using XExten.Advance.CacheFramework.MongoDbCache;

namespace Test.MogoTest
{
    public class MogoDbTest
    {
        private static void SetConn()
        {
            Caches.DbName = "logs";
            Caches.MongoDBConnectionString = "mongodb://127.0.0.1:27017";
        }
        public static void DeleteOne()
        {
            SetConn();
        }
    }
    public class ExceptionLog
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }
        /// <summary>
        /// 堆栈
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// 日志时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 日志链
        /// </summary>
        public string TraceId { get; set; }
        /// <summary>
        /// 所属服务
        /// </summary>
        public string SystemService { get; set; }
    }
}
