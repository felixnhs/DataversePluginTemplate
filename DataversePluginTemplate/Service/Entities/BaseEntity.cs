using DataversePluginTemplate.Service.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq.Expressions;

namespace DataversePluginTemplate.Service.Entities
{
    /// <summary>
    /// Base for all custom wrapper classes around the <see cref="Microsoft.Xrm.Sdk.Entity"/> class.
    /// Provides functionalities to interact with the data inside the entities AttributeCollection 
    /// in a type-safe way.
    /// Your class should represent a dataverse table in your environment.
    /// Example usage in inherited classes:
    /// <code>
    /// [LogicalName("...")]
    /// class Person : BaseEntity<Person> 
    /// {
    ///     [LogicalName("...")]
    ///     public string Name { get => Get(x => x.Name); set => Set(x => x.Name, value);}
    /// }
    /// </code>
    /// This way enables the use of properties to interact with entity attributes.
    /// This approach is required for <see cref="Queries.QueryContext{T}"/>.
    /// </summary>
    /// <typeparam name="TChild">The type of child class itself. For typesafty in child classes.</typeparam>
    public abstract class BaseEntity<TChild>
    {
        private readonly Entity _entity;
        public Entity Entity => _entity;
        public EntityReference Reference => _entity.ToEntityReference();

        public virtual Guid Id { get => _entity.Id; set => _entity.Id = value; }

        protected BaseEntity(Entity entity = null)
        {
            if (entity != null)
                _entity = entity;
            else
                _entity = new Entity(typeof(TChild).GetLogicalName());
        }

        protected void Set<TProperty, TValue>(Expression<Func<TChild, TProperty>> propertyExpression, TValue value)
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            _entity.Attributes[logicalName] = value;
        }

        protected TProperty Get<TProperty>(Expression<Func<TChild, TProperty>> propertyExpression)
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            return _entity.GetAttributeValue<TProperty>(logicalName);
        }

        protected TProperty Get<TProperty, TValue>(Expression<Func<TChild, TProperty>> propertyExpression, Func<TValue, TProperty> valueSelector)
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            return valueSelector(_entity.GetAttributeValue<TValue>(logicalName));
        }

        protected TProperty? GetEnum<TProperty>(Expression<Func<TChild, TProperty?>> propertyExpression)
            where TProperty : struct, Enum
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            var value = _entity.GetAttributeValue<OptionSetValue>(logicalName);
            return value == null ? (TProperty?)null : (TProperty)Enum.ToObject(typeof(TProperty), value.Value);
        }

        protected void SetEnum<TProperty>(Expression<Func<TChild, TProperty?>> propertyExpression, TProperty? value)
            where TProperty : struct, Enum
        {
            var property = propertyExpression.GetPropertyInfo();
            var logicalName = property.GetLogicalName();
            _entity.Attributes[logicalName] = new OptionSetValue(Convert.ToInt32(value));
        }

        protected TProperty GetEnumArray<TProperty>(Expression<Func<TChild, TProperty>> propertyExpression)
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

        protected void SetEnumArray<TProperty>(Expression<Func<TChild, TProperty>> propertyExpression, TProperty value)
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
