using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate.Examples
{
    /// <summary>
    /// Beispiel-Plugin für benutzerdefinierte Logik in Dynamics 365.
    /// </summary>
    public class CustomGenericPlugin : BasePlugin<EntityReference>, IPlugin
    {
        /// <summary>
        /// Standardkonstruktor für das Plugin.
        /// </summary>
        public CustomGenericPlugin() : base(nameof(CustomGenericPlugin)) { }

        /// <summary>
        /// Konstruktor für das Plugin mit ungesicherter Konfiguration.
        /// </summary>
        /// <param name="unsecureConfiguration">Ungesicherte Konfigurationsdaten.</param>
        public CustomGenericPlugin(string unsecureConfiguration) : base(nameof(CustomGenericPlugin), unsecureConfiguration) { }

        /// <summary>
        /// Konstruktor für das Plugin mit ungesicherter und gesicherter Konfiguration.
        /// </summary>
        /// <param name="unsecureConfiguration">Ungesicherte Konfigurationsdaten.</param>
        /// <param name="secureConfiguration">Gesicherte Konfigurationsdaten.</param>
        public CustomGenericPlugin(string unsecureConfiguration, string secureConfiguration) : base(nameof(CustomGenericPlugin), unsecureConfiguration, secureConfiguration) { }

        /// <summary>
        /// Überschriebene Methode, die beim Ausführen des Plugins aufgerufen wird.
        /// Kann in abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext, der den Ausführungskontext und Dienste enthält.</param>
        /// <param name="target">Das Zielobjekt, mit dem das Plugin arbeitet.</param>
        protected override void OnExecute(PluginContext context, EntityReference target)
        {
            // Der Typ von target basiert auf dem generischen Parameter der Klasse

            // TODO: Implementiere hier deine Unternehmenslogik
        }
    }
}
