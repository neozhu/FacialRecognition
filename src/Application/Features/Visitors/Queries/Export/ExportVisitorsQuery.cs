// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Queries.Pagination;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.Export;

public class ExportVisitorsQuery : OrderableFilterBase, IRequest<Result<byte[]>>
{
        [CompareTo("Name", "Description")] // <-- This filter will be applied to Name or Description.
        [StringFilterOptions(StringFilterOption.Contains)]
        public string? Keyword { get; set; }
        [CompareTo(typeof(SearchVisitorsWithListView), "Id")]
        public VisitorListView ListView { get; set; } = VisitorListView.All;
}
    
public class ExportVisitorsQueryHandler :
         IRequestHandler<ExportVisitorsQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportVisitorsQueryHandler> _localizer;
        private readonly VisitorDto _dto = new();
        public ExportVisitorsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportVisitorsQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }

        public async Task<Result<byte[]>> Handle(ExportVisitorsQuery request, CancellationToken cancellationToken)
        {
  
            var data = await _context.Visitors.ApplyOrder(request)
                       .ProjectTo<VisitorDto>(_mapper.ConfigurationProvider)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<VisitorDto, object?>>()
                {
                    {_localizer[_dto.GetMemberDescription(x=>x.Id)],item => item.Id}, 
{_localizer[_dto.GetMemberDescription(x=>x.Name)],item => item.Name},
{_localizer[_dto.GetMemberDescription(x=>x.Status)],item => item.Status},
{_localizer[_dto.GetMemberDescription(x=>x.Interviewee)],item => item.Interviewee}, 
{_localizer[_dto.GetMemberDescription(x=>x.Description)],item => item.Description}, 
{_localizer[_dto.GetMemberDescription(x=>x.PurposeOfVisit)],item => item.PurposeOfVisit}, 
{_localizer[_dto.GetMemberDescription(x=>x.Documentation)],item => item.Documentation}, 
{_localizer[_dto.GetMemberDescription(x=>x.DateOfVisit)],item => item.DateOfVisit},
{_localizer[_dto.GetMemberDescription(x=>x.Photos)],item => JsonSerializer.Serialize(item.Photos)},
{_localizer[_dto.GetMemberDescription(x=>x.VisitHistories)],item => JsonSerializer.Serialize(item.VisitHistories)},
                }
                , _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);;
        }
}
