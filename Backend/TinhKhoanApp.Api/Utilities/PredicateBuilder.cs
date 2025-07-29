using System.Linq.Expressions;

namespace TinhKhoanApp.Api.Utilities
{
    /// <summary>
    /// Utility class để kết hợp các điều kiện predicate trong LINQ expressions
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Kết hợp hai expression predicate với phép AND
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            var parameter = Expression.Parameter(typeof(T));

            var visitor = new ReplaceParameterVisitor(b.Parameters[0], parameter);
            var bBody = visitor.Visit(b.Body);

            visitor = new ReplaceParameterVisitor(a.Parameters[0], parameter);
            var aBody = visitor.Visit(a.Body);

            var body = Expression.AndAlso(aBody, bBody);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        /// Kết hợp hai expression predicate với phép OR
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            var parameter = Expression.Parameter(typeof(T));

            var visitor = new ReplaceParameterVisitor(b.Parameters[0], parameter);
            var bBody = visitor.Visit(b.Body);

            visitor = new ReplaceParameterVisitor(a.Parameters[0], parameter);
            var aBody = visitor.Visit(a.Body);

            var body = Expression.OrElse(aBody, bBody);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _old;
            private readonly ParameterExpression _new;

            public ReplaceParameterVisitor(ParameterExpression old, ParameterExpression @new)
            {
                _old = old;
                _new = @new;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _old ? _new : base.VisitParameter(node);
            }
        }
    }
}
