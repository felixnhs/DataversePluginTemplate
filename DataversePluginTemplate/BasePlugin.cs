using DataversePluginTemplate.Model;
using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;

namespace DataversePluginTemplate
{
    public abstract class BasePlugin : IPlugin
    {
        // Private modifer, damit Unterklassen nicht dran kommen
        private const string TARGET = "Target";

        protected readonly string _PluginName;

        protected string UnsecureConfiguration { get; }
        protected string SecureConfiguration { get; }

        public BasePlugin() : this(nameof(BasePlugin)) { }

        public BasePlugin(string pluginName)
        {
            _PluginName = pluginName;
        }

        public BasePlugin(string pluginName, string unsecureConfiguration) : this(pluginName)
        {
            UnsecureConfiguration = unsecureConfiguration;
        }

        public BasePlugin(string pluginName, string unsecureConfiguration, string secureConfiguration) : this(pluginName, unsecureConfiguration)
        {
            SecureConfiguration = secureConfiguration;
        }


        public void Execute(IServiceProvider serviceProvider)
        {
            var pluginContext = new PluginContext(serviceProvider);

            try
            {
                switch (pluginContext.Context.MessageName)
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
                string message = $"[{_PluginName}] ERROR: {orgServiceFault.ToString()}";
                throw new InvalidPluginExecutionException(message, orgServiceFault);
            }
        }


        protected virtual void OnCreate(PluginContext context, Entity entity) { }
        protected virtual void OnUpdate(PluginContext context, Entity entity) { }
        protected virtual void OnDelete(PluginContext context, EntityReference entityReference) { }
        protected virtual void OnAssociate(PluginContext context, EntityReference entityReference) { }
        protected virtual void OnDisassociate(PluginContext context, EntityReference entityReference) { }


        protected void Log(ITracingService tracingService, string message)
        {
            tracingService.Trace($"[{_PluginName}] {message}");
        }

        protected void Log(ITracingService tracingService, Exception ex)
        {
            Log(tracingService, ex.Message, ex.StackTrace);
        }

        protected void Log(ITracingService tracingService, string message, params string[] args)
        {
            tracingService.Trace($"[{_PluginName}] {message}" + "{0}", args); // TODO: Debuggen
        }

        protected void Log(PluginContext context, string message)
        {
            Log(context.TracingService, message);
        }

        protected void Log(PluginContext context, Exception ex)
        {
            Log(context.TracingService, ex);
        }

        protected void Log(PluginContext context, string message, params string[] args)
        {
            Log(context.TracingService, message, args);
        }


        private void HandleExecute<T>(PluginContext pluginContext, Action<PluginContext, T> executeCallback)
        {
            if (pluginContext.Context.InputParameters.Contains(TARGET) && pluginContext.Context.InputParameters[TARGET] is T target)
                executeCallback(pluginContext, target);
        }
    }

    public abstract class BasePlugin<T> : BasePlugin, IPlugin
    {
        // Duplicate weil private in base
        private const string TARGET = "Target";

        public BasePlugin() : this(nameof(BasePlugin<T>)) { }

        public BasePlugin(string pluginName) : base(pluginName) { }

        public BasePlugin(string pluginName, string unsecureConfiguration) : base(pluginName, unsecureConfiguration) { }

        public BasePlugin(string pluginName, string unsecureConfiguration, string secureConfiguration) : base(pluginName, unsecureConfiguration, secureConfiguration) { }

        public new void Execute(IServiceProvider serviceProvider)
        {
            var pluginContext = new PluginContext(serviceProvider);

            try
            {
                HandleExecute<T>(pluginContext, OnExecute);

                switch (pluginContext.Context.MessageName)
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
                string message = $"[{_PluginName}] ERROR: {orgServiceFault.ToString()}";
                throw new InvalidPluginExecutionException(message, orgServiceFault);
            }
        }

        protected virtual void OnExecute(PluginContext context, T target) { }


        // Duplicate weil private in Base
        private void HandleExecute<T>(PluginContext pluginContext, Action<PluginContext, T> executeCallback)
        {
            if (pluginContext.Context.InputParameters.Contains(TARGET) && pluginContext.Context.InputParameters[TARGET] is T target)
                executeCallback(pluginContext, target);
        }
    }
}
