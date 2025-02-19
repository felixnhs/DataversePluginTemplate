using System;

namespace DataversePluginTemplate.Service.Entities
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class PrimaryKeyAttribute : Attribute { }
}
