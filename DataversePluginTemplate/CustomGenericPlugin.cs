using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate
{
    /// <summary>
    /// Beispiel für ein Plugin mit generischem Targettype
    /// </summary>
    public class CustomGenericPlugin : BasePlugin<EntityReference>, IPlugin
    {
        public CustomGenericPlugin() : base(nameof(CustomGenericPlugin)) { }
        public CustomGenericPlugin(string unsecureConfiguration) : base(nameof(CustomGenericPlugin), unsecureConfiguration) { }
        public CustomGenericPlugin(string unsecureConfiguration, string secureConfiguration) : base(nameof(CustomGenericPlugin), unsecureConfiguration, secureConfiguration) { }

        protected override void OnExecute(PluginContext context, EntityReference target)
        {
            // Der Typ von target basiert auf dem generischen Parameter der Klasse

            // TODO: Implementiere hier deine Unternehmenslogik
        }
    }
}
