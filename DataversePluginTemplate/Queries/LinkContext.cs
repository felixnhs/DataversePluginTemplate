using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

    /// <summary>
    /// Stellt den Kontext für die Konfiguration von Verknüpfungen (Links) zwischen Entitäten im CRM-System dar.
    /// </summary>
    /// <typeparam name="TInner">Der Typ der inneren Entität.</typeparam>
    /// <typeparam name="TOuter">Der Typ der äußeren Entität.</typeparam>
    internal sealed class LinkContext<TInner, TOuter>
        where TInner : BaseEntity<TInner>
        where TOuter : BaseEntity<TOuter>
    {
        private readonly LinkEntity _linkEntity;
        private readonly string _entityName;
        private readonly string _fromColumn;
        private readonly string _toColumn;

        /// <summary>
        /// Initialisiert eine neue Instanz des LinkContext für die angegebene Link-Entity und deren Konfiguration.
        /// </summary>
        /// <param name="linkEntity">Die Link-Entity, die konfiguriert werden soll.</param>
        /// <param name="entityName">Der Name der äußeren Entität.</param>
        /// <param name="fromColumn">Die Spalte in der inneren Entität, die mit der äußeren Entität verknüpft ist.</param>
        /// <param name="toColumn">Die Spalte in der äußeren Entität, die mit der inneren Entität verknüpft ist.</param>
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
        internal LinkContext<TInner, TOuter> Alias(string alias)
        {
            _linkEntity.EntityAlias = alias;
            return this;
        }

        /// <summary>
        /// Legt die Spaltenauswahl für die Link-Entity basierend auf dem angegebenen Property-Selektor fest.
        /// </summary>
        /// <param name="propertySelector">Lambda-Ausdruck, der die auszuwählenden Eigenschaften definiert.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext<TInner, TOuter> Columns(Expression<Func<TOuter, object[]>> propertySelector)
        {
            if (propertySelector.Body is NewArrayExpression newArrayExpr)
            {
                var propertyInfos = new List<PropertyInfo>();
                foreach (var expression in newArrayExpr.Expressions)
                {
                    if (expression is MemberExpression memberExpression)
                    {
                        var propertyInfo = memberExpression.GetPropertyInfo();
                        propertyInfos.Add(propertyInfo);
                    }
                    else if (expression is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operandMember)
                    {
                        var propertyInfo = operandMember.GetPropertyInfo();
                        propertyInfos.Add(propertyInfo);
                    }
                    else
                    {
                        throw new ArgumentException("Der Ausdruck muss auf eine Eigenschaft zugreifen.", nameof(propertySelector));
                    }
                }

                _linkEntity.Columns.AddColumns(propertyInfos
                    .Select(prop => prop.GetLogicalName())
                    .Where(name => name != null)
                    .ToArray());
            }

            return this;
        }

        /// <summary>
        /// Legt fest, ob alle Spalten der Link-Entity zurückgegeben werden sollen.
        /// </summary>
        /// <param name="allColumns">Ein boolescher Wert, der angibt, ob alle Spalten zurückgegeben werden sollen. Standardmäßig ist dies <c>true</c>.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext<TInner, TOuter> AllColumns(bool allColumns = true)
        {
            _linkEntity.Columns.AllColumns = allColumns;
            return this;
        }

        /// <summary>
        /// Führt einen Join mit einer anderen Entität basierend auf den angegebenen Spaltenauswahlen durch.
        /// </summary>
        /// <typeparam name="T">Der Typ der anderen Entität, mit der gejoint werden soll.</typeparam>
        /// <param name="fromColumnSelector">Lambda-Ausdruck, der die Spalte aus der äußeren Entität auswählt.</param>
        /// <param name="toColumnSelector">Lambda-Ausdruck, der die Spalte aus der anderen Entität auswählt.</param>
        /// <param name="configureLink">Aktion zum Konfigurieren des Link-Kontexts.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext<TInner, TOuter> Join<T>(Expression<Func<TOuter, object>> fromColumnSelector, Expression<Func<T, object>> toColumnSelector, Action<LinkContext<T, TOuter>> configureLink)
            where T : BaseEntity<T>
        {
            return Join(fromColumnSelector, toColumnSelector, JoinOperator.Inner, configureLink);
        }

        /// <summary>
        /// Führt einen Join mit einer anderen Entität basierend auf den angegebenen Spaltenauswahlen und dem Join-Operator durch.
        /// </summary>
        /// <typeparam name="T">Der Typ der anderen Entität, mit der gejoint werden soll.</typeparam>
        /// <param name="fromColumnSelector">Lambda-Ausdruck, der die Spalte aus der äußeren Entität auswählt.</param>
        /// <param name="toColumnSelector">Lambda-Ausdruck, der die Spalte aus der anderen Entität auswählt.</param>
        /// <param name="joinOperator">Der Join-Operator, der den Join-Typ angibt (z. B. Inner, Left Outer).</param>
        /// <param name="configureLink">Aktion zum Konfigurieren des Link-Kontexts.</param>
        /// <returns>Die aktuelle Instanz des LinkContext zur Verkettung weiterer Methodenaufrufe.</returns>
        internal LinkContext<TInner, TOuter> Join<T>(Expression<Func<TOuter, object>> fromColumnSelector, Expression<Func<T, object>> toColumnSelector, JoinOperator joinOperator, Action<LinkContext<T, TOuter>> configureLink)
            where T : BaseEntity<T>
        {
            var entityName = typeof(T).GetLogicalName();
            var fromColumn = fromColumnSelector.GetPropertyInfo().GetLogicalName();
            var toColumn = toColumnSelector.GetPropertyInfo().GetLogicalName();

            var linkEntity = new LinkEntity(_entityName, entityName, fromColumn, toColumn, joinOperator);
            var linkContext = new LinkContext<T, TOuter>(linkEntity, entityName, fromColumn, toColumn);
            configureLink(linkContext);
            _linkEntity.LinkEntities.Add(linkEntity);
            return this;
        }
    }

}
