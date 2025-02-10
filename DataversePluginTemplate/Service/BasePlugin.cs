using DataversePluginTemplate.Service.API;
using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.ServiceModel;

namespace DataversePluginTemplate.Service
{
    public abstract class BasePlugin : IPlugin
    {
        protected const string TARGET = "Target";
        
        protected string UnsecureConfiguration { get; }
        protected string SecureConfiguration { get; }

        public BasePlugin() { }
        public BasePlugin(string unsecureConfiguration) : this()
        {
            UnsecureConfiguration = unsecureConfiguration;
        }
        public BasePlugin(string unsecureConfiguration, string secureConfiguration)
            : this(unsecureConfiguration)
        {
            SecureConfiguration = secureConfiguration;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var pluginContext = new PluginContext(serviceProvider);

            try
            {
                OnExecute(pluginContext);

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
                    default:
                        OnCustomMessage(pluginContext);
                        break;
                }
            }
            catch (APIException apiException)
            {
#if DEBUG
                pluginContext.TracingService.DebugLog($"An {nameof(APIException)} was thrown. {apiException.StackTrace}");
#endif
                pluginContext.TracingService.Trace($"{(int)apiException.StatusCode}: {apiException.Message}");
                throw new InvalidPluginExecutionException(apiException.Message, apiException.StatusCode);
            }
            catch (FaultException<OrganizationServiceFault> orgServiceFault)
            {
#if DEBUG
                pluginContext.TracingService.DebugLog($"A {nameof(FaultException)} was thrown. {orgServiceFault.StackTrace}");
#endif
                string message = $"[ERROR]: {orgServiceFault}";
                throw new InvalidPluginExecutionException(message, orgServiceFault);
            }
            catch (Exception ex)
            {
#if DEBUG
                pluginContext.TracingService.DebugLog($"A {ex.GetType().Name} was thrown. {ex.StackTrace}");
#endif
                string message = $"[ERROR]: {ex.Message}";
                throw new InvalidPluginExecutionException(message, ex);
            }
        }

        protected virtual void OnExecute(PluginContext context) { }
        protected virtual void OnCreate(PluginContext context, Entity entity) { }
        protected virtual void OnUpdate(PluginContext context, Entity entity) { }
        protected virtual void OnDelete(PluginContext context, EntityReference entityReference) { }
        protected virtual void OnAssociate(PluginContext context, EntityReference entityReference) { }
        protected virtual void OnDisassociate(PluginContext context, EntityReference entityReference) { }
        protected virtual void OnCustomMessage(PluginContext context) { }


        private void HandleExecute<T>(PluginContext pluginContext, Action<PluginContext, T> executeCallback)
        {
            if (pluginContext.ExecutionContext.InputParameters.Contains(TARGET)
                && pluginContext.ExecutionContext.InputParameters[TARGET] is T target)
            {
                executeCallback(pluginContext, target);
            }
        }
    }

    public abstract class BasePlugin<T> : BasePlugin, IPlugin
    {
        public BasePlugin() : base() { }
        public BasePlugin(string unsecureConfiguration) : base(unsecureConfiguration) { }
        public BasePlugin(string unsecureConfiguration, string secureConfiguration)
            : base(unsecureConfiguration, secureConfiguration) { }

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
            catch (APIException apiException)
            {
#if DEBUG
                pluginContext.TracingService.DebugLog($"An {nameof(APIException)} was thrown. {apiException.StackTrace}");
#endif
                pluginContext.TracingService.Trace($"{(int)apiException.StatusCode}: {apiException.Message}");
                throw new InvalidPluginExecutionException(apiException.Message, apiException.StatusCode);
            }
            catch (FaultException<OrganizationServiceFault> orgServiceFault)
            {
#if DEBUG
                pluginContext.TracingService.DebugLog($"A {nameof(FaultException)} was thrown. {orgServiceFault.StackTrace}");
#endif
                string message = $"[ERROR]: {orgServiceFault}";
                throw new InvalidPluginExecutionException(message, orgServiceFault);
            }
            catch (Exception ex)
            {
#if DEBUG
                pluginContext.TracingService.DebugLog($"A {ex.GetType().Name} was thrown. {ex.StackTrace}");
#endif
                string message = $"[ERROR]: {ex.Message}";
                throw new InvalidPluginExecutionException(message, ex);
            }
        }

        protected virtual void OnExecute(PluginContext context, T target) { }

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
