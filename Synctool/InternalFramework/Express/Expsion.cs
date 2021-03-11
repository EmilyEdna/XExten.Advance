using Synctool.InternalFramework.Express.Common;
using Synctool.LinqFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Synctool.InternalFramework.Express
{
    internal static class Expsion
    {
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonValue"></param>
        /// <param name="Param"></param>
        internal static void SetProptertiesValue<T>(Dictionary<string, object> JsonValue, T Param) where T : class, new()
        {
            var type = typeof(T);
            foreach (var NameValue in JsonValue)
            {
                var property = type.GetProperty(NameValue.Key);

                var objectParameterExpression = Expression.Parameter(typeof(Object), "obj");
                var objectUnaryExpression = Expression.Convert(objectParameterExpression, type);

                var valueParameterExpression = Expression.Parameter(typeof(Object), "val");
                var valueUnaryExpression = Expression.Convert(valueParameterExpression, property.PropertyType);

                // 调用给属性赋值的方法
                var body = Expression.Call(objectUnaryExpression, property.GetSetMethod(), valueUnaryExpression);
                var expression = Expression.Lambda<Action<T, Object>>(body, objectParameterExpression, valueParameterExpression);

                var Actions = expression.Compile();
                Actions(Param, NameValue.Value);
            };
        }

        /// <summary>
        ///  返回一个new表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        internal static Expression<Func<T, object>> GetExpression<T>(params string[] PropertyName) where T : class, new()
        {
            List<MemberBinding> Exps = new List<MemberBinding>();
            ParameterExpression Parameter = Expression.Parameter(typeof(T), "t");
            PropertyName.ForArrayEach<string>(Item =>
            {
                MemberExpression PropertyExpress = Expression.Property(Parameter, Item);
                UnaryExpression ConvterExpress = Expression.Convert(PropertyExpress, typeof(T).GetProperty(Item).PropertyType);
                Exps.Add(Expression.Bind(typeof(T).GetProperty(Item), PropertyExpress));
            });
            MemberInitExpression Member = Expression.MemberInit(Expression.New(typeof(T)), Exps);
            return Expression.Lambda<Func<T, Object>>(Member, Parameter);
        }

        /// <summary>
        ///  返回一个bool表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Property"></param>
        /// <param name="Data"></param>
        /// <param name="QueryType"></param>
        /// <returns></returns>
        internal static Expression<Func<T, bool>> GetExpression<T>(string Property, object Data, QType QueryType)
        {
            ParameterExpression Parameter = Expression.Parameter(typeof(T), "t");
            if (typeof(T).GetProperty(Property) == null)
            {
                throw new MissingFieldException("Field not found,please Check");
            }
            MemberExpression Left = Expression.Property(Parameter, typeof(T).GetProperty(Property));
            ConstantExpression Right = Expression.Constant(Data, Data.GetType());
            Expression Filter = null;
            switch (QueryType)
            {
                case QType.Like:
                    Filter = Expression.Call(Left, typeof(String).GetMethod("Contains", new Type[] { typeof(String) }), Right);
                    break;

                case QType.NotLike:
                    Filter = Expression.Not(Expression.Call(Left, typeof(String).GetMethod("Contains", new Type[] { typeof(String) }), Right));
                    break;

                case QType.Equals:
                    Filter = Expression.Equal(Left, Right);
                    break;

                case QType.NotEquals:
                    Filter = Expression.NotEqual(Left, Right);
                    break;

                case QType.GreaterThan:
                    Filter = Expression.GreaterThan(Left, Right);
                    break;

                case QType.GreaterThanOrEqual:
                    Filter = Expression.GreaterThanOrEqual(Left, Right);
                    break;

                case QType.LessThan:
                    Filter = Expression.LessThan(Left, Right);
                    break;

                case QType.LessThanOrEqual:
                    Filter = Expression.LessThanOrEqual(Left, Right);
                    break;

                default:
                    Filter = Expression.Equal(Left, Right);
                    break;
            }
            return Expression.Lambda<Func<T, bool>>(Filter, Parameter);
        }

        /// <summary>
        /// 合并表达式 ExprOne AND ExprTwo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ExprOne"></param>
        /// <param name="ExprTwo"></param>
        /// <returns></returns>
        internal static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> ExprOne, Expression<Func<T, bool>> ExprTwo)
        {
            if (ExprOne == null)
                return ExprTwo;
            else if (ExprTwo == null)
                return ExprOne;

            ParameterExpression newParameter = Expression.Parameter(typeof(T), "t");
            NewExpressionVisitor visitor = new NewExpressionVisitor(newParameter);

            var left = visitor.Replace(ExprOne.Body);
            var right = visitor.Replace(ExprTwo.Body);
            var body = Expression.And(left, right);
            return Expression.Lambda<Func<T, bool>>(body, newParameter);
        }

        /// <summary>
        /// 合并表达式 ExprOne or ExprTwo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ExprOne"></param>
        /// <param name="ExprTwo"></param>
        /// <returns></returns>
        internal static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> ExprOne, Expression<Func<T, bool>> ExprTwo)
        {
            if (ExprOne == null)
                return ExprTwo;
            else if (ExprTwo == null)
                return ExprOne;

            ParameterExpression newParameter = Expression.Parameter(typeof(T), "t");
            NewExpressionVisitor visitor = new NewExpressionVisitor(newParameter);

            var left = visitor.Replace(ExprOne.Body);
            var right = visitor.Replace(ExprTwo.Body);
            var body = Expression.Or(left, right);
            return Expression.Lambda<Func<T, bool>>(body, newParameter);
        }
    }
}
