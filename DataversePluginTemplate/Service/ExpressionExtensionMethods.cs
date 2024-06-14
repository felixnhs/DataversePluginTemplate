using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DataversePluginTemplate.Service
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
            if (!(propertySelector.Body is MemberExpression member))
                throw new ArgumentException("Expression is not a MemberExpression.");

            var propInfo = member.GetPropertyInfo();
            Type type = typeof(TType);

            // Sicherstellen, dass TChild der gleiche Typ ist, wie die Klasse inder der die Property ist, bzw. eine Subklasse davon
            if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new Exception($"ReflectedType is not equal or derived from Type {type.Name}");

            return propInfo;
        }

        // TODO: Debuggen ob das mit dem Parameter auch funktioniert
        internal static PropertyInfo GetPropertyInfo<TType, TProperty>(this Expression<Func<TProperty>> propertySelector)
        {
            if (!(propertySelector.Body is MemberExpression member))
                throw new ArgumentException("Expression is not a MemberExpression.");

            var propInfo = member.GetPropertyInfo();
            Type type = typeof(TType);

            // Sicherstellen, dass TChild der gleiche Typ ist, wie die Klasse inder der die Property ist, bzw. eine Subklasse davon
            if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new Exception($"ReflectedType is not equal or derived from Type {type.Name}");

            return propInfo;
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
    }
}
