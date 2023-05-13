// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Visitors.EventHandlers;

    public class VisitorUpdatedEventHandler : INotificationHandler<VisitorUpdatedEvent>
    {
        private readonly ILogger<VisitorUpdatedEventHandler> _logger;

        public VisitorUpdatedEventHandler(
            ILogger<VisitorUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(VisitorUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
