using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DataversePluginTemplate.Service.API
{
    /// <summary>
    /// Die Basisklasse für einen API-Input. Die Basisklasse stellt Funktionen bereit, um den Input aus dem <see cref="PluginContext"/>
    /// zu verarbeiten.
    /// Verwendung:
    /// <code>
    /// var valid = BaseInputModel<CHILD_KLASSE>.TryParse(pluginKontext, out var inputModel);
    /// </code>
    /// Kann auch direkt mit der Child-Klasse verwendet werden.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public abstract class BaseInputModel<TInput>
        where TInput : BaseInputModel<TInput>, new()
    {
        private const string EMAIL_REGEX = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
        private const string SQL_SANITIZE_REGEX = @"\b(SELECT|INSERT|UPDATE|DELETE|DROP|--|OR|AND)\b";
        private const string HTML_SANITIZE_REGEX = "<.*?>";

        protected BaseInputModel() { }

        public static bool TryParse(PluginContext context, out TInput inputModel)
        {
            inputModel = null;

            if (context.ExecutionContext.InputParameters == null)
                return false;

            // null Parameter entfernen
            for (int i = context.ExecutionContext.InputParameters.Count - 1; i >= 0; i--)
            {
                var element = context.ExecutionContext.InputParameters.ElementAt(i);
                if (element.Value == null || element.Value == default)
                    context.ExecutionContext.InputParameters.Remove(element.Key);
            }

            var inputParameters = context.ExecutionContext.InputParameters;

            if (!inputParameters.Any())
                return false;

            var inputType = typeof(TInput);
            var properties = inputType.GetProperties();

            inputModel = new TInput();

            foreach (var property in properties)
            {
                var apiInputAttribute = property.GetCustomAttribute<APIParameterAttribute>();
                if (apiInputAttribute == null)
                    continue;

                bool isRequired = false;
                var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute != null)
                    isRequired = true;

                if (!inputParameters.TryGetValue(apiInputAttribute.Name, out var inputValue) && isRequired)
                    throw new APIException(PluginHttpStatusCode.BadGateway, $"{apiInputAttribute.Name} is required.");

                if (inputValue == null)
                    continue;

                if (!HandlePropertyInputValue(context, property, ref inputModel, inputValue, apiInputAttribute.InputType, isRequired))
                    throw new APIException(PluginHttpStatusCode.BadRequest, $"Invalid input '{apiInputAttribute.Name}'.");
            }

            return true;
        }

        private static bool HandlePropertyInputValue(PluginContext context, PropertyInfo property, ref TInput inputModel, object inputValue, ParameterType inputType, bool isRequired)
        {
            var inputValueType = inputValue.GetType();

            switch (inputType)
            {
                case ParameterType.String:
                    return HandleStringInput(context, property, inputModel, inputValue, isRequired);

                case ParameterType.Guid:
                    return HandleGuidInput(context, property, inputModel, inputValue, isRequired);

                case ParameterType.Integer:
                    return HandleIntegerInput(context, property, inputModel, inputValue);

                case ParameterType.Decimal:
                    return HandleDecimalInput(context, property, inputModel, inputValue);

                case ParameterType.Boolean:
                    return HandleBooleanInput(context, property, inputModel, inputValue);

                case ParameterType.Email:
                    return HandleEmailInput(context, property, inputModel, inputValue, isRequired);

                case ParameterType.StringArray:
                    return HandleStringArrayInput(context, property, inputModel, inputValue, isRequired);

                case ParameterType.Enum:
                    return HandleEnumInput(context, property, inputModel, inputValue, isRequired);

                case ParameterType.EnumArray:
                    return HandleEnumArrayInput(context, property, inputModel, inputValue, isRequired);

                case ParameterType.DateTime:
                    return HandleDateTimeInput(context, property, inputModel, inputValue, isRequired);

                case ParameterType.Entity:
                    return HandleEntityInput(context, property, inputModel, inputValue);

                case ParameterType.EntityReference:
                    return HandleEntityReferenceInput(context, property, inputModel, inputValue);

                case ParameterType.EntityCollection:
                    return HandleEntityCollectionInput(context, property, inputModel, inputValue);

                case ParameterType.Choice:
                    return HandleChoiceInput(context, property, inputModel, inputValue);

                default:
                    throw new APIException(PluginHttpStatusCode.InternalServerError);
            }
        }

        private static bool HandleStringInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue, bool isRequired)
        {
            var stringInput = GetInputValue<string>(inputValue);
            var sanitizedInput = Sanitize(stringInput);

            if (isRequired && string.IsNullOrWhiteSpace(sanitizedInput))
                return false;

            property.SetValue(inputModel, sanitizedInput);
            return true;
        }

        private static bool HandleGuidInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue, bool isRequired)
        {
            var guidInput = GetInputValue<Guid?>(inputValue);
            if (isRequired && !guidInput.IsValid())
                return false;

            property.SetValue(inputModel, guidInput.Value);
            return true;
        }

        private static bool HandleIntegerInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue)
        {
            SetInputValue<int>(inputModel, property, inputValue);
            return true;
        }

        private static bool HandleDecimalInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue)
        {
            SetInputValue<decimal>(inputModel, property, inputValue);
            return true;
        }

        private static bool HandleBooleanInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue)
        {
            SetInputValue<bool>(inputModel, property, inputValue);
            return true;
        }

        private static bool HandleEmailInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue, bool isRequired)
        {
            var emailInput = GetInputValue<string>(inputValue);

            var sanitizedInput = Sanitize(emailInput);
            if (isRequired && string.IsNullOrWhiteSpace(emailInput))
                return false;

            if (!Regex.IsMatch(sanitizedInput, EMAIL_REGEX))
                throw new APIException(PluginHttpStatusCode.BadRequest, $"Invalid input '{inputValue}'");

            property.SetValue(inputModel, sanitizedInput);
            return true;
        }

        private static bool HandleStringArrayInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue, bool isRequired)
        {
            var inputArray = GetInputValue<string[]>(inputValue);
            var sanitizedArray = inputArray?.Select(x => Sanitize(x)).ToArray();

            if (isRequired && (sanitizedArray == null || sanitizedArray.Length == 0))
                return false;

            property.SetValue(inputModel, sanitizedArray);
            return true;
        }

        private static bool HandleEnumInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue, bool isRequired)
        {
            Type enumType = property.PropertyType;

            if (!property.PropertyType.IsEnum)
            {
                if (!(property.PropertyType.IsNullable() && Nullable.GetUnderlyingType(enumType).IsEnum))
                {
                    throw new APIException(PluginHttpStatusCode.InternalServerError, "Property type is invalid. Please contact the admin.");
                }

                enumType = Nullable.GetUnderlyingType(enumType);
            }

            var enumValue = GetInputValue<OptionSetValue>(inputValue);

            if (isRequired && (enumValue == null || !Enum.IsDefined(enumType, enumValue.Value)))
                return false;

            property.SetValue(inputModel, Enum.ToObject(enumType, enumValue.Value));
            return true;
        }

        private static bool HandleEnumArrayInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue, bool isRequired)
        {
            if (!property.PropertyType.IsArray)
                throw new APIException(PluginHttpStatusCode.InternalServerError, "Property type is invalid. Please contact the admin.");

            var elementType = property.PropertyType.GetElementType();

            if (!elementType.IsEnum)
                throw new APIException(PluginHttpStatusCode.InternalServerError, "Property type is invalid. Please contact the admin.");

            var enumArray = GetInputValue<string[]>(inputValue)
                .Select(x => Sanitize(x))
                .ToArray();

            if (isRequired && (enumArray == null || enumArray.Length == 0))
                return false;

            if (enumArray.Any(x => !(int.TryParse(x, out int n) && Enum.IsDefined(elementType, n))))
                throw new APIException(PluginHttpStatusCode.BadRequest, $"Invalid input '{string.Join(",", enumArray)}'");

            var enumTypeArray = Array.CreateInstance(elementType, enumArray.Length);
            for (int i = 0; i < enumArray.Length; i++)
                enumTypeArray.SetValue(Enum.ToObject(elementType, Convert.ToInt32(enumArray[i])), i);

            property.SetValue(inputModel, enumTypeArray);
            return true;
        }

        private static bool HandleDateTimeInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue, bool isRequired)
        {
            var inputDate = GetInputValue<DateTime?>(inputValue);
            if (!inputDate.IsValid())
            {
                if (isRequired)
                    return false;

                // Wenn nicht required, dann default Wert durch null austauschen
                property.SetValue(inputModel, null);
            }
            else
                property.SetValue(inputModel, inputDate.Value);

            return true;
        }

        private static bool HandleEntityInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue)
        {
            SetInputValue<Entity>(inputModel, property, inputValue);
            return true;
        }

        private static bool HandleEntityReferenceInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue)
        {
            SetInputValue<EntityReference>(inputModel, property, inputValue);
            return true;
        }

        private static bool HandleEntityCollectionInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue)
        {
            SetInputValue<EntityCollection>(inputModel, property, inputValue);
            return true;
        }

        private static bool HandleChoiceInput(PluginContext context, PropertyInfo property, TInput inputModel, object inputValue)
        {
            SetInputValue<OptionSetValue>(inputModel, property, inputValue);
            return true;
        }

        private static void SetInputValue<TValue>(TInput inputModel, PropertyInfo property, object inputValue)
        {
            property.SetValue(inputModel, GetInputValue<TValue>(inputValue));
        }

        private static TValue GetInputValue<TValue>(object inputValue)
        {
            if (!(inputValue is TValue))
                throw new APIException(PluginHttpStatusCode.BadRequest, $"Invalid input '{inputValue}'");

            return (TValue)inputValue;
        }

        private static string Sanitize(string unsafeInput)
        {
            string safeSQLInput = Regex.Replace(unsafeInput, SQL_SANITIZE_REGEX, "", RegexOptions.IgnoreCase);
            string safeHTMLInput = Regex.Replace(safeSQLInput, HTML_SANITIZE_REGEX, "");
            return safeHTMLInput;
        }
    }
}
