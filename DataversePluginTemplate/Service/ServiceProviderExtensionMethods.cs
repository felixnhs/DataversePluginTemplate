using System;

namespace DataversePluginTemplate
{
    /// <summary>
    /// Stellt Erweiterungsmethoden für <see cref="IServiceProvider"/> bereit.
    /// Diese Methoden erleichtern den Zugriff auf Dienste, die von einem Dienstanbieter bereitgestellt werden.
    /// </summary>
    internal static class ServiceProviderExtensionMethods
    {
        /// <summary>
        /// Ruft einen Dienst vom angegebenen Typ aus dem <see cref="IServiceProvider"/> ab.
        /// </summary>
        /// <typeparam name="T">Der Typ des Dienstes, der abgerufen werden soll.</typeparam>
        /// <param name="serviceProvider">Der <see cref="IServiceProvider"/>, der die Dienste bereitstellt.</param>
        /// <returns>Der Dienst des angegebenen Typs oder <c>null</c>, wenn der Dienst nicht gefunden wird.</returns>
        /// <exception cref="InvalidCastException">Wird ausgelöst, wenn der abgerufene Dienst nicht in den angegebenen Typ <typeparamref name="T"/> konvertiert werden kann.</exception>
        internal static T GetService<T>(this IServiceProvider serviceProvider) => (T)serviceProvider.GetService(typeof(T));
    }

}
