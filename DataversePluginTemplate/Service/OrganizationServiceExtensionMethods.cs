using DataversePluginTemplate.Model.Enums.Internal;
using DataversePluginTemplate.Queries;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataversePluginTemplate.Service
{
    /// <summary>
    /// Erweiterungsmethoden für die Arbeit mit der IOrganizationService-Schnittstelle.
    /// </summary>
    internal static class OrganizationServiceExtensionMethods
    {
        /// <summary>
        /// Führt mehrere Anfragen des angegebenen Typs TRequest aus.
        /// </summary>
        /// <typeparam name="TRequest">Der Typ der Anfrage, der von OrganizationRequest erben muss.</typeparam>
        /// <param name="orgService">Die IOrganizationService-Instanz.</param>
        /// <param name="requests">Eine Auflistung von Anfragen, die ausgeführt werden sollen.</param>
        /// <param name="configureRequest">Eine optionale Aktion, um das ExecuteMultipleRequest-Objekt zu konfigurieren.</param>
        /// <returns>Die Antwort des OrganizationService nach der Ausführung der Anfragen.</returns>
        internal static OrganizationResponse ExecuteMultiple<TRequest>(
            this IOrganizationService orgService,
            IEnumerable<TRequest> requests,
            Action<ExecuteMultipleRequest> configureRequest = null)
            where TRequest : OrganizationRequest
        {
            var executeMultiRequest = new ExecuteMultipleRequest();
            executeMultiRequest.Settings = new ExecuteMultipleSettings();
            executeMultiRequest.Requests = new OrganizationRequestCollection();
            configureRequest?.Invoke(executeMultiRequest);

            foreach (var request in requests)
                executeMultiRequest.Requests.Add(request);

            return orgService.Execute(executeMultiRequest);
        }

        /// <summary>
        /// Führt mehrere Anfragen des angegebenen Typs TRequest aus, die aus Elementen vom Typ TItem erstellt wurden.
        /// </summary>
        /// <typeparam name="TRequest">Der Typ der Anfrage, der von OrganizationRequest erben und einen parameterlosen Konstruktor haben muss.</typeparam>
        /// <typeparam name="TItem">Der Typ der Elemente, die zur Erstellung und Konfiguration der Anfragen verwendet werden.</typeparam>
        /// <param name="orgService">Die IOrganizationService-Instanz.</param>
        /// <param name="requestItems">Eine Auflistung von Elementen, aus denen die Anfragen erstellt und konfiguriert werden.</param>
        /// <param name="configureRequestItem">Eine Aktion, um jede Anfrage aus einem Element zu konfigurieren.</param>
        /// <param name="configureRequest">Eine optionale Aktion, um das ExecuteMultipleRequest-Objekt zu konfigurieren.</param>
        /// <returns>Die Antwort des OrganizationService nach der Ausführung der Anfragen.</returns>
        internal static OrganizationResponse ExecuteMultiple<TRequest, TItem>(
            this IOrganizationService orgService,
            IEnumerable<TItem> requestItems,
            Action<TRequest, TItem> configureRequestItem,
            Action<ExecuteMultipleRequest> configureRequest = null)
            where TRequest : OrganizationRequest, new()
        {
            var requests = new List<TRequest>();

            foreach (var item in requestItems)
            {
                var request = new TRequest();
                configureRequestItem(request, item);
                requests.Add(request);
            }

            return orgService.ExecuteMultiple<TRequest>(requests, configureRequest);
        }

        /// <summary>
        /// Erstellt mehrere Entitäten im OrganizationService.
        /// </summary>
        /// <param name="orgService">Die IOrganizationService-Instanz.</param>
        /// <param name="entities">Eine Auflistung von Entitäten, die erstellt werden sollen.</param>
        /// <param name="configureRequest">Eine optionale Aktion, um das ExecuteMultipleRequest-Objekt zu konfigurieren.</param>
        /// <returns>Die Antwort des OrganizationService nach dem Erstellen der Entitäten.</returns>
        internal static OrganizationResponse CreateMultiple(
            this IOrganizationService orgService,
        IEnumerable<Entity> entities,
            Action<ExecuteMultipleRequest> configureRequest = null)
        {
            return orgService.ExecuteMultiple<CreateRequest, Entity>(entities, (request, entity) =>
            {
                request.Target = entity;
            }, configureRequest);
        }

        /// <summary>
        /// Erstellt mehrere Entitäten im OrganizationService mit Standardeinstellungen.
        /// </summary>
        /// <param name="orgService">Die IOrganizationService-Instanz.</param>
        /// <param name="entities">Eine Auflistung von Entitäten, die erstellt werden sollen.</param>
        /// <returns>Die Antwort des OrganizationService nach dem Erstellen der Entitäten.</returns>
        internal static OrganizationResponse CreateMultiple(
            this IOrganizationService orgService,
            IEnumerable<Entity> entities)
        {
            return orgService.CreateMultiple(entities, configure =>
            {
                configure.Settings.ContinueOnError = true;
                configure.Settings.ReturnResponses = true;
            });
        }

        /// <summary>
        /// Aktualisiert mehrere Entitäten im OrganizationService.
        /// </summary>
        /// <param name="orgService">Die IOrganizationService-Instanz.</param>
        /// <param name="entities">Eine Auflistung von Entitäten, die aktualisiert werden sollen.</param>
        /// <param name="configureRequest">Eine optionale Aktion, um das ExecuteMultipleRequest-Objekt zu konfigurieren.</param>
        /// <returns>Die Antwort des OrganizationService nach der Aktualisierung der Entitäten.</returns>
        internal static OrganizationResponse UpdateMultiple(
            this IOrganizationService orgService,
        IEnumerable<Entity> entities,
            Action<ExecuteMultipleRequest> configureRequest = null)
        {
            return orgService.ExecuteMultiple<UpdateRequest, Entity>(entities, (request, entity) =>
            {
                request.Target = entity;
            }, configureRequest);
        }

        /// <summary>
        /// Aktualisiert mehrere Entitäten im OrganizationService mit Standardeinstellungen.
        /// </summary>
        /// <param name="orgService">Die IOrganizationService-Instanz.</param>
        /// <param name="entities">Eine Auflistung von Entitäten, die aktualisiert werden sollen.</param>
        /// <returns>Die Antwort des OrganizationService nach der Aktualisierung der Entitäten.</returns>
        internal static OrganizationResponse UpdateMultiple(
            this IOrganizationService orgService,
            IEnumerable<Entity> entities)
        {
            return orgService.UpdateMultiple(entities, configure =>
            {
                configure.Settings.ContinueOnError = true;
                configure.Settings.ReturnResponses = true;
            });
        }

        /// <summary>
        /// Löscht mehrere Entitäten im OrganizationService.
        /// </summary>
        /// <param name="orgService">Die IOrganizationService-Instanz.</param>
        /// <param name="entityReferences">Eine Auflistung von Entitätsreferenzen, die gelöscht werden sollen.</param>
        /// <param name="configureRequest">Eine optionale Aktion, um das ExecuteMultipleRequest-Objekt zu konfigurieren.</param>
        /// <returns>Die Antwort des OrganizationService nach dem Löschen der Entitäten.</returns>
        internal static OrganizationResponse DeleteMultiple(
            this IOrganizationService orgService,
            IEnumerable<EntityReference> entityReferences,
            Action<ExecuteMultipleRequest> configureRequest = null)
        {
            return orgService.ExecuteMultiple<DeleteRequest, EntityReference>(entityReferences, (request, entityReference) =>
            {
                request.Target = entityReference;
            }, configureRequest);
        }

        /// <summary>
        /// Löscht mehrere Entitäten im OrganizationService mit Standardeinstellungen.
        /// </summary>
        /// <param name="orgService">Die IOrganizationService-Instanz.</param>
        /// <param name="entityReferences">Eine Auflistung von Entitätsreferenzen, die gelöscht werden sollen.</param>
        /// <returns>Die Antwort des OrganizationService nach dem Löschen der Entitäten.</returns>
        internal static OrganizationResponse DeleteMultiple(
            this IOrganizationService orgService,
            IEnumerable<EntityReference> entityReferences)
        {
            return orgService.DeleteMultiple(entityReferences, configure =>
            {
                configure.Settings.ContinueOnError = true;
                configure.Settings.ReturnResponses = true;
            });
        }

        internal static Entity Retrieve(this IOrganizationService orgService, EntityReference entityReference, params string[] columns)
        {
            return orgService.Retrieve(entityReference, new ColumnSet(columns));
        }
        internal static Entity Retrieve(this IOrganizationService orgService, EntityReference entityReference, bool allColumns = true)
        {
            return orgService.Retrieve(entityReference, new ColumnSet(allColumns));
        }
        internal static Entity Retrieve(this IOrganizationService orgService, EntityReference entityReference, ColumnSet columnSet)
        {
            if (entityReference == null)
                return null;
            return orgService.Retrieve(entityReference.LogicalName, entityReference.Id, columnSet);
        }

        internal static Entity Retrieve(this IOrganizationService orgService, Entity entity, params string[] columns)
        {
            return orgService.Retrieve(entity, new ColumnSet(columns));
        }
        internal static Entity Retrieve(this IOrganizationService orgService, Entity entity, bool allColumns = true)
        {
            return orgService.Retrieve(entity, new ColumnSet(allColumns));
        }
        internal static Entity Retrieve(this IOrganizationService orgService, Entity entity, ColumnSet columnSet)
        {
            if (entity == null)
                return null;
            return orgService.Retrieve(entity.ToEntityReference(), columnSet);
        }

        internal static Entity Retrieve(this IOrganizationService orgService, string entityName, Guid id, params string[] columns)
        {
            return orgService.Retrieve(entityName, id, new ColumnSet(columns));
        }
        internal static Entity Retrieve(this IOrganizationService orgService, string entityName, Guid id, bool allColumns = true)
        {
            return orgService.Retrieve(entityName, id, new ColumnSet(allColumns));
        }
        internal static Entity Retrieve(this IOrganizationService orgService, string entityName, Guid id, ColumnSet columnSet)
        {
            if (id == null || string.IsNullOrWhiteSpace(entityName))
                return null;

            return orgService.Retrieve(entityName, id, columnSet);
        }

        internal static T Retrieve<T>(this IOrganizationService orgService, EntityReference entityReference, params string[] columns)
            where T : BaseEntity<T>
        {
            return orgService.Retrieve<T>(entityReference, new ColumnSet(columns));
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, EntityReference entityReference, bool allColumns)
            where T : BaseEntity<T>
        {
            return orgService.Retrieve<T>(entityReference, new ColumnSet(allColumns));
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, EntityReference entityReference, Columns columns = Columns.DefinedOnly)
            where T : BaseEntity<T>
        {
            return orgService.Retrieve<T>(entityReference, GetColumnSet<T>(columns));
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, EntityReference entityReference, Expression<Func<T, object[]>> propertySelector)
            where T : BaseEntity<T>
        {
            ColumnSet columnSet = GetFromPropertySelector(propertySelector);
            return orgService.Retrieve<T>(entityReference, columnSet);
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, EntityReference entityReference, ColumnSet columnSet)
            where T : BaseEntity<T>
        {
            if (entityReference == null)
                return null;

            return orgService.Retrieve(entityReference.LogicalName, entityReference.Id, columnSet)
            .As<T>();
        }

        internal static T Retrieve<T>(this IOrganizationService orgService, Entity entity, params string[] columns)
            where T : BaseEntity<T>
        {
            return orgService.Retrieve<T>(entity, new ColumnSet(columns));
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, Entity entity, bool allColumns)
            where T : BaseEntity<T>
        {
            return orgService.Retrieve<T>(entity, new ColumnSet(allColumns));
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, Entity entity, Columns columns = Columns.DefinedOnly)
            where T : BaseEntity<T>
        {
            return orgService.Retrieve<T>(entity, GetColumnSet<T>(columns));
        }

        

        internal static T Retrieve<T>(this IOrganizationService orgService, Entity entity, Expression<Func<T, object[]>> propertySelector)
            where T : BaseEntity<T>
        {
            ColumnSet columnSet = GetFromPropertySelector(propertySelector);
            return orgService.Retrieve<T>(entity, columnSet);
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, Entity entity, ColumnSet columnSet)
            where T : BaseEntity<T>
        {
            if (entity == null)
                return null;

            return orgService.Retrieve<T>(entity.ToEntityReference(), columnSet);
        }

        internal static T Retrieve<T>(this IOrganizationService orgService, Guid id, params string[] columns)
            where T : BaseEntity<T>
        {
            return orgService.Retrieve<T>(id, new ColumnSet(columns));
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, Guid id, bool allColumns)
            where T : BaseEntity<T>
        {
            return orgService.Retrieve<T>(id, new ColumnSet(allColumns));
        }

        internal static T Retrieve<T>(this IOrganizationService orgService, Guid id, Columns columns = Columns.DefinedOnly)
            where T : BaseEntity<T>
        {
            return orgService.Retrieve<T>(id, GetColumnSet<T>(columns));
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, Guid id, Expression<Func<T, object[]>> propertySelector)
            where T : BaseEntity<T>
        {
            ColumnSet columnSet = GetFromPropertySelector(propertySelector);
            return orgService.Retrieve<T>(id, columnSet);
        }
        internal static T Retrieve<T>(this IOrganizationService orgService, Guid id, ColumnSet columnSet)
            where T : BaseEntity<T>
        {
            if (id == null)
                return null;

            var logicalName = typeof(T).GetLogicalName();
            return orgService.Retrieve(logicalName, id, columnSet)
                .As<T>();
        }

        /// <summary>
        /// Initialisiert einen Abfragekontext für den angegebenen Entitätsnamen.
        /// </summary>
        /// <param name="orgService">Die IOrganizationService-Instanz.</param>
        /// <param name="entityName">Der logische Name der Entität, die abgefragt werden soll.</param>
        /// <returns>Ein QueryContext-Objekt zur Erstellung und Ausführung von Abfragen.</returns>
        internal static QueryContext Select(
            this IOrganizationService orgService,
            string entityName)
        {
            return new QueryContext(orgService, entityName);
        }

        /// <summary>
        /// Erstellt und initialisiert einen Abfragekontext für den angegebenen Entitätstyp T.
        /// </summary>
        /// <typeparam name="T">Der Typ der Entität, für die die Abfrage erstellt werden soll.</typeparam>
        /// <param name="orgService">Die IOrganizationService-Instanz, die für die Kommunikation mit dem CRM-System verwendet wird.</param>
        /// <returns>Ein QueryContext-Objekt, das für die Erstellung und Ausführung von Abfragen verwendet werden kann.</returns>
        internal static QueryContext<T> Select<T>(this IOrganizationService orgService)
            where T : BaseEntity<T>
        {
            return new QueryContext<T>(orgService);
        }

        internal static void Update<T>(this IOrganizationService orgService, T entity)
            where T : BaseEntity<T>
        {
            orgService.Update(entity.Entity);
        }

        internal static void Update<T>(this IOrganizationService orgService, T entity, Expression<Func<T, object[]>> properySelector)
            where T : BaseEntity<T>
        {
            var updateEntity = new Entity(entity.Entity.LogicalName, entity.Id);

            var properties = properySelector.GetPropertyInfos();
            foreach (var propertyInfo in properties)
            {
                var logicalName = propertyInfo.GetLogicalName();
                updateEntity.Attributes.Add(logicalName, entity.Entity.Attributes[logicalName]);
            }

            orgService.Update(updateEntity);
        }

        internal static TOutput SendRequest<TInput, TOutput>(this IOrganizationService orgService, BaseInputModel<TInput> input)
            where TInput : BaseInputModel<TInput>, new()
            where TOutput : class, new()
        {
            OrganizationRequest request = input.AsRequest();

            var response = orgService.Execute(request);
            if (response == null)
                return null;

            var result = new TOutput();

            foreach (var propertyInfo in typeof(TOutput).GetProperties())
            {
                var apiParamAttribute = propertyInfo.GetCustomAttribute<APIParameterAttribute>();
                if (apiParamAttribute == null)
                    continue;

                if (response.Results.ContainsKey(apiParamAttribute.Name))
                    propertyInfo.SetValue(result, Convert.ChangeType(response.Results[apiParamAttribute.Name], propertyInfo.PropertyType));
            }

            return result;
        }

        private static ColumnSet GetFromPropertySelector<T>(Expression<Func<T, object[]>> propertySelector)
        {
            ColumnSet columnSet = new ColumnSet();

            var properties = propertySelector.GetPropertyInfos();

            columnSet.AddColumns(properties
                .Select(prop => prop.GetLogicalName())
                .Where(name => name != null)
                .ToArray());

            return columnSet;
        }

        private static ColumnSet GetColumnSet<T>(Columns columns) where T : BaseEntity<T>
        {
            switch (columns)
            {
                case Columns.DefinedOnly:
                    return new ColumnSet(typeof(T).GetProperties()
                        .Where(prop => prop.GetCustomAttribute<LogicalNameAttribute>() != null)
                        .Select(prop => prop.GetCustomAttribute<LogicalNameAttribute>().Name)
                        .ToArray());

                case Columns.All:
                    return new ColumnSet(true);

                default:
                    return new ColumnSet(false);
            }
        }
    }

}
