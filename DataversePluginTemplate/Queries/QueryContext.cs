using DataversePluginTemplate.Service.Entities;
using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataversePluginTemplate.Queries
{
    /// <summary>
    /// Wrapper for building dataverse queries and retrieve entites from dataverse.
    /// </summary>
    public sealed class QueryContext
    {
        private readonly IOrganizationService _orgService;
        private readonly QueryExpression _expression;

        public QueryContext(IOrganizationService orgService, string entityName)
        {
            _orgService = orgService;
            _expression = new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet(),
                Criteria = new FilterExpression(LogicalOperator.And),
            };
        }

        public QueryContext Columns(params string[] columns)
        {
            _expression.ColumnSet.AddColumns(columns);
            return this;
        }

        public QueryContext Columns(IEnumerable<string> columns)
        {
            foreach (var column in columns)
                _expression.ColumnSet.AddColumn(column);

            return this;
        }

        public QueryContext AllColumns(bool allColumns = true)
        {
            _expression.ColumnSet.AllColumns = allColumns;
            return this;
        }

        public QueryContext NoColumns()
        {
            _expression.ColumnSet.AllColumns = false;
            return this;
        }

        public QueryContext Top(int count)
        {
            _expression.TopCount = count;
            return this;
        }

        public QueryContext Conditions(LogicalOperator logicalOperator, Action<FilterContext> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext(filterExpression);
            configureFilter(filterContext);
            _expression.Criteria.AddFilter(filterExpression);

            return this;
        }

        public QueryContext Join(string entityName, string fromColumn, string toColumn, Action<LinkContext> configureLink)
        {
            return Join(entityName, fromColumn, toColumn, JoinOperator.Inner, configureLink);
        }

        public QueryContext Join(string entityName, string fromColumn, string toColumn, JoinOperator joinOperator, Action<LinkContext> configureLink)
        {
            var linkEntity = new LinkEntity(_expression.EntityName, entityName, fromColumn, toColumn, joinOperator);
            var linkContext = new LinkContext(linkEntity, entityName);
            configureLink(linkContext);
            _expression.LinkEntities.Add(linkEntity);
            return this;
        }

        public EntityCollection Execute()
        {
            return _orgService.RetrieveMultiple(_expression);
        }
    }

    /// <summary>
    /// Wrapper for building dataverse queries in a typesafe way and retrieve entities from dataverse.
    /// Use in combination with <see cref="BaseEntity{TChild}"/>.
    /// The methods will construct a <see cref="QueryExpression"/> for dataverse, and when
    /// executed will convert the results into instances of your class.
    /// Entities, that are queried, mus have a <see cref="LogicalNameAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The qeruy result type.</typeparam>
    public sealed class QueryContext<T>
        where T : BaseEntity<T>
    {
        private readonly IOrganizationService _orgService;
        private readonly QueryExpression _expression;
        private readonly List<IncludeEntity> _includes = new List<IncludeEntity>();

        public QueryContext(IOrganizationService orgService)
        {
            _orgService = orgService;
            _expression = new QueryExpression(typeof(T).GetLogicalName());
        }

        public QueryContext<T> Columns(Expression<Func<T, object[]>> propertySelector)
        {
            var propertyInfos = propertySelector.GetPropertyInfos();

            _expression.ColumnSet.AddColumns(propertyInfos
                .Select(prop => prop.GetLogicalName())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray());

            return this;
        }

        public QueryContext<T> Columns(Columns columns)
        {
            switch (columns)
            {
                case Queries.Columns.All:
                    return AllColumns(true);

                case Queries.Columns.None:
                    return AllColumns(false);

                case Queries.Columns.DefinedOnly:
                    return AllDefinedColumns();
            }

            return this;
        }

        public QueryContext<T> AllDefinedColumns()
        {
            _expression.ColumnSet.AddColumns(typeof(T).GetAllDefinedLogicalNames());
            return this;
        }

        public QueryContext<T> AllColumns(bool allColumns = true)
        {
            _expression.ColumnSet.AllColumns = allColumns;
            return this;
        }

        public QueryContext<T> NoColumns()
        {
            _expression.ColumnSet.AllColumns = false;
            return this;
        }

        public QueryContext<T> Top(int count)
        {
            _expression.TopCount = count;
            return this;
        }

        public QueryContext<T> Conditions(LogicalOperator logicalOperator, Action<FilterContext<T>, T> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext<T>(filterExpression);
            configureFilter(filterContext, null);
            _expression.Criteria.AddFilter(filterExpression);

            return this;
        }

        public QueryContext<T> Join<TOuter>(Expression<Func<T, object>> fromColumnSelector, Expression<Func<TOuter, object>> toColumnSelector, Action<LinkContext<T, TOuter>> configureLink)
            where TOuter : BaseEntity<TOuter>
        {
            return Join(fromColumnSelector, toColumnSelector, JoinOperator.Inner, configureLink);
        }

        public QueryContext<T> Join<TOuter>(Expression<Func<T, object>> fromColumnSelector, Expression<Func<TOuter, object>> toColumnSelector, JoinOperator joinOperator, Action<LinkContext<T, TOuter>> configureLink)
            where TOuter : BaseEntity<TOuter>
        {
            var entityName = typeof(TOuter).GetLogicalName();
            var fromColumn = fromColumnSelector.GetPropertyInfo().GetLogicalName();
            var toColumn = toColumnSelector.GetPropertyInfo().GetLogicalName();

            var linkEntity = new LinkEntity(_expression.EntityName, entityName, fromColumn, toColumn, joinOperator);
            var linkContext = new LinkContext<T, TOuter>(linkEntity, entityName);
            configureLink(linkContext);
            _expression.LinkEntities.Add(linkEntity);
            return this;
        }

        public QueryContext<T> Join<TOuter>(Expression<Func<TOuter, object>> toColumnSelector, Action<LinkContext<T, TOuter>> configureLink)
            where TOuter : BaseEntity<TOuter>
        {
            return Join(toColumnSelector, JoinOperator.Inner, configureLink);
        }

        public QueryContext<T> Join<TOuter>(Expression<Func<TOuter, object>> toColumnSelector, JoinOperator joinOperator, Action<LinkContext<T, TOuter>> configureLink)
            where TOuter : BaseEntity<TOuter>
        {
            var entityName = typeof(TOuter).GetLogicalName();
            var fromColumn = typeof(T).GetPrimaryKeyName();
            var toColumn = toColumnSelector.GetPropertyInfo().GetLogicalName();

            var linkEntity = new LinkEntity(_expression.EntityName, entityName, fromColumn, toColumn, joinOperator);
            var linkContext = new LinkContext<T, TOuter>(linkEntity, entityName);
            configureLink(linkContext);
            _expression.LinkEntities.Add(linkEntity);
            return this;
        }

        public QueryContext<T> Include<TOuter>(Expression<Func<T, TOuter>> includePropertySelector, Action<IncludeContext<T, TOuter>> configureInclude)
           where TOuter : BaseEntity<TOuter>
        {
            return Include<TOuter>(includePropertySelector, JoinOperator.LeftOuter, configureInclude);
        }

        /// <summary>
        /// Join in another <see cref="BaseEntity{TChild}"/>, that is a property of <see cref="T"/> in code and a lookup in dataverse.
        /// When the results are retrieved and the instances of <see cref="T"/> are created, the joined/included instances
        /// are also processed.
        /// <seealso cref="Service.Entities.IncludableAttribute"/>
        /// </summary>
        /// <typeparam name="TOuter">The type to include</typeparam>
        /// <param name="includePropertySelector">Selector expression of the property to include</param>
        /// <param name="joinOperator">How dataverse should join the data.</param>
        /// <param name="configureInclude">Configure the included entity</param>
        /// <returns>The same QueryContext for chaining.</returns>
        public QueryContext<T> Include<TOuter>(Expression<Func<T, TOuter>> includePropertySelector, JoinOperator joinOperator, Action<IncludeContext<T, TOuter>> configureInclude)
            where TOuter : BaseEntity<TOuter>
        {
            var entityName = typeof(TOuter).GetLogicalName();
            var targetProperty = includePropertySelector.GetPropertyInfo();
            var fromColumn = targetProperty.GetLogicalName();
            var toColumn = typeof(TOuter).GetPrimaryKeyName();

            var includeEntity = new IncludeEntity(typeof(TOuter), entityName, fromColumn, targetProperty);
            var linkEntity = new LinkEntity(_expression.EntityName, entityName, fromColumn, toColumn, joinOperator);
            var includeContext = new IncludeContext<T, TOuter>(linkEntity, includeEntity);
            configureInclude(includeContext);
            _expression.LinkEntities.Add(linkEntity);

            _includes.Add(includeEntity);
            _expression.ColumnSet.AddColumn(fromColumn);

            return this;
        }

        /// <summary>
        /// Makes the call to dataverse to retrieve the entities and convert them into your instances.
        /// </summary>
        /// <returns>The entites as your type.</returns>
        public IEnumerable<T> Execute()
        {
            var queryResults = _orgService.RetrieveMultiple(_expression).As<T>();

            if (!_includes.Any())
                return queryResults;

            foreach (var entity in queryResults)
                ProcessIncludes(_includes, entity, entity, entity.Entity);

            return queryResults;
        }

        private void ProcessIncludes(List<IncludeEntity> includes, T sourceEntity, object target, Entity targetEntity)
        {
            foreach (var link in includes)
            {
                var constructor = link.EntityType.GetConstructor(new Type[] { typeof(Entity) });
                if (constructor == null)
                    return;

                if (link.TargetProperty.GetCustomAttribute<IncludableAttribute>() == null)
                    continue;


                if (!targetEntity.TryGetAttributeValue<EntityReference>(link.LookupLogicalName, out var lookupER))
                    if (!targetEntity.TryGetAttributeValue<EntityReference>($"{link.EntityAlias}.{link.LookupLogicalName}", out lookupER))
                        continue;

                var remAttributes = new List<string>();
                var joinedEntity = new Entity(link.EntityLogicalName, lookupER.Id);
                foreach (var attr in sourceEntity.Entity.Attributes.Where(a => a.Value is AliasedValue alias && alias.EntityLogicalName == link.EntityLogicalName && a.Key.StartsWith(link.EntityAlias)))
                {
                    var alias = (AliasedValue)attr.Value;
                    joinedEntity.Attributes.Add(alias.AttributeLogicalName, alias.Value);
                    remAttributes.Add(attr.Key);
                }

                foreach (var remName in remAttributes)
                    sourceEntity.Entity.Attributes.Remove(remName);

                var includedEntity = constructor.Invoke(new object[] { joinedEntity });
                link.TargetProperty.SetValue(target, includedEntity);

                if (link.InnerIncludes.Any())
                    ProcessIncludes(link.InnerIncludes, sourceEntity, includedEntity, joinedEntity);
            }
        }
    }

}
