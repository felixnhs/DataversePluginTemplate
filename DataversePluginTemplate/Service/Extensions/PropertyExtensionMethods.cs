using DataversePluginTemplate.Service.Entities;
using System;
using System.Reflection;

namespace DataversePluginTemplate.Service.Extensions
{
    internal static class PropertyExtensionMethods
    {
        internal static string GetLogicalName(this PropertyInfo propertyInfo)
        {
            var logicalNameAttribute = (LogicalNameAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(LogicalNameAttribute));
            if (logicalNameAttribute == null)
                throw new Exception("Property does not have Logicalname.");

            return logicalNameAttribute.Name;
        }

        internal static bool IsPrimaryKey(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<PrimaryKeyAttribute>() != null;
        }

        internal static Type GetEnumType(this PropertyInfo propertyInfo)
        {
            Type enumType = propertyInfo.PropertyType;
            if (enumType.IsEnum)
                return enumType;

            if (!(enumType.IsNullable() && Nullable.GetUnderlyingType(enumType).IsEnum))
                return enumType;

            enumType = Nullable.GetUnderlyingType(enumType);

            return enumType;
        }

        internal static bool IsEnumProperty(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsClass)
                return false;

            return propertyInfo.GetEnumType()?.IsEnum ?? false;
        }
    }
}
