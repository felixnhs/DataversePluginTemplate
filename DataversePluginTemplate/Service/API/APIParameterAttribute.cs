using System;

namespace DataversePluginTemplate.Service.API
{
    /// <summary>
    /// Used for properties of <see cref="BaseInputModel{TInput}"/> which allows mapping
    /// values of the plugins InputParameters onto the properties of the instance.
    /// </summary>
    public class APIParameterAttribute : Attribute
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
