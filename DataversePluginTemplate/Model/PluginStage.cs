namespace DataversePluginTemplate.Model
{
    /// <summary>
    /// Gibt die Stufen der Plugin-Ausführung an, die die verschiedenen Phasen 
    /// der Ausführungspipeline in einem CRM-System darstellen.
    /// </summary>
    internal enum PluginStage
    {
        /// <summary>
        /// Die Pre-Validation-Stufe. 
        /// In dieser Phase können die Eingaben validiert werden, noch bevor 
        /// die Daten im CRM-System verarbeitet werden.
        /// </summary>
        PreValidation = 10,

        /// <summary>
        /// Die Pre-Operation-Stufe. 
        /// Diese Phase findet nach der Validierung und vor der eigentlichen Operation statt.
        /// Hier können Änderungen an den Eingabedaten vorgenommen werden, bevor sie gespeichert werden.
        /// </summary>
        PreOperation = 20,

        /// <summary>
        /// Die Post-Operation-Stufe. 
        /// Diese Phase findet nach der eigentlichen Operation statt, nachdem die Daten bereits 
        /// im CRM-System verarbeitet und gespeichert wurden.
        /// </summary>
        PostOperation = 40,
    }

}
