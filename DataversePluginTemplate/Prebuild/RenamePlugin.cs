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
    public abstract class RenamePlugin : EntityPreprocessingPlugin, IPlugin
    {
        // Feld zur Speicherung des Namens der Spalte, die den Namen enthält.
        private readonly string _nameColumn;

        /// <summary>
        /// Konstruktor, der den Namen der Spalte setzt.
        /// </summary>
        /// <param name="nameColumn">Name der Spalte, die den Namen enthält.</param>
        public RenamePlugin(string nameColumn) : base()
        {
            _nameColumn = nameColumn;
        }

        /// <summary>
        /// Konstruktor, der den Namen der Spalte und die ungesicherte Konfiguration setzt.
        /// </summary>
        /// <param name="nameColumn">Name der Spalte, die den Namen enthält.</param>
        /// <param name="unsecureConfiguration">Ungesicherte Konfiguration für das Plugin.</param>
        public RenamePlugin(string nameColumn, string unsecureConfiguration) : base(unsecureConfiguration)
        {
            _nameColumn = nameColumn;
        }

        /// <summary>
        /// Konstruktor, der den Namen der Spalte, die ungesicherte und gesicherte Konfiguration setzt.
        /// </summary>
        /// <param name="nameColumn">Name der Spalte, die den Namen enthält.</param>
        /// <param name="unsecureConfiguration">Ungesicherte Konfiguration für das Plugin.</param>
        /// <param name="secureConfiguration">Gesicherte Konfiguration für das Plugin.</param>
        public RenamePlugin(string nameColumn, string unsecureConfiguration, string secureConfiguration) : base(unsecureConfiguration, secureConfiguration)
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
        /// Diese Methode überschreibt die abstrakte <see cref="Process"/>-Methode, um spezifische Attribute der Entität zu verarbeiten.
        /// Der Hauptzweck dieser Methode besteht darin, die Teile eines Namens zusammenzusetzen und als Attribut-Wert-Paar zurückzugeben.
        /// </summary>
        /// <param name="context">Der <see cref="PluginContext"/>, der Informationen über den aktuellen Ausführungszustand des Plugins enthält.</param>
        /// <param name="entity">Die <see cref="Entity"/>, die verarbeitet wird. Diese Entität enthält die Daten, die für die Namensbildung verwendet werden.</param>
        /// <returns>
        /// Eine Aufzählung von Tupeln, die den Attributnamen und den zusammengesetzten Namen als Wert enthalten.
        /// In diesem Fall wird nur ein Attribut-Wert-Paar zurückgegeben, das den neuen Namen der Entität repräsentiert.
        /// </returns>
        public sealed override IEnumerable<(string attribut, object value)> Process(PluginContext context, Entity entity)
        {
            // Abrufen der Teile des neuen Namens durch Aufruf der abstrakten Methode BuildName.
            var nameParts = BuildName(context, entity)
                .GetEnumerator();

            StringBuilder stringBuilder = new StringBuilder();

            // Durchlaufen der Teile und Zusammenfügen in den StringBuilder.
            while (nameParts.MoveNext())
                stringBuilder.Append(nameParts.Current);

            yield return (_nameColumn, stringBuilder.ToString());
        }
    }
}
