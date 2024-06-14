using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate.Examples
{
    [LogicalName("passagiere")]
    internal class Passagier : BaseEntity<Passagier>
    {
        [LogicalName("vorname")]
        public string Vorname { get; set; }

        [LogicalName("nachname")]
        public string Nachname { get; set; }

        [LogicalName("alter")]
        public int Alter { get; set; }

        public Passagier(Entity entity) : base(entity) { }
    }
}
