﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Domain.Events;
using Helpers.Mapping;
using SecureChat.Common.Events.EventBus.Events;

namespace Users.API.Application.IntegrationEvents.Events
{
    public class UserNameUpdatedIntegrationEvent : IntegrationEvent, IMapFrom<UserNameUpdatedIntegrationEvent>
    {
        public string UserId { get; }
        public string NewUserName { get; }

        public UserNameUpdatedIntegrationEvent(string userId, string newUserName)
        {
            UserId = userId;
            NewUserName = newUserName;
        }
    }
}
