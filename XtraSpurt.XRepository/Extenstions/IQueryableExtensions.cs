﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XtraSpurt.XRepository
{
    public static class IQueryableExtensions
    {
        private static readonly MethodInfo OrderByMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "OrderBy" && method.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

        private static readonly MethodInfo ThenByMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "ThenBy" && method.GetParameters().Length == 2);

        private static readonly MethodInfo ThenByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "ThenByDescending" && method.GetParameters().Length == 2);

        /// <summary>
        ///     Check if the property exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private static bool PropertyExists<T>(string propertyName)
        {
            return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                                                       BindingFlags.Public | BindingFlags.Instance) != null;
        }

        /// <summary>
        ///     Orders the source by the comma separated names list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyNameList">The property names list.</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByProperties<T>(
            this IQueryable<T> source, string propertyNameList)
        {
            var result = source;
            var orders = propertyNameList.Split(',');
            foreach (var order in orders)
            {
                var orderCurrent = order.Trim();
                string field;
                if (orderCurrent.EndsWith(" desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    field = orderCurrent.Replace(" desc", "");
                    result = result.OrderByPropertyDescending(field);
                }
                else
                {
                    field = orderCurrent.Replace(" asc", "");
                    result = result.OrderByPropertyAscending(field);
                }
            }

            return (IOrderedQueryable<T>)result;
        }

        /// <summary>
        ///     Orders the source by the property name ascending.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="Exception">The name does not match any property</exception>
        public static IOrderedQueryable<T> OrderByPropertyAscending<T>(
            this IQueryable<T> source, string propertyName)
        {
            if (!PropertyExists<T>(propertyName))
                throw new Exception("The name " + propertyName + " does not match any property");
            var paramterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
            var lambda = Expression.Lambda(orderByProperty, paramterExpression);
            MethodInfo genericMethod;
            if (source.Expression.Type == typeof(IOrderedQueryable<T>))
                genericMethod = ThenByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            else
                genericMethod = OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);

            var ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)ret;
        }

        /// <summary>
        ///     Orders the source by the property name descending.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="Exception">The name does not match any property</exception>
        public static IOrderedQueryable<T> OrderByPropertyDescending<T>(
            this IQueryable<T> source, string propertyName)
        {
            if (!PropertyExists<T>(propertyName))
                throw new Exception("The name " + propertyName + " does not match any property");
            var paramterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
            var lambda = Expression.Lambda(orderByProperty, paramterExpression);
            MethodInfo genericMethod;
            if (source.Expression.Type == typeof(IOrderedQueryable<T>))
                genericMethod = ThenByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            else
                genericMethod = OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);

            var ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)ret;
        }
    }
}