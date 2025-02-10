using System;

namespace DataversePluginTemplate.Service.Entities
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class LogicalNameAttribute : Attribute
    {
        public string Name { get; }

        public LogicalNameAttribute(string name)
        {
            Name = name;
        }
    }

}
