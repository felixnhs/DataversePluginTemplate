using System;

namespace DataversePluginTemplate.Service.Entities
{
    /// <summary>
    /// Required for <see cref="BaseEntity{TChild}"/> and their properties.
    /// During plugin execution the logicalname is used to access values in the
    /// entities AttributeCollection and in queries or other database operations.
    /// </summary>
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
