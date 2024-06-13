namespace DataversePluginTemplate.Model
{
    /// <summary>
    /// Stellt eine Sammlung von Nachrichten dar, die in der Plugin-Ausführung verwendet werden können.
    /// Diese Nachrichten entsprechen den wichtigsten Operationen, die in einem CRM-System ausgeführt werden.
    /// </summary>
    internal static class PluginMessages
    {
        /// <summary>
        /// Nachricht für die Erstellungsoperation.
        /// Wird verwendet, wenn ein neues Datensatzobjekt erstellt wird.
        /// </summary>
        internal const string CREATE = "Create";

        /// <summary>
        /// Nachricht für die Aktualisierungsoperation.
        /// Wird verwendet, wenn ein vorhandener Datensatz aktualisiert wird.
        /// </summary>
        internal const string UPDATE = "Update";

        /// <summary>
        /// Nachricht für die Löschoperation.
        /// Wird verwendet, wenn ein vorhandener Datensatz gelöscht wird.
        /// </summary>
        internal const string DELETE = "Delete";

        /// <summary>
        /// Nachricht für die Assoziationsoperation.
        /// Wird verwendet, wenn zwei Datensätze miteinander verknüpft werden.
        /// </summary>
        internal const string ASSOCIATE = "Associate";

        /// <summary>
        /// Nachricht für die Disassoziationsoperation.
        /// Wird verwendet, wenn die Verknüpfung zwischen zwei Datensätzen entfernt wird.
        /// </summary>
        internal const string DISASSOCIATE = "Disassociate";
    }

}
