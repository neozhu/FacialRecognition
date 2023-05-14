// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.GetById;

    public class GetVisitorByIdQuery :FilterBase, ICacheableRequest<VisitorDto>
    {
       [OperatorComparison(OperatorType.Equal)]
       public required int Id { get; set; }
       [IgnoreFilter]
       public string CacheKey => VisitorCacheKey.GetByIdCacheKey($"{Id}");
       [IgnoreFilter]
       public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
    }
    
    public class GetVisitorByIdQueryHandler :
         IRequestHandler<GetVisitorByIdQuery, VisitorDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetVisitorByIdQueryHandler> _localizer;

        public GetVisitorByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetVisitorByIdQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<VisitorDto> Handle(GetVisitorByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Visitors.ApplyFilter(request)
                         .ProjectTo<VisitorDto>(_mapper.ConfigurationProvider)
                         .FirstAsync(cancellationToken) ?? throw new NotFoundException($"Visitor with id: [{request.Id}] not found.");;
            return data;
        }
    }


