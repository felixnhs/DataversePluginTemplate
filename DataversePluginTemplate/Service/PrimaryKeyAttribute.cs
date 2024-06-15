using System;

namespace DataversePluginTemplate.Service
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class PrimaryKeyAttribute : Attribute
    {
    }
}
