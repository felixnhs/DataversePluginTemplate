﻿using DataversePluginTemplate.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using System;

namespace DataversePluginTemplate.Service
{
    public sealed class PluginContext
    {
        private readonly IServiceProvider _serviceProvider;

        internal IPluginExecutionContext ExecutionContext { get; }
        internal ITracingService TracingService { get; }
        internal IServiceEndpointNotificationService NotificationService { get; }
        internal ILogger Logger { get; }
        internal IOrganizationService PluginUserService { get; }
        /// <summary>
        /// Alias für <see cref="PluginUserService"/>
        /// </summary>
        internal IOrganizationService OrgService => PluginUserService;
        internal IOrganizationService InitiatinUserService { get; }

        internal PluginStage PluginStage { get; }
        internal PluginExecutionMode ExecutionMode { get; }

        internal PluginContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new InvalidPluginExecutionException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;

            ExecutionContext = _serviceProvider.GetService<IPluginExecutionContext>();
            NotificationService = _serviceProvider.GetService<IServiceEndpointNotificationService>();
            TracingService = _serviceProvider.GetService<ITracingService>();
            Logger = _serviceProvider.GetService<ILogger>();
            PluginUserService = _serviceProvider.GetOrganizationService(ExecutionContext.UserId);
            InitiatinUserService = _serviceProvider.GetOrganizationService(ExecutionContext.InitiatingUserId);

            PluginStage = (PluginStage)ExecutionContext.Stage;
            ExecutionMode = (PluginExecutionMode)ExecutionContext.Mode;
        }

        internal PluginContext(IServiceProvider serviceProvider, Action<PluginContext> configureContext) : this(serviceProvider)
        {
            configureContext?.Invoke(this); // TODO: Debuggen
        }
    }
}
