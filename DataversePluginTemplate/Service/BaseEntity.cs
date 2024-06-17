using Microsoft.Xrm.Sdk;
using System;
using System.Linq.Expressions;

namespace DataversePluginTemplate.Service
{
    /// <summary>
    /// Abstrakte Basisklasse für Entitäten im CRM-System.
    /// </summary>
    /// <typeparam name="TChild">Der abgeleitete Typ der Entität.</typeparam>
    internal abstract class BaseEntity<TChild>
    {
        private readonly Entity _entity;

        /// <summary>
        /// Initialisiert eine neue Instanz der BaseEntity mit der angegebenen CRM-Entität.
        /// </summary>
        /// <param name="entity">Die CRM-Entität, die die Daten enthält.</param>
        internal BaseEntity(Entity entity)
        {
            _entity = entity;
            Init(); // Initialisiert die Eigenschaften der Entität aus der CRM-Entität.
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft der Entität basierend auf dem Property-Expression und dem neuen Wert.
        /// </summary>
        /// <typeparam name="TProperty">Der Typ der Eigenschaft.</typeparam>
        /// <typeparam name="TValue">Der Typ des neuen Werts.</typeparam>
        /// <param name="propertyExpression">Lambda-Ausdruck, der die Eigenschaft auswählt.</param>
        /// <param name="value">Der neue Wert, der gesetzt werden soll.</param>
        public void Set<TProperty, TValue>(Expression<Func<TChild, TProperty>> propertyExpression, TValue value)
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();

            _entity.Attributes[logicalName] = value;
            property.SetValue(this, value);
        }

        /// <summary>
        /// Ruft den Wert einer Eigenschaft der Entität basierend auf dem Property-Expression ab.
        /// </summary>
        /// <typeparam name="TProperty">Der Typ der Eigenschaft.</typeparam>
        /// <typeparam name="TValue">Der Typ des zurückgegebenen Werts.</typeparam>
        /// <param name="propertyExpression">Lambda-Ausdruck, der die Eigenschaft auswählt.</param>
        /// <returns>Der Wert der ausgewählten Eigenschaft.</returns>
        public TProperty Get<TProperty>(Expression<Func<TChild, TProperty>> propertyExpression)
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            return _entity.GetAttributeValue<TProperty>(logicalName);
        }

        /// <summary>
        /// Initialisiert die Eigenschaften der Entität basierend auf den Attributen der CRM-Entität.
        /// </summary>
        private void Init()
        {
            var properties = typeof(TChild).GetProperties(); // Holt alle Eigenschaften des abgeleiteten Typs.
            foreach (var property in properties)
            {
                var logicalName = property.GetLogicalName(); // Holt den logischen Namen der Eigenschaft.
                if (string.IsNullOrWhiteSpace(logicalName))
                    continue; // Überspringt Eigenschaften ohne logischen Namen.

                if (!_entity.Attributes.Contains(logicalName))
                    continue; // Überspringt Eigenschaften, die nicht in den CRM-Attributen enthalten sind.

                property.SetValue(this, _entity.Attributes[logicalName]); // Setzt den Wert der Eigenschaft aus den CRM-Attributen.
            }
        }
    }
}
