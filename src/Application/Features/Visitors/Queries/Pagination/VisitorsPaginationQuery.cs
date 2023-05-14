// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.Pagination;

public class VisitorsWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<VisitorDto>>
{
    [CompareTo("Name", "Description", "Interviewee", "PurposeOfVisit")] // <-- This filter will be applied to Name or Description.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? Keyword { get; set; }
    [CompareTo(typeof(SearchVisitorsWithListView), "Id")]
    public VisitorListView ListView { get; set; } = VisitorListView.All; //<-- When the user selects a different ListView,
                                                                               // a custom query expression is executed on the filter.
    public override string ToString()
    {
        return $"Listview:{ListView},Search:{Keyword},Sort:{Sort},SortBy:{SortBy},{Page},{PerPage}";
    }
    [IgnoreFilter]
    public string CacheKey => VisitorCacheKey.GetPaginationCacheKey($"{this}");
    [IgnoreFilter]
    public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
}
    
public class VisitorsWithPaginationQueryHandler :
         IRequestHandler<VisitorsWithPaginationQuery, PaginatedData<VisitorDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<VisitorsWithPaginationQueryHandler> _localizer;

        public VisitorsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<VisitorsWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<VisitorDto>> Handle(VisitorsWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.Visitors.ApplyFilterWithoutPagination(request)
                .ProjectTo<VisitorDto>(_mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.Page, request.PerPage);
            return data;
        }
}

public class VisitorsPaginationSpecification : Specification<Visitor>
{
    public VisitorsPaginationSpecification(VisitorsWithPaginationQuery query)
    {
        Criteria = q => q.Name != null;
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            And(x => x.Name.Contains(query.Keyword));
        }
       
    }
}
public class SearchVisitorsWithListView : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd",CultureInfo.CurrentCulture) + " 00:00:00", CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd",CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        var end30 = Convert.ToDateTime(today.AddDays(30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        var listview = (VisitorListView)value;
        return listview switch {
            VisitorListView.All => expressionBody,
            VisitorListView.CreatedToday => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "Created"), 
                                                                          Expression.Constant(start, typeof(DateTime?)))
                                            .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "Created"), 
                                                     Expression.Constant(end, typeof(DateTime?))), 
                                                     CombineType.And),
            VisitorListView.Created30Days => Expression.GreaterThanOrEqual(Expression.Property(expressionBody, "Created"), 
                                                                          Expression.Constant(start, typeof(DateTime?)))
                                            .Combine(Expression.LessThanOrEqual(Expression.Property(expressionBody, "Created"), 
                                                     Expression.Constant(end30, typeof(DateTime?))), 
                                                     CombineType.And),
            _=> expressionBody
        };
    }
}
public enum VisitorListView
{
    [Description("All")]
    All,
    [Description("Created Toady")]
    CreatedToday,
    [Description("Created within the last 30 days")]
    Created30Days
}