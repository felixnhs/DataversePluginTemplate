using DataversePluginTemplate.Service;
using DataversePluginTemplate.Service.API;
using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Reflection;

namespace DataversePluginTemplate.Prebuild
{
    /// <summary>
    /// Base class for plugins, that are used as dataverse Custom APIs.
    /// Inputs from the api are automatically parsed and passed into your plugin.
    /// Refer to <see cref="BaseInputModel{TInput}"/> for a usage example.
    /// </summary>
    /// <typeparam name="TInputModel">The type of input for your api.</typeparam>
    /// <typeparam name="TOutputModel">The the of output of your api.</typeparam>
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
                context.TracingService.Trace($"Unexpected Message [{context.ExecutionContext.MessageName}]");
                throw new APIException(PluginHttpStatusCode.NotFound);
            }

#if DEBUG
            DebugLogInput(context);
#endif

            if (!BaseInputModel<TInputModel>.TryParse(context, out TInputModel inputModel))
            {
#if DEBUG
                context.TracingService.DebugLog($"Error validatin Input");
#endif
                throw new APIException(PluginHttpStatusCode.BadRequest, "Invalid Input");
            }

            var outputModel = new TOutputModel();

            HandleRequest(context, inputModel, outputModel);

            SetOutputParameter(context, outputModel);

#if DEBUG
            DebugLogOutput(context);
#endif
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

#if DEBUG
        protected void DebugLogInput(PluginContext context)
        {
            context.TracingService.DebugLogSeparator("Inputs");
            context.TracingService.DebugLog(context.ExecutionContext.InputParameters.Aggregate("Keys: ", (acc, cur) => acc += $";{cur.Key}"));

            foreach (var inputParameter in context.ExecutionContext.InputParameters)
                context.TracingService.DebugLog($"Key: {inputParameter.Key}; Type: {inputParameter.Value?.GetType().Name}; Value: {TracingServiceExtensionMethods.GetValueString(inputParameter.Value)}");

            context.TracingService.DebugLogSection();
        }

        protected void DebugLogOutput(PluginContext context)
        {
            context.TracingService.DebugLogSeparator("Ouputs");
            context.TracingService.DebugLog(context.ExecutionContext.OutputParameters.Aggregate("Keys: ", (acc, cur) => acc += $";{cur.Key}"));

            foreach (var outputParameter in context.ExecutionContext.OutputParameters)
                context.TracingService.DebugLog($"Key: {outputParameter.Key}; Type: {outputParameter.Value?.GetType().Name}; Value: {TracingServiceExtensionMethods.GetValueString(outputParameter.Value)}");

            context.TracingService.DebugLogSection();
        }
#endif

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
