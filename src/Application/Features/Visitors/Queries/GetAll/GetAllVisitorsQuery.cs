﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.GetAll;

    public class GetAllVisitorsQuery : ICacheableRequest<IEnumerable<VisitorDto>>
    {
       [IgnoreFilter]
       public string CacheKey => VisitorCacheKey.GetAllCacheKey;
       [IgnoreFilter]
       public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
    }
    
    public class GetAllVisitorsQueryHandler :
         IRequestHandler<GetAllVisitorsQuery, IEnumerable<VisitorDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetAllVisitorsQueryHandler> _localizer;

        public GetAllVisitorsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllVisitorsQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IEnumerable<VisitorDto>> Handle(GetAllVisitorsQuery request, CancellationToken cancellationToken)
        {
             var data = await _context.Visitors
                         .ProjectTo<VisitorDto>(_mapper.ConfigurationProvider)
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
            return data;
        }
    }


