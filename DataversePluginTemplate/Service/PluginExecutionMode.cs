namespace DataversePluginTemplate.Service
{
    /// <summary>
    /// Gibt die Ausführungsmodi eines Plugins an.
    /// </summary>
    internal enum PluginExecutionMode
    {
        /// <summary>
        /// Das Plugin wird synchron ausgeführt. 
        /// Dies bedeutet, dass die Ausführung des Plugins in Echtzeit erfolgt und 
        /// andere Operationen in der Pipeline anhalten, bis die Plugin-Ausführung abgeschlossen ist.
        /// </summary>
        Synchronous = 0,

        /// <summary>
        /// Das Plugin wird asynchron ausgeführt. 
        /// Dies bedeutet, dass das Plugin in einer separaten Warteschlange ausgeführt wird und 
        /// die Hauptpipeline fortgesetzt wird, ohne auf die Ausführung des Plugins zu warten.
        /// </summary>
        Asynchronous = 1,
    }

}
