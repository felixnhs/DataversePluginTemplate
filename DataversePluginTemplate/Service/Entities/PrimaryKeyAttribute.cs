using System;

namespace DataversePluginTemplate.Service.Entities
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class PrimaryKeyAttribute : Attribute { }
}
