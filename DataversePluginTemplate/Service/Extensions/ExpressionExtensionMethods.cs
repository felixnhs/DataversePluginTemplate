using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DataversePluginTemplate.Service.Extensions
{
    internal static class ExpressionExtensionMethods
    {
        internal static PropertyInfo GetPropertyInfo<TType, TProperty>(this Expression<Func<TType, TProperty>> propertySelector)
        {
            if (propertySelector.Body is MemberExpression memberExpression)
                return memberExpression.GetPropertyInfo();

            else if (propertySelector.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operandMember)
                return operandMember.GetPropertyInfo();

            else
                throw new ArgumentException("Invalid Expression");
        }

        internal static PropertyInfo GetPropertyInfo<TType, TProperty>(this Expression<Func<TProperty>> propertySelector)
        {
            if (propertySelector.Body is MemberExpression memberExpression)
                return memberExpression.GetPropertyInfo();

            else if (propertySelector.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operandMember)
                return operandMember.GetPropertyInfo();

            else
                throw new ArgumentException("Invalid Expression");
        }

        internal static PropertyInfo GetPropertyInfo(this MemberExpression memberExpression)
        {
            if (!(memberExpression.Member is PropertyInfo propInfo))
                throw new ArgumentException("Member is not a property.");

            return propInfo;
        }

        internal static PropertyInfo[] GetPropertyInfos<T>(this Expression<Func<T, object[]>> propertySelector)
        {
            if (!(propertySelector.Body is NewArrayExpression newArrayExpression))
                throw new ArgumentException($"Body is not a {nameof(NewArrayExpression)}");

            var propertyInfos = new List<PropertyInfo>();

            foreach (var expression in newArrayExpression.Expressions)
            {
                if (expression is MemberExpression memberExpression)
                    propertyInfos.Add(GetPropertyInfo(memberExpression));

                else if (expression is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operandMember)
                    propertyInfos.Add(GetPropertyInfo(operandMember));

                else
                    throw new ArgumentException("Der Ausdruck muss auf eine Eigenschaft zugreifen.", nameof(propertySelector));
            }

            return propertyInfos.ToArray();
        }
    }
}
