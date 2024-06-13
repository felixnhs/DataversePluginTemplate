using DataversePluginTemplate.Model;
using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;

namespace DataversePluginTemplate
{
    /// <summary>
    /// Abstrakte Basisklasse für Plugins, die grundlegende Funktionen und 
    /// Konfigurationsmöglichkeiten bereitstellt.
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        // Eine konstante Zeichenfolge, die als Schlüssel für das Zielobjekt in den Eingabeparametern verwendet wird.
        private const string TARGET = "Target";

        // Der Name des Plugins. Dieser wird von abgeleiteten Klassen verwendet.
        protected readonly string _PluginName;

        // Unsichere Konfiguration, die von Plugins genutzt werden kann.
        protected string UnsecureConfiguration { get; }

        // Sichere Konfiguration, die von Plugins genutzt werden kann.
        protected string SecureConfiguration { get; }

        /// <summary>
        /// Standardkonstruktor, der den Plugin-Namen auf den Standardwert "BasePlugin" setzt.
        /// </summary>
        public BasePlugin() : this(nameof(BasePlugin)) { }

        /// <summary>
        /// Konstruktor, der den Plugin-Namen auf den angegebenen Wert setzt.
        /// </summary>
        /// <param name="pluginName">Der Name des Plugins.</param>
        public BasePlugin(string pluginName)
        {
            _PluginName = pluginName;
        }

        /// <summary>
        /// Konstruktor, der den Plugin-Namen und die unsichere Konfiguration setzt.
        /// </summary>
        /// <param name="pluginName">Der Name des Plugins.</param>
        /// <param name="unsecureConfiguration">Die unsichere Konfiguration des Plugins.</param>
        public BasePlugin(string pluginName, string unsecureConfiguration) : this(pluginName)
        {
            UnsecureConfiguration = unsecureConfiguration;
        }

        /// <summary>
        /// Konstruktor, der den Plugin-Namen, die unsichere und die sichere Konfiguration setzt.
        /// </summary>
        /// <param name="pluginName">Der Name des Plugins.</param>
        /// <param name="unsecureConfiguration">Die unsichere Konfiguration des Plugins.</param>
        /// <param name="secureConfiguration">Die sichere Konfiguration des Plugins.</param>
        public BasePlugin(string pluginName, string unsecureConfiguration, string secureConfiguration)
            : this(pluginName, unsecureConfiguration)
        {
            SecureConfiguration = secureConfiguration;
        }

        /// <summary>
        /// Führt die Hauptlogik des Plugins basierend auf dem aktuellen Kontext und den übergebenen Parametern aus.
        /// </summary>
        /// <param name="serviceProvider">Der Dienstanbieter, der für den aktuellen Kontext bereitgestellt wird.</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            var pluginContext = new PluginContext(serviceProvider);

