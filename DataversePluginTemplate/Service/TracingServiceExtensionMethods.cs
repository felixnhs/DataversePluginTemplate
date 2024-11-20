using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Linq;

namespace DataversePluginTemplate.Service
{
    internal static class TracingServiceExtensionMethods
    {
        public static void DebugLog(this ITracingService tracingService, string message)
        {
#if DEBUG
            tracingService.Trace($"[DEBUG]: {message}");
#endif
        }

        public static void DebugLog(this ITracingService tracingService, string message, object value)
        {
#if DEBUG
            tracingService.Trace($"[DEBUG]: {message}: {value}");
#endif
        }

        public static void DebugLog(this ITracingService tracingService, Exception ex)
        {
#if DEBUG
            tracingService.Trace("[EXCEPTION]: {0} {1} {2}", ex.Message, ex.StackTrace, ex.InnerException?.Message ?? string.Empty);
#endif
        }

        public static void DebugLogSeparator(this ITracingService tracingService, string title = "")
        {
#if DEBUG
            tracingService.DebugLog(string.IsNullOrWhiteSpace(title)
                ? new string('=', 40)
                : $"{new string('=', 15)} {title.ToUpper()} {new string('=', 15)}");
#endif
        }

        public static void DebugLogSection(this ITracingService tracingService)
        {
#if DEBUG
            tracingService.DebugLog("\n\n");
#endif
        }

        public static void DebugLogTrace(this ITracingService tracingService)
        {
#if DEBUG
            var stackTrace = new StackTrace(1);
            tracingService.Trace($"[TRACE]: {stackTrace}");
#endif
        }

        public static void DebugLogEntity(this ITracingService tracingService, Entity entity)
        {
#if DEBUG
            tracingService.DebugLog($"EntityName: '{entity.LogicalName}'; Id: '{entity.Id}'");

            foreach (var attribute in entity.Attributes)
            {
                tracingService.DebugLog($"|Key: '{attribute.Key}'; Type: '{attribute.Value?.GetType().Name}'; Value: '{GetValueString(attribute.Value)}'");
            }

            tracingService.DebugLogSeparator();
#endif
        }

        public static string GetValueString(object value)
        {
            string valueString = value?.ToString();

            if (value is OptionSetValue osv)
                valueString = osv.Value.ToString();

            else if (value is Money money)
                valueString = money.Value.ToString();

            else if (value is EntityReference er)
                valueString = $"{er.LogicalName}({er.Id})";

            else if (value is Entity e)
                valueString = $"{e.LogicalName}({e.Id}){Environment.NewLine}{e.Attributes.Aggregate(string.Empty, (acc, cur) => acc += $"|{cur.Key}: {GetValueString(cur.Value)}\n")}";

            else if (value is AliasedValue alias)
                valueString = $"{alias.AttributeLogicalName}({alias.Value})";

            return valueString;
        }
    }
}
