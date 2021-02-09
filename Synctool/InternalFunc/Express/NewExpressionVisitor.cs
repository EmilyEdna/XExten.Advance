using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Synctool.InternalFunc.Express
{
    internal class NewExpressionVisitor : ExpressionVisitor
    {
        public ParameterExpression NewParameter { get; private set; }

        public NewExpressionVisitor(ParameterExpression param)
        {
            NewParameter = param;
        }

        public Expression Replace(Expression exp)
        {
            return Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return NewParameter;
        }
    }
}
