using DataversePluginTemplate.Service;
using DataversePluginTemplate.Service.Extensions;
using DataversePluginTemplate.Service.Notification;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataversePluginTemplate.Examples
{
    /// <summary>
    /// Beispiel-Plugin für benutzerdefinierte Logik in Dynamics 365.
    /// </summary>
    public class CustomPlugin : BasePlugin, IPlugin
    {
        /// <summary>
        /// Standardkonstruktor für das Plugin.
        /// </summary>
        public CustomPlugin() : base() { }

        /// <summary>
        /// Konstruktor für das Plugin mit ungesicherter Konfiguration.
        /// </summary>
        /// <param name="unsecureConfiguration">Ungesicherte Konfigurationsdaten.</param>
        public CustomPlugin(string unsecureConfiguration) : base(unsecureConfiguration) { }

        /// <summary>
        /// Konstruktor für das Plugin mit ungesicherter und gesicherter Konfiguration.
        /// </summary>
        /// <param name="unsecureConfiguration">Ungesicherte Konfigurationsdaten.</param>
        /// <param name="secureConfiguration">Gesicherte Konfigurationsdaten.</param>
        public CustomPlugin(string unsecureConfiguration, string secureConfiguration) : base(unsecureConfiguration, secureConfiguration) { }

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


            /*
             * Es können auch alle einträge einer Tabelle abgefragt werden...
             */
            EntityCollection passagierCollection = context.OrgService.Select("Passagier")
                .AllColumns()
                .Execute();

            /*
             * ... und mit der erstellten Klasse 'Passagier' ist Type-Safty gegeben.
             * 
             * Die Klasse erbt von BaseEntity...
             */
            var passagiere = passagierCollection.As<Passagier>();


            /*
             * ... wodurch die Abfragen auch sicherer gestaltet werden können.
             * 
             * Dabei ist wichtig, dass alle Eigenschaften und die Klasse selbst ein 
             * LogicalName-Attribut haben, die den Logischen Namen des Attributs, bzw.
             * der Entität, angeben.
             */
            IEnumerable<Passagier> passagierListe = context.OrgService.Select<Passagier>()
                .Columns(passagier => new object[] { passagier.Nachname, passagier.Alter })
                .Top(1)
                .Conditions(LogicalOperator.And, (filter, passagier) =>
                    filter.Equals(() => passagier.Nachname, "Müller")
                    .IsNotNull(() => passagier.Alter))
                .Join<PassagiereImFlug>(passagier => passagier.Id, pif => pif.PassagierId, JoinOperator.Inner, passagiereImFlug =>
                {
                    passagiereImFlug
                        .Columns(pif => new object[] { pif.FlugId })
                        .Alias("Lieblings_Passagiere");
                })
                .Execute();

            foreach (var passagier in passagierListe)
            {
                passagier.Alter += 1;
                context.TracingService.DebugLog($"Happy Birthday, {passagier.Vorname}!");
            }

            Notification.Create(context, new Guid("User Id here"))
                .AddTitle("Test Nachricht")
                .AddMessage("Lorem ipsum")
                .AddIcon(NotificationIcon.Success)
                .SetNotificationType(NotificationType.Hidden)
                .Send();
        }
    }
}
