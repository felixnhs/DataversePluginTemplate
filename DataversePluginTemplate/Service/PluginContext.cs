using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using System;
using System.Text;

namespace DataversePluginTemplate.Service
{
    /// <summary>
    /// Provides access to plugin execution information and essential dataverse services.
    /// </summary>
    public sealed class PluginContext
    {
        private readonly IServiceProvider _serviceProvider;

        internal IPluginExecutionContext ExecutionContext { get; }
        internal ITracingService TracingService { get; }
        internal IServiceEndpointNotificationService NotificationService { get; }
        internal ILogger Logger { get; }
        internal IOrganizationService PluginUserService { get; }
        internal IOrganizationService OrgService => PluginUserService;
        internal IOrganizationService InitiatinUserService { get; }
        internal PluginStage PluginStage { get; }
        internal PluginExecutionMode ExecutionMode { get; }

        internal PluginContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new InvalidPluginExecutionException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;

            ExecutionContext = _serviceProvider.GetService<IPluginExecutionContext>();
            NotificationService = _serviceProvider.GetService<IServiceEndpointNotificationService>();
            TracingService = _serviceProvider.GetService<ITracingService>();
            Logger = _serviceProvider.GetService<ILogger>();
            PluginUserService = _serviceProvider.GetOrganizationService(ExecutionContext.UserId);
            InitiatinUserService = _serviceProvider.GetOrganizationService(ExecutionContext.InitiatingUserId);

            PluginStage = (PluginStage)ExecutionContext.Stage;
            ExecutionMode = (PluginExecutionMode)ExecutionContext.Mode;
        }

        internal PluginContext(IServiceProvider serviceProvider, Action<PluginContext> configureContext)
            : this(serviceProvider)
        {
            configureContext?.Invoke(this);
        }

#if DEBUG
        internal void DebugLog()
        {
            TracingService.DebugLogSeparator("Plugin Context");
            TracingService.DebugLog(Environment.NewLine + GetContextLogStr(ExecutionContext));
        }

        private string GetContextLogStr(IPluginExecutionContext context, int depth = 0)
        {
            string indent = new string('-', depth);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{indent}CorrelationId: {context.CorrelationId}");
            sb.AppendLine($"{indent}Depth: {context.Depth}");
            sb.AppendLine($"{indent}InitiatingUserId: {context.InitiatingUserId}");
            sb.AppendLine($"{indent}MessageName: {context.MessageName}");
            sb.AppendLine($"{indent}Mode: {context.Mode}");
            sb.AppendLine($"{indent}OperationCreatedOn: {context.OperationCreatedOn}");
            sb.AppendLine($"{indent}OperationId: {context.OperationId}");
            sb.AppendLine($"{indent}PrimaryEntityId: {context.PrimaryEntityId}");
            sb.AppendLine($"{indent}PrimaryEntityName: {context.PrimaryEntityName}");
            sb.AppendLine($"{indent}RequestId: {context.RequestId}");
            sb.AppendLine($"{indent}Stage: {context.Stage}");
            sb.AppendLine($"{indent}UserId: {context.UserId}");

            sb.AppendLine($"{indent}InputParameters: {context.InputParameters.Count}");
            foreach (var p in context.InputParameters)
                sb.AppendLine($"{indent}-{p.Key}: {TracingServiceExtensionMethods.GetValueString(p.Value)}");

            sb.AppendLine($"{indent}SharedVariables: {context.SharedVariables.Count}");
            foreach (var v in context.SharedVariables)
                sb.AppendLine($"{indent}-{v.Key}: {TracingServiceExtensionMethods.GetValueString(v.Value)}");

            if (context.ParentContext != null)
            {
                sb.AppendLine($"{indent}ParentContext:");
                sb.AppendLine(GetContextLogStr(context.ParentContext, depth + 1));
            }

            return sb.ToString();
        }
#endif
    }

}
