using XExten.Advance.InternalFramework.Securities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using AutoMapper;
using System.Linq.Expressions;
using MessagePack;
using MessagePack.Resolvers;
using XExten.Advance.InternalFramework.Express;
using Newtonsoft.Json.Linq;
using XExten.Advance.AopFramework;
using DryIoc;
using Mapster;

namespace XExten.Advance.LinqFramework
{
    /// <summary>
    /// LinqExtension
    /// </summary>
    public static partial class SyncLinq
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

        /// <summary>
        /// 序列化对象MsgPack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static string ToJsonLight<T>(this T param, bool isPublic = true)
        {
            MessagePackSerializerOptions Options;
            if (isPublic)
                Options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
            else
                Options = MessagePackSerializerOptions.Standard.WithResolver(DynamicObjectResolverAllowPrivate.Instance);
            return MessagePackSerializer.SerializeToJson(param, Options);
        }

        /// <summary>
        /// 反序列化对象MsgPack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="bytes"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public static T ToModelLight<T>(this T param, out byte[] bytes, bool isPublic = true)
        {
            MessagePackSerializerOptions Options;
            if (isPublic)
                Options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
            else
                Options = MessagePackSerializerOptions.Standard.WithResolver(DynamicObjectResolverAllowPrivate.Instance);
            bytes = MessagePackSerializer.Serialize(param, Options);
            return MessagePackSerializer.Deserialize<T>(bytes, Options);
        }

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
        #endregion

        #region By
        /// <summary>
        /// 将实体转为URL参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="MapFied"></param>
        /// <returns></returns>
        public static string ByUri<T>(this T param, IDictionary<string, string> MapFied = null)
        {
            List<string> result = new List<string>();
            if (param is JObject)
            {
                var NEW = param as JObject;
                NEW.Properties().ForEnumerEach(t =>
                {
                    if (MapFied != null)
                    {
                        MapFied.TryGetValue(t.Name, out string value);
                        if (value.IsNullOrEmpty())
                            result.Add($"{t.Name.ToLower()}={t.Value}");
                        else
                            result.Add($"{value.ToLower()}={t.Value}");
                    }
                    else
                        result.Add($"{t.Name.ToLower()}={t.Value}");
                });
                return $"?{string.Join("&", result)}";
            }
            param.GetType().GetProperties().ForEnumerEach(t =>
            {
                if (MapFied != null)
                {
                    MapFied.TryGetValue(t.Name, out string value);
                    if (value.IsNullOrEmpty())
                        result.Add($"{t.Name.ToLower()}={t.GetValue(param)}");
                    else
                        result.Add($"{value.ToLower()}={t.GetValue(param)}");
                }
                result.Add($"{t.Name.ToLower()}={t.GetValue(param)}");
            });
            return $"?{string.Join("&", result)}";
        }

        /// <summary>
        /// 将键值对转为URL参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ByUri(this List<KeyValuePair<string, string>> param)
        {
            List<string> result = new List<string>();
            param.ForEach(t =>
            {
                result.Add($"{t.Key.ToLower()}={t.Value}");
            });
            return $"?{string.Join("&", result)}";
        }
        #endregion

        #region  For
        /// <summary>
        /// 遍历字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static void ForDicEach<T, K>(this IDictionary<T, K> param, Action<T, K> action)
        {
            foreach (KeyValuePair<T, K> item in param)
                action(item.Key, item.Value);
        }

        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static void ForArrayEach<T>(this Array param, Action<T> action)
        {
            foreach (var item in param)
                action((T)item);
        }

        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="action"></param>
        public static void ForEnumerEach<T>(this IEnumerable<T> param, Action<T> action)
        {
            foreach (var item in param)
                action(item);
        }
        #endregion

        #region With
        /// <summary>
        /// 返回实体中所有的字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<string> WithNames<T>(this IEnumerable<T> param) where T : class, new()
        {
            List<String> Names = new List<String>();
            param.FirstOrDefault().GetType().GetProperties().ForEnumerEach(t =>
            {
                Names.Add(t.Name);
            });
            return Names;
        }

        /// <summary>
        /// 返回一个实体中所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Dictionary<string, object> WithKeyValue<T>(T param) where T : class,new()
        {
            var result = new Dictionary<string, object>();

            param.GetType().GetProperties().ForEnumerEach(item =>
            {
                result.Add(item.Name, item.GetValue(param));
            });

            return result;
        }
        #endregion

        #region Other
        /// <summary>
        /// 合并表达式 ExprOne AND ExprTwo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ExprOne"></param>
        /// <param name="ExprTwo"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> ExprOne, Expression<Func<T, bool>> ExprTwo)
        {
            return Expsion.And(ExprOne, ExprTwo);
        }

        /// <summary>
        /// 合并表达式 ExprOne or ExprTwo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ExprOne"></param>
        /// <param name="ExprTwo"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> ExprOne, Expression<Func<T, bool>> ExprTwo)
        {
            return Expsion.Or(ExprOne, ExprTwo);
        }
        #endregion

        #region Aop
        /// <summary>
        /// 注册代理
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static DryIoc.IContainer RegistAop<T>(this DryIoc.IContainer container)
        {
            var Class = typeof(T);
            Class.Assembly.GetTypes()
               .Where(t => t.IsClass)
               .Where(t => t.GetInterfaces().Contains(Class))
               .ForEnumerEach(item =>
               {
                   container.Register(Class, AopProxy.CreateProxyOfRealize(Class, item).GetType(), Reuse.Singleton);
               });
            return container;
        }
        #endregion
    }
}
