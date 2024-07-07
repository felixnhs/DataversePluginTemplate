using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;

namespace DataversePluginTemplate.Examples
{
    [LogicalName("passagiere_im_flug")]
    internal class PassagiereImFlug : BaseEntity<PassagiereImFlug>
    {
        [LogicalName("passagierid")]
        public string PassagierId
        {
            get => Get(p => p.PassagierId);
            set => Set(p => p.PassagierId, value);
        }

        [LogicalName("flugid")]
        public string FlugId 
        {
            get => Get(p => p.FlugId);
            set => Set(p => p.FlugId, value);
        }

        public PassagiereImFlug(Entity entity = null) : base(entity) { }
    }
}
