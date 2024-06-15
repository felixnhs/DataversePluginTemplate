using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DataversePluginTemplate.Prebuild
{
    /// <summary>
    /// Abstrakte Klasse, die die Grundfunktionen für ein Plugin zur Vorverarbeitung von Entitäten bereitstellt.
    /// Mithilfe der bereitgestellten <see cref="Process(PluginContext, Entity)"/>-Funktion, können Attribute der
    /// Entität vor dem Speichern angepasst werden.
    /// Diese Klasse erweitert <see cref="BasePlugin"/> und implementiert <see cref="IPlugin"/>,
    /// um Plugin-spezifische Operationen zu definieren.
    /// </summary>
    public abstract class EntityPreprocessingPlugin : BasePlugin, IPlugin
    {
        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="EntityPreprocessingPlugin"/> Klasse mit einem Plugin-Namen.
        /// </summary>
        /// <param name="pluginName">Der Name des Plugins.</param>
        public EntityPreprocessingPlugin(string pluginName) : base(pluginName) { }

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="EntityPreprocessingPlugin"/> Klasse mit einem Plugin-Namen und einer ungesicherten Konfiguration.
        /// </summary>
        /// <param name="pluginName">Der Name des Plugins.</param>
        /// <param name="unsecureConfiguration">Die ungesicherte Konfiguration.</param>
        public EntityPreprocessingPlugin(string pluginName, string unsecureConfiguration) : base(pluginName, unsecureConfiguration) { }

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="EntityPreprocessingPlugin"/> Klasse mit einem Plugin-Namen, einer ungesicherten und einer gesicherten Konfiguration.
        /// </summary>
        /// <param name="pluginName">Der Name des Plugins.</param>
        /// <param name="unsecureConfiguration">Die ungesicherte Konfiguration.</param>
        /// <param name="secureConfiguration">Die gesicherte Konfiguration.</param>
        public EntityPreprocessingPlugin(string pluginName, string unsecureConfiguration, string secureConfiguration)
            : base(pluginName, unsecureConfiguration, secureConfiguration) { }

        /// <summary>
        /// Abstrakte Methode, die von abgeleiteten Klassen implementiert werden muss, um die Attribute der Entität zu verarbeiten.
        /// Diese Methode gibt eine Aufzählung von Attributen und ihren entsprechenden Werten zurück, die für die Entität festgelegt werden sollen.
        /// </summary>
        /// <param name="context">Der <see cref="PluginContext"/>, der den aktuellen Ausführungszustand und zusätzliche Informationen über den Plugin-Vorgang bereitstellt.</param>
        /// <param name="entity">Die <see cref="Entity"/>, die verarbeitet werden soll.</param>
        /// <returns>Eine Aufzählung von Tupeln, die die Attributnamen und ihre zu setzenden Werte enthalten.</returns>
        public abstract IEnumerable<(string attribut, object value)> Process(PluginContext context, Entity entity);

        /// <summary>
        /// Überschreibt die OnCreate-Methode, um die Attribute der Entität bei der Erstellung festzulegen.
        /// Diese Methode ist final und kann in abgeleiteten Klassen nicht überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext, der Ausführungsinformationen enthält.</param>
        /// <param name="entity">Die Entität, die erstellt wird.</param>
        protected sealed override void OnCreate(PluginContext context, Entity entity)
        {
            ProcessInternal(context, entity);
        }

        /// <summary>
        /// Überschreibt die OnUpdate-Methode, um die Attribute der Entität bei der Aktualisierung festzulegen.
        /// Diese Methode ist final und kann in abgeleiteten Klassen nicht überschrieben werden.
        /// </summary>
        /// <param name="context">Der Plugin-Kontext, der Ausführungsinformationen enthält.</param>
        /// <param name="entity">Die Entität, die aktualisiert wird.</param>
        protected sealed override void OnUpdate(PluginContext context, Entity entity)
        {
            ProcessInternal(context, entity);
        }

        // Diese Methoden werden sealed, damit sie nicht weiter überschrieben werden können.
        protected sealed override void OnDelete(PluginContext context, EntityReference entityReference) { }
        protected sealed override void OnAssociate(PluginContext context, EntityReference entityReference) { }
        protected sealed override void OnDisassociate(PluginContext context, EntityReference entityReference) { }

        /// <summary>
        /// Interne Methode zur Verarbeitung der Entität. Diese Methode wird von <see cref="OnCreate"/> und <see cref="OnUpdate"/> aufgerufen,
        /// um die Attribute der Entität festzulegen. Die Methode überprüft, ob der aktuelle Plugin-Kontext in der Pre-Operation-Phase ist,
        /// und setzt die Attribute der Entität entsprechend der von <see cref="Process"/> zurückgegebenen Werte.
        /// </summary>
        /// <param name="context">Der <see cref="PluginContext"/>, der Informationen über den aktuellen Ausführungszustand des Plugins enthält.</param>
        /// <param name="entity">Die <see cref="Entity"/>, die verarbeitet wird.</param>
        private void ProcessInternal(PluginContext context, Entity entity)
        {
            if (context.PluginStage != Model.PluginStage.PreOperation)
                return;

            // Ruft die Process-Methode auf und erhält eine Aufzählung von Attributsname-Wert-Paaren.
            var attributEnumerator = Process(context, entity)
                .GetEnumerator();

            while(attributEnumerator.MoveNext())
            {
                if (!entity.Attributes.Contains(attributEnumerator.Current.attribut))
                    entity.Attributes.Add(attributEnumerator.Current.attribut, attributEnumerator.Current.value);

                else
                    entity.Attributes[attributEnumerator.Current.attribut] = attributEnumerator.Current.value;
            }
        }
    }
}
