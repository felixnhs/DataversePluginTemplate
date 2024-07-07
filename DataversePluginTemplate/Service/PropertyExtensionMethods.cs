using System;
using System.Reflection;

namespace DataversePluginTemplate.Service
{
    internal static class PropertyExtensionMethods
    { 
        /// <summary>
        /// Bestimmt den logischen Namen einer Eigenschaft, die durch das übergebene PropertyInfo-Objekt beschrieben wird.
        /// Die Eigenschaft muss mit einem <see cref="LogicalNameAttribute"/> versehen sein.
        /// </summary>
        /// <param name="propertyInfo">Das PropertyInfo-Objekt, das die zu untersuchende Eigenschaft beschreibt.</param>
        /// <returns>Der logische Name der Eigenschaft, wie er im <see cref="LogicalNameAttribute"/> definiert ist.</returns>
        /// <exception cref="Exception">Wird ausgelöst, wenn die Eigenschaft nicht mit einem <see cref="LogicalNameAttribute"/> versehen ist.</exception>
        internal static string GetLogicalName(this PropertyInfo propertyInfo)
        {
            var logicalNameAttribute = propertyInfo.GetCustomAttribute<LogicalNameAttribute>();
            if (logicalNameAttribute == null)
                throw new Exception("Property does not have Logicalname.");

            return logicalNameAttribute.Name;
        }

        /// <summary>
        /// Überprüft, ob eine Eigenschaft, die durch das übergebene PropertyInfo-Objekt beschrieben wird,
        /// als Primärschlüssel definiert ist. Dies erfolgt durch das Vorhandensein eines <see cref="PrimaryKeyAttribute"/>.
        /// </summary>
        /// <param name="propertyInfo">Das PropertyInfo-Objekt, das die zu untersuchende Eigenschaft beschreibt.</param>
        /// <returns><c>true</c>, wenn die Eigenschaft ein <see cref="PrimaryKeyAttribute"/> besitzt; andernfalls <c>false</c>.</returns>
        internal static bool IsPrimaryKey(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<PrimaryKeyAttribute>() != null;
        }
    }
}
