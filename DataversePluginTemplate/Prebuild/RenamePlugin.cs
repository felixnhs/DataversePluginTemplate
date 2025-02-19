using DataversePluginTemplate.Service;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Text;

namespace DataversePluginTemplate.Prebuild
{
    public abstract class RenamePlugin : EntityPreprocessingPlugin, IPlugin
    {
        private readonly string _nameColumn;

        public RenamePlugin(string nameColumn) : base()
        {
            _nameColumn = nameColumn;
        }

        public RenamePlugin(string nameColumn, string unsecureConfiguration) : base(unsecureConfiguration)
        {
            _nameColumn = nameColumn;
        }

        public RenamePlugin(string nameColumn, string unsecureConfiguration, string secureConfiguration) : base(unsecureConfiguration, secureConfiguration)
        {
            _nameColumn = nameColumn;
        }

        protected abstract IEnumerable<string> BuildName(PluginContext context, Entity entity);

        protected sealed override IEnumerable<(string attribut, object value)> Process(PluginContext context, Entity entity)
        {
            var nameParts = BuildName(context, entity)
                .GetEnumerator();

            StringBuilder stringBuilder = new StringBuilder();

            while (nameParts.MoveNext())
                stringBuilder.Append(nameParts.Current);

            yield return (_nameColumn, stringBuilder.ToString());
        }
    }
}
