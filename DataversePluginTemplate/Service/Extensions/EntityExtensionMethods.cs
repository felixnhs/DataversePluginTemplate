using DataversePluginTemplate.Service.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataversePluginTemplate.Service.Extensions
{
    public static class EntityExtensionMethods
    {
        public static void SetPropertyValue<TEntity, TProperty, TValue>(this Entity entity, TEntity earlyBoundInstance, Expression<Func<TEntity, TProperty>> propertyExpression, TValue value)
            where TEntity : Entity, new()
        {
            var logicalName = GetPropertyLogicalName(propertyExpression);
            if (logicalName == null)
                return;

            entity[logicalName] = value;
        }

        public static TValue GetPropertyValue<TEntity, TProperty, TValue>(this Entity entity, TEntity earlyBoundInstance, Expression<Func<TEntity, TProperty>> propertyExpression)
            where TEntity : Entity, new()
            where TValue : class, new()
        {
            var logicalName = GetPropertyLogicalName(propertyExpression);
            if (logicalName == null)
                return null;

            return entity[logicalName] as TValue;
        }

        public static T As<T>(this Entity entity)
            where T : BaseEntity<T>
        {
            // Konstruktor suchen
            var constructor = typeof(T).GetConstructor(new Type[] { typeof(Entity) });
            if (constructor == null)
            {
                throw new InvalidOperationException($"Type {typeof(T)} does not have a constructor that takes an Entity parameter.");
            }

            // Konstruktor aufrufen und T dadurch erstellen
            var target = (T)constructor.Invoke(new object[] { entity });
            return target;
        }

        public static IEnumerable<T> As<T>(this IEnumerable<Entity> entities)
            where T : BaseEntity<T>
        {
            return entities.Select(entity => entity.As<T>());
        }

        public static IEnumerable<T> As<T>(this EntityCollection entityCollection)
            where T : BaseEntity<T>
        {
            return entityCollection.Entities.As<T>();
        }

        public static void Update<T>(this BaseEntity<T> entity, PluginContext context, Action<T> configure)
            where T : BaseEntity<T>
        {
            var updateEntity = new Entity(entity.Entity.LogicalName, entity.Id)
                .As<T>();

            configure(updateEntity);

            context.OrgService.Update(updateEntity.Entity);
        }

        public static void Update<T>(this T entity, PluginContext context, Expression<Func<T, object[]>> propertySelector)
            where T : BaseEntity<T>
        {
            var propertyInfos = propertySelector.GetPropertyInfos();
            if (propertyInfos.Length == 0)
                return;

            var updateEntity = new Entity(entity.Entity.LogicalName, entity.Id);

            foreach(var property in propertyInfos)
            {
                var value = property.GetValue(entity);
                updateEntity.Attributes.Add(property.GetLogicalName(), value);
            }

            context.OrgService.Update(updateEntity);
        }

        public static void SetEntityValue<TEntity, TInput, TProp>(this TEntity entity, Expression<Func<TEntity, TProp>> propertySelector, TInput inputValue, bool? shouldClear)
            where TEntity : BaseEntity<TEntity>
        {
            var propertyInfo = propertySelector.GetPropertyInfo();
            if (shouldClear == true)
                propertyInfo.SetValue(entity, null);

            else
            {
                if (inputValue != null)
                    propertyInfo.SetValue(entity, inputValue);
            }
        }

        private static string GetPropertyLogicalName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
            where TEntity : Entity, new()
        {
            if (!(propertyExpression.Body is MemberExpression member))
                return null;

            if (!(member.Member is PropertyInfo propInfo))
                return null;

            Type type = typeof(TEntity);
            if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                return null;

            try
            {
                var attribute = propInfo.GetCustomAttribute<AttributeLogicalNameAttribute>();
                return attribute.LogicalName;
            }
            catch
            {
                return null;
            }
        }
    }
}
