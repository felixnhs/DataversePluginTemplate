using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DataversePluginTemplate.Service.Extensions
{
    internal static class ExpressionExtensionMethods
    {
        /// <summary>
        /// Ruft die PropertyInfo einer durch einen Lambda-Ausdruck spezifizierten Eigenschaft ab.
        /// </summary>
        /// <typeparam name="TType">Der Typ, der die Eigenschaft enthält.</typeparam>
        /// <typeparam name="TProperty">Der Typ der Eigenschaft.</typeparam>
        /// <param name="propertySelector">Lambda-Ausdruck, der die Eigenschaft spezifiziert.</param>
        /// <returns>Das PropertyInfo-Objekt, das die angegebene Eigenschaft darstellt.</returns>
        /// <exception cref="ArgumentException">Ausnahme, die ausgelöst wird, wenn der Ausdruck keine MemberExpression ist.</exception>
        /// <exception cref="Exception">Ausnahme, die ausgelöst wird, wenn der ReflectedType der Eigenschaft nicht mit TType übereinstimmt oder von diesem abgeleitet ist.</exception>
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

        /// <summary>
        /// Ruft die PropertyInfo aus einem MemberExpression ab, der eine Eigenschaft darstellt.
        /// </summary>
        /// <param name="memberExpression">MemberExpression, der eine Eigenschaft darstellt.</param>
        /// <returns>Das PropertyInfo-Objekt, das die Eigenschaft darstellt.</returns>
        /// <exception cref="ArgumentException">Ausnahme, die ausgelöst wird, wenn das Member keine Eigenschaft ist.</exception>
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
