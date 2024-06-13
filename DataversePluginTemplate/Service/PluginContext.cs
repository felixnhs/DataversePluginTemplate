using DataversePluginTemplate.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using System;

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
    }

}
