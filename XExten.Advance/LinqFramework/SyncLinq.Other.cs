using System;
using System.Linq.Expressions;
using XExten.Advance.InternalFramework.Express;

namespace XExten.Advance.LinqFramework
{
    public static partial class SyncLinq
    {
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
    }
}
