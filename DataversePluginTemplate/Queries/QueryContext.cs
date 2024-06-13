using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

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

        internal QueryContext Conditions(LogicalOperator logicalOperator, Action<FilterContext> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext(filterExpression);
            configureFilter(filterContext);

            // TODO: Check ob Filterexpression jetzt aktuell durch Reference
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
            var linkContext = new LinkContext(linkEntity, entityName, fromColumn, toColumn);
            configureLink(linkContext);

            // TODO: Check ob Filterexpression jetzt aktuell durch Reference
            _expression.LinkEntities.Add(linkEntity);
            return this;
        }

        internal EntityCollection Execute()
        {
            return _orgService.RetrieveMultiple(_expression);
        }
    }
}
