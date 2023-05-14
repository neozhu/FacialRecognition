// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using static System.Net.Mime.MediaTypeNames;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Formats.Png;
using MimeKit;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using DocumentFormat.OpenXml.Drawing;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.GetById;

public class SearchVisitorByImage : IRequest<Result<VisitorDto>>
{
    public string ImageDataString { get; set; }

}

public class SearchVisitorByImageHandler :
     IRequestHandler<SearchVisitorByImage, Result<VisitorDto>>
{
    private readonly IUploadService _uploadService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<SearchVisitorByImageHandler> _logger;
    private readonly IStringLocalizer<SearchVisitorByImageHandler> _localizer;

    public SearchVisitorByImageHandler(
        IUploadService uploadService,
        IHttpClientFactory httpClientFactory,
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<SearchVisitorByImageHandler> logger,
        IStringLocalizer<SearchVisitorByImageHandler> localizer
        )
    {
        _uploadService = uploadService;
        _httpClientFactory = httpClientFactory;
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<Result<VisitorDto>> Handle(SearchVisitorByImage request, CancellationToken cancellationToken)
    {

        var name =await searchFace(request.ImageDataString);
        if (string.IsNullOrEmpty(name))
        {
            return await Result<VisitorDto>.FailureAsync(new string[] { "No record was matched" });
        }
        var data = await _context.Visitors.Where(x => x.Name.ToLower() == name)
                     .ProjectTo<VisitorDto>(_mapper.ConfigurationProvider)
                     .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"Visitor name:{name} not found."); ;
        return await Result<VisitorDto>.SuccessAsync(data);
    }
    private async Task<string?> searchFace(string imagedatastr)
    {
        try
        {
            byte[] imageData = Convert.FromBase64String(imagedatastr.Split(',')[1]);
            using (var outstream = new MemoryStream()) {
                using (var image = Image.Load(imageData))
                {
                    image.Mutate(x => x
                        .Flip(FlipMode.Horizontal) //To match mirrored webcam image
                    );
                    image.Save(outstream, PngFormat.Instance);
                }
                var filename = $"{Guid.NewGuid()}.jpg";
                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(outstream.ToArray()), "file", filename);
                using (var client = _httpClientFactory.CreateClient("Insightface"))
                {
                    try
                    {
                        var queryString = new Dictionary<string, string> { { "limit", "1" } };
                    var requestpara =await (new FormUrlEncodedContent(queryString)).ReadAsStringAsync();
                    var httpResponseMessage = await client.PostAsync("/face-search?" + requestpara, content);
                    //httpResponseMessage.EnsureSuccessStatusCode();
                   
                        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                        _logger.LogInformation(responseContent);
                        var result = JsonSerializer.Deserialize<facesearchResponse>(responseContent);
                        if (result.status_code == 200)
                        {
                            return result.result.similar_faces.FirstOrDefault()?.name;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }catch(Exception e)
                    {
                        _logger.LogError(e, "searchFace error");
                        return string.Empty;
                    }
                    
                }
            }

           

            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "searchFace error");
            return string.Empty;
        }
    }
    internal class facesearchResponse
    {
        public int status_code { get; set; }    
        public facesearchResult result { get; set; }
    }
    internal class facesearchResult
    {
        public List<similar_face>? similar_faces { get; set; }
    }
    internal class similar_face
    {
        public int id { get; set; }
        public string name { get; set; }

    }
}


