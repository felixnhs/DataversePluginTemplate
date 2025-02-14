using System;

namespace DataversePluginTemplate.Service.Entities
{
    /// <summary>
    /// Properties marked with this, which are a <see cref="BaseEntity{TChild}"/>, can be
    /// included in data retreival operations using <see cref="Queries.QueryContext{T}"/>
    /// and <see cref="Queries.IncludeContext{TInner, TOuter}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class IncludableAttribute : Attribute { }
}
