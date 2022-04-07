using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public static class TopicEventQueryableExtensions
    {
        public static IQueryable<EventMeshCloudEvent> Query(this IQueryable<EventMeshCloudEvent> queries, List<TopicExpression> topics)
        {
            const string topicName = "Topic";
            var parameter = Expression.Parameter(typeof(EventMeshCloudEvent), "t");
            var property = Expression.Property(parameter, topicName);
            Expression parent = null;
            foreach(var topic in topics)
            {
                Expression child = null;
                switch(topic.Type)
                {
                    case TopicExpressionTypes.EQUAL:
                        child = Expression.Equal(property, Expression.Constant(topic.Path));
                        break;
                    case TopicExpressionTypes.STARTWITH:
                        var startsWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                        child = Expression.Call(property, startsWith, Expression.Constant(topic.Path));
                        break;
                    case TopicExpressionTypes.ENDWITH:
                        var endsWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                        child = Expression.Call(property, endsWith, Expression.Constant(topic.Path));
                        break;
                }

                if(child != null)
                {
                    if (parent == null)
                    {
                        parent = child;
                    }
                    else
                    {
                        parent = Expression.And(parent, child);
                    }
                }
            }

            var whereLambda = Expression.Lambda<Func<EventMeshCloudEvent, bool>>(parent, parameter);
            var enumerableType = typeof(Queryable);
            var whereMethod = enumerableType.GetMethods()
                .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(EventMeshCloudEvent));
            var whereExpr = Expression.Call(whereMethod, Expression.Constant(queries), whereLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<EventMeshCloudEvent>), "f");
            var finalSelectRequestBody = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
            return (IQueryable<EventMeshCloudEvent>)finalSelectRequestBody.Compile().DynamicInvoke(queries);
        }
    }
}
