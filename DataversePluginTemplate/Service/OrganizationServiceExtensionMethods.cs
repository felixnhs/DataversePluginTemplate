using DataversePluginTemplate.Queries;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

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

    /// <summary>
    /// Ruft eine Entität aus dem OrganizationService ab, indem eine Entitätsreferenz und die angegebenen Spalten verwendet werden.
    /// </summary>
    /// <param name="orgService">Die IOrganizationService-Instanz.</param>
    /// <param name="entityReference">Die Referenz zur abzurufenden Entität.</param>
    /// <param name="columns">Ein Array von Spaltennamen, die abgerufen werden sollen.</param>
    /// <returns>Die abgerufene Entität oder null, wenn die Entitätsreferenz null ist.</returns>
    internal static Entity Retrieve(
        this IOrganizationService orgService, 
        EntityReference entityReference, 
        params string[] columns)
    {
        if (entityReference == null)
            return null;

        return orgService.Retrieve(entityReference.LogicalName, entityReference.Id, new ColumnSet(columns));
    }

    /// <summary>
    /// Ruft eine Entität aus dem OrganizationService ab, indem eine Entität und die angegebenen Spalten verwendet werden.
    /// </summary>
    /// <param name="orgService">Die IOrganizationService-Instanz.</param>
    /// <param name="entity">Die Entität, die abgerufen werden soll.</param>
    /// <param name="columns">Ein Array von Spaltennamen, die abgerufen werden sollen.</param>
    /// <returns>Die abgerufene Entität oder null, wenn die Entität null ist.</returns>
    internal static Entity Retrieve(
        this IOrganizationService orgService, 
        Entity entity, 
        params string[] columns)
    {
        if (entity == null)
            return null;

        return orgService.Retrieve(entity.ToEntityReference(), columns);
    }

    /// <summary>
    /// Ruft eine Entität aus dem OrganizationService ab, indem eine Entitätsreferenz und eine Option zum Abrufen aller Spalten verwendet werden.
    /// </summary>
    /// <param name="orgService">Die IOrganizationService-Instanz.</param>
    /// <param name="entityReference">Die Referenz zur abzurufenden Entität.</param>
    /// <param name="allColumns">Gibt an, ob alle Spalten abgerufen werden sollen.</param>
    /// <returns>Die abgerufene Entität oder null, wenn die Entitätsreferenz null ist.</returns>
    internal static Entity Retrieve(
        this IOrganizationService orgService, 
        EntityReference entityReference, 
        bool allColumns = true)
    {
        if (entityReference == null)
            return null;

        return orgService.Retrieve(entityReference.LogicalName, entityReference.Id, new ColumnSet(allColumns));
    }

    /// <summary>
    /// Ruft eine Entität aus dem OrganizationService ab, indem eine Entität und eine Option zum Abrufen aller Spalten verwendet werden.
    /// </summary>
    /// <param name="orgService">Die IOrganizationService-Instanz.</param>
    /// <param name="entity">Die Entität, die abgerufen werden soll.</param>
    /// <param name="allColumns">Gibt an, ob alle Spalten abgerufen werden sollen.</param>
    /// <returns>Die abgerufene Entität oder null, wenn die Entität null ist.</returns>
    internal static Entity Retrieve(
        this IOrganizationService orgService, 
        Entity entity, 
        bool allColumns = true)
    {
        if (entity == null)
            return null;

        return orgService.Retrieve(entity.ToEntityReference(), allColumns);
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
}

}
