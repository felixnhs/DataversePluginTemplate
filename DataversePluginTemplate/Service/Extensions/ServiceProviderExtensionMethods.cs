using System;

namespace DataversePluginTemplate.Service.Extensions
{
    internal static class ServiceProviderExtensionMethods
    {
        internal static T GetService<T>(this IServiceProvider serviceProvider) => (T)serviceProvider.GetService(typeof(T));
    }

}