            try
            {
                switch (pluginContext.ExecutionContext.MessageName)
                {
                    case PluginMessages.CREATE:
                        HandleExecute<Entity>(pluginContext, OnCreate);
                        break;
                    case PluginMessages.UPDATE:
                        HandleExecute<Entity>(pluginContext, OnUpdate);
                        break;
                    case PluginMessages.DELETE:
                        HandleExecute<EntityReference>(pluginContext, OnDelete);
                        break;
                    case PluginMessages.ASSOCIATE:
                        HandleExecute<EntityReference>(pluginContext, OnAssociate);
                        break;
                    case PluginMessages.DISASSOCIATE:
                        HandleExecute<EntityReference>(pluginContext, OnDisassociate);
                        break;
                }
            }
            catch (FaultException<OrganizationServiceFault> orgServiceFault)
            {
                string message = $"[{_PluginName}] ERROR: {orgServiceFault}";
                throw new InvalidPluginExecutionException(message, orgServiceFault);
            }
        }

        /// <summary>
        /// Virtuelle Methode, die bei der Erstellung eines Entität ausgeführt wird.
        /// Kann in abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext.</param>
        /// <param name="entity">Die zu erstellende Entität.</param>
        protected virtual void OnCreate(PluginContext context, Entity entity) { }

        /// <summary>
        /// Virtuelle Methode, die bei der Aktualisierung einer Entität ausgeführt wird.
        /// Kann in abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext.</param>
        /// <param name="entity">Die zu aktualisierende Entität.</param>
        protected virtual void OnUpdate(PluginContext context, Entity entity) { }

        /// <summary>
        /// Virtuelle Methode, die bei der Löschung einer Entität ausgeführt wird.
        /// Kann in abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext.</param>
        /// <param name="entityReference">Die Referenz auf die zu löschende Entität.</param>
        protected virtual void OnDelete(PluginContext context, EntityReference entityReference) { }

        /// <summary>
        /// Virtuelle Methode, die bei der Zuordnung einer Entität ausgeführt wird.
        /// Kann in abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext.</param>
        /// <param name="entityReference">Die Referenz auf die zuzuordnende Entität.</param>
        protected virtual void OnAssociate(PluginContext context, EntityReference entityReference) { }

        /// <summary>
        /// Virtuelle Methode, die bei der Aufhebung der Zuordnung einer Entität ausgeführt wird.
        /// Kann in abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext.</param>
        /// <param name="entityReference">Die Referenz auf die zu entfernende Zuordnung.</param>
        protected virtual void OnDisassociate(PluginContext context, EntityReference entityReference) { }

        /// <summary>
        /// Protokolliert eine Nachricht mithilfe des angegebenen Tracing-Dienstes.
        /// </summary>
        /// <param name="tracingService">Der Tracing-Dienst zum Protokollieren.</param>
        /// <param name="message">Die zu protokollierende Nachricht.</param>
        protected void Log(ITracingService tracingService, string message)
        {
            tracingService.Trace($"[{_PluginName}] {message}");
        }

        /// <summary>
        /// Protokolliert eine Ausnahme mithilfe des angegebenen Tracing-Dienstes.
        /// </summary>
        /// <param name="tracingService">Der Tracing-Dienst zum Protokollieren.</param>
        /// <param name="ex">Die zu protokollierende Ausnahme.</param>
        protected void Log(ITracingService tracingService, Exception ex)
        {
            Log(tracingService, ex.Message, ex.StackTrace);
        }

        /// <summary>
        /// Protokolliert eine formatierte Nachricht mithilfe des angegebenen Tracing-Dienstes.
        /// </summary>
        /// <param name="tracingService">Der Tracing-Dienst zum Protokollieren.</param>
        /// <param name="message">Die zu protokollierende Nachricht.</param>
        /// <param name="args">Die Argumente für die Nachricht.</param>
        protected void Log(ITracingService tracingService, string message, params string[] args)
        {
            tracingService.Trace($"[{_PluginName}] {message}" + "{0}", args); // TODO: Debuggen
        }

        /// <summary>
        /// Protokolliert eine Nachricht mithilfe des Tracing-Dienstes des angegebenen Plugin-Kontextes.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext.</param>
        /// <param name="message">Die zu protokollierende Nachricht.</param>
        protected void Log(PluginContext context, string message)
        {
            Log(context.TracingService, message);
        }

        /// <summary>
        /// Protokolliert eine Ausnahme mithilfe des Tracing-Dienstes des angegebenen Plugin-Kontextes.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext.</param>
        /// <param name="ex">Die zu protokollierende Ausnahme.</param>
        protected void Log(PluginContext context, Exception ex)
        {
            Log(context.TracingService, ex);
        }

        /// <summary>
        /// Protokolliert eine formatierte Nachricht mithilfe des Tracing-Dienstes des angegebenen Plugin-Kontextes.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext.</param>
        /// <param name="message">Die zu protokollierende Nachricht.</param>
        /// <param name="args">Die Argumente für die Nachricht.</param>
        protected void Log(PluginContext context, string message, params string[] args)
        {
            Log(context.TracingService, message, args);
        }

        /// <summary>
        /// Führt die angegebene Aktion für das Zielobjekt im Plugin-Kontext aus.
        /// </summary>
        /// <typeparam name="T">Der Typ des Zielobjekts.</typeparam>
        /// <param name="pluginContext">Der Plugin-Kontext.</param>
        /// <param name="executeCallback">Die Aktion, die ausgeführt werden soll.</param>
        private void HandleExecute<T>(PluginContext pluginContext, Action<PluginContext, T> executeCallback)
        {
            if (pluginContext.ExecutionContext.InputParameters.Contains(TARGET)
                && pluginContext.ExecutionContext.InputParameters[TARGET] is T target)
            {
                executeCallback(pluginContext, target);
            }
        }
    }

    /// <summary>
    /// Abstrakte generische Basisklasse für Plugins, die zusätzliche Typsicherheit für das Zielobjekt bietet.
    /// </summary>
    /// <typeparam name="T">Der Typ des Zielobjekts, mit dem das Plugin arbeitet.</typeparam>
    public abstract class BasePlugin<T> : BasePlugin, IPlugin
    {
        // Eine konstante Zeichenfolge, die als Schlüssel für das Zielobjekt in den Eingabeparametern verwendet wird.
        private const string TARGET = "Target";

        /// <summary>
        /// Standardkonstruktor, der den Plugin-Namen auf den Standardwert "BasePlugin<T>" setzt.
        /// </summary>
        public BasePlugin() : this(nameof(BasePlugin<T>)) { }

        /// <summary>
        /// Konstruktor, der den Plugin-Namen auf den angegebenen Wert setzt.
        /// </summary>
        /// <param name="pluginName">Der Name des Plugins.</param>
        public BasePlugin(string pluginName) : base(pluginName) { }

        /// <summary>
        /// Konstruktor, der den Plugin-Namen und die unsichere Konfiguration setzt.
        /// </summary>
        /// <param name="pluginName">Der Name des Plugins.</param>
        /// <param name="unsecureConfiguration">Die unsichere Konfiguration des Plugins.</param>
        public BasePlugin(string pluginName, string unsecureConfiguration) : base(pluginName, unsecureConfiguration) { }

        /// <summary>
        /// Konstruktor, der den Plugin-Namen, die unsichere und die sichere Konfiguration setzt.
        /// </summary>
        /// <param name="pluginName">Der Name des Plugins.</param>
        /// <param name="unsecureConfiguration">Die unsichere Konfiguration des Plugins.</param>
        /// <param name="secureConfiguration">Die sichere Konfiguration des Plugins.</param>
        public BasePlugin(string pluginName, string unsecureConfiguration, string secureConfiguration)
            : base(pluginName, unsecureConfiguration, secureConfiguration) { }

        /// <summary>
        /// Führt die Hauptlogik des Plugins basierend auf dem aktuellen Kontext und den übergebenen Parametern aus.
        /// Diese Methode bietet zusätzliche Typsicherheit für das Zielobjekt.
        /// </summary>
        /// <param name="serviceProvider">Der Dienstanbieter, der für den aktuellen Kontext bereitgestellt wird.</param>
        public new void Execute(IServiceProvider serviceProvider)
        {
            var pluginContext = new PluginContext(serviceProvider);

            try
            {
                HandleExecute<T>(pluginContext, OnExecute);

                switch (pluginContext.ExecutionContext.MessageName)
                {
                    case PluginMessages.CREATE:
                        HandleExecute<Entity>(pluginContext, OnCreate);
                        break;
                    case PluginMessages.UPDATE:
                        HandleExecute<Entity>(pluginContext, OnUpdate);
                        break;
                    case PluginMessages.DELETE:
                        HandleExecute<EntityReference>(pluginContext, OnDelete);
                        break;
                    case PluginMessages.ASSOCIATE:
                        HandleExecute<EntityReference>(pluginContext, OnAssociate);
                        break;
                    case PluginMessages.DISASSOCIATE:
                        HandleExecute<EntityReference>(pluginContext, OnDisassociate);
                        break;
                }
            }
            catch (FaultException<OrganizationServiceFault> orgServiceFault)
            {
                string message = $"[{_PluginName}] ERROR: {orgServiceFault}";
                throw new InvalidPluginExecutionException(message, orgServiceFault);
            }
        }

        /// <summary>
        /// Virtuelle Methode, die beim Ausführen des Plugins aufgerufen wird.
        /// Kann in abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext.</param>
        /// <param name="target">Das Zielobjekt, mit dem das Plugin arbeitet.</param>
        protected virtual void OnExecute(PluginContext context, T target) { }

        /// <summary>
        /// Führt die angegebene Aktion für das Zielobjekt im Plugin-Kontext aus.
        /// Diese Methode bietet zusätzliche Typsicherheit für das Zielobjekt.
        /// </summary>
        /// <typeparam name="T">Der Typ des Zielobjekts.</typeparam>
        /// <param name="pluginContext">Der Plugin-Kontext.</param>
        /// <param name="executeCallback">Die Aktion, die ausgeführt werden soll.</param>
        private void HandleExecute<T>(PluginContext pluginContext, Action<PluginContext, T> executeCallback)
        {
            if (pluginContext.ExecutionContext.InputParameters.Contains(TARGET)
                && pluginContext.ExecutionContext.InputParameters[TARGET] is T target)
            {
                executeCallback(pluginContext, target);
            }
        }
    }

}
