using Microsoft.Xrm.Sdk;
using System;
using System.Text;

namespace DataversePluginTemplate.Service
{
    internal class PluginTracingService : ITracingService
    {
        private readonly ITracingService _tracingService;
        private DateTime _previousTraceTime;
        private bool _showDelta = true;

        internal PluginTracingService(IServiceProvider serviceProvider, bool showDelta) 
        {
            _showDelta = showDelta;

            var utcNow = DateTime.UtcNow;
            var context = serviceProvider.GetService<IExecutionContext>();

            DateTime initialTime = context.OperationCreatedOn;
            if (initialTime > utcNow)
                initialTime = utcNow;

            _tracingService = serviceProvider.GetService<ITracingService>();
            _previousTraceTime = initialTime;
        }

        public void Trace(string format, params object[] args)
        {
            var utcNow = DateTime.UtcNow;

            var deltaMilliseconds = utcNow.Subtract(_previousTraceTime).TotalMilliseconds;

            try
            {
                StringBuilder sb = new StringBuilder();
                if (_showDelta)
                    sb.Append($"[+{deltaMilliseconds}] ");



                if (args == null || args.Length == 0)
                    sb.Append(format);
                else
                    sb.Append(string.Format(format, args));

                _tracingService.Trace(sb.ToString());
            }
            catch (FormatException ex)
            {
                throw new InvalidPluginExecutionException($"Failed to write trace message due to error {ex.Message}", ex);
            }

            _previousTraceTime = utcNow;
        }
    }
}
