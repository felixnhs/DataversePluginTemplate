using System;

namespace DataversePluginTemplate.Service
{
    /// <summary>
    /// Statische Klasse, die Erweiterungsmethoden für die <see cref="System.Type"/>-Klasse bereitstellt.
    /// Diese Methoden erleichtern die Arbeit mit Nullable-Typen und Typüberprüfungen.
    /// </summary>
    internal static class TypeExtensionMethods
    {
        /// <summary>
        /// Überprüft, ob der angegebene <see cref="Type"/> ein Nullable-Typ ist oder ein Verweistyp (Klasse oder Schnittstelle).
        /// </summary>
        /// <param name="type">Der zu überprüfende <see cref="Type"/>.</param>
        /// <returns><c>true</c>, wenn der <paramref name="type"/> ein Nullable-Typ, eine Klasse oder eine Schnittstelle ist; andernfalls <c>false</c>.</returns>
        internal static bool IsNullable(this Type type) => Nullable.GetUnderlyingType(type) != null || type.IsClass || type.IsInterface;

        /// <summary>
        /// Überprüft, ob der angegebene <see cref="Type"/> entweder dem angegebenen Zieltyp entspricht oder ein Nullable-Typ ist, dessen zugrunde liegender Typ dem Zieltyp entspricht.
        /// </summary>
        /// <param name="type">Der zu überprüfende <see cref="Type"/>.</param>
        /// <param name="target">Der Zieltyp, mit dem verglichen werden soll.</param>
        /// <returns><c>true</c>, wenn der <paramref name="type"/> entweder dem <paramref name="target"/> entspricht oder ein Nullable-Typ ist, dessen zugrunde liegender Typ dem <paramref name="target"/> entspricht; andernfalls <c>false</c>.</returns>
        internal static bool IsTypeOrNullable(this Type type, Type target) => type == target || Nullable.GetUnderlyingType(type) == target;

        /// <summary>
        /// Überprüft, ob der angegebene <see cref="Type"/> entweder dem angegebenen generischen Typ <typeparamref name="T"/> entspricht oder ein Nullable-Typ ist, dessen zugrunde liegender Typ dem generischen Typ <typeparamref name="T"/> entspricht.
        /// </summary>
        /// <typeparam name="T">Der Zieltyp, mit dem verglichen werden soll.</typeparam>
        /// <param name="type">Der zu überprüfende <see cref="Type"/>.</param>
        /// <returns><c>true</c>, wenn der <paramref name="type"/> entweder dem Typ <typeparamref name="T"/> entspricht oder ein Nullable-Typ ist, dessen zugrunde liegender Typ dem Typ <typeparamref name="T"/> entspricht; andernfalls <c>false</c>.</returns>
        internal static bool IsTypeOrNullable<T>(this Type type) => type == typeof(T) || Nullable.GetUnderlyingType(type) == typeof(T);
    }

}
