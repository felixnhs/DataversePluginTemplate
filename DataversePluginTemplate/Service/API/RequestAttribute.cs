using System;

namespace DataversePluginTemplate.Service.API
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class RequestAttribute : Attribute
    {
        public string Name { get; }

        public RequestAttribute(string name)
        {
            Name = name;
        }
    }
}
