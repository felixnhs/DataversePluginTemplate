using Microsoft.Xrm.Sdk;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DataversePluginTemplate.Service
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
    }
}
