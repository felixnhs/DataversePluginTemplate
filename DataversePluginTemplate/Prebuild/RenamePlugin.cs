using DataversePluginTemplate.Model;
using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Text;

namespace DataversePluginTemplate.Prebuild
{
    /// <summary>
    /// Abstrakte Klasse, die als Basis für Plugins dient, die Namen von Entitäten ändern.
    /// Erbt von <see cref="BasePlugin"/> und implementiert <see cref="IPlugin"/>.
    /// </summary>
    public abstract class RenamePlugin : BasePlugin, IPlugin
    {
        // Feld zur Speicherung des Namens der Spalte, die den Namen enthält.
        private readonly string _nameColumn;

        /// <summary>
        /// Konstruktor, der den Plugin-Namen und den Namen der Spalte setzt.
        /// </summary>
        /// <param name="pluginName">Name des Plugins.</param>
        /// <param name="nameColumn">Name der Spalte, die den Namen enthält.</param>
        public RenamePlugin(string pluginName, string nameColumn) : base(pluginName)
        {
            _nameColumn = nameColumn;
        }

        /// <summary>
        /// Konstruktor, der den Plugin-Namen, den Namen der Spalte und die ungesicherte Konfiguration setzt.
        /// </summary>
        /// <param name="pluginName">Name des Plugins.</param>
        /// <param name="nameColumn">Name der Spalte, die den Namen enthält.</param>
        /// <param name="unsecureConfiguration">Ungesicherte Konfiguration für das Plugin.</param>
        public RenamePlugin(string pluginName, string nameColumn, string unsecureConfiguration) : base(pluginName, unsecureConfiguration)
        {
            _nameColumn = nameColumn;
        }

        /// <summary>
        /// Konstruktor, der den Plugin-Namen, den Namen der Spalte, die ungesicherte und gesicherte Konfiguration setzt.
        /// </summary>
        /// <param name="pluginName">Name des Plugins.</param>
        /// <param name="nameColumn">Name der Spalte, die den Namen enthält.</param>
        /// <param name="unsecureConfiguration">Ungesicherte Konfiguration für das Plugin.</param>
        /// <param name="secureConfiguration">Gesicherte Konfiguration für das Plugin.</param>
        public RenamePlugin(string pluginName, string nameColumn, string unsecureConfiguration, string secureConfiguration) : base(pluginName, unsecureConfiguration, secureConfiguration)
        {
            _nameColumn = nameColumn;
        }

        /// <summary>
        /// Abstrakte Methode, die implementiert werden muss, um den neuen Namen für eine Entität zu erstellen.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext, der Ausführungsinformationen enthält.</param>
        /// <param name="entity">Die Entität, deren Name geändert werden soll.</param>
        /// <returns>Eine Auflistung von Zeichenfolgen, die den neuen Namen der Entität darstellen.</returns>
        internal abstract IEnumerable<string> BuildName(PluginContext context, Entity entity);

        /// <summary>
        /// Überschreibt die OnCreate-Methode, um den Namen der Entität bei der Erstellung festzulegen.
        /// Diese Methode ist final und kann in abgeleiteten Klassen nicht überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext, der Ausführungsinformationen enthält.</param>
        /// <param name="entity">Die Entität, die erstellt wird.</param>
        protected sealed override void OnCreate(PluginContext context, Entity entity)
        {
            SetName(context, entity);
        }

        /// <summary>
        /// Überschreibt die OnUpdate-Methode, um den Namen der Entität bei der Aktualisierung festzulegen.
        /// Diese Methode ist final und kann in abgeleiteten Klassen nicht überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext, der Ausführungsinformationen enthält.</param>
        /// <param name="entity">Die Entität, die aktualisiert wird.</param>
        protected sealed override void OnUpdate(PluginContext context, Entity entity)
        {
            SetName(context, entity);
        }

        // Diese Methoden werden sealed, damit sie nicht weiter überschrieben werden können.
        protected sealed override void OnDelete(PluginContext context, EntityReference entityReference) { }
        protected sealed override void OnAssociate(PluginContext context, EntityReference entityReference) { }
        protected sealed override void OnDisassociate(PluginContext context, EntityReference entityReference) { }

        /// <summary>
        /// Hilfsmethode zur Festlegung des Namens einer Entität.
        /// Der Name wird nur festgelegt, wenn sich das Plugin in der Pre-Operation-Phase befindet.
        /// Der neue Name wird basierend auf den von <see cref="BuildName"/> gelieferten Teilen zusammengesetzt.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext, der Ausführungsinformationen enthält.</param>
        /// <param name="entity">Die Entität, deren Name festgelegt werden soll.</param>
        private void SetName(PluginContext context, Entity entity)
        {
            // Überprüfen, ob die aktuelle Phase die Pre-Operation ist.
            // Falls nicht, wird die Ausführung abgebrochen.
            if (context.ExecutionContext.Stage != (int)PluginStage.PreOperation)
                return;

            // Abrufen der Teile des neuen Namens durch Aufruf der abstrakten Methode BuildName.
            var nameParts = BuildName(context, entity)
                .GetEnumerator();

            StringBuilder stringBuilder = new StringBuilder();

            // Durchlaufen der Teile und Zusammenfügen in den StringBuilder.
            while (nameParts.MoveNext())
                stringBuilder.Append(nameParts.Current);

            // Den zusammengestellten Namen der Entität hinzufügen
            if (entity.Attributes.Contains(_nameColumn))
                entity.Attributes[_nameColumn] = stringBuilder.ToString();

            else
                entity.Attributes.Add(_nameColumn, stringBuilder.ToString());
        }
    }
}
