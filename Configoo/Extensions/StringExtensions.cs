﻿using System;
using System.Linq.Expressions;

namespace Configoo
{
    internal static class StringExtensions
    {
        /// <summary>
        /// </summary>
        public static string Name<T>(this Expression<Func<T, object>> expression)
        {
            Expression body = expression.Body;
            return GetMemberName(body);
        }

        /// <summary>
        /// </summary>
        public static string Name<T, TReturn>(this Expression<Func<T, TReturn>> expression)
        {
            Expression body = expression.Body;
            return GetMemberName(body);
        }

        static string GetMemberName(Expression expression)
        {
            const string memberAppender = "";

            if (expression is MemberExpression)
            {
                var memberExpression = (MemberExpression)expression;

                if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    return GetMemberName(memberExpression.Expression) + memberAppender + memberExpression.Member.Name;
                }

                return memberExpression.Member.Name;
            }

            if (expression is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)expression;

                if (unaryExpression.NodeType != ExpressionType.Convert)
                    throw new Exception(string.Format("Cannot interpret member from {0}", expression));

                return GetMemberName(unaryExpression.Operand);
            }

            if (expression is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)expression;

                return methodCallExpression.Method.Name;
            }

            throw new Exception(string.Format("Could not determine member from {0}", expression));
        }
    }
}