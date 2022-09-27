using FaasNet.EventMesh.Client.StateMachines;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace FaasNet.EventMesh.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> InvokeOrderBy<T>(this IQueryable<T> source, string propertyName, SortOrders order)
        {
            var piParametr = Expression.Parameter(typeof(T), "r");
            var property = Expression.Property(piParametr, propertyName);
            var lambdaExpr = Expression.Lambda(property, piParametr);
            return (IQueryable<T>)Expression.Call(
                typeof(Queryable),
                order == SortOrders.ASC ? "OrderBy" : "OrderByDescending",
                new Type[] { typeof(T), property.Type },
                source.Expression,
                lambdaExpr)
                .Method.Invoke(null, new object[] { source, lambdaExpr });
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> source, ComparisonExpression comparison)
        {
            var piParametr = Expression.Parameter(typeof(T), "r");
            var property = Expression.Property(piParametr, comparison.Key);
            var comparisonExpr = Expression.Equal(property, Expression.Constant(comparison.Value));
            var lambdaExpr = Expression.Lambda(comparisonExpr, piParametr);
            var whereMethod = typeof(Queryable).GetMethods()
                 .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                 .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(T));
            return (IQueryable<T>)Expression.Call(whereMethod, source.Expression, lambdaExpr)
                .Method.Invoke(null, new object[] { source, lambdaExpr });
        }
    }
}
