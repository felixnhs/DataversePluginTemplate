using System;

namespace DataversePluginTemplate.Service
{


    internal class APIParameterAttribute : Attribute
    {
        public string Name { get; private set; }
        public ParameterType InputType { get; private set; }

        public APIParameterAttribute(string name, ParameterType inputType) : base()
        {
            Name = name;
            InputType = inputType;
        }
    }
}
