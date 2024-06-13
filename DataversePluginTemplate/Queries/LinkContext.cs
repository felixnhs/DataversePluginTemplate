using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace DataversePluginTemplate.Queries
{
    /// <summary>
    /// Stellt Kontextmethoden bereit, um eine Link-Entity in einer QueryExpression zu konfigurieren.
    /// </summary>
    internal sealed class LinkContext
    {
        private readonly LinkEntity _linkEntity;
        private readonly string _entityName;
        private readonly string _fromColumn;
        private readonly string _toColumn;

        /// <summary>
        /// Initialisiert eine neue Instanz des LinkContext mit den angegebenen Parametern.
        /// </summary>
        /// <param name="linkEntity">Die LinkEntity, die konfiguriert werden soll.</param>
        /// <param name="entityName">Der Name der Entität, mit der verknüpft wird.</param>
        /// <param name="fromColumn">Der Name der Spalte in der Ausgangsentität.</param>
        /// <param name="toColumn">Der Name der Spalte in der Zielentität.</param>
        internal LinkContext(LinkEntity linkEntity, string entityName, string fromColumn, string toColumn)
        {
            _entityName = entityName;
            _fromColumn = fromColumn;
            _toColumn = toColumn;
            _linkEntity = linkEntity;
        }

        /// <summary>
        /// Legt den Alias für die Link-Entity fest.
        /// </summary>
        /// <param name="alias">Der Alias für die Link-Entity.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext Alias(string alias)
        {
            _linkEntity.EntityAlias = alias;
            return this;
        }

        /// <summary>
        /// Fügt der Link-Entity die angegebenen Spalten hinzu.
        /// </summary>
        /// <param name="columns">Die Spalten, die der Link-Entity hinzugefügt werden sollen.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext Columns(params string[] columns)
        {
            _linkEntity.Columns.AddColumns(columns);
            return this;
        }

        /// <summary>
        /// Fügt der Link-Entity die angegebenen Spalten hinzu.
        /// </summary>
        /// <param name="columns">Eine Auflistung der Spalten, die der Link-Entity hinzugefügt werden sollen.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext Columns(IEnumerable<string> columns)
        {
            foreach (var column in columns)
                _linkEntity.Columns.AddColumn(column);

            return this;
        }

        /// <summary>
        /// Legt fest, ob alle Spalten der Link-Entity zurückgegeben werden sollen.
        /// </summary>
        /// <param name="allColumns">Ein boolescher Wert, der angibt, ob alle Spalten zurückgegeben werden sollen. Standardmäßig ist dies <c>true</c>.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext AllColumns(bool allColumns = true)
        {
            _linkEntity.Columns.AllColumns = allColumns;
            return this;
        }

        /// <summary>
        /// Fügt der Link-Entity Bedingungen mit dem angegebenen logischen Operator hinzu.
        /// </summary>
        /// <param name="logicalOperator">Der logische Operator, der für die Bedingung verwendet wird.</param>
        /// <param name="configureFilter">Eine Aktion, die den Filter für die Link-Entity konfiguriert.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext Conditions(LogicalOperator logicalOperator, Action<FilterContext> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext(filterExpression);
            configureFilter(filterContext);

            _linkEntity.LinkCriteria.AddFilter(filterExpression);

            return this;
        }

        /// <summary>
        /// Fügt eine innere Verknüpfung mit der angegebenen Entität und den Spalten hinzu.
        /// </summary>
        /// <param name="entityName">Der Name der Entität, mit der verknüpft werden soll.</param>
        /// <param name="fromColumn">Der Name der Spalte in der Ausgangsentität.</param>
        /// <param name="toColumn">Der Name der Spalte in der Zielentität.</param>
        /// <param name="configureLink">Eine Aktion, die den Verknüpfungskontext konfiguriert.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext Join(string entityName, string fromColumn, string toColumn, Action<LinkContext> configureLink)
        {
            return Join(entityName, fromColumn, toColumn, JoinOperator.Inner, configureLink);
        }

        /// <summary>
        /// Fügt eine Verknüpfung mit der angegebenen Entität, den Spalten und dem Verknüpfungsoperator hinzu.
        /// </summary>
        /// <param name="entityName">Der Name der Entität, mit der verknüpft werden soll.</param>
        /// <param name="fromColumn">Der Name der Spalte in der Ausgangsentität.</param>
        /// <param name="toColumn">Der Name der Spalte in der Zielentität.</param>
        /// <param name="joinOperator">Der Operator, der für die Verknüpfung verwendet wird.</param>
        /// <param name="configureLink">Eine Aktion, die den Verknüpfungskontext konfiguriert.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
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
