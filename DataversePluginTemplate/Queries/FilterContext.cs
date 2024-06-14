using Microsoft.Xrm.Sdk.Query;
using System;

namespace DataversePluginTemplate.Queries
{
    /// <summary>
    /// Stellt Methoden bereit, um Bedingungsausdrücke zur Filterung in einer FilterExpression zu konfigurieren.
    /// </summary>
    internal sealed class FilterContext
    {
        private readonly FilterExpression _filterExpression;

        /// <summary>
        /// Initialisiert eine neue Instanz des FilterContext mit der angegebenen FilterExpression.
        /// </summary>
        /// <param name="filterExpression">Die FilterExpression, die konfiguriert werden soll.</param>
        internal FilterContext(FilterExpression filterExpression)
        {
            _filterExpression = filterExpression;
        }

        #region - Methoden für Condtions -

        internal FilterContext Equals(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Equal);
        internal FilterContext NotEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotEqual);
        internal FilterContext Greater(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.GreaterThan);
        internal FilterContext LessThan(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LessThan);
        internal FilterContext GreaterEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.GreaterEqual);
        internal FilterContext LessEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LessEqual);
        internal FilterContext Like(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Like);
        internal FilterContext NotLike(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotLike);
        internal FilterContext In(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.In);
        internal FilterContext NotIn(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotIn);
        internal FilterContext Between(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Between);
        internal FilterContext NotBetween(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotBetween);
        internal FilterContext IsNull(string columnName) => HandleInternal(columnName, null, ConditionOperator.Null);
        internal FilterContext IsNotNull(string columnName) => HandleInternal(columnName, null, ConditionOperator.NotNull);
        internal FilterContext Yesterday(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Yesterday);
        internal FilterContext Today(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Today);
        internal FilterContext Tomorrow(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Tomorrow);
        internal FilterContext Last7Days(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Last7Days);
        internal FilterContext Next7Days(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Next7Days);
        internal FilterContext LastWeek(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastWeek);
        internal FilterContext ThisWeek(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisWeek);
        internal FilterContext NextWeek(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextWeek);
        internal FilterContext LastMonth(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastMonth);
        internal FilterContext ThisMonth(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisMonth);
        internal FilterContext NextMonth(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextMonth);
        internal FilterContext On(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.On);
        internal FilterContext OnOrBefore(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OnOrBefore);
        internal FilterContext OnOrAfter(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OnOrAfter);
        internal FilterContext LastYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastYear);
        internal FilterContext ThisYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisYear);
        internal FilterContext NextYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextYear);
        internal FilterContext LastXHours(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXHours);
        internal FilterContext NextXHours(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXHours);
        internal FilterContext LastXDays(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXDays);
        internal FilterContext NextXDays(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXDays);
        internal FilterContext LastXWeeks(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXWeeks);
        internal FilterContext NextXWeeks(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXWeeks);
        internal FilterContext LastXMonths(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXMonths);
        internal FilterContext NextXMonths(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXMonths);
        internal FilterContext LastXYears(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXYears);
        internal FilterContext NextXYears(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXYears);
        internal FilterContext EqualUserId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserId);
        internal FilterContext NotEqualUserId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotEqualUserId);
        internal FilterContext EqualBusinessId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualBusinessId);
        internal FilterContext NotEqualBusinessId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotEqualBusinessId);
        internal FilterContext ChildOf(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.ChildOf);
        internal FilterContext Mask(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Mask);
        internal FilterContext NotMask(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotMask);
        internal FilterContext MasksSelect(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.MasksSelect);
        internal FilterContext Contains(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Contains);
        internal FilterContext DoesNotContain(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.DoesNotContain);
        internal FilterContext EqualUserLanguage(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserLanguage);
        internal FilterContext NotOn(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotOn);
        internal FilterContext OlderThanXMonths(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXMonths);
        internal FilterContext BeginsWith(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.BeginsWith);
        internal FilterContext DoesNotBeginWith(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.DoesNotBeginWith);
        internal FilterContext EndsWith(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EndsWith);
        internal FilterContext DoesNotEndWith(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.DoesNotEndWith);
        internal FilterContext ThisFiscalYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisFiscalYear);
        internal FilterContext ThisFiscalPeriod(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisFiscalPeriod);
        internal FilterContext NextFiscalYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextFiscalYear);
        internal FilterContext NextFiscalPeriod(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextFiscalPeriod);
        internal FilterContext LastFiscalYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastFiscalYear);
        internal FilterContext LastFiscalPeriod(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastFiscalPeriod);
        internal FilterContext LastXFiscalYears(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastXFiscalYears);
        internal FilterContext LastXFiscalPeriods(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastXFiscalPeriods);
        internal FilterContext NextXFiscalYears(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXFiscalYears);
        internal FilterContext NextXFiscalPeriods(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXFiscalPeriods);
        internal FilterContext InFiscalYear(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InFiscalYear);
        internal FilterContext InFiscalPeriod(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InFiscalPeriod);
        internal FilterContext InFiscalPeriodAndYear(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InFiscalPeriodAndYear);
        internal FilterContext InOrBeforeFiscalPeriodAndYear(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
        internal FilterContext InOrAfterFiscalPeriodAndYear(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InOrAfterFiscalPeriodAndYear);
        internal FilterContext EqualUserTeams(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserTeams);
        internal FilterContext EqualUserOrUserTeams(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserOrUserTeams);
        internal FilterContext Under(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Under);
        internal FilterContext NotUnder(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotUnder);
        internal FilterContext UnderOrEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.UnderOrEqual);
        internal FilterContext Above(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Above);
        internal FilterContext AboveOrEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.AboveOrEqual);
        internal FilterContext EqualUserOrUserHierarchy(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserOrUserHierarchy);
        internal FilterContext EqualUserOrUserHierarchyAndTeams(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
        internal FilterContext OlderThanXYears(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXYears);
        internal FilterContext OlderThanXWeeks(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXWeeks);
        internal FilterContext OlderThanXDays(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXDays);
        internal FilterContext OlderThanXHours(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXHours);
        internal FilterContext OlderThanXMinutes(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXMinutes);
        internal FilterContext ContainValues(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.ContainValues);
        internal FilterContext DoesNotContainValues(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.DoesNotContainValues);
        internal FilterContext EqualRoleBusinessId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualRoleBusinessId);

        #endregion

        /// <summary>
        /// Fügt der Abfrage Bedingungen mit dem angegebenen logischen Operator hinzu.
        /// </summary>
        /// <param name="logicalOperator">Der logische Operator, der für die Bedingung verwendet wird.</param>
        /// <param name="configureFilter">Eine Aktion, die den Filter konfiguriert.</param>
        /// <returns>Die aktuelle Instanz der <see cref="FilterContext"/> zur Verkettung weiterer Methodenaufrufe.</returns>
        internal FilterContext Conditions(LogicalOperator logicalOperator, Action<FilterContext> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);
            var filterContext = new FilterContext(filterExpression);
            configureFilter(filterContext);
        
            _filterExpression.Filters.Add(filterExpression);
        
            return this;
        }
            
        /// <summary>
        /// Fügt eine Bedingung mit dem angegebenen Spaltennamen, Wert und Bedingungsoperator zur FilterExpression hinzu.
        /// </summary>
        /// <param name="columnName">Der Name der Spalte, auf die die Bedingung angewendet wird.</param>
        /// <param name="value">Der Wert, mit dem die Bedingung verglichen wird.</param>
        /// <param name="conditionOperator">Der Operator, der angibt, wie die Bedingung ausgewertet wird.</param>
        /// <returns>Eine Instanz von FilterContext für method chaining.</returns>
        private FilterContext HandleInternal(string columnName, object value, ConditionOperator conditionOperator)
        {
            var condition = new ConditionExpression(columnName, ConditionOperator.Equal, value);
            _filterExpression.Conditions.Add(condition);
            return this;
        }
    }
}
