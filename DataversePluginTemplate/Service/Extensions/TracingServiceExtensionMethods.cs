﻿using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DataversePluginTemplate.Service.Extensions
{
    public static class TracingServiceExtensionMethods
    {
#if DEBUG
        public static void DebugLog(this ITracingService tracingService, string message)
        {
            tracingService.Trace($"[DEBUG]: {message}");
        }

        public static void DebugLog(this ITracingService tracingService, string message, object value)
        {
            tracingService.Trace($"[DEBUG]: {message}: {value}");
        }

        public static void DebugLog(this ITracingService tracingService, Exception ex)
        {
            tracingService.Trace("[EXCEPTION]: {0} {1} {2}", ex.Message, ex.StackTrace, ex.InnerException?.Message ?? string.Empty);
        }

        public static void DebugLogSeparator(this ITracingService tracingService, string title = "")
        {
            tracingService.DebugLog(string.IsNullOrWhiteSpace(title)
                ? new string('=', 40)
                : $"{new string('=', 15)} {title.ToUpper()} {new string('=', 15)}");
        }

        public static void DebugLogSection(this ITracingService tracingService)
        {
            tracingService.DebugLog("\n\n");
        }

        public static void DebugLogTrace(this ITracingService tracingService)
        {
            var stackTrace = new StackTrace(1);
            tracingService.Trace($"[TRACE]: {stackTrace}");
        }

        public static void DebugLogEntity(this ITracingService tracingService, Entity entity)
        {
            tracingService.DebugLog($"EntityName: '{entity.LogicalName}'; Id: '{entity.Id}'");

            foreach (var attribute in entity.Attributes)
            {
                tracingService.DebugLog($"|Key: '{attribute.Key}'; Type: '{attribute.Value?.GetType().Name}'; Value: '{GetValueString(attribute.Value)}'");
            }

            tracingService.DebugLogSeparator();
        }
#endif

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
                valueString = $"{alias.AttributeLogicalName}({GetValueString(alias.Value)})";

            else if (value is EntityCollection collection)
            {
                var sb = new StringBuilder();
                foreach (var entity in collection.Entities)
                    sb.AppendLine($"{entity.LogicalName}({entity.Id}){Environment.NewLine}{entity.Attributes.Aggregate(string.Empty, (acc, cur) => acc += $"|{cur.Key}: {GetValueString(cur.Value)}\n")}");

                valueString = sb.ToString();
            }

            return valueString;
        }
    }
}
