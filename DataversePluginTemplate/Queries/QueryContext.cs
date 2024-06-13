using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace DataversePluginTemplate.Queries
{
    /// <summary>
    /// Stellt eine kontextspezifische Abstraktion für das Erstellen und Ausführen von Abfragen gegen ein CRM-System bereit.
    /// Diese Klasse bietet Methoden zum Aufbau komplexer Abfragen mit Filtern, Verknüpfungen und Spaltenauswahlen.
    /// </summary>
    internal sealed class QueryContext
    {
        // Der Dienst zum Ausführen von Abfragen gegen das CRM-System.
        private readonly IOrganizationService _orgService;

        // Der Ausdruck, der die Abfrage definiert, die gegen das CRM-System ausgeführt werden soll.
        private readonly QueryExpression _expression;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="QueryContext"/> Klasse mit dem angegebenen Dienst und Entitätsnamen.
        /// </summary>
        /// <param name="orgService">Der <see cref="IOrganizationService"/>, der die Abfrage ausführt.</param>
        /// <param name="entityName">Der Name der Entität, gegen die die Abfrage ausgeführt wird.</param>
        internal QueryContext(IOrganizationService orgService, string entityName)
        {
            _orgService = orgService;
            _expression = new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet(),
                Criteria = new FilterExpression(LogicalOperator.And),
            };
        }

        /// <summary>
        /// Fügt der Abfrage die angegebenen Spalten hinzu.
        /// </summary>
        /// <param name="columns">Die Spalten, die der Abfrage hinzugefügt werden sollen.</param>
        /// <returns>Die aktuelle Instanz der <see cref="QueryContext"/> zur Verkettung weiterer Methodenaufrufe.</returns>
        internal QueryContext Columns(params string[] columns)
        {
            _expression.ColumnSet.AddColumns(columns);
            return this;
        }

        /// <summary>
        /// Fügt der Abfrage die angegebenen Spalten hinzu.
        /// </summary>
        /// <param name="columns">Eine Auflistung der Spalten, die der Abfrage hinzugefügt werden sollen.</param>
        /// <returns>Die aktuelle Instanz der <see cref="QueryContext"/> zur Verkettung weiterer Methodenaufrufe.</returns>
        internal QueryContext Columns(IEnumerable<string> columns)
        {
            foreach (var column in columns)
                _expression.ColumnSet.AddColumn(column);

            return this;
        }

        /// <summary>
        /// Gibt an, ob alle Spalten der Entität in die Abfrage aufgenommen werden sollen.
        /// </summary>
        /// <param name="allColumns">Ein boolescher Wert, der angibt, ob alle Spalten aufgenommen werden sollen. Der Standardwert ist <c>true</c>.</param>
        /// <returns>Die aktuelle Instanz der <see cref="QueryContext"/> zur Verkettung weiterer Methodenaufrufe.</returns>
        internal QueryContext AllColumns(bool allColumns = true)
        {
            _expression.ColumnSet.AllColumns = allColumns;
            return this;
        }

        /// <summary>
        /// Beschränkt die Abfrage auf eine bestimmte Anzahl von Ergebnissen.
        /// </summary>
        /// <param name="count">Die maximale Anzahl von Ergebnissen, die zurückgegeben werden sollen.</param>
        /// <returns>Die aktuelle Instanz der <see cref="QueryContext"/> zur Verkettung weiterer Methodenaufrufe.</returns>
        internal QueryContext Top(int count)
        {
            _expression.TopCount = count;
            return this;
        }

        /// <summary>
        /// Fügt der Abfrage Bedingungen mit dem angegebenen logischen Operator hinzu.
        /// </summary>
        /// <param name="logicalOperator">Der logische Operator, der für die Bedingung verwendet wird.</param>
        /// <param name="configureFilter">Eine Aktion, die den Filter konfiguriert.</param>
        /// <returns>Die aktuelle Instanz der <see cref="QueryContext"/> zur Verkettung weiterer Methodenaufrufe.</returns>
        internal QueryContext Conditions(LogicalOperator logicalOperator, Action<FilterContext> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);

            var filterContext = new FilterContext(filterExpression);
            configureFilter(filterContext);

            // TODO: Check ob Filterexpression jetzt aktuell durch Reference
            _expression.Criteria.AddFilter(filterExpression);

            return this;
        }

        /// <summary>
        /// Fügt eine innere Verknüpfung mit der angegebenen Entität und den Spalten hinzu.
        /// </summary>
        /// <param name="entityName">Der Name der zu verknüpfenden Entität.</param>
        /// <param name="fromColumn">Der Name der Spalte in der Ausgangsentität, die verknüpft werden soll.</param>
        /// <param name="toColumn">Der Name der Spalte in der Zielentität, die verknüpft werden soll.</param>
        /// <param name="configureLink">Eine Aktion, die den Verknüpfungskontext konfiguriert.</param>
        /// <returns>Die aktuelle Instanz der <see cref="QueryContext"/> zur Verkettung weiterer Methodenaufrufe.</returns>
        internal QueryContext Join(string entityName, string fromColumn, string toColumn, Action<LinkContext> configureLink)
        {
            return Join(entityName, fromColumn, toColumn, JoinOperator.Inner, configureLink);
        }

        /// <summary>
        /// Fügt eine Verknüpfung mit der angegebenen Entität, den Spalten und dem Verknüpfungsoperator hinzu.
        /// </summary>
        /// <param name="entityName">Der Name der zu verknüpfenden Entität.</param>
        /// <param name="fromColumn">Der Name der Spalte in der Ausgangsentität, die verknüpft werden soll.</param>
        /// <param name="toColumn">Der Name der Spalte in der Zielentität, die verknüpft werden soll.</param>
        /// <param name="joinOperator">Der Operator, der für die Verknüpfung verwendet wird.</param>
        /// <param name="configureLink">Eine Aktion, die den Verknüpfungskontext konfiguriert.</param>
        /// <returns>Die aktuelle Instanz der <see cref="QueryContext"/> zur Verkettung weiterer Methodenaufrufe.</returns>
        internal QueryContext Join(string entityName, string fromColumn, string toColumn, JoinOperator joinOperator, Action<LinkContext> configureLink)
        {
            var linkEntity = new LinkEntity(_expression.EntityName, entityName, fromColumn, toColumn, joinOperator);
            var linkContext = new LinkContext(linkEntity, entityName, fromColumn, toColumn);
            configureLink(linkContext);

            // TODO: Check ob Filterexpression jetzt aktuell durch Reference
            _expression.LinkEntities.Add(linkEntity);
            return this;
        }

        /// <summary>
        /// Führt die konfigurierte Abfrage aus und gibt die Ergebnisse als <see cref="EntityCollection"/> zurück.
        /// </summary>
        /// <returns>Die <see cref="EntityCollection"/>, die die Ergebnisse der Abfrage enthält.</returns>
        internal EntityCollection Execute()
        {
            return _orgService.RetrieveMultiple(_expression);
        }
    }
}
