using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DataversePluginTemplate
{
    /// <summary>
    /// Beispiel-Plugin für benutzerdefinierte Logik in Dynamics 365.
    /// </summary>
    public class CustomPlugin : BasePlugin, IPlugin
    {
        /// <summary>
        /// Standardkonstruktor für das Plugin.
        /// </summary>
        public CustomPlugin() : base(nameof(CustomPlugin)) { }

        /// <summary>
        /// Konstruktor für das Plugin mit ungesicherter Konfiguration.
        /// </summary>
        /// <param name="unsecureConfiguration">Ungesicherte Konfigurationsdaten.</param>
        public CustomPlugin(string unsecureConfiguration) : base(nameof(CustomPlugin), unsecureConfiguration) { }

        /// <summary>
        /// Konstruktor für das Plugin mit ungesicherter und gesicherter Konfiguration.
        /// </summary>
        /// <param name="unsecureConfiguration">Ungesicherte Konfigurationsdaten.</param>
        /// <param name="secureConfiguration">Gesicherte Konfigurationsdaten.</param>
        public CustomPlugin(string unsecureConfiguration, string secureConfiguration) : base(nameof(CustomPlugin), unsecureConfiguration, secureConfiguration) { }

        /// <summary>
        /// Überschriebene Methode, die beim Erstellen einer Entität aufgerufen wird.
        /// </summary>
        /// <param name="context">Plugin-Kontext, der den Ausführungskontext und Dienste enthält.</param>
        /// <param name="entity">Die erstellte Entität.</param>
        protected override void OnCreate(PluginContext context, Entity entity)
        {

            /*
             * TODO: Ersetzte die Query durch deine eigene Unternehmenslogik
             * 
             * =============================================================
             * 
             * Daten abrufen - How to
             * 
             * Diese Zeile führt eine Abfrage in der CRM-Umgebung aus, um Flüge mit bestimmten Bedingungen zu selektieren und 
             * die Ergebnisse in einer EntityCollection zu speichern. Die Abfrage umfasst die Auswahl bestimmter Spalten wie 
             * "Startzeit", "Landezeit" und "Flugzeug". Es werden Filterbedingungen definiert, die sicherstellen, dass der Flug 
             * als abgeschlossen markiert ist, die Flugzeug-ID nicht null ist und die Flugdauer größer als 5 ist. Zusätzlich 
             * wird eine Verknüpfung (Join) mit der Tabelle "PassagiereImFlug" und dann mit der Tabelle "Kontakt" durchgeführt. 
             * Dabei werden alle Spalten der Kontakt-Tabelle ausgewählt und deren Alias auf "Passagier" gesetzt. Die Abfrage 
             * wird schließlich mit der Methode Execute() ausgeführt und die Ergebnisse werden in der Variable entities gespeichert.
             */

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
