using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace DataversePluginTemplate.Queries
{
    internal sealed class LinkContext
    {
        private readonly LinkEntity _linkEntity;
        private readonly string _entityName;
        private readonly string _fromColumn;
        private readonly string _toColumn;
        internal LinkContext(LinkEntity linkEntity, string entityName, string fromColumn, string toColumn)
        {
            _entityName = entityName;
            _fromColumn = fromColumn;
            _toColumn = toColumn;
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
            var linkContext = new LinkContext(linkEntity, entityName, fromColumn, toColumn);
            configureLink(linkContext);

            _linkEntity.LinkEntities.Add(linkEntity);
            return this;
        }
    }
}
