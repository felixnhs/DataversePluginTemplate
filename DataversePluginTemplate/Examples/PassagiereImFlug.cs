using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate.Examples
{
    [LogicalName("passagiere_im_flug")]
    internal class PassagiereImFlug : BaseEntity<PassagiereImFlug>
    {
        [LogicalName("passagierid")]
        public string PassagierId { get; set; }

        [LogicalName("flugid")]
        public string FlugId { get; set; }

        public PassagiereImFlug(Entity entity) : base(entity) { }
    }
}
