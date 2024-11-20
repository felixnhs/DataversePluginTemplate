using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate.Examples
{
    [LogicalName(LOGICAL_NAME)]
    internal class PassagiereImFlug : BaseEntity<PassagiereImFlug>
    {
        internal const string LOGICAL_NAME = "passagiere_im_flug";
        internal const string PASSAGIER_ID = "passagierid";
        internal const string FLUG_ID = "flugid";

        [LogicalName(PASSAGIER_ID)]
        public string PassagierId { get; set; }

        [LogicalName(FLUG_ID)]
        public string FlugId { get; set; }

        public PassagiereImFlug(Entity entity = null) : base(entity) { }
    }
}
