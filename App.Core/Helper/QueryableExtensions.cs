using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace App.Core.Helper
{
    public class OrderOption
    {
        public string FieldName { get; set; }
        public string Direction { get; set; }
    }
    public static class QueryableExtensions
    {

        public static IQueryable<T> SortBy<T>(this IQueryable<T> source,
            IEnumerable<OrderOption> fieldsToSort)
        {
            Expression expression = source.Expression;
            bool firstTime = true;

            foreach (var f in fieldsToSort)
            {
                // { x }
                var parameter = Expression.Parameter(typeof(T), "x");

                // { x.FIELD }, e.g, { x.ID }, { x.Name }, etc

                //var selector = Expression.PropertyOrField(parameter, f.FieldName);
                Expression selector = parameter;
                foreach (var member in f.FieldName.Split('.'))
                {
                    selector = Expression.PropertyOrField(selector, member);
                }

                // { x => x.FIELD }
                var lambda = Expression.Lambda(selector, parameter);



                // You can include sorting directions for advanced cases
                var method = firstTime
                    ? (f.Direction == "desc" ? "OrderByDescending" : "OrderBy")
                    : (f.Direction == "desc" ? "ThenByDescending" : "ThenBy");

                // { OrderBy(x => x.FIELD) }
                expression = Expression.Call(
                    typeof(Queryable),
                    method,
                    new Type[] { source.ElementType, selector.Type },
                    expression,
                    Expression.Quote(lambda)
                );

                firstTime = false;
            }

            return firstTime
                ? source
                : source.Provider.CreateQuery<T>(expression);
        }

    }

}
