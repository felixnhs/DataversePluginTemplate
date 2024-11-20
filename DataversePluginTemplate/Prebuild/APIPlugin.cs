using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Reflection;

namespace DataversePluginTemplate.Prebuild
{
    /// <summary>
    /// Basisklasse für Plugins, die als Custom API registriert werden.
    /// Die Basisklasse validiert den Input der Request und gibt diesen an die API-Klasse weiter
    /// </summary>
    /// <typeparam name="TInputModel">Input Datenmodell</typeparam>
    /// <typeparam name="TOutputModel">Output Datenmodell</typeparam>
    public abstract class APIPlugin<TInputModel, TOutputModel> : BasePlugin
        where TInputModel : BaseInputModel<TInputModel>, new()
        where TOutputModel : class, new()
    {
        private string _MessageName;

        public APIPlugin() : base()
        {
            SetMessageName();
        }

        public APIPlugin(string unsecureConfiguration) : base(unsecureConfiguration)
        {
            SetMessageName();
        }

        public APIPlugin(string unsecureConfiguration, string secureConfiguration) : base(unsecureConfiguration, secureConfiguration)
        {
            SetMessageName();
        }

        protected override sealed void OnCustomMessage(PluginContext context)
        {
            if (!ShouldExecute(context))
                return;

            if (context.ExecutionContext.MessageName != _MessageName)
            {
                Log(context, $"Unexpected Message [{context.ExecutionContext.MessageName}]");
                throw new APIException(PluginHttpStatusCode.NotFound);
            }

            DebugLogInput(context);

            if (!BaseInputModel<TInputModel>.TryParse(context, out TInputModel inputModel))
            {
                DebugLog(context, $"Error validatin Input");
                throw new APIException(PluginHttpStatusCode.BadRequest, "Invalid Input");
            }

            var outputModel = new TOutputModel();

            HandleRequest(context, inputModel, outputModel);

            SetOutputParameter(context, outputModel);

            DebugLogOutput(context);
        }

        /// <summary>
        /// Funktion, die von den APIs überschrieben werden muss um die Request zu verarbeiten.
        /// </summary>
        /// <param name="context">Plugin Kontext</param>
        /// <param name="input">Input der Webrequest. Wird vollständig an das Plugin übergeben.</param>
        /// <param name="output">Output der Webrequest. Muss vom Plugin gesetzt werden.</param>
        protected abstract void HandleRequest(PluginContext context, TInputModel input, TOutputModel output);

        protected virtual bool ShouldExecute(PluginContext context) => true;

        protected override sealed void OnAssociate(PluginContext context, EntityReference entityReference) { }
        protected override sealed void OnCreate(PluginContext context, Entity entity) { }
        protected override sealed void OnDelete(PluginContext context, EntityReference entityReference) { }
        protected override sealed void OnDisassociate(PluginContext context, EntityReference entityReference) { }
        protected override sealed void OnUpdate(PluginContext context, Entity entity) { }


        #region Logging

        protected void DebugLogInput(PluginContext context)
        {
#if DEBUG
            context.TracingService.DebugLogSeparator("Inputs");
            DebugLog(context, context.ExecutionContext.InputParameters.Aggregate("Keys: ", (acc, cur) => acc += $";{cur.Key}"));

            foreach (var inputParameter in context.ExecutionContext.InputParameters)
                DebugLog(context, $"Key: {inputParameter.Key}; Type: {inputParameter.Value?.GetType().Name}; Value: {TracingServiceExtensionMethods.GetValueString(inputParameter.Value)}");

            DebugLogSection(context);
#endif
        }

        protected void DebugLogOutput(PluginContext context)
        {
#if DEBUG
            context.TracingService.DebugLogSeparator("Ouputs");
            DebugLog(context, context.ExecutionContext.OutputParameters.Aggregate("Keys: ", (acc, cur) => acc += $";{cur.Key}"));

            foreach (var outputParameter in context.ExecutionContext.OutputParameters)
                DebugLog(context, $"Key: {outputParameter.Key}; Type: {outputParameter.Value?.GetType().Name}; Value: {TracingServiceExtensionMethods.GetValueString(outputParameter.Value)}");

            DebugLogSection(context);
#endif
        }

        #endregion

        private void SetOutputParameter(PluginContext context, TOutputModel outputModel)
        {
            var outputType = typeof(TOutputModel);
            var properties = outputType.GetProperties();

            foreach (var property in properties)
            {
                var apiParameterAttribute = property.GetCustomAttribute<APIParameterAttribute>();
                if (apiParameterAttribute == null)
                    continue;

                context.ExecutionContext.OutputParameters[apiParameterAttribute.Name] = property.GetValue(outputModel);
            }
        }
        private void SetMessageName()
        {
            var requestAttribute = typeof(TInputModel).GetCustomAttribute<RequestAttribute>();
            if (requestAttribute == null)
                throw new Exception("Request Model has no request name attribute configured");

            _MessageName = requestAttribute.Name;
        }
    }
}
