﻿using DataversePluginTemplate.Service.API;
using Microsoft.Xrm.Sdk;
using System;
using System.Reflection;

namespace DataversePluginTemplate.Service.Extensions
{
    public static class InputExtensionMethods
    {
        public static bool IsValid(this Guid guid) => guid != null && guid != default && guid != Guid.Empty;
        public static bool IsValid(this Guid? guid) => guid != null && guid.HasValue && guid.Value != default && guid.Value != Guid.Empty;

        public static bool IsValid(this DateTime dateTime) => dateTime != null && dateTime != default;
        public static bool IsValid(this DateTime? dateTime) => dateTime != null && dateTime.HasValue && dateTime.Value != default;


        public static DateTime GetUtcTime(this DateTime dateTime)
        {
            var specifiedLocalTime = dateTime.Kind != DateTimeKind.Local
                ? DateTime.SpecifyKind(dateTime, DateTimeKind.Local)
                : dateTime;

            return specifiedLocalTime.ToUniversalTime();
        }

        public static OrganizationRequest AsRequest<T>(this BaseInputModel<T> inputModel)
            where T : BaseInputModel<T>, new()
        {
            var requestAttribute = typeof(T).GetCustomAttribute<RequestAttribute>();
            if (requestAttribute == null)
                throw new Exception("Request Model has no request name attribute configured");

            var request = new OrganizationRequest(requestAttribute.Name);

            foreach (var property in typeof(T).GetProperties())
            {
                var apiAttribute = property.GetCustomAttribute<APIParameterAttribute>();
                if (apiAttribute == null)
                    continue;

                request.Parameters[apiAttribute.Name] = property.GetValue(inputModel);
            }

            return request;
        }
    }
}
