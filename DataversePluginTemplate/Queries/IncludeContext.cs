using DataversePluginTemplate.Service.Entities;
using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DataversePluginTemplate.Queries
{
    /// <summary>
    /// Wrapper for processing the dataverse join operation in queries, where another
    /// <see cref="BaseEntity{TChild}"/> is included.
    /// </summary>
    /// <typeparam name="TInner">The type of entity used as inner part of the join.</typeparam>
    /// <typeparam name="TOuter">The type of entity used as outer part of the join.</typeparam>
    internal class IncludeContext<TInner, TOuter>
        where TInner : BaseEntity<TInner>
        where TOuter : BaseEntity<TOuter>
    {
        private readonly LinkEntity _linkEntity;
        private readonly IncludeEntity _includeEntity;

        public IncludeContext(LinkEntity linkEntity, IncludeEntity includeEntity)
        {
            _linkEntity = linkEntity;
            _includeEntity = includeEntity;
            _linkEntity.EntityAlias = includeEntity.EntityLogicalName;
        }

        internal IncludeContext<TInner, TOuter> Alias(string entityAlias)
        {
            _linkEntity.EntityAlias = entityAlias;
            _includeEntity.EntityAlias = entityAlias;
            return this;
        }

        internal IncludeContext<TInner, TOuter> Columns(Expression<Func<TOuter, object[]>> propertySelector)
        {
            var propertyInfos = propertySelector.GetPropertyInfos();

            _linkEntity.Columns.AddColumns(propertyInfos
                .Select(prop => prop.GetLogicalName())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray());

            return this;
        }

        internal IncludeContext<TInner, TOuter> Columns(Columns columns)
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

        internal IncludeContext<TInner, TOuter> AllDefinedColumns()
        {
            _linkEntity.Columns.AddColumns(typeof(TOuter).GetAllDefinedLogicalNames());
            return this;
        }

        internal IncludeContext<TInner, TOuter> Conditions(LogicalOperator logicalOperator, Action<FilterContext<TOuter>, TOuter> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext<TOuter>(filterExpression);
            configureFilter(filterContext, null);

            _linkEntity.LinkCriteria.AddFilter(filterExpression);

            return this;
        }
        internal IncludeContext<TInner, TOuter> AllColumns(bool allColumns = true)
        {
            _linkEntity.Columns.AllColumns = allColumns;
            return this;
        }

        internal IncludeContext<TInner, TOuter> ThenInclude<T>(Expression<Func<TOuter, T>> includePropertySelector, Action<IncludeContext<TOuter, T>> configureLink)
            where T : BaseEntity<T>
        {
            return ThenInclude<T>(includePropertySelector, JoinOperator.LeftOuter, configureLink);
        }

        internal IncludeContext<TInner, TOuter> ThenInclude<T>(Expression<Func<TOuter, T>> includePropertySelector, JoinOperator joinOperator, Action<IncludeContext<TOuter, T>> configureLink)
            where T : BaseEntity<T>
        {
            var entityName = typeof(T).GetLogicalName();
            var targetProperty = includePropertySelector.GetPropertyInfo();
            var fromColumn = targetProperty.GetLogicalName();
            var toColumn = typeof(T).GetPrimaryKeyName();

            var includeEntity = new IncludeEntity(typeof(T), entityName, fromColumn, targetProperty);
            var linkEntity = new LinkEntity(_includeEntity.EntityLogicalName, entityName, fromColumn, toColumn, joinOperator);
            var includeContext = new IncludeContext<TOuter, T>(linkEntity, includeEntity);
            configureLink(includeContext);
            _linkEntity.LinkEntities.Add(linkEntity);
            _includeEntity.InnerIncludes.Add(includeEntity);
            _linkEntity.Columns.AddColumn(fromColumn);
            return this;
        }
    }
}
