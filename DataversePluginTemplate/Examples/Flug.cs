using DataversePluginTemplate.Service.Entities;
using Microsoft.Xrm.Sdk;
using System;

namespace DataversePluginTemplate.Examples
{
    [LogicalName(LOGICAL_NAME)]
    internal class Flug : BaseEntity<Flug>
    {
        internal const string LOGICAL_NAME = "flug";

        [PrimaryKey]
        [LogicalName("flugid")]
        public new Guid Id => base.Id;

        [Includable]
        [LogicalName("flugzeugid")]
        public Flugzeug Flugzeug { get; set; }

        public Flug(Entity entity = null) : base(entity) { }
    }
}
