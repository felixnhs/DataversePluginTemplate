using DataversePluginTemplate.Service.Entities;
using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq.Expressions;

namespace DataversePluginTemplate.Queries
{
    internal sealed class FilterContext
    {
        private readonly FilterExpression _filterExpression;

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
        internal FilterContext IsNull(string columnName) => HandleInternal(columnName, ConditionOperator.Null);
        internal FilterContext IsNotNull(string columnName) => HandleInternal(columnName, ConditionOperator.NotNull);
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

        internal FilterContext Conditions(LogicalOperator logicalOperator, Action<FilterContext> configureFilter)
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

    internal sealed class FilterContext<T>
        where T : BaseEntity<T>
    {
        private readonly FilterExpression _filterExpression;

        internal FilterContext(FilterExpression filterExpression)
        {
            _filterExpression = filterExpression;
        }

        #region - Methoden für Condtions -

        internal FilterContext<T> Equals<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Equal);
        internal FilterContext<T> NotEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotEqual);
        internal FilterContext<T> Greater<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.GreaterThan);
        internal FilterContext<T> LessThan<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LessThan);
        internal FilterContext<T> GreaterEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.GreaterEqual);
        internal FilterContext<T> LessEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LessEqual);
        internal FilterContext<T> Like<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Like);
        internal FilterContext<T> NotLike<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotLike);
        internal FilterContext<T> In<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.In);
        internal FilterContext<T> NotIn<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotIn);
        internal FilterContext<T> Between<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Between);
        internal FilterContext<T> NotBetween<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotBetween);
        internal FilterContext<T> IsNull<TProperty>(Expression<Func<TProperty>> propertySelector) => HandleInternal(propertySelector, ConditionOperator.Null);
        internal FilterContext<T> IsNotNull<TProperty>(Expression<Func<TProperty>> propertySelector) => HandleInternal(propertySelector, ConditionOperator.NotNull);
        internal FilterContext<T> Yesterday<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Yesterday);
        internal FilterContext<T> Today<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Today);
        internal FilterContext<T> Tomorrow<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Tomorrow);
        internal FilterContext<T> Last7Days<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Last7Days);
        internal FilterContext<T> Next7Days<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.Next7Days);
        internal FilterContext<T> LastWeek<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastWeek);
        internal FilterContext<T> ThisWeek<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisWeek);
        internal FilterContext<T> NextWeek<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextWeek);
        internal FilterContext<T> LastMonth<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastMonth);
        internal FilterContext<T> ThisMonth<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisMonth);
        internal FilterContext<T> NextMonth<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextMonth);
        internal FilterContext<T> On<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.On);
        internal FilterContext<T> OnOrBefore<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OnOrBefore);
        internal FilterContext<T> OnOrAfter<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OnOrAfter);
        internal FilterContext<T> LastYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastYear);
        internal FilterContext<T> ThisYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisYear);
        internal FilterContext<T> NextYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextYear);
        internal FilterContext<T> LastXHours<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXHours);
        internal FilterContext<T> NextXHours<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXHours);
        internal FilterContext<T> LastXDays<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXDays);
        internal FilterContext<T> NextXDays<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXDays);
        internal FilterContext<T> LastXWeeks<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXWeeks);
        internal FilterContext<T> NextXWeeks<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXWeeks);
        internal FilterContext<T> LastXMonths<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXMonths);
        internal FilterContext<T> NextXMonths<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXMonths);
        internal FilterContext<T> LastXYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.LastXYears);
        internal FilterContext<T> NextXYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXYears);
        internal FilterContext<T> EqualUserId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserId);
        internal FilterContext<T> NotEqualUserId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotEqualUserId);
        internal FilterContext<T> EqualBusinessId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualBusinessId);
        internal FilterContext<T> NotEqualBusinessId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotEqualBusinessId);
        internal FilterContext<T> ChildOf<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.ChildOf);
        internal FilterContext<T> Mask<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Mask);
        internal FilterContext<T> NotMask<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotMask);
        internal FilterContext<T> MasksSelect<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.MasksSelect);
        internal FilterContext<T> Contains<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Contains);
        internal FilterContext<T> DoesNotContain<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.DoesNotContain);
        internal FilterContext<T> EqualUserLanguage<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserLanguage);
        internal FilterContext<T> NotOn<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotOn);
        internal FilterContext<T> OlderThanXMonths<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXMonths);
        internal FilterContext<T> BeginsWith<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.BeginsWith);
        internal FilterContext<T> DoesNotBeginWith<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.DoesNotBeginWith);
        internal FilterContext<T> EndsWith<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EndsWith);
        internal FilterContext<T> DoesNotEndWith<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.DoesNotEndWith);
        internal FilterContext<T> ThisFiscalYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisFiscalYear);
        internal FilterContext<T> ThisFiscalPeriod<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.ThisFiscalPeriod);
        internal FilterContext<T> NextFiscalYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextFiscalYear);
        internal FilterContext<T> NextFiscalPeriod<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.NextFiscalPeriod);
        internal FilterContext<T> LastFiscalYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastFiscalYear);
        internal FilterContext<T> LastFiscalPeriod<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastFiscalPeriod);
        internal FilterContext<T> LastXFiscalYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastXFiscalYears);
        internal FilterContext<T> LastXFiscalPeriods<TProperty>(Expression<Func<TProperty>> propertySelector, object value = null) => HandleInternal(propertySelector, value, ConditionOperator.LastXFiscalPeriods);
        internal FilterContext<T> NextXFiscalYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXFiscalYears);
        internal FilterContext<T> NextXFiscalPeriods<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NextXFiscalPeriods);
        internal FilterContext<T> InFiscalYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InFiscalYear);
        internal FilterContext<T> InFiscalPeriod<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InFiscalPeriod);
        internal FilterContext<T> InFiscalPeriodAndYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InFiscalPeriodAndYear);
        internal FilterContext<T> InOrBeforeFiscalPeriodAndYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InOrBeforeFiscalPeriodAndYear);
        internal FilterContext<T> InOrAfterFiscalPeriodAndYear<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.InOrAfterFiscalPeriodAndYear);
        internal FilterContext<T> EqualUserTeams<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserTeams);
        internal FilterContext<T> EqualUserOrUserTeams<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserOrUserTeams);
        internal FilterContext<T> Under<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Under);
        internal FilterContext<T> NotUnder<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.NotUnder);
        internal FilterContext<T> UnderOrEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.UnderOrEqual);
        internal FilterContext<T> Above<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.Above);
        internal FilterContext<T> AboveOrEqual<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.AboveOrEqual);
        internal FilterContext<T> EqualUserOrUserHierarchy<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserOrUserHierarchy);
        internal FilterContext<T> EqualUserOrUserHierarchyAndTeams<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualUserOrUserHierarchyAndTeams);
        internal FilterContext<T> OlderThanXYears<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXYears);
        internal FilterContext<T> OlderThanXWeeks<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXWeeks);
        internal FilterContext<T> OlderThanXDays<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXDays);
        internal FilterContext<T> OlderThanXHours<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXHours);
        internal FilterContext<T> OlderThanXMinutes<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.OlderThanXMinutes);
        internal FilterContext<T> ContainValues<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.ContainValues);
        internal FilterContext<T> DoesNotContainValues<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.DoesNotContainValues);
        internal FilterContext<T> EqualRoleBusinessId<TProperty>(Expression<Func<TProperty>> propertySelector, object value) => HandleInternal(propertySelector, value, ConditionOperator.EqualRoleBusinessId);

        #endregion

        internal FilterContext<T> Conditions(LogicalOperator logicalOperator, Action<FilterContext<T>> configureFilter)
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
