using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq.Expressions;

namespace DataversePluginTemplate.Service.Entities
{
    /// <summary>
    /// Abstrakte Basisklasse für Entitäten im CRM-System.
    /// </summary>
    /// <typeparam name="TChild">Der abgeleitete Typ der Entität.</typeparam>
    internal abstract class BaseEntity<TChild>
    {
        private readonly Entity _entity;
        internal Entity Entity => _entity;
        internal EntityReference Reference => _entity.ToEntityReference();

        internal Guid Id { get => _entity.Id; set => _entity.Id = value; }

        /// <summary>
        /// Initialisiert eine neue Instanz der BaseEntity mit der angegebenen CRM-Entität.
        /// </summary>
        /// <param name="entity">Die CRM-Entität, die die Daten enthält.</param>
        internal BaseEntity(Entity entity = null)
        {
            if (entity != null)
                _entity = entity;
            else
                _entity = new Entity(typeof(TChild).GetLogicalName());
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

        public TProperty Get<TProperty, TValue>(Expression<Func<TChild, TProperty>> propertyExpression, Func<TValue, TProperty> valueSelector)
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            return valueSelector(_entity.GetAttributeValue<TValue>(logicalName));
        }

        public TProperty? GetEnum<TProperty>(Expression<Func<TChild, TProperty?>> propertyExpression)
            where TProperty : struct, Enum
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            var value = _entity.GetAttributeValue<OptionSetValue>(logicalName);
            return value == null ? (TProperty?)null : (TProperty)Enum.ToObject(typeof(TProperty), value.Value);
        }

        public void SetEnum<TProperty>(Expression<Func<TChild, TProperty?>> propertyExpression, TProperty? value)
            where TProperty : struct, Enum
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            _entity.Attributes[logicalName] = new OptionSetValue(Convert.ToInt32(value));
        }

        public TProperty GetEnumArray<TProperty>(Expression<Func<TChild, TProperty>> propertyExpression)
            where TProperty : class
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            var value = _entity.GetAttributeValue<OptionSetValueCollection>(logicalName);
            var enumType = property.PropertyType.GetElementType();

            var enumArray = Array.CreateInstance(enumType, value.Count);
            for (int i = 0; i < enumArray.Length; i++)
                enumArray.SetValue(Enum.ToObject(enumType, Convert.ToInt32(value[i].Value)), i);

            return enumArray as TProperty;
        }

        public void SetEnumArray<TProperty>(Expression<Func<TChild, TProperty>> propertyExpression, TProperty value)
            where TProperty : class
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();

            if (!(value is Array arr))
                throw new Exception();

            _entity.Attributes[logicalName] = new OptionSetValueCollection();

            foreach (var item in arr)
                ((OptionSetValueCollection)_entity.Attributes[logicalName]).Add(new OptionSetValue(Convert.ToInt32(item)));
        }
    }
}
