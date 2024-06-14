using DataversePluginTemplate.Prebuild;
using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DataversePluginTemplate.Examples
{
    /// <summary>
    /// Eine spezifische Implementierung des <see cref="RenamePlugin"/>, die den Namen einer Entität auf einen benutzerdefinierten Namen festlegt.
    /// </summary>
    public class CustomRenamePlugin : RenamePlugin, IPlugin
    {
        // Statische Konstante, die den Namen der Spalte definiert, die den Namen der Entität enthält.
        private const string NAME_COLUMN = "primaryName";

        public CustomRenamePlugin() : base(nameof(CustomRenamePlugin), NAME_COLUMN) { }
        public CustomRenamePlugin(string unsecureConfiguration) : base(nameof(CustomRenamePlugin), NAME_COLUMN, unsecureConfiguration) { }
        public CustomRenamePlugin(string unsecureConfiguration, string secureConfiguration) : base(nameof(CustomRenamePlugin), NAME_COLUMN, unsecureConfiguration, secureConfiguration) { }

        /// <summary>
        /// Implementiert die Logik zur Erstellung eines neuen Namens für eine Entität.
        /// Diese Methode gibt eine Auflistung von Zeichenfolgen zurück, die den neuen Namen zusammensetzen.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext, der Informationen zur aktuellen Ausführung enthält.</param>
        /// <param name="entity">Die Entität, deren Name geändert wird.</param>
        /// <returns>Eine Auflistung von Zeichenfolgen, die den neuen Namen der Entität darstellen.</returns>
        internal override IEnumerable<string> BuildName(PluginContext context, Entity entity)
        {
            // Füge "Meine " an den Anfang des Namens hinzu.
            yield return "Meine ";

            // Füge den aktuellen Namen der Entität hinzu.
            yield return entity.GetAttributeValue<string>(NAME_COLUMN);

            // Füge " 1" an das Ende des Namens hinzu.
            yield return " 1";
        }
    }
}
