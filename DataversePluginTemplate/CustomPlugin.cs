using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate
{
    public class CustomPlugin : BasePlugin, IPlugin
    {
        public CustomPlugin() : base(nameof(CustomPlugin)) { }
        public CustomPlugin(string unsecureConfiguration) : base(nameof(CustomPlugin), unsecureConfiguration) { }
        public CustomPlugin(string unsecureConfiguration, string secureConfiguration) : base(nameof(CustomPlugin), unsecureConfiguration, secureConfiguration) { }

        protected override void OnCreate(PluginContext context, Entity entity)
        {
            // TODO: Implementiere hier deine Unternehmenslogik
        }
    }
}
