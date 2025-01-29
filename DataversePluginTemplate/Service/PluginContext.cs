using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using System;
using System.Text;

namespace DataversePluginTemplate.Service
{
    /// <summary>
    /// Stellt den Kontext eines Plugins bereit, einschließlich der benötigten Dienste und 
    /// des Ausführungsstatus.
    /// </summary>
    public sealed class PluginContext
    {
        // Der Dienstanbieter, der verwendet wird, um die verschiedenen Dienste bereitzustellen.
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Der Ausführungskontext des Plugins, der Informationen über die aktuelle 
        /// Plugin-Ausführung enthält.
        /// </summary>
        internal IPluginExecutionContext ExecutionContext { get; }

        /// <summary>
        /// Der Tracing-Dienst, der zum Protokollieren von Nachrichten verwendet wird.
        /// </summary>
        internal ITracingService TracingService { get; }

        /// <summary>
        /// Der Benachrichtigungsdienst für Dienstendpunkte, der verwendet wird, um Nachrichten 
        /// an externe Dienstendpunkte zu senden.
        /// </summary>
        internal IServiceEndpointNotificationService NotificationService { get; }

        /// <summary>
        /// Der Logger, der verwendet wird, um Nachrichten und Fehler zu protokollieren.
        /// </summary>
        internal ILogger Logger { get; }

        /// <summary>
        /// Der Organisationsdienst, der im Kontext des Plugin-Benutzers ausgeführt wird.
        /// </summary>
        internal IOrganizationService PluginUserService { get; }

        /// <summary>
        /// Ein Alias für <see cref="PluginUserService"/>.
        /// </summary>
        internal IOrganizationService OrgService => PluginUserService;

        /// <summary>
        /// Der Organisationsdienst, der im Kontext des ursprünglichen Benutzers, der die 
        /// Aktion initiiert hat, ausgeführt wird.
        /// </summary>
        internal IOrganizationService InitiatinUserService { get; }

        /// <summary>
        /// Die Stufe des Plugins, die angibt, in welchem Stadium der Pipeline das Plugin 
        /// ausgeführt wird.
        /// </summary>
        internal PluginStage PluginStage { get; }

        /// <summary>
        /// Der Ausführungsmodus des Plugins, der angibt, ob das Plugin synchron oder 
        /// asynchron ausgeführt wird.
        /// </summary>
        internal PluginExecutionMode ExecutionMode { get; }

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="PluginContext"/> Klasse und 
        /// stellt die notwendigen Dienste bereit.
        /// </summary>
        /// <param name="serviceProvider">Der Dienstanbieter, der für den aktuellen Kontext 
        /// bereitgestellt wird.</param>
        /// <exception cref="InvalidPluginExecutionException">Wird ausgelöst, wenn der 
        /// Dienstanbieter null ist.</exception>
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

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="PluginContext"/> Klasse, stellt die 
        /// notwendigen Dienste bereit und ermöglicht eine benutzerdefinierte Konfiguration.
        /// </summary>
        /// <param name="serviceProvider">Der Dienstanbieter, der für den aktuellen Kontext 
        /// bereitgestellt wird.</param>
        /// <param name="configureContext">Eine Aktion, die zur weiteren Konfiguration des 
        /// Plugin-Kontexts verwendet wird.</param>
        internal PluginContext(IServiceProvider serviceProvider, Action<PluginContext> configureContext)
            : this(serviceProvider)
        {
            configureContext?.Invoke(this);
        }

        internal void DebugLog()
        {
#if DEBUG
            TracingService.DebugLogSeparator("Plugin Context");
            TracingService.DebugLog(Environment.NewLine + GetContextLogStr(ExecutionContext));
#endif
        }

        private string GetContextLogStr(IPluginExecutionContext context, int depth = 0)
        {
#if DEBUG
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
#endif
        }
    }

}
