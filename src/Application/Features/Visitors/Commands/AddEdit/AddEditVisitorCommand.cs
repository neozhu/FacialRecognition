// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.AddEdit;

public class AddEditVisitorCommand : IMapFrom<VisitorDto>, ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Name")]
    public string Name { get; set; } = String.Empty;
    [Description("Interviewee")]
    public string? Interviewee { get; set; }
    [Description("Description")]
    public string? Description { get; set; }
    [Description("Purpose Of Visit")]
    public string? PurposeOfVisit { get; set; }
    [Description("Documentation")]
    public string? Documentation { get; set; }
    [Description("Date Of Visit")]
    public DateTime? DateOfVisit { get; set; }

    [Description("Status")]
    public VisitStatus Status { get; set; } = VisitStatus.New;
    [Description("Photo")]
    public List<Photo>? Photos { get; set; }
    [Description("VisitHistories")]
    public List<VisitHistory>? VisitHistories { get; set; }

    public IReadOnlyList<IBrowserFile>? UploadPhotos { get; set; }
    public string CacheKey => VisitorCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
}

public class AddEditVisitorCommandHandler : IRequestHandler<AddEditVisitorCommand, Result<int>>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<AddEditVisitorCommandHandler> _localizer;
    public AddEditVisitorCommandHandler(
        IHttpClientFactory httpClientFactory,
        IApplicationDbContext context,
        IStringLocalizer<AddEditVisitorCommandHandler> localizer,
        IMapper mapper
        )
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }
    public async Task<Result<int>> Handle(AddEditVisitorCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement AddEditVisitorCommandHandler method 
        var dto = _mapper.Map<VisitorDto>(request);
        if (request.Id > 0)
        {
            var item = await _context.Visitors.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"Visitor with id: [{request.Id}] not found.");
            item = _mapper.Map(dto, item);
            // raise a update domain event
            item.AddDomainEvent(new VisitorUpdatedEvent(item));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = _mapper.Map<Visitor>(dto);
            var result = await uploadPhoto(dto.Photos.First().Url, dto.Name);
            // raise a create domain event
            item.Description = result;
            item.AddDomainEvent(new VisitorCreatedEvent(item));
            _context.Visitors.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }

    }
    private async Task<string> uploadPhoto(string url, string name)
    {
        try
        {
            var photofile = Path.Combine(Directory.GetCurrentDirectory(), url);
            bool exists = File.Exists(photofile);
            if (exists)
            {
                var filename = Path.GetFileName(photofile);
                var fileContent = await File.ReadAllBytesAsync(photofile);
                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(fileContent), "file", filename);
                using (var client = _httpClientFactory.CreateClient("Insightface"))
                {
                    var queryString = new Dictionary<string, string> { { "name", name } };
                    var requestpara = new FormUrlEncodedContent(queryString).ReadAsStringAsync().Result;
                    var httpResponseMessage = await client.PostAsync("/add?" + requestpara, content);
                    //httpResponseMessage.EnsureSuccessStatusCode();
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

