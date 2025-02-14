using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataversePluginTemplate.Queries
{
    /// <summary>
    /// Contains information about entities that are included in queries.
    /// For internal use only in <see cref="IncludeContext{TInner, TOuter}"/>
    /// and <see cref="QueryContext{T}"/>.
    /// </summary>
    internal class IncludeEntity
    {
        public Type EntityType { get; private set; }
        public string EntityLogicalName { get; private set; }
        public string LookupLogicalName { get; private set; }
        public string EntityAlias { get; set; }
        public PropertyInfo TargetProperty { get; private set; }

        public List<IncludeEntity> InnerIncludes { get; private set; } = new List<IncludeEntity>();

        public IncludeEntity(Type entityType, string entityLogicalName, string lookupLogicalName, PropertyInfo targetProperty)
        {
            EntityType = entityType;
            EntityLogicalName = entityLogicalName;
            LookupLogicalName = lookupLogicalName;
            TargetProperty = targetProperty;
            EntityAlias = entityLogicalName;
        }
    }
}
