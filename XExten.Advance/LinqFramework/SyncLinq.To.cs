using AutoMapper;
using Mapster;
using MessagePack;
using MessagePack.Resolvers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using XExten.Advance.InternalFramework.Securities;

namespace XExten.Advance.LinqFramework
{
    public static partial  class SyncLinq
    {
        #region To
        /// <summary>
        ///保留小数非四舍五入
        /// </summary>
        /// <param name="param"></param>
        /// <param name="parrent"></param>
        /// <returns></returns>
        public static decimal ToRod(this decimal param, int parrent)
        {
            var pw = (decimal)Math.Pow(10, parrent);
            var tc = Math.Truncate(param * pw);
            return tc / pw;
        }

        /// <summary>
        /// Md5
        /// </summary>
        /// <param name="param"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static string ToMd5(this string param, int bit = 32)
        {
            return bit == 32 ? Md5.Md5By32(param) : Md5.Md5By16(param);
        }

        /// <summary>
        /// 时间格式化
        /// </summary>
        /// <param name="param"></param>
        /// <param name="fmtType">
        /// 0 : default
        /// 1 : yyyy-MM-dd HH:mm:ss:ffff
        /// 2 : yyyy年MM月dd日
        /// </param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        public static string ToFmtDate(this DateTime param, int fmtType = 0, string fmt = null)
        {
            if (fmtType == 0)
                return param.ToString("yyyy-MM-dd HH:mm:ss");
            else if (fmtType == 1)
                return param.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            else if (fmtType == 2)
                return param.ToString("yyyy年MM月dd日");
            else
            {
                if (!string.IsNullOrEmpty(fmt))
                    return param.ToString(fmt);
                else
                    throw new ArgumentNullException("paramter fmt should't null");
            }
        }

        /// <summary>
        /// DataTable 2 Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<T> ToEntity<T>(this DataTable param) where T : new()
        {
            List<T> entities = new List<T>();
            if (param == null)
                return null;
            foreach (DataRow row in param.Rows)
            {
                T entity = new T();
                foreach (var item in entity.GetType().GetProperties())
                {
                    if (param.Columns.Contains(item.Name))
                        if (DBNull.Value != row[item.Name])
                        {
                            Type newType = item.PropertyType;
                            if (newType.IsGenericType && newType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                NullableConverter nullableConverter = new NullableConverter(newType);
                                newType = nullableConverter.UnderlyingType;
                            }
                            item.SetValue(entity, Convert.ChangeType(row[item.Name], newType), null);
                        }
                }
                entities.Add(entity);
            }
            return entities;
        }

        /// <summary>
        /// 序列化 JsonNet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="Option"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T param, JsonSerializerSettings Option = null)
        {
            if (Option == null)
                return JsonConvert.SerializeObject(param);
            else
                return JsonConvert.SerializeObject(param, Option);
        }

        private static MessagePackSerializerOptions MsgOption(bool isPublic) => isPublic ? MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance) :
            MessagePackSerializerOptions.Standard.WithResolver(DynamicObjectResolverAllowPrivate.Instance);

        /// <summary>
        /// 序列化对象MsgPack
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static byte[] ToBytes<T>(this T param, bool isPublic = true) => MessagePackSerializer.Serialize(param, MsgOption(isPublic));

        /// <summary>
        /// 序列化Bytes为Json
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static string ToJsonLight(this byte[] param, bool isPublic = true) => MessagePackSerializer.ConvertToJson(param, MsgOption(isPublic));

        /// <summary>
        /// 反序列化对象MsgPack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static T ToModelLight<T>(this byte[] param, bool isPublic = true) => MessagePackSerializer.Deserialize<T>(param, MsgOption(isPublic));

        /// <summary>
        /// 反序列化 JsonNet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="Option"></param>
        /// <returns></returns>
        public static T ToModel<T>(this string param, JsonSerializerSettings Option = null)
        {
            if (Option == null)
                return (T)JsonConvert.DeserializeObject(param, typeof(T));
            else
                return (T)JsonConvert.DeserializeObject(param, typeof(T), Option);
        }

