// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using System.Net.Http;


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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<DeleteVisitorCommandHandler> _localizer;
    public DeleteVisitorCommandHandler(
         IHttpClientFactory httpClientFactory,
        IApplicationDbContext context,
        IStringLocalizer<DeleteVisitorCommandHandler> localizer,
         IMapper mapper
        )
    {
        _httpClientFactory = httpClientFactory;
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
            await deletePhoto(item.Photos.First().Url, item.Name);
            _context.Visitors.Remove(item);
        }
        var result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }

    private async Task<string> deletePhoto(string url, string name)
    {
        try
        {
            var photofile = Path.Combine(Directory.GetCurrentDirectory(), url);
            bool exists = File.Exists(photofile);
            if (exists)
            {
                var filename = Path.GetFileName(photofile);
                var fileContent = await File.ReadAllBytesAsync(photofile);
                File.Delete(photofile);
                using (var client = _httpClientFactory.CreateClient("Insightface"))
                {
                    var queryString = new Dictionary<string, string> { { "name", name.ToLower() } };
                    var requestpara = new FormUrlEncodedContent(queryString).ReadAsStringAsync().Result;
                    var httpResponseMessage = await client.DeleteAsync("/delete?" + requestpara);
                    var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                    return responseContent;
                }
               
            }
            return $"no exists photo:{url}";
        }
        catch (Exception e)
        {
            return e.Message;
        }

    }
}


