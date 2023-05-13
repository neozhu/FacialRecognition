// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Visitors.EventHandlers;

    public class VisitorDeletedEventHandler : INotificationHandler<VisitorDeletedEvent>
    {
        private readonly ILogger<VisitorDeletedEventHandler> _logger;

        public VisitorDeletedEventHandler(
            ILogger<VisitorDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(VisitorDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
