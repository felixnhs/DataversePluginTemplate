using DataversePluginTemplate.Service.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataversePluginTemplate.Service.Extensions
{
    internal static class EntityExtensionMethods
    {
        /// <summary>
        /// Bei Early-bound Entities werden alle Felder aktualisiert. Diese Funktion ist für late-bound Entities geeignet, weil diese
        /// immer nur die Attribute gesetzt bekommen müssen, die auch tatsächlich aktualisiert werden.
        /// Da man die Attribute aber per String wie bei einem Dictionary angeben muss, kann es leicht zu Fehlern kommen.
        /// Diese Funktion kombiniert Early und late bound, indem die zu aktualisierende Property per Expression angegeben werden kann. Z.B. so:
        /// <code>
        ///  Entity lateBound = new Entity(EarlyBound.EntityLogicalName, earlybound.Id);
        ///  lateBound.SetPropertyValue(earlyBound, e => e.attribute, false);
        ///  orgService.update(lateBound);
        /// </code>
        /// Die Funktion aktualisiert nicht die Eigenschaft der early-bound Instanz.
        /// </summary>
        /// <typeparam name="TEntity">Die Early-Bound Entity Class</typeparam>
        /// <typeparam name="TProperty">Der Typ der Eigenschaft, die abgerufen werden soll</typeparam>
        /// <typeparam name="TValue">Der Typ des zurückgegebenen Wertes</typeparam>
        /// <param name="entity">Die late-bound Entity, von der der Wert abgerufen werden soll</param>
        /// <param name="earlyBoundInstance">Eine Instanz der Early-Bound Entity, die als Vorlage für die Eigenschaft dient</param>
        /// <param name="propertyExpression">Eine Lambda-Expression, die die abzurufende Eigenschaft angibt</param>
        /// <param name="value">Der Wert, der der Eigenschaft zugewiesen werden soll</param>
        internal static void SetPropertyValue<TEntity, TProperty, TValue>(this Entity entity, TEntity earlyBoundInstance, Expression<Func<TEntity, TProperty>> propertyExpression, TValue value)
            where TEntity : Entity, new()
        {
            var logicalName = GetPropertyLogicalName(propertyExpression);
            if (logicalName == null)
                return;

            entity[logicalName] = value;
        }

        /// <summary>
        /// Bei Early-bound Entities werden alle Felder aktualisiert. Diese Funktion ist für late-bound Entities geeignet, weil diese
        /// immer nur die Attribute gesetzt bekommen müssen, die auch tatsächlich aktualisiert werden.
        /// Da man die Attribute aber per String wie bei einem Dictionary angeben muss, kann es leicht zu Fehlern kommen.
        /// Diese Funktion kombiniert Early und late bound, indem die gewünschte Property per Expression angegeben werden kann. Z.B. so:
        /// <code>
        ///  Entity lateBound = new Entity(EarlyBound.EntityLogicalName, earlybound.Id);
        ///  var attributeValue = lateBound.GetPropertyValue(earlyBound, e => e.attribute);
        /// </code>
        /// </summary>
        /// <typeparam name="TEntity">Die Early-Bound Entity Class</typeparam>
        /// <typeparam name="TProperty">Der Typ der Eigenschaft, die abgerufen werden soll</typeparam>
        /// <typeparam name="TValue">Der Typ des zurückgegebenen Wertes</typeparam>
        /// <param name="entity">Die late-bound Entity, von der der Wert abgerufen werden soll</param>
        /// <param name="earlyBoundInstance">Eine Instanz der Early-Bound Entity, die als Vorlage für die Eigenschaft dient</param>
        /// <param name="propertyExpression">Eine Lambda-Expression, die die abzurufende Eigenschaft angibt</param>
        /// <returns>Der Wert der angegebenen Eigenschaft als <typeparamref name="TValue"/>, oder <c>null</c>, wenn die Eigenschaft nicht gefunden wird oder der Typ nicht übereinstimmt</returns>
        internal static TValue GetPropertyValue<TEntity, TProperty, TValue>(this Entity entity, TEntity earlyBoundInstance, Expression<Func<TEntity, TProperty>> propertyExpression)
            where TEntity : Entity, new()
            where TValue : class, new()
        {
            var logicalName = GetPropertyLogicalName(propertyExpression);
            if (logicalName == null)
                return null;

            return entity[logicalName] as TValue;
        }

        /// <summary>
        /// Erstellt eine Instanz des Typs <typeparamref name="T"/> aus einer gegebenen <see cref="Entity"/>.
        /// Der Typ <typeparamref name="T"/> muss einen Konstruktor haben, der eine <see cref="Entity"/> als Parameter akzeptiert.
        /// </summary>
        /// <typeparam name="T">Der Typ, der von <see cref="BaseEntity{T}"/> abgeleitet ist und eine <see cref="Entity"/> im Konstruktor akzeptiert.</typeparam>
        /// <param name="entity">Die <see cref="Entity"/>, die zur Erstellung der Instanz von <typeparamref name="T"/> verwendet wird.</param>
        /// <returns>Eine Instanz des Typs <typeparamref name="T"/>, die aus der <paramref name="entity"/> erstellt wurde.</returns>
        /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn der Typ <typeparamref name="T"/> keinen Konstruktor hat, der eine <see cref="Entity"/> als Parameter akzeptiert.</exception>
        internal static T As<T>(this Entity entity)
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

        /// <summary>
        /// Wandelt eine Auflistung von <see cref="Entity"/>-Objekten in eine Auflistung von Instanzen des Typs <typeparamref name="T"/> um.
        /// Der Typ <typeparamref name="T"/> muss einen Konstruktor haben, der eine <see cref="Entity"/> als Parameter akzeptiert.
        /// </summary>
        /// <typeparam name="T">Der Typ, der von <see cref="BaseEntity{T}"/> abgeleitet ist und eine <see cref="Entity"/> im Konstruktor akzeptiert.</typeparam>
        /// <param name="entities">Die Auflistung von <see cref="Entity"/>-Objekten, die in Instanzen des Typs <typeparamref name="T"/> umgewandelt werden sollen.</param>
        /// <returns>Eine Auflistung von Instanzen des Typs <typeparamref name="T"/>, die aus den <paramref name="entities"/> erstellt wurden.</returns>
        internal static IEnumerable<T> As<T>(this IEnumerable<Entity> entities)
            where T : BaseEntity<T>
        {
            return entities.Select(entity => entity.As<T>());
        }

        /// <summary>
        /// Wandelt eine <see cref="EntityCollection"/> in eine Auflistung von Instanzen des Typs <typeparamref name="T"/> um.
        /// Der Typ <typeparamref name="T"/> muss einen Konstruktor haben, der eine <see cref="Entity"/> als Parameter akzeptiert.
        /// </summary>
        /// <typeparam name="T">Der Typ, der von <see cref="BaseEntity{T}"/> abgeleitet ist und eine <see cref="Entity"/> im Konstruktor akzeptiert.</typeparam>
        /// <param name="entityCollection">Die <see cref="EntityCollection"/>, die in eine Auflistung von Instanzen des Typs <typeparamref name="T"/> umgewandelt werden soll.</param>
        /// <returns>Eine Auflistung von Instanzen des Typs <typeparamref name="T"/>, die aus der <paramref name="entityCollection"/> erstellt wurden.</returns>
        internal static IEnumerable<T> As<T>(this EntityCollection entityCollection)
            where T : BaseEntity<T>
        {
            return entityCollection.Entities.As<T>();
        }

        internal static void Update<T>(this BaseEntity<T> entity, PluginContext context, Action<T> configure)
            where T : BaseEntity<T>
        {
            var updateEntity = new Entity(entity.Entity.LogicalName, entity.Id)
                .As<T>();

            configure(updateEntity);

            context.OrgService.Update(updateEntity.Entity);
        }

        internal static void Update<T>(this T entity, PluginContext context, Expression<Func<T, object[]>> propertySelector)
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

        /// <summary>
        /// Lädt den Logicalname aus dem <see cref="AttributeLogicalNameAttribute"/> der Property aus der Early-Bound Entität.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns>Der Logical Name. Bei einem Fehler wird null zurückgegeben</returns>
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

        internal static void SetEntityValue<TEntity, TInput, TProp>(this TEntity entity, Expression<Func<TEntity, TProp>> propertySelector, TInput inputValue, bool? shouldClear)
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
    }
}
