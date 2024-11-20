using Microsoft.Xrm.Sdk;
using System;

namespace DataversePluginTemplate.Service
{
    internal class APIException : Exception
    {
        public PluginHttpStatusCode StatusCode { get; set; } = PluginHttpStatusCode.InternalServerError;

        public APIException(string message) : base(message) { }

        public APIException(PluginHttpStatusCode statusCode) : base()
        {
            StatusCode = statusCode;   
        }

        public APIException(PluginHttpStatusCode statusCode, string message) : base(message) { }
        public APIException(PluginHttpStatusCode statusCode, string message, Exception innerException) : base(message, innerException) { }
    }
}
