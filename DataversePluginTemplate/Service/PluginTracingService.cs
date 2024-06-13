using Microsoft.Xrm.Sdk;
using System;
using System.Text;

namespace DataversePluginTemplate.Service
{
    /// <summary>
    /// Stellt einen Tracing-Dienst bereit, der Protokollnachrichten aufzeichnet und 
    /// optionale Zeitdeltas zwischen den Aufzeichnungen anzeigen kann.
    /// </summary>
    internal sealed class PluginTracingService : ITracingService
    {
        // Der zugrunde liegende Tracing-Dienst, der für die eigentliche Protokollierung verwendet wird.
        private readonly ITracingService _tracingService;

        // Die Zeit des letzten Protokolleintrags, um Zeitdeltas zu berechnen.
        private DateTime _previousTraceTime;

        // Gibt an, ob Zeitdeltas zwischen den Protokolleinträgen angezeigt werden sollen.
        private bool _showDelta = true;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="PluginTracingService"/> Klasse mit dem angegebenen Dienstanbieter.
        /// </summary>
        /// <param name="serviceProvider">Der Dienstanbieter, der die erforderlichen Dienste bereitstellt.</param>
        /// <param name="showDelta">Gibt an, ob Zeitdeltas zwischen den Protokolleinträgen angezeigt werden sollen.</param>
        /// <exception cref="InvalidPluginExecutionException">Wird ausgelöst, wenn der Dienstanbieter keinen gültigen 
        /// Tracing-Dienst bereitstellen kann.</exception>
        internal PluginTracingService(IServiceProvider serviceProvider, bool showDelta)
        {
            _showDelta = showDelta;

            var utcNow = DateTime.UtcNow;
            var context = serviceProvider.GetService<IExecutionContext>();

            // Bestimmt die Anfangszeit des Protokollierens, um sicherzustellen, dass sie nicht in der Zukunft liegt.
            DateTime initialTime = context.OperationCreatedOn;
            if (initialTime > utcNow)
                initialTime = utcNow;

            _tracingService = serviceProvider.GetService<ITracingService>();
            _previousTraceTime = initialTime;
        }

        /// <summary>
        /// Protokolliert eine Nachricht mit optionalen Argumenten.
        /// </summary>
        /// <param name="format">Das Nachrichtenformat.</param>
        /// <param name="args">Optionale Argumente zur Formatierung der Nachricht.</param>
        /// <exception cref="InvalidPluginExecutionException">Wird ausgelöst, wenn die Formatierung der Nachricht fehlschlägt.</exception>
        public void Trace(string format, params object[] args)
        {
            var utcNow = DateTime.UtcNow;

            // Berechnet das Zeitdelta seit dem letzten Protokolleintrag.
            var deltaMilliseconds = utcNow.Subtract(_previousTraceTime).TotalMilliseconds;

            try
            {
                StringBuilder sb = new StringBuilder();
                if (_showDelta)
                    sb.Append($"[+{deltaMilliseconds}] ");

                // Fügt die formatierte Nachricht hinzu.
                if (args == null || args.Length == 0)
                    sb.Append(format);
                else
                    sb.Append(string.Format(format, args));

                // Sendet die Nachricht an den zugrunde liegenden Tracing-Dienst.
                _tracingService.Trace(sb.ToString());
            }
            catch (FormatException ex)
            {
                throw new InvalidPluginExecutionException($"Failed to write trace message due to error {ex.Message}", ex);
            }

            // Aktualisiert die Zeit des letzten Protokolleintrags.
            _previousTraceTime = utcNow;
        }
    }

}
