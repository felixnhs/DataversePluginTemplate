using System;

namespace DataversePluginTemplate.Service
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
