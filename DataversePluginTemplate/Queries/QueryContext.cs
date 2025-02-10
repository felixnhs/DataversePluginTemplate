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
    internal sealed class QueryContext
    {
        private readonly IOrganizationService _orgService;
        private readonly QueryExpression _expression;

        internal QueryContext(IOrganizationService orgService, string entityName)
        {
            _orgService = orgService;
            _expression = new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet(),
                Criteria = new FilterExpression(LogicalOperator.And),
            };
        }

        internal QueryContext Columns(params string[] columns)
        {
            _expression.ColumnSet.AddColumns(columns);
            return this;
        }

        internal QueryContext Columns(IEnumerable<string> columns)
        {
            foreach (var column in columns)
                _expression.ColumnSet.AddColumn(column);

            return this;
        }

        internal QueryContext AllColumns(bool allColumns = true)
        {
            _expression.ColumnSet.AllColumns = allColumns;
            return this;
        }

        internal QueryContext NoColumns()
        {
            _expression.ColumnSet.AllColumns = false;
            return this;
        }

        internal QueryContext Top(int count)
        {
            _expression.TopCount = count;
            return this;
        }

        internal QueryContext Conditions(LogicalOperator logicalOperator, Action<FilterContext> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext(filterExpression);
            configureFilter(filterContext);
            _expression.Criteria.AddFilter(filterExpression);

            return this;
        }

        internal QueryContext Join(string entityName, string fromColumn, string toColumn, Action<LinkContext> configureLink)
        {
            return Join(entityName, fromColumn, toColumn, JoinOperator.Inner, configureLink);
        }

        internal QueryContext Join(string entityName, string fromColumn, string toColumn, JoinOperator joinOperator, Action<LinkContext> configureLink)
        {
            var linkEntity = new LinkEntity(_expression.EntityName, entityName, fromColumn, toColumn, joinOperator);
            var linkContext = new LinkContext(linkEntity, entityName);
            configureLink(linkContext);
            _expression.LinkEntities.Add(linkEntity);
            return this;
        }

        internal EntityCollection Execute()
        {
            return _orgService.RetrieveMultiple(_expression);
        }
    }

    internal sealed class QueryContext<T>
        where T : BaseEntity<T>
    {
        private readonly IOrganizationService _orgService;
        private readonly QueryExpression _expression;
        private readonly List<IncludeEntity> _includes = new List<IncludeEntity>();

        internal QueryContext(IOrganizationService orgService)
        {
            _orgService = orgService;
            _expression = new QueryExpression(typeof(T).GetLogicalName());
        }

        internal QueryContext<T> Columns(Expression<Func<T, object[]>> propertySelector)
        {
            var propertyInfos = propertySelector.GetPropertyInfos();

            _expression.ColumnSet.AddColumns(propertyInfos
                .Select(prop => prop.GetLogicalName())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray());

            return this;
        }

        internal QueryContext<T> Columns(Columns columns)
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

        internal QueryContext<T> AllDefinedColumns()
        {
            _expression.ColumnSet.AddColumns(typeof(T).GetAllDefinedLogicalNames());
            return this;
        }

        internal QueryContext<T> AllColumns(bool allColumns = true)
        {
            _expression.ColumnSet.AllColumns = allColumns;
            return this;
        }

        internal QueryContext<T> NoColumns()
        {
            _expression.ColumnSet.AllColumns = false;
            return this;
        }

        internal QueryContext<T> Top(int count)
        {
            _expression.TopCount = count;
            return this;
        }

        internal QueryContext<T> Conditions(LogicalOperator logicalOperator, Action<FilterContext<T>, T> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext<T>(filterExpression);
            configureFilter(filterContext, null);
            _expression.Criteria.AddFilter(filterExpression);

            return this;
        }

        internal QueryContext<T> Join<TOuter>(Expression<Func<T, object>> fromColumnSelector, Expression<Func<TOuter, object>> toColumnSelector, Action<LinkContext<T, TOuter>> configureLink)
            where TOuter : BaseEntity<TOuter>
        {
            return Join(fromColumnSelector, toColumnSelector, JoinOperator.Inner, configureLink);
        }

        internal QueryContext<T> Join<TOuter>(Expression<Func<T, object>> fromColumnSelector, Expression<Func<TOuter, object>> toColumnSelector, JoinOperator joinOperator, Action<LinkContext<T, TOuter>> configureLink)
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

        internal QueryContext<T> Join<TOuter>(Expression<Func<TOuter, object>> toColumnSelector, Action<LinkContext<T, TOuter>> configureLink)
            where TOuter : BaseEntity<TOuter>
        {
            return Join(toColumnSelector, JoinOperator.Inner, configureLink);
        }

        internal QueryContext<T> Join<TOuter>(Expression<Func<TOuter, object>> toColumnSelector, JoinOperator joinOperator, Action<LinkContext<T, TOuter>> configureLink)
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

        internal QueryContext<T> Include<TOuter>(Expression<Func<T, TOuter>> includePropertySelector, Action<IncludeContext<T, TOuter>> configureInclude)
           where TOuter : BaseEntity<TOuter>
        {
            return Include<TOuter>(includePropertySelector, JoinOperator.LeftOuter, configureInclude);
        }

        internal QueryContext<T> Include<TOuter>(Expression<Func<T, TOuter>> includePropertySelector, JoinOperator joinOperator, Action<IncludeContext<T, TOuter>> configureInclude)
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

        internal IEnumerable<T> Execute()
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
