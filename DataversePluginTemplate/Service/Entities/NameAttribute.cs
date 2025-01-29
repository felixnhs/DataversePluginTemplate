using System;

namespace DataversePluginTemplate.Service.Entities
{
    internal class NameAttribute : Attribute
    {
        public string Name { get; private set; }
        public NameAttribute(string name) : base() 
        {
            Name = name;
        }
    }
}
