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
using SixLabors.ImageSharp.Formats.Jpeg;
using Flurl;
using Path = System.IO.Path;
using CleanArchitecture.Blazor.Application.Services.CompreFace;
using Exadel.Compreface.DTOs.RecognizeFaceFromImageDTOs.RecognizeFaceFromImage;
using Exadel.Compreface.DTOs.FaceDetectionDTOs.FaceDetection;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.GetById;

public class SearchVisitorByImage : IRequest<Result<VisitorDto>>
{
    public string ImageDataString { get; set; }

}

public class SearchVisitorByImageHandler :
     IRequestHandler<SearchVisitorByImage, Result<VisitorDto>>
{
    private readonly CompreFaceService _compreFaceService;
    private readonly IUploadService _uploadService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<SearchVisitorByImageHandler> _logger;
    private readonly IStringLocalizer<SearchVisitorByImageHandler> _localizer;

    public SearchVisitorByImageHandler(
         CompreFaceService compreFaceService,
        IUploadService uploadService,
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<SearchVisitorByImageHandler> logger,
        IStringLocalizer<SearchVisitorByImageHandler> localizer
        )
    {
        _compreFaceService = compreFaceService;
        _uploadService = uploadService;
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<Result<VisitorDto>> Handle(SearchVisitorByImage request, CancellationToken cancellationToken)
    {

        var name = await searchFace(request.ImageDataString);
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
            try
            {
                try
                {
                    var detRes = await _compreFaceService.DetectFace(new FaceDetectionRequestByBytes() { DetProbThreshold = 0.5m, ImageInBytes = imageData });

                    if (detRes.Result.Any())
                    {
                        var response = await _compreFaceService.RecognizeFace(new RecognizeFaceFromImageRequestByBytes() { DetProbThreshold = 0.5m, ImageInBytes = imageData });
                        var username = response.Result.First().Subjects.First().Subject;
                        return username;
                    }
                }
                catch
                {

                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "RecognizeFace Error");
            }
            
            return string.Empty;

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


