// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Update;

    public class UpdateVisitorCommand: IMapFrom<VisitorDto>,ICacheInvalidatorRequest<Result<int>>
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

    public string CacheKey => VisitorCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VisitorCacheKey.SharedExpiryTokenSource();
    }

    public class UpdateVisitorCommandHandler : IRequestHandler<UpdateVisitorCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<UpdateVisitorCommandHandler> _localizer;
        public UpdateVisitorCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<UpdateVisitorCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(UpdateVisitorCommand request, CancellationToken cancellationToken)
        {
           // TODO: Implement UpdateVisitorCommandHandler method 
           var item =await _context.Visitors.FindAsync( new object[] { request.Id }, cancellationToken)?? throw new NotFoundException($"Visitor with id: [{request.Id}] not found.");;
           var dto = _mapper.Map<VisitorDto>(request);
           item = _mapper.Map(dto, item);
		    // raise a update domain event
		   item.AddDomainEvent(new VisitorUpdatedEvent(item));
           await _context.SaveChangesAsync(cancellationToken);
           return await Result<int>.SuccessAsync(item.Id);
        }
    }

