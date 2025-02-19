using System;

namespace DataversePluginTemplate.Service.API
{
    /// <summary>
    /// Use this to specify the dataverse Custom API message. Use on <see cref="BaseInputModel{TInput}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequestAttribute : Attribute
    {
        public string Name { get; }

        public RequestAttribute(string name)
        {
            Name = name;
        }
    }
}
