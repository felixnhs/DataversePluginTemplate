using System;
using System.Reflection;

namespace DataversePluginTemplate.Service.Extensions
{
    public static class EnumExtensionMethods
    {
        public static TAttribute GetCustomAttribute<TAttribute>(Enum enumValue)
            where TAttribute : Attribute
        {
            var enumType = enumValue.GetType();
            var memberInfo = enumType.GetMember(enumValue.ToString());

            if (memberInfo.Length == 0)
                return null;

            return memberInfo[0].GetCustomAttribute<TAttribute>(false);
        }
    }
}
