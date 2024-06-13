using DataversePluginTemplate.Queries;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace DataversePluginTemplate.Service
{
    internal static class OrganizationServiceExtensionMethods
    {
        internal static OrganizationResponse ExecuteMultiple<TRequest>(this IOrganizationService orgService, IEnumerable<TRequest> requests, Action<ExecuteMultipleRequest> configureRequest = null)
            where TRequest : OrganizationRequest
        {
            var executeMultiRequest = new ExecuteMultipleRequest();
            executeMultiRequest.Settings = new ExecuteMultipleSettings();
            configureRequest?.Invoke(executeMultiRequest);

            foreach (var request in requests)
                executeMultiRequest.Requests.Add(request);

            return orgService.Execute(executeMultiRequest);
        }

        internal static OrganizationResponse ExecuteMultiple<TRequest, TItem>(this IOrganizationService orgService, IEnumerable<TItem> requestItems, Action<TRequest, TItem> configureRequestItem, Action<ExecuteMultipleRequest> configureRequest = null)
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
    

        internal static OrganizationResponse CreateMultiple(this IOrganizationService orgService, IEnumerable<Entity> entities, Action<ExecuteMultipleRequest> configureRequest = null)
        {
            return orgService.ExecuteMultiple<CreateRequest, Entity>(entities, (request, entity) =>
            {
                request.Target = entity;
            }, configureRequest);
        }

        internal static OrganizationResponse CreateMultiple(this IOrganizationService orgService, IEnumerable<Entity> entities)
        {
            return orgService.CreateMultiple(entities, configure =>
            {
                configure.Settings.ContinueOnError = true;
                configure.Settings.ReturnResponses = true;
            });
        }

        internal static OrganizationResponse UpdateMultiple(this IOrganizationService orgService, IEnumerable<Entity> entities, Action<ExecuteMultipleRequest> configureRequest = null)
        {
            return orgService.ExecuteMultiple<UpdateRequest, Entity>(entities, (request, entity) =>
            {
                request.Target = entity;
            }, configureRequest);
        }

        internal static OrganizationResponse UpdateMultiple(this IOrganizationService orgService, IEnumerable<Entity> entities)
        {
            return orgService.UpdateMultiple(entities, configure =>
            {
                configure.Settings.ContinueOnError = true;
                configure.Settings.ReturnResponses = true;
            });
        }

        internal static OrganizationResponse DeleteMultiple(this IOrganizationService orgService, IEnumerable<EntityReference> entityReferences, Action<ExecuteMultipleRequest> configureRequest = null)
        {
            return orgService.ExecuteMultiple<DeleteRequest, EntityReference>(entityReferences, (request, entityReference) =>
            {
                request.Target = entityReference;
            }, configureRequest);
        }

        internal static OrganizationResponse DeleteMultiple(this IOrganizationService orgService, IEnumerable<EntityReference> entityReferences)
        {
            return orgService.DeleteMultiple(entityReferences, configure =>
            {
                configure.Settings.ContinueOnError = true;
                configure.Settings.ReturnResponses = true;
            });
        }
    


        internal static Entity Retrieve(this IOrganizationService orgService, EntityReference entityReference, params string[] columns)
        {
            if (entityReference == null)
                return null;

            return orgService.Retrieve(entityReference.LogicalName, entityReference.Id, new ColumnSet(columns));
        }

        internal static Entity Retrieve(this IOrganizationService orgService, Entity entity, params string[] columns)
        {
            if (entity == null)
                return null;

            return orgService.Retrieve(entity.ToEntityReference(), columns);
        }

        internal static Entity Retrieve(this IOrganizationService orgService, EntityReference entityReference, bool allColumns = true)
        {
            if (entityReference == null)
                return null;

            return orgService.Retrieve(entityReference.LogicalName, entityReference.Id, new ColumnSet(allColumns));
        }

        internal static Entity Retrieve(this IOrganizationService orgService, Entity entity, bool allColumns = true)
        {
            if (entity == null)
                return null;

            return orgService.Retrieve(entity.ToEntityReference(), allColumns);
        }


        internal static QueryContext Select(this IOrganizationService orgService, string entityName)
        {
            return new QueryContext(orgService, entityName);
        }
    }
}
