using System;
using System.Reflection;

namespace DataversePluginTemplate.Service
{
    internal static class PropertyExtensionMethods
    {
        internal static string GetLogicalName(this PropertyInfo propertyInfo)
        {
            var logicalNameAttribute = (LogicalNameAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(LogicalNameAttribute));
            if (logicalNameAttribute == null)
                return null;

            return logicalNameAttribute.Name;
        }
    }
}
