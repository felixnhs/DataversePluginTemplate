using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate.Examples
{
    [LogicalName("passagiere")]
    internal class Passagier : BaseEntity<Passagier>
    {
        [LogicalName("vorname")]
        public string Vorname
        {
            get => Get(p => p.Vorname);
            set => Set(p => p.Vorname, value);
        }

        [LogicalName("nachname")]
        public string Nachname 
        {
            get => Get(p => p.Nachname);
            set => Set(p => p.Nachname, value);
        }

        [LogicalName("alter")]
        public int Alter 
        {
            get => Get(p => p.Alter);
            set => Set(p => p.Alter, value);
        }

        public Passagier(Entity entity = null) : base(entity) { }
    }
}
