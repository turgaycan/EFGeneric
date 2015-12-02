using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFGeneric.Base.Util
{

    /// <summary>
    /// Ref : https://www.simple-talk.com/dotnet/.net-framework/giving-clarity-to-linq-queries-by-extending-expressions/
    /// </summary>
    public static class PredicateExtensions
    {

        public static Expression<Func<T, bool>> EqualOrAddNullValueCondition<T>(string propertyName, object value, bool isNullable = true)
        {
            var objectExpression = Expression.Parameter(typeof(T));
            var propertyExpression = Expression.Property(objectExpression, propertyName);

            var propLamdaExpression = Expression.Lambda<Func<T, bool>>(Expression.Equal(propertyExpression,
                                                                        Expression.Convert(Expression.Constant(value),
                                                                        propertyExpression.Type)),
                                                                        objectExpression);

            if (!isNullable)
            {
                return propLamdaExpression;
            }

            var isNullableExpression = Expression.Lambda<Func<T, bool>>(Expression.Equal(propertyExpression,
                Expression.Convert(Expression.Constant(null),
                    propertyExpression.Type)),
                objectExpression);

            var lambdaOr = Or(propLamdaExpression, isNullableExpression);

            return lambdaOr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            return CombineLambdas(left, right, ExpressionType.AndAlso);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return CombineLambdas(left, right, ExpressionType.Or);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return CombineLambdas(left, right, ExpressionType.OrElse);
        }

        private static Expression<Func<T, bool>> CombineLambdas<T>(this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right, ExpressionType expressionType)
        {

            if (IsExpressionBodyConstant(left))
            {
                return right;                
            }

            ParameterExpression p = left.Parameters[0];

            SubstituteParameterVisitor visitor = new SubstituteParameterVisitor();
            visitor.Sub[right.Parameters[0]] = p;

            Expression body = Expression.MakeBinary(expressionType, left.Body, visitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        private static bool IsExpressionBodyConstant<T>(Expression<Func<T, bool>> left)
        {
            return left.Body.NodeType == ExpressionType.Constant;
        }

        internal class SubstituteParameterVisitor : ExpressionVisitor
        {
            public Dictionary<Expression, Expression> Sub = new Dictionary<Expression, Expression>();

            protected override Expression VisitParameter(ParameterExpression node)
            {
                Expression newValue;
                if (Sub.TryGetValue(node, out newValue))
                {
                    return newValue;
                }
                return node;
            }
        }


        /// <summary>
        /// 
        /// Useage => TSet.Where(QueryUtil.Equal("IsDeleted", false));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Func<T, bool> Equal<T>(string property, object value)
        {
            var entity = Expression.Parameter(typeof(T));
            var propertyReference = Expression.Property(entity, property);
            var valueReference = Expression.Constant(value);
            return Expression.Lambda<Func<T, bool>>
                (Expression.Equal(propertyReference, valueReference),
                new[] { entity }).Compile();
        }

        /// <summary>
        ///  Useage => var anyList = AnyDbSet.DistinctBy(x=> x.AnyField);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="items"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, V>(IEnumerable<T> items, Func<T, V> selector)
        {
            var uniques = new HashSet<V>();
            return items.Where(item => uniques.Add(selector(item)));
        }
    }
}
