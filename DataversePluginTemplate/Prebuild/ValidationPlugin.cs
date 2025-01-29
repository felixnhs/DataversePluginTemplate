using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate.Prebuild
{
    public abstract class ValidationPlugin : BasePlugin, IPlugin
    {
        public ValidationPlugin() : base() { }
        public ValidationPlugin(string unsecureConfiguration) : base(unsecureConfiguration) { }
        public ValidationPlugin(string unsecureConfiguration, string secureConfiguration) : base(unsecureConfiguration, secureConfiguration) { }

        /// <summary>
        /// Validiert die Erstellung der Entität. Diese Methode dient dazu, sicherzustellen, dass die Entität
        /// den festgelegten Regeln und Anforderungen entspricht, bevor sie erstellt wird.
        /// Die konkrete Validierungslogik wird von den abgeleiteten Klassen implementiert.
        /// </summary>
        /// <param name="context">Der <see cref="PluginContext"/>, der den aktuellen Ausführungszustand und zusätzliche Informationen über den Plugin-Vorgang bereitstellt.</param>
        /// <param name="entity">Die zu validierende <see cref="Entity"/>, die erstellt werden soll. Diese Entität enthält die Daten, die geprüft werden müssen.</param>
        /// <exception cref="InvalidPluginExecutionException">
        /// Wird ausgelöst, wenn die Validierung der Entität fehlschlägt.
        /// Dies signalisiert, dass der Erstellungsvorgang nicht fortgesetzt werden kann und abgebrochen werden soll.
        /// </exception>
        /// <example>
        /// <code>
        /// // Beispiel für die Implementierung in einer abgeleiteten Klasse:
        /// protected override void Validate(PluginContext context, Entity entity)
        /// {
        ///     if (string.IsNullOrWhiteSpace(entity.Name))
        ///     {
        ///         throw new InvalidPluginExecutionException("Der Name der Entität darf nicht leer sein.");
        ///     }
        /// }
        /// </code>
        /// </example>
        protected abstract void Validate(PluginContext context, Entity entity);


        /// <summary>
        /// Überschreibt und versiegelt die <c>OnCreate</c>-Methode, um sicherzustellen, dass sie von
        /// abgeleiteten Klassen nicht weiter überschrieben werden kann.
        /// Diese Methode wird während der Erstellung einer Entität aufgerufen und sorgt dafür,
        /// dass die <see cref="Validate"/>-Methode aufgerufen wird, um die Entität vor der Erstellung zu validieren.
        /// </summary>
        /// <param name="context">Der <see cref="PluginContext"/>, der Informationen über den aktuellen Plugin-Ausführungszustand bereitstellt, wie z.B. die Phase der Ausführung.</param>
        /// <param name="entity">Die <see cref="Entity"/>, die erstellt wird. Diese Entität wird zur Validierung an die <see cref="Validate"/>-Methode übergeben.</param>
        /// <remarks>
        /// Die Methode stellt sicher, dass die Validierung der Entität nur in der Pre-Validation-Phase erfolgt,
        /// um sicherzustellen, dass alle notwendigen Bedingungen erfüllt sind, bevor die Entität tatsächlich erstellt wird.
        /// </remarks>
        protected sealed override void OnCreate(PluginContext context, Entity entity)
        {
            if (context.PluginStage == PluginStage.PreValidation)
                Validate(context, entity);
        }


        // Andere Methoden Schließen, damit diese nicht überschrieben werden können
        protected sealed override void OnUpdate(PluginContext context, Entity entity) { }
        protected sealed override void OnDelete(PluginContext context, EntityReference entityReference) { }
        protected sealed override void OnAssociate(PluginContext context, EntityReference entityReference) { }
        protected sealed override void OnDisassociate(PluginContext context, EntityReference entityReference) { }
    }
}
