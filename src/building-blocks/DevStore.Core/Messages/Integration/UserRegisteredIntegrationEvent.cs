using System;

namespace DevStore.Core.Messages.Integration
{
    public class UserRegisteredIntegrationEvent(Guid id, string name, string email, string socialNumber) : IntegrationEvent
    {
        public Guid Id { get; private set; } = id;
        public string Name { get; private set; } = name;
        public string Email { get; private set; } = email;
        public string SocialNumber { get; private set; } = socialNumber;
    }
}