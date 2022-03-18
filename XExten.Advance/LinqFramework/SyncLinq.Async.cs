using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.LinqFramework
{
    public static partial class SyncLinq
    {
        #region To
        /// <summary>
        ///保留小数非四舍五入
        /// </summary>
        /// <param name="param"></param>
        /// <param name="parrent"></param>
        /// <returns></returns>
        public static async Task<decimal> ToRodAsync(this decimal param, int parrent)
        {
            return await Task.Factory.StartNew(() => ToRod(param, parrent));
        }

        /// <summary>
        /// Md5
        /// </summary>
        /// <param name="param"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static async Task<string> ToMd5Async(this string param, int bit = 32)
        {
            return await Task.Factory.StartNew(() => ToMd5(param, bit));
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
        public static async Task<string> ToFmtDateAsync(this DateTime param, int fmtType = 0, string fmt = null)
        {
            return await Task.Factory.StartNew(() => ToFmtDate(param, fmtType, fmt));
        }

        /// <summary>
        /// DataTable 2 Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToEntityAsync<T>(this DataTable param) where T : new()
        {
            return await Task.Factory.StartNew(() => ToEntity<T>(param));
        }

        /// <summary>
        /// 序列化 JsonNet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="Option"></param>
        /// <returns></returns>
        public static async Task<string> ToJsonAsync<T>(this T param, JsonSerializerSettings Option = null)
        {
            return await Task.Factory.StartNew(() => ToJson(param, Option));
        }

        /// <summary>
        /// 反序列化 JsonNet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="Option"></param>
        /// <returns></returns>
        public static async Task<T> ToModelAsync<T>(this string param, JsonSerializerSettings Option = null)
        {
            return await Task.Factory.StartNew(() => ToModel<T>(param, Option));
        }

        /// <summary>
        /// 返回指定的枚举描述值
        /// </summary>
        /// <param name="Param"></param>
        /// <returns>枚举描叙</returns>
        public static async Task<string> ToDesAsync(this Enum Param)
        {
            return await Task.Factory.StartNew(() => ToDes(Param));
        }

        /// <summary>
        ///  返回具有标记为描述属性字段的属性值的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <param name="Expres"></param>
        /// <returns></returns>
        public static async Task<string> ToDesAsync<T>(this T Param, Expression<Func<T, object>> Expres)
        {
            return await Task.Factory.StartNew(() => ToDes(Param, Expres));
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
        public static async Task<TAttribute> ToAttrAsync<TSource, TAttribute>(this TSource Param, string FieldName, bool IsProperty = false) where TAttribute : Attribute
        {
            return await Task.Factory.StartNew(() => ToAttr<TSource, TAttribute>(Param, FieldName, IsProperty));
        }

        /// <summary>
        /// 将集合转换为数据表并返回数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static async Task<DataTable> ToTablesAsync<T>(this IList<T> queryable)
        {
            return await Task.Factory.StartNew(() => ToTables(queryable));
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<T> ToMapperAsync<T>(this object param)
        {
            return await Task.Factory.StartNew(() => ToMapper<T>(param));
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static async Task<object> ToMapperAsync<T>(this object param, Type target)
        {
            return await Task.Factory.StartNew(() => ToMapper(param, target));
        }

        /// <summary>
        /// 映射集合
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<List<T>> ToMapperAsync<K, T>(this object param)
        {
            return await Task.Factory.StartNew(() => ToMapper<K, T>(param));
        }

        /// <summary>
        /// 映射集合
        /// </summary>
        /// <param name="param"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static async Task<object> ToMapperAsync(this object param, Type source, Type target, Type targets)
        {
            return await Task.Factory.StartNew(() => ToMapper(param, source, target, targets));
        }
        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<T> ToMapestAsync<T>(this object param, TypeAdapterConfig config = null)
        {
            return await Task.Factory.StartNew(() => ToMapest<T>(param, config));
        }

        /// <summary>
        /// 映射对象
        /// </summary>
        /// <param name="param"></param>
        /// <param name="destinationType"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<object> ToMapestAsync(this object param, Type destinationType, TypeAdapterConfig config = null)
        {
            return await Task.Factory.StartNew(() => ToMapest(param, destinationType, config));
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
        public static async Task<string> ByUriAsync<T>(this T param, IDictionary<string, string> MapFied = null)
        {
            return await Task.Run(() => ByUri(param, MapFied));
        }

        /// <summary>
        /// 将键值对转为URL参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<string> ByUriAsync(this List<KeyValuePair<string, string>> param)
        {
            return await Task.Run(() => ByUri(param));
        }
        #endregion

        #region With
        /// <summary>
        /// 返回实体中所有的字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<List<string>> WithNamesAsync<T>(this IEnumerable<T> param) where T : class, new()
        {
            return await Task.Factory.StartNew(() => WithNames(param));
        }

        /// <summary>
        /// 返回一个实体中所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, object>> WithKeyValueAsync<T>(T param) where T : class, new()
        {
            return await Task.Factory.StartNew(() => WithKeyValue(param));
        }
        #endregion
    }
}
