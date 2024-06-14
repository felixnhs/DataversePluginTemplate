using System;

namespace DataversePluginTemplate.Service
{
    /// <summary>
    /// Stellt ein benutzerdefiniertes Attribut dar, das den logischen Namen einer Eigenschaft oder Klasse angibt.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class LogicalNameAttribute : Attribute
    {
        /// <summary>
        /// Der logische Name der Eigenschaft oder Klasse.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initialisiert eine neue Instanz der LogicalNameAttribute-Klasse mit dem angegebenen Namen.
        /// </summary>
        /// <param name="name">Der logische Name, der der Eigenschaft oder Klasse zugeordnet ist.</param>
        public LogicalNameAttribute(string name)
        {
            Name = name;
        }
    }

}
