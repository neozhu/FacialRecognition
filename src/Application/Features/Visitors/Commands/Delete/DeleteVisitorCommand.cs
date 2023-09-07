// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using System.Net.Http;
using CleanArchitecture.Blazor.Application.Services.CompreFace;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Delete;

public class DeleteVisitorCommand : FilterBase, ICacheInvalidatorRequest<Result<int>>
{
    [ArraySearchFilter()]
    public int[] Id { get; }
    public string CacheKey => VisitorCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    public DeleteVisitorCommand(int[] id)
    {
        Id = id;
    }
}

public class DeleteVisitorCommandHandler :
             IRequestHandler<DeleteVisitorCommand, Result<int>>

{
    private readonly CompreFaceService _compreFaceService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<DeleteVisitorCommandHandler> _localizer;
    public DeleteVisitorCommandHandler(
        CompreFaceService compreFaceService,
        IApplicationDbContext context,
        IStringLocalizer<DeleteVisitorCommandHandler> localizer,
         IMapper mapper
        )
    {
        _compreFaceService = compreFaceService;
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }
    public async Task<Result<int>> Handle(DeleteVisitorCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Visitors.ApplyFilter(request).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            // raise a delete domain event
            item.AddDomainEvent(new VisitorDeletedEvent(item));
            await deletePhoto(item.Name);
            _context.Visitors.Remove(item);
        }
        var result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }

    private async Task deletePhoto(string subject)
    {
        try
        {
            var result = await _compreFaceService.DeleteCollection(new Exadel.Compreface.DTOs.FaceCollectionDTOs.DeleteAllSubjectExamples.DeleteAllExamplesRequest() { Subject= subject });

        }
        catch (Exception e)
        {
            
        }

    }
}


