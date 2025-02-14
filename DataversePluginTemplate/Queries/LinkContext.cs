using DataversePluginTemplate.Service.Entities;
using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataversePluginTemplate.Queries
{
    /// <summary>
    /// Wrapper for joining entities in <see cref="QueryContext"/>.
    /// </summary>
    internal sealed class LinkContext
    {
        private readonly LinkEntity _linkEntity;
        private readonly string _entityName;

        internal LinkContext(LinkEntity linkEntity, string entityName)
        {
            _entityName = entityName;
            _linkEntity = linkEntity;
        }

        internal LinkContext Alias(string alias)
        {
            _linkEntity.EntityAlias = alias;
            return this;
        }

        internal LinkContext Columns(params string[] columns)
        {
            _linkEntity.Columns.AddColumns(columns);
            return this;
        }

        internal LinkContext Columns(IEnumerable<string> columns)
        {
            foreach (var column in columns)
                _linkEntity.Columns.AddColumn(column);

            return this;
        }

        internal LinkContext AllColumns(bool allColumns = true)
        {
            _linkEntity.Columns.AllColumns = allColumns;
            return this;
        }

        internal LinkContext Conditions(LogicalOperator logicalOperator, Action<FilterContext> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext(filterExpression);
            configureFilter(filterContext);

            _linkEntity.LinkCriteria.AddFilter(filterExpression);

            return this;
        }

        internal LinkContext Join(string entityName, string fromColumn, string toColumn, Action<LinkContext> configureLink)
        {
            return Join(entityName, fromColumn, toColumn, JoinOperator.Inner, configureLink);
        }

        internal LinkContext Join(string entityName, string fromColumn, string toColumn, JoinOperator joinOperator, Action<LinkContext> configureLink)
        {
            var linkEntity = new LinkEntity(_entityName, entityName, fromColumn, toColumn, joinOperator);
            var linkContext = new LinkContext(linkEntity, entityName);
            configureLink(linkContext);

            _linkEntity.LinkEntities.Add(linkEntity);
            return this;
        }
    }

    /// <summary>
    /// Wrapper for joining entities in <see cref="QueryContext{T}"/>.
    /// </summary>
    /// <typeparam name="TInner">The type of entity used as inner part of the join.</typeparam>
    /// <typeparam name="TOuter">The type of entity used as outer part of the join.</typeparam>
    internal sealed class LinkContext<TInner, TOuter>
        where TInner : BaseEntity<TInner>
        where TOuter : BaseEntity<TOuter>
    {
        private readonly LinkEntity _linkEntity;
        private readonly string _entityName;

        internal LinkContext(LinkEntity linkEntity, string entityName)
        {
            _entityName = entityName;
            _linkEntity = linkEntity;
        }

        internal LinkContext<TInner, TOuter> Alias(string alias)
        {
            _linkEntity.EntityAlias = alias;
            return this;
        }

        internal LinkContext<TInner, TOuter> Columns(Expression<Func<TOuter, object[]>> propertySelector)
        {
            var propertyInfos = propertySelector.GetPropertyInfos();

            _linkEntity.Columns.AddColumns(propertyInfos
                .Select(prop => prop.GetLogicalName())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray());

            return this;
        }

        internal LinkContext<TInner, TOuter> Columns(Columns columns)
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

        internal LinkContext<TInner, TOuter> AllDefinedColumns()
        {
            _linkEntity.Columns.AddColumns(typeof(TOuter).GetAllDefinedLogicalNames());
            return this;
        }

        internal LinkContext<TInner, TOuter> Conditions(LogicalOperator logicalOperator, Action<FilterContext<TOuter>, TOuter> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext<TOuter>(filterExpression);
            configureFilter(filterContext, null);

            _linkEntity.LinkCriteria.AddFilter(filterExpression);

            return this;
        }

        internal LinkContext<TInner, TOuter> AllColumns(bool allColumns = true)
        {
            _linkEntity.Columns.AllColumns = allColumns;
            return this;
        }

        internal LinkContext<TInner, TOuter> Join<T>(Expression<Func<TOuter, object>> fromColumnSelector, Expression<Func<T, object>> toColumnSelector, Action<LinkContext<T, TOuter>> configureLink)
            where T : BaseEntity<T>
        {
            return Join(fromColumnSelector, toColumnSelector, JoinOperator.Inner, configureLink);
        }

        internal LinkContext<TInner, TOuter> Join<T>(Expression<Func<TOuter, object>> fromColumnSelector, Expression<Func<T, object>> toColumnSelector, JoinOperator joinOperator, Action<LinkContext<T, TOuter>> configureLink)
            where T : BaseEntity<T>
        {
            var entityName = typeof(T).GetLogicalName();
            var fromColumn = fromColumnSelector.GetPropertyInfo().GetLogicalName();
            var toColumn = toColumnSelector.GetPropertyInfo().GetLogicalName();

            var linkEntity = new LinkEntity(_entityName, entityName, fromColumn, toColumn, joinOperator);
            var linkContext = new LinkContext<T, TOuter>(linkEntity, entityName);
            configureLink(linkContext);
            _linkEntity.LinkEntities.Add(linkEntity);
            return this;
        }

        internal LinkContext<TInner, TOuter> Join<T>(Expression<Func<T, object>> toColumnSelector, Action<LinkContext<T, TOuter>> configureLink)
            where T : BaseEntity<T>
        {
            return Join(toColumnSelector, JoinOperator.Inner, configureLink);
        }

        internal LinkContext<TInner, TOuter> Join<T>(Expression<Func<T, object>> toColumnSelector, JoinOperator joinOperator, Action<LinkContext<T, TOuter>> configureLink)
            where T : BaseEntity<T>
        {
            var entityName = typeof(T).GetLogicalName();
            var fromColumn = typeof(TOuter).GetPrimaryKeyName();
            var toColumn = toColumnSelector.GetPropertyInfo().GetLogicalName();

            var linkEntity = new LinkEntity(_entityName, entityName, fromColumn, toColumn, joinOperator);
            var linkContext = new LinkContext<T, TOuter>(linkEntity, entityName);
            configureLink(linkContext);
            _linkEntity.LinkEntities.Add(linkEntity);
            return this;
        }
    }
}
