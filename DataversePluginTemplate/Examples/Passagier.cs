using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using System;

namespace DataversePluginTemplate.Examples
{
    [LogicalName(LOGICAL_NAME)]
    internal class Passagier : BaseEntity<Passagier>
    {
        internal const string LOGICAL_NAME = "passagiere";
        internal const string ID = "passagierid";
        internal const string VORNAME = "vorname";
        internal const string NACHNAME = "nachname";
        internal const string ALTER = "alter";

        // Optional, wenn Id für Joins benötigt wird
        [LogicalName(ID)]
        [PrimaryKey]
        public new Guid Id => base.Id;

        [LogicalName(VORNAME)]
        public string Vorname 
        {
            get => Get(p => p.Vorname);
            set => Set(p => p.Vorname, value);
        }

        [LogicalName(NACHNAME)]
        public string Nachname
        {
            get => Get(p => p.Nachname);
            set => Set(p => p.Nachname, value);
        }

        [LogicalName(ALTER)]
        public int Alter 
        {
            get => Get(p => p.Alter);
            set => Set(p => p.Alter, value);
        }

        public Passagier(Entity entity = null) : base(entity) { }
    }
}
