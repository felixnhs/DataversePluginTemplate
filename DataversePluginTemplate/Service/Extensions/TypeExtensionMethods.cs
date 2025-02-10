using DataversePluginTemplate.Service.Entities;
using System;
using System.Linq;
using System.Reflection;

namespace DataversePluginTemplate.Service.Extensions
{
    internal static class TypeExtensionMethods
    {
        internal static bool IsNullable(this Type type) => Nullable.GetUnderlyingType(type) != null || type.IsClass || type.IsInterface;
        internal static bool IsTypeOrNullable(this Type type, Type target) => type == target || Nullable.GetUnderlyingType(type) == target;
        internal static bool IsTypeOrNullable<T>(this Type type) => type == typeof(T) || Nullable.GetUnderlyingType(type) == typeof(T);

        internal static string GetLogicalName(this Type type)
        {
            var logicalNameAttribute = (LogicalNameAttribute)Attribute.GetCustomAttribute(type, typeof(LogicalNameAttribute));
            if (logicalNameAttribute == null)
                throw new Exception("Type does not have a Logicalname attribute");

            return logicalNameAttribute.Name;
        }

        internal static string GetPrimaryKeyName(this Type type)
        {
            foreach (var property in type.GetProperties())
            {
                if (property.IsPrimaryKey())
                    return property.GetLogicalName();
            }

            throw new Exception($"Type {type.Name} has no primary key defined.");
        }

        internal static string[] GetAllDefinedLogicalNames(this Type type)
        {
            return type.GetProperties()
                .Where(prop => prop.GetCustomAttribute<LogicalNameAttribute>() != null)
                .Select(prop => prop.GetCustomAttribute<LogicalNameAttribute>().Name)
                .ToArray();
        }
    }
}
