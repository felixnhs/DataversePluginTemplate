using DataversePluginTemplate.Service.Entities;
using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq.Expressions;

namespace DataversePluginTemplate.Queries
{
    /// <summary>
    /// Wrapper for handleing filter operations in queries.
    /// <seealso cref="QueryContext"/>.
    /// </summary>
    public sealed class FilterContext
    {
        private readonly FilterExpression _filterExpression;

        internal FilterContext(FilterExpression filterExpression)
        {
            _filterExpression = filterExpression;
        }

        #region - Methoden für Condtions -

        public FilterContext Equals(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Equal);
        public FilterContext NotEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotEqual);
        public FilterContext Greater(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.GreaterThan);
        public FilterContext LessThan(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LessThan);
        public FilterContext GreaterEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.GreaterEqual);
        public FilterContext LessEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LessEqual);
        public FilterContext Like(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Like);
        public FilterContext NotLike(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotLike);
        public FilterContext In(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.In);
        public FilterContext NotIn(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotIn);
        public FilterContext Between(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Between);
        public FilterContext NotBetween(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotBetween);
        public FilterContext IsNull(string columnName) => HandleInternal(columnName, ConditionOperator.Null);
        public FilterContext IsNotNull(string columnName) => HandleInternal(columnName, ConditionOperator.NotNull);
        public FilterContext Yesterday(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Yesterday);
        public FilterContext Today(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Today);
        public FilterContext Tomorrow(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Tomorrow);
        public FilterContext Last7Days(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Last7Days);
        public FilterContext Next7Days(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.Next7Days);
        public FilterContext LastWeek(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastWeek);
        public FilterContext ThisWeek(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisWeek);
        public FilterContext NextWeek(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextWeek);
        public FilterContext LastMonth(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastMonth);
        public FilterContext ThisMonth(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisMonth);
        public FilterContext NextMonth(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextMonth);
        public FilterContext On(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.On);
        public FilterContext OnOrBefore(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OnOrBefore);
        public FilterContext OnOrAfter(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OnOrAfter);
        public FilterContext LastYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastYear);
        public FilterContext ThisYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisYear);
        public FilterContext NextYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextYear);
        public FilterContext LastXHours(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXHours);
        public FilterContext NextXHours(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXHours);
        public FilterContext LastXDays(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXDays);
        public FilterContext NextXDays(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXDays);
        public FilterContext LastXWeeks(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXWeeks);
        public FilterContext NextXWeeks(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXWeeks);
        public FilterContext LastXMonths(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXMonths);
        public FilterContext NextXMonths(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXMonths);
        public FilterContext LastXYears(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.LastXYears);
        public FilterContext NextXYears(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXYears);
        public FilterContext EqualUserId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserId);
        public FilterContext NotEqualUserId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotEqualUserId);
        public FilterContext EqualBusinessId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualBusinessId);
        public FilterContext NotEqualBusinessId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotEqualBusinessId);
        public FilterContext ChildOf(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.ChildOf);
        public FilterContext Mask(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Mask);
        public FilterContext NotMask(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotMask);
        public FilterContext MasksSelect(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.MasksSelect);
        public FilterContext Contains(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Contains);
        public FilterContext DoesNotContain(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.DoesNotContain);
        public FilterContext EqualUserLanguage(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserLanguage);
        public FilterContext NotOn(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotOn);
        public FilterContext OlderThanXMonths(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXMonths);
        public FilterContext BeginsWith(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.BeginsWith);
        public FilterContext DoesNotBeginWith(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.DoesNotBeginWith);
        public FilterContext EndsWith(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EndsWith);
        public FilterContext DoesNotEndWith(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.DoesNotEndWith);
        public FilterContext ThisFiscalYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisFiscalYear);
        public FilterContext ThisFiscalPeriod(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.ThisFiscalPeriod);
        public FilterContext NextFiscalYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextFiscalYear);
        public FilterContext NextFiscalPeriod(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.NextFiscalPeriod);
        public FilterContext LastFiscalYear(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastFiscalYear);
        public FilterContext LastFiscalPeriod(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastFiscalPeriod);
        public FilterContext LastXFiscalYears(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastXFiscalYears);
        public FilterContext LastXFiscalPeriods(string columnName, object value = null) => HandleInternal(columnName, value, ConditionOperator.LastXFiscalPeriods);
        public FilterContext NextXFiscalYears(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXFiscalYears);
        public FilterContext NextXFiscalPeriods(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NextXFiscalPeriods);
        public FilterContext InFiscalYear(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InFiscalYear);
        public FilterContext InFiscalPeriod(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InFiscalPeriod);
        public FilterContext InFiscalPeriodAndYear(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InFiscalPeriodAndYear);
        public FilterContext InOrBeforeFiscalPeriodAndYear(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
        public FilterContext InOrAfterFiscalPeriodAndYear(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.InOrAfterFiscalPeriodAndYear);
        public FilterContext EqualUserTeams(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserTeams);
        public FilterContext EqualUserOrUserTeams(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserOrUserTeams);
        public FilterContext Under(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Under);
        public FilterContext NotUnder(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.NotUnder);
        public FilterContext UnderOrEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.UnderOrEqual);
        public FilterContext Above(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.Above);
        public FilterContext AboveOrEqual(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.AboveOrEqual);
        public FilterContext EqualUserOrUserHierarchy(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserOrUserHierarchy);
        public FilterContext EqualUserOrUserHierarchyAndTeams(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
        public FilterContext OlderThanXYears(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXYears);
        public FilterContext OlderThanXWeeks(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXWeeks);
        public FilterContext OlderThanXDays(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXDays);
        public FilterContext OlderThanXHours(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXHours);
        public FilterContext OlderThanXMinutes(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.OlderThanXMinutes);
        public FilterContext ContainValues(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.ContainValues);
        public FilterContext DoesNotContainValues(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.DoesNotContainValues);
        public FilterContext EqualRoleBusinessId(string columnName, object value) => HandleInternal(columnName, value, ConditionOperator.EqualRoleBusinessId);

        #endregion

        public FilterContext Conditions(LogicalOperator logicalOperator, Action<FilterContext> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);
            var filterContext = new FilterContext(filterExpression);
            configureFilter(filterContext);

            _filterExpression.Filters.Add(filterExpression);

            return this;
        }

        private FilterContext HandleInternal(string columnName, object value, ConditionOperator conditionOperator)
        {
            var condition = new ConditionExpression(columnName, conditionOperator, value);
            _filterExpression.Conditions.Add(condition);
            return this;
        }
        
        private FilterContext HandleInternal(string columnName, ConditionOperator conditionOperator)
        {
            var condition = new ConditionExpression(columnName, conditionOperator);
            _filterExpression.Conditions.Add(condition);
            return this;
        }
    }

    /// <summary>
    /// Wrapper for handleing filter operations in queries.
    /// <seealso cref="QueryContext{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of entity to query.</typeparam>
    public sealed class FilterContext<T>
        where T : BaseEntity<T>
    {
        private readonly FilterExpression _filterExpression;

        internal FilterContext(FilterExpression filterExpression)
        {
            _filterExpression = filterExpression;
        }

        #region - Methoden für Condtions -

        public FilterContext<T> Equals<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Equal);
        public FilterContext<T> NotEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotEqual);
        public FilterContext<T> Greater<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.GreaterThan);
        public FilterContext<T> LessThan<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LessThan);
        public FilterContext<T> GreaterEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.GreaterEqual);
        public FilterContext<T> LessEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LessEqual);
        public FilterContext<T> Like<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Like);
        public FilterContext<T> NotLike<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotLike);
        public FilterContext<T> In<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.In);
        public FilterContext<T> NotIn<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotIn);
        public FilterContext<T> Between<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Between);
        public FilterContext<T> NotBetween<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotBetween);
        public FilterContext<T> IsNull<TProperty>(Expression<Func<TProperty>> propertySelector) => HandleInternal(propertySelector, ConditionOperator.Null);
        public FilterContext<T> IsNotNull<TProperty>(Expression<Func<TProperty>> propertySelector) => HandleInternal(propertySelector, ConditionOperator.NotNull);
        public FilterContext<T> Yesterday<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Yesterday);
        public FilterContext<T> Today<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Today);
        public FilterContext<T> Tomorrow<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Tomorrow);
        public FilterContext<T> Last7Days<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Last7Days);
        public FilterContext<T> Next7Days<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Next7Days);
        public FilterContext<T> LastWeek<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastWeek);
        public FilterContext<T> ThisWeek<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisWeek);
        public FilterContext<T> NextWeek<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextWeek);
        public FilterContext<T> LastMonth<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastMonth);
        public FilterContext<T> ThisMonth<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisMonth);
        public FilterContext<T> NextMonth<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextMonth);
        public FilterContext<T> On<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.On);
        public FilterContext<T> OnOrBefore<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OnOrBefore);
        public FilterContext<T> OnOrAfter<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OnOrAfter);
        public FilterContext<T> LastYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastYear);
        public FilterContext<T> ThisYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisYear);
        public FilterContext<T> NextYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextYear);
        public FilterContext<T> LastXHours<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXHours);
        public FilterContext<T> NextXHours<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXHours);
        public FilterContext<T> LastXDays<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXDays);
        public FilterContext<T> NextXDays<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXDays);
        public FilterContext<T> LastXWeeks<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXWeeks);
        public FilterContext<T> NextXWeeks<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXWeeks);
        public FilterContext<T> LastXMonths<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXMonths);
        public FilterContext<T> NextXMonths<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXMonths);
        public FilterContext<T> LastXYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXYears);
        public FilterContext<T> NextXYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXYears);
        public FilterContext<T> EqualUserId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserId);
        public FilterContext<T> NotEqualUserId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotEqualUserId);
        public FilterContext<T> EqualBusinessId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualBusinessId);
        public FilterContext<T> NotEqualBusinessId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotEqualBusinessId);
        public FilterContext<T> ChildOf<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.ChildOf);
        public FilterContext<T> Mask<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Mask);
        public FilterContext<T> NotMask<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotMask);
        public FilterContext<T> MasksSelect<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.MasksSelect);
        public FilterContext<T> Contains<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Contains);
        public FilterContext<T> DoesNotContain<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.DoesNotContain);
        public FilterContext<T> EqualUserLanguage<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserLanguage);
        public FilterContext<T> NotOn<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotOn);
        public FilterContext<T> OlderThanXMonths<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXMonths);
        public FilterContext<T> BeginsWith<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.BeginsWith);
        public FilterContext<T> DoesNotBeginWith<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.DoesNotBeginWith);
        public FilterContext<T> EndsWith<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EndsWith);
        public FilterContext<T> DoesNotEndWith<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.DoesNotEndWith);
        public FilterContext<T> ThisFiscalYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisFiscalYear);
        public FilterContext<T> ThisFiscalPeriod<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisFiscalPeriod);
        public FilterContext<T> NextFiscalYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextFiscalYear);
        public FilterContext<T> NextFiscalPeriod<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextFiscalPeriod);
        public FilterContext<T> LastFiscalYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastFiscalYear);
        public FilterContext<T> LastFiscalPeriod<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastFiscalPeriod);
        public FilterContext<T> LastXFiscalYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastXFiscalYears);
        public FilterContext<T> LastXFiscalPeriods<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastXFiscalPeriods);
        public FilterContext<T> NextXFiscalYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXFiscalYears);
        public FilterContext<T> NextXFiscalPeriods<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXFiscalPeriods);
        public FilterContext<T> InFiscalYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InFiscalYear);
        public FilterContext<T> InFiscalPeriod<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InFiscalPeriod);
        public FilterContext<T> InFiscalPeriodAndYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InFiscalPeriodAndYear);
        public FilterContext<T> InOrBeforeFiscalPeriodAndYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
        public FilterContext<T> InOrAfterFiscalPeriodAndYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InOrAfterFiscalPeriodAndYear);
        public FilterContext<T> EqualUserTeams<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserTeams);
        public FilterContext<T> EqualUserOrUserTeams<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserOrUserTeams);
        public FilterContext<T> Under<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Under);
        public FilterContext<T> NotUnder<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotUnder);
        public FilterContext<T> UnderOrEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.UnderOrEqual);
        public FilterContext<T> Above<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Above);
        public FilterContext<T> AboveOrEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.AboveOrEqual);
        public FilterContext<T> EqualUserOrUserHierarchy<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserOrUserHierarchy);
        public FilterContext<T> EqualUserOrUserHierarchyAndTeams<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
        public FilterContext<T> OlderThanXYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXYears);
        public FilterContext<T> OlderThanXWeeks<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXWeeks);
        public FilterContext<T> OlderThanXDays<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXDays);
        public FilterContext<T> OlderThanXHours<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXHours);
        public FilterContext<T> OlderThanXMinutes<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXMinutes);
        public FilterContext<T> ContainValues<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.ContainValues);
        public FilterContext<T> DoesNotContainValues<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.DoesNotContainValues);
        public FilterContext<T> EqualRoleBusinessId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualRoleBusinessId);

        #endregion

        public FilterContext<T> Conditions(LogicalOperator logicalOperator, Action<FilterContext<T>> configureFilter)
        {
            var filterExpression = new FilterExpression(logicalOperator);
            var filterContext = new FilterContext<T>(filterExpression);
            configureFilter(filterContext);

            _filterExpression.Filters.Add(filterExpression);

            return this;
        }

        private FilterContext<T> HandleInternal<TProperty>(Expression<Func<TProperty>> propertySelector, ConditionOperator conditionOperator)
        {
            var propertyInfo = ExpressionExtensionMethods.GetPropertyInfo<T, TProperty>(propertySelector);
            var logicalName = propertyInfo.GetLogicalName();
            var condition = new ConditionExpression(logicalName, conditionOperator);

            _filterExpression.Conditions.Add(condition);
            return this;
        }

        private FilterContext<T> HandleInternal<TProperty>(Expression<Func<TProperty>> propertySelector, object value, ConditionOperator conditionOperator)
        {
            var propertyInfo = ExpressionExtensionMethods.GetPropertyInfo<T, TProperty>(propertySelector);
            var logicalName = propertyInfo.GetLogicalName();

            if (propertyInfo.IsEnumProperty())
                value = Convert.ToInt32(value);

            var condition = new ConditionExpression(logicalName, conditionOperator, value);
            _filterExpression.Conditions.Add(condition);
            return this;
        }
    }
}
