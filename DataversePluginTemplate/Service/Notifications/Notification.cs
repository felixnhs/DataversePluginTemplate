using Microsoft.Xrm.Sdk;
using System;

namespace DataversePluginTemplate.Service.Notification
{
    internal sealed class Notification
    {
        private const string SYSTEMUSER_LOGICALNAME = "systemuser";
        private const string NOTIFICATIONID = "NotificationId";

        private readonly PluginContext _context;

        private readonly NotificationRequest _request;


        private Notification(PluginContext context, EntityReference userER)
        {
            _context = context;
            _request = new NotificationRequest(userER);
        }

        public static Notification Create(PluginContext context, EntityReference empfaenger)
        {
            return new Notification(context, empfaenger)
                .AddIcon(NotificationIcon.Info)
                .SetNotificationType(NotificationType.Timed);
        }

        public static Notification Create(PluginContext context, Guid userId)
        {
            return Create(context, new EntityReference(SYSTEMUSER_LOGICALNAME, userId));
        }
        

        public Notification AddTitle(string title)
        {
            _request.Title = title;
            return this;
        }

        public Notification AddMessage(string message)
        {
            _request.Message = message;
            return this;
        }

        public Notification AddIcon(NotificationIcon icon)
        {
            _request.Icon = new OptionSetValue((int)icon);
            return this;
        }

        public Notification SetNotificationType(NotificationType type)
        {
            _request.NotificationType = new OptionSetValue((int)type);
            return this;
        }


        public Guid? Send(IOrganizationService organizationService)
        {
            var response = organizationService.Execute(_request.Request);
            if (!response.Results.Contains(NOTIFICATIONID))
                return null;

            return (Guid)response.Results[NOTIFICATIONID];
        }

        public Guid? Send() => Send(_context.OrgService);

        private sealed class NotificationRequest
        {
            private const string REQUEST_NAME = "SendAppNotification";
            private const string TITLE = "Title";
            private const string MESSAGE = "Body";
            private const string RECIPIENT = "Recipient";
            private const string ICONTYPE = "IconType";
            private const string NOTIFICATIONTYPE = "ToastType";
            private const string ACTIONS = "Actions";

            private readonly OrganizationRequest _request;

            public OrganizationRequest Request => _request;

            public string Title
            {
                get
                {
                    if (_request.Parameters.ContainsKey(TITLE))
                        return (string)_request.Parameters[TITLE];

                    return null;
                }
                set
                {
                    _request.Parameters[TITLE] = value;
                }
            }

            public string Message
            {
                get
                {
                    if (_request.Parameters.ContainsKey(MESSAGE))
                        return (string)_request.Parameters[MESSAGE];

                    return null;
                }
                set
                {
                    _request.Parameters[MESSAGE] = value;
                }
            }

            public EntityReference Empfaenger
            {
                get
                {
                    if (_request.Parameters.ContainsKey(RECIPIENT))
                        return (EntityReference)_request.Parameters[RECIPIENT];

                    return null;
                }
                set
                {
                    _request.Parameters[RECIPIENT] = value;
                }
            }

            public OptionSetValue Icon
            {
                get
                {
                    if (_request.Parameters.ContainsKey(ICONTYPE))
                        return (OptionSetValue)_request.Parameters[ICONTYPE];

                    return null;
                }
                set
                {
                    _request.Parameters[ICONTYPE] = value;
                }
            }

            public OptionSetValue NotificationType 
                {
                get
                {
                    if (_request.Parameters.ContainsKey(NOTIFICATIONTYPE))
                        return (OptionSetValue)_request.Parameters[NOTIFICATIONTYPE];

                    return null;
                }
                set
                {
                    _request.Parameters[NOTIFICATIONTYPE] = value;
                }
            }


            public NotificationRequest()
            {
                _request = new OrganizationRequest()
                {
                    RequestName = REQUEST_NAME
                };
            }

            public NotificationRequest(EntityReference recipient) : this()
            {
                Empfaenger = recipient;
            }
        }
    }
}
