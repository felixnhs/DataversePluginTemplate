using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate.Prebuild
{
    public abstract class ValidationPlugin : BasePlugin, IPlugin
    {
        public ValidationPlugin() : base() { }
        public ValidationPlugin(string unsecureConfiguration) : base(unsecureConfiguration) { }
        public ValidationPlugin(string unsecureConfiguration, string secureConfiguration) : base(unsecureConfiguration, secureConfiguration) { }

        protected abstract void Validate<T>(PluginContext context, T entity);

        protected sealed override void OnCreate(PluginContext context, Entity entity) => HandleValidate(context, entity);
        protected sealed override void OnUpdate(PluginContext context, Entity entity) => HandleValidate(context, entity);
        protected sealed override void OnDelete(PluginContext context, EntityReference entityReference) => HandleValidate(context, entityReference);
        protected sealed override void OnAssociate(PluginContext context, EntityReference entityReference) => HandleValidate(context, entityReference);
        protected sealed override void OnDisassociate(PluginContext context, EntityReference entityReference) => HandleValidate(context, entityReference);

        private void HandleValidate<T>(PluginContext context, T input)
        {
            if (context.PluginStage == PluginStage.PreValidation)
                Validate(context, input);
        }
    }
}