        /// <summary>
        /// 返回指定的枚举描述值
        /// </summary>
        /// <param name="Param"></param>
        /// <returns>枚举描叙</returns>
        public static string ToDes(this Enum Param)
        {
            return (Param.GetType().GetField(Param.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute).Description;
        }

        /// <summary>
        ///  返回具有标记为描述属性字段的属性值的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static string ToDes<T>(this T Param, Expression<Func<T, object>> Expres)
        {
            MemberExpression Exp = (MemberExpression)Expres.Body;
            var Obj = Exp.Member.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            return (Obj as DescriptionAttribute).Description;
        }

        /// <summary>
        /// 获取指定字段的Attribute
        /// </summary>
        /// <typeparam name="TSource">指定实体</typeparam>
        /// <typeparam name="TAttribute">特性</typeparam>
        /// <param name="Param">指定实体</param>
        /// <param name="FieldName">字段名</param>
        /// <param name="IsProperty">是否获取属性</param>
        /// <returns>Attribute</returns>
        public static TAttribute ToAttr<TSource, TAttribute>(this TSource Param, string FieldName, bool IsProperty = false) where TAttribute : Attribute
        {
            if (IsProperty)
                return (Param.GetType().GetProperty(FieldName).GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute);
            return (Param.GetType().GetField(FieldName).GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute);
        }

        /// <summary>
        /// 将集合转换为数据表并返回数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static DataTable ToTables<T>(this IList<T> queryable)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo item in typeof(T).GetProperties())
            {
                Type property = item.PropertyType;
                if ((property.IsGenericType) && (property.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    property = property.GetGenericArguments()[0];
                }
                dt.Columns.Add(new DataColumn(item.Name, property));
            }
            //创建数据行
            if (queryable.Count > 0)
            {
                for (int i = 0; i < queryable.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo item in typeof(T).GetProperties())
                    {
                        object obj = item.GetValue(queryable[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T ToMapper<T>(this object param)
        {
            if (param == null) return default;
            IMapper mapper = new MapperConfiguration(t => t.CreateMap(param.GetType(), typeof(T))).CreateMapper();
            return mapper.Map<T>(param);
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <param name="param"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static object ToMapper(this object param, Type target)
        {
            if (param == null) return default;
            IMapper mapper = new MapperConfiguration(t => t.CreateMap(param.GetType(), target)).CreateMapper();
            return mapper.Map(param, param.GetType(), target);
        }

        /// <summary>
        /// 映射集合
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<T> ToMapper<K, T>(this object param)
        {
            if (param == null) return default;
            IMapper mapper = new MapperConfiguration(t => t.CreateMap(typeof(K), typeof(T))).CreateMapper();
            return mapper.Map<List<T>>(param);
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <param name="param"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static object ToMapper(this object param, Type source, Type target, Type targets)
        {
            if (param == null) return default;
            IMapper mapper = new MapperConfiguration(t => t.CreateMap(source, target)).CreateMapper();
            return mapper.Map(param, param.GetType(), targets);
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static T ToMapest<T>(this object param, TypeAdapterConfig config = null)
        {
            return config == null ? param.Adapt<T>() : param.Adapt<T>(config);
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <param name="param"></param>
        /// <param name="destinationType"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static object ToMapest(this object param, Type destinationType, TypeAdapterConfig config = null)
        {
            return config == null ? param.Adapt(param.GetType(), destinationType) : param.Adapt(param.GetType(), destinationType, config);
        }

        /// <summary>
        /// 随机数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static List<T> ToRandom<T>(this List<T> param, int index)
        {
            if (param.Count <= index) return param;

            List<T> newArray = new List<T>(index);
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int temp = 0;//接收产生的随机数
            ArrayList list = new ArrayList();
            for (int i = 1; i <= index; i++)
            {
                temp = random.Next(param.Count - 1);//将产生的随机数作为被抽数组的索引
                if (!list.Contains(temp))
                {
                    newArray.Add(param[temp]);
                    list.Add(temp);
                }
                else
                    i--;
            }
            return newArray;
        }

        /// <summary>
        /// 根据系数间隔获取集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static List<T> ToFactor<T>(this List<T> param, double factor)
        {
            var len = (int)Math.Floor(param.Count / factor);
            List<T> copy = new List<T>();

            for (int index = 0; index <= len; index++)
            {
                copy.Add(param.ElementAtOrDefault((int)(index * factor)));
            }
            return copy.Where(t => t != null).ToList();
        }

        /// <summary>
        /// 乱序列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<T> ToDisorder<T>(this List<T> param)
        {
            List<T> NewList = new List<T>();
            Random Rand = new Random(Guid.NewGuid().GetHashCode());
            foreach (var item in param)
            {
                NewList.Insert(Rand.Next(NewList.Count()), item);
            }
            return NewList;
        }
        #endregion

        #region Async
        /// <summary>
        ///保留小数非四舍五入
        /// </summary>
        /// <param name="param"></param>
        /// <param name="parrent"></param>
        /// <returns></returns>
        public static async Task<decimal> ToRodAsync(this decimal param, int parrent) => await Task.Run(() => ToRod(param, parrent));

        /// <summary>
        /// Md5
        /// </summary>
        /// <param name="param"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static async Task<string> ToMd5Async(this string param, int bit = 32) => await Task.Run(() => ToMd5(param, bit));

        /// <summary>
        /// 时间格式化
        /// </summary>
        /// <param name="param"></param>
        /// <param name="fmtType">
        /// 0 : default
        /// 1 : yyyy-MM-dd HH:mm:ss:ffff
        /// 2 : yyyy年MM月dd日
        /// </param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        public static async Task<string> ToFmtDateAsync(this DateTime param, int fmtType = 0, string fmt = null)=> await Task.Run(() => ToFmtDate(param, fmtType,fmt));

        /// <summary>
        /// DataTable 2 Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToEntityAsync<T>(this DataTable param) where T : new() => await Task.Run(() => ToEntity<T>(param));

        /// <summary>
        /// 序列化 JsonNet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="Option"></param>
        /// <returns></returns>
        public static async Task<string> ToJsonAsync<T>(this T param, JsonSerializerSettings Option = null) => await Task.Run(() => ToJson(param, Option));

        /// <summary>
        /// 序列化对象MsgPack
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static async Task<byte[]> ToBytesAsync<T>(this T param, bool isPublic = true) => await Task.Run(() => ToBytes(param, isPublic));

        /// <summary>
        /// 序列化Bytes为Json
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static async Task<string> ToJsonLightAsync(this byte[] param, bool isPublic = true) => await Task.Run(() => ToJsonLight(param, isPublic));

        /// <summary>
        /// 反序列化对象MsgPack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static async Task<T> ToModelLightAsync<T>(this byte[] param, bool isPublic = true) => await Task.Run(() => ToModelLight<T>(param, isPublic));

        /// <summary>
        /// 反序列化 JsonNet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="Option"></param>
        /// <returns></returns>
        public static async Task<T> ToModelAsync<T>(this string param, JsonSerializerSettings Option = null) => await Task.Run(() => ToModel<T>(param, Option));

        /// <summary>
        /// 返回指定的枚举描述值
        /// </summary>
        /// <param name="param"></param>
        /// <returns>枚举描叙</returns>
        public static async Task<string> ToDesAsync(this Enum param) => await Task.Run(() => ToDes(param));

        /// <summary>
        ///  返回具有标记为描述属性字段的属性值的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static async Task<string> ToDesAsync<T>(this T Param, Expression<Func<T, object>> Expres)=> await Task.Run(() => ToDes(Param, Expres));

        /// <summary>
        /// 获取指定字段的Attribute
        /// </summary>
        /// <typeparam name="TSource">指定实体</typeparam>
        /// <typeparam name="TAttribute">特性</typeparam>
        /// <param name="Param">指定实体</param>
        /// <param name="FieldName">字段名</param>
        /// <param name="IsProperty">是否获取属性</param>
        /// <returns>Attribute</returns>
        public static async Task<TAttribute> ToAttrAsync<TSource, TAttribute>(this TSource Param, string FieldName, bool IsProperty = false) where TAttribute : Attribute 
            => await Task.Run(() => ToAttr<TSource, TAttribute>(Param, FieldName, IsProperty));

        /// <summary>
        /// 将集合转换为数据表并返回数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static async Task<DataTable> ToTablesAsync<T>(this IList<T> queryable) => await Task.Run(() => ToTables(queryable));

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<T> ToMapperAsync<T>(this object param) => await Task.Run(() => ToMapper<T>(param));

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <param name="param"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static async Task<object> ToMapperAsync(this object param, Type target) => await Task.Run(() => ToMapper(param, target));

        /// <summary>
        /// 映射集合
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToMapperAsync<K, T>(this object param) => await Task.Run(() => ToMapper<K,T>(param));

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <param name="param"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static async Task<object> ToMapperAsync(this object param, Type source, Type target, Type targets) => await Task.Run(() => ToMapper(param, source, target, targets));

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<T> ToMapestAsync<T>(this object param, TypeAdapterConfig config = null) => await Task.Run(() => ToMapest<T>(param, config));

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <param name="param"></param>
        /// <param name="destinationType"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<object> ToMapestAsync(this object param, Type destinationType, TypeAdapterConfig config = null) => await Task.Run(() => ToMapest(param, destinationType, config));

        /// <summary>
        /// 随机数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToRandomAsync<T>(this List<T> param, int index) => await Task.Run(() =>ToRandom(param,index)) ;

        /// <summary>
        /// 根据系数间隔获取集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToFactorAsync<T>(this List<T> param, double factor)=> await Task.Run(() => ToFactor(param, factor));

        /// <summary>
        /// 乱序列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToDisorderAsync<T>(this List<T> param) => await Task.Run(() => ToDisorder(param));
        #endregion
    }
}
