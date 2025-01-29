using DataversePluginTemplate.Service;
using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DataversePluginTemplate.Examples
{
    public class IncludeExamplePlugin : BasePlugin, IPlugin
    {
        protected override void OnExecute(PluginContext context)
        {
            /* 
             * Die neue Include-Funktion ermöglicht es direkt verknüpfte Datensätze (Lookups) and die Ergebnisse zu mappen.
             * Derr Passagier hat eine Flug-Eigenschaft, die über die Include-Funktion gefüllt wird.
             * 
             * Die Eigenschaft benötigt das [Includable] und das [LogicalName] Attribut, wobei der Logicalname des Lookups verwendet werden muss.
             */
            var passagiere = context.OrgService.Select<Passagier>()
                .AllDefinedColumns()
                .Include<Flug>(passagier => passagier.Flug, configrueFlug =>
                {
                    configrueFlug.ThenInclude<Flugzeug>(f => f.Flugzeug, configureFlugzeug =>
                    {
                        configureFlugzeug.AllDefinedColumns();
                    });
                })
                .Execute();

            foreach (var passagier in passagiere)
            {
                context.TracingService.Trace(passagier.Flug.Flugzeug.Name);
            }


            IEnumerable<Flug> alleFleuge = context.OrgService.Select<Flug>()
                .Include(flug => flug.Flugzeug, configure =>
                {
                    configure.Columns(flugzeug => new object[] { flugzeug.Name });
                })
                .Execute();

            foreach (var flug in alleFleuge)
                context.TracingService.DebugLog(flug.Flugzeug.Name);
        }
    }
}
