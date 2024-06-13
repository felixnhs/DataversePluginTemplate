using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

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

            // Beispiel für ein Select
            EntityCollection entities = context.OrgService.Select("Flüge")
                .Columns("Startzeit", "Landezeit", "Flugzeug")
                .Conditions(LogicalOperator.And, filter =>
                    filter.Equals("Abgeschlossen", true)
                    .IsNotNull("FlugzeugId")
                    .Greater("FlugDauer", 5))
                .Join("PassagiereImFlug", "FlugId", "PassagiereImFlug_FlugId", passagiereImFlug =>
                {
                    passagiereImFlug
                        .Join("Kontakt", "PassagiereImFlug_KontaktId", "KontaktId", kontakt =>
                        {
                            kontakt.AllColumns()
                                .Alias("Passagier");
                        });
                })
                .Execute();
        }
    }
}
