// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using SixLabors.ImageSharp.Formats.Png;
using static CleanArchitecture.Blazor.Application.Features.Visitors.Queries.GetById.SearchVisitorByImageHandler;
using System.Net.Http;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Update;

public class UpdateVisitorCommand : IMapFrom<VisitorDto>, ICacheInvalidatorRequest<Result<int>>
{

    public int Id { get; set; }
    public string ImageDataString { get; set; }

    public string CacheKey => VisitorCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
}

public class UpdateVisitorCommandHandler : IRequestHandler<UpdateVisitorCommand, Result<int>>
{
    private readonly IUploadService _uploadService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<UpdateVisitorCommandHandler> _localizer;
    public UpdateVisitorCommandHandler(
        IUploadService uploadService,
        IApplicationDbContext context,
        IStringLocalizer<UpdateVisitorCommandHandler> localizer,
         IMapper mapper
        )
    {
        _uploadService = uploadService;
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }
    public async Task<Result<int>> Handle(UpdateVisitorCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Visitors.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"Visitor with id: [{request.Id}] not found."); ;
        var saved =await saveImage(request.ImageDataString);
        if(item.VisitHistories is null)
        {
            item.VisitHistories = new List<VisitHistory>();
        }
        item.VisitHistories.Add(new VisitHistory() { CheckDateTime = DateTime.Now, Signature = item.Name, TakePhoto = saved, Tracking = "Checked" });
        // raise a update domain event
        item.AddDomainEvent(new VisitorUpdatedEvent(item));
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }

    private async Task<string> saveImage(string imagedatastr)
    {
        byte[] imageData = Convert.FromBase64String(imagedatastr.Split(',')[1]);
        using (var outstream = new MemoryStream())
        {
            using (var image = Image.Load(imageData))
            {
                image.Mutate(x => x
                    .Flip(FlipMode.Horizontal) //To match mirrored webcam image
                );
                image.Save(outstream, PngFormat.Instance);
            }
            var filename = $"{Guid.NewGuid()}.jpg";
            var result = await _uploadService.UploadAsync(new UploadRequest(filename, UploadType.Photos,outstream.ToArray()));
            return result;

        }
   
    }
}

