using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DataversePluginTemplate.Prebuild
{
    public abstract class EntityPreprocessingPlugin : BasePlugin, IPlugin
    {
        public EntityPreprocessingPlugin() : base() { }
        public EntityPreprocessingPlugin(string unsecureConfiguration) : base(unsecureConfiguration) { }
        public EntityPreprocessingPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(unsecureConfiguration, secureConfiguration) { }

        protected abstract IEnumerable<(string attribut, object value)> Process(PluginContext context, Entity entity);

        
        protected sealed override void OnCreate(PluginContext context, Entity entity)
        {
            ProcessInternal(context, entity);
        }

        protected sealed override void OnUpdate(PluginContext context, Entity entity)
        {
            ProcessInternal(context, entity);
        }

        // Seal the methods to prevent overriding them further down
        protected sealed override void OnDelete(PluginContext context, EntityReference entityReference) { }
        protected sealed override void OnAssociate(PluginContext context, EntityReference entityReference) { }
        protected sealed override void OnDisassociate(PluginContext context, EntityReference entityReference) { }

        private void ProcessInternal(PluginContext context, Entity entity)
        {
            if (context.PluginStage != PluginStage.PreOperation)
                return;

            var attributEnumerator = Process(context, entity)
                .GetEnumerator();

            while (attributEnumerator.MoveNext())
            {
                if (!entity.Attributes.Contains(attributEnumerator.Current.attribut))
                    entity.Attributes.Add(attributEnumerator.Current.attribut, attributEnumerator.Current.value);

                else
                    entity.Attributes[attributEnumerator.Current.attribut] = attributEnumerator.Current.value;
            }
        }
    }
}
