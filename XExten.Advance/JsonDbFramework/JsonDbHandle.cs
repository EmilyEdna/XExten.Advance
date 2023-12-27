using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XExten.Advance.JsonDbFramework
{
    /// <summary>
    /// JsonDb执行器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonDbHandle<T>
    {
        private JsonDbContext Context;
        private List<T> JsonData;
        private List<Expression<Func<T, bool>>> ExpressList;
        private List<Expression<Func<T, bool>>> WhereList;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Json"></param>
        /// <param name="DbContext"></param>
        public JsonDbHandle(List<T> Json, JsonDbContext DbContext)
        {
            this.JsonData = Json;
            this.Context= DbContext;
            ExpressList = new List<Expression<Func<T, bool>>>();
            WhereList = new List<Expression<Func<T, bool>>>();
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public JsonDbHandle<T> SetColumns(Expression<Func<T, bool>> columns)
        {
            ExpressList.Add(columns);
            return this;
        }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public JsonDbHandle<T> Where(Expression<Func<T, bool>> columns)
        {
            WhereList.Add(columns);
            return this;
        }
        /// <summary>
        /// 判断条件
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public JsonDbHandle<T> WhereIF(bool Condition, Expression<Func<T, bool>> columns)
        {
            if (Condition)
                WhereList.Add(columns);
            return this;
        }
        /// <summary>
        /// 单个
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public T First(Expression<Func<T, bool>> columns)
        {
            return JsonData.Where(columns.Compile()).First();
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonDbHandle<T> Insert(T input)
        {
            JsonData.Add(input);
            return this;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public JsonDbHandle<T> Delete(Expression<Func<T, bool>> columns)
        {
            var func = columns.Compile();
            JsonData.RemoveAll(t => func(t));
            return this;
        }
        /// <summary>
        /// 全集合
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll()
        {
            return JsonData;
        }
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <returns></returns>
        public JsonDbContext ExcuteDelete() {
            Context.SetString(JsonData);
            return Context;
        }
        /// <summary>
        /// 执行插入
        /// </summary>
        /// <returns></returns>
        public JsonDbContext ExuteInsert()
        {
            Context.SetString(JsonData);
            return Context;
        }
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public JsonDbContext ExcuteUpdate()
        {
            bool result = false;
            IQueryable<T> Querys = JsonData.AsQueryable();
            WhereList.ForEach(item =>
            {
                Querys = JsonData.Where(item.Compile()).AsQueryable();
            });
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            foreach (var item in ExpressList)
            {
                BinaryExpression binaryExpression = (BinaryExpression)item.Body;
                if (binaryExpression == null)
                {
                    result = true;
                    break;
                }
                if (binaryExpression.NodeType != ExpressionType.Equal)
                {
                    result = true;
                    break;
                }
                string leftOperand = ((MemberExpression)binaryExpression.Left).Member.Name;
                string rightOperand = Expression.Lambda(binaryExpression.Right).Compile().DynamicInvoke().ToString();
                pairs[leftOperand] = rightOperand;
            }
            if (result) throw new Exception("更新只支持全等");
            var data = Querys.ToList();
            data.ForEach(item =>
             {
                 item.GetType().GetProperties().Where(t => pairs.Keys.Contains(t.Name)).ToList().ForEach(node =>
                 {
                     node.SetValue(item, Converter(node, pairs));
                 });
             });
            Context.SetString(JsonData);
            return Context;
        }
        private object Converter(PropertyInfo node, Dictionary<string, object> pairs)
        {
            object value = null;
            if (node.PropertyType == typeof(int))
                value = int.Parse(pairs[node.Name].ToString());
            if (node.PropertyType == typeof(decimal))
                value = decimal.Parse(pairs[node.Name].ToString());
            if (node.PropertyType == typeof(double))
                value = double.Parse(pairs[node.Name].ToString());
            if (node.PropertyType == typeof(float))
                value = float.Parse(pairs[node.Name].ToString());
            if (node.PropertyType == typeof(DateTime))
                value = DateTime.Parse(pairs[node.Name].ToString());
            if (node.PropertyType == typeof(bool))
                value = bool.Parse(pairs[node.Name].ToString());
            return value;
        }
    }
}
