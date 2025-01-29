using DataversePluginTemplate.Service.Entities;
using Microsoft.Xrm.Sdk;
using System;

namespace DataversePluginTemplate.Examples
{
    [LogicalName("flugzeug")]
    internal class Flugzeug : BaseEntity<Flugzeug>
    {
        [PrimaryKey]
        [LogicalName("flugzeugid")]
        public new Guid Id => base.Id;

        [LogicalName("name")]
        public string Name
        {
            get => Get(f => f.Name);
            set => Set(f => f.Name, value);
        }

        public Flugzeug(Entity entity = null) : base(entity) { }
    }
}
