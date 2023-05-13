// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Create;

    public class CreateVisitorCommand: IMapFrom<VisitorDto>,ICacheInvalidatorRequest<Result<int>>
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
    
    public class CreateVisitorCommandHandler : IRequestHandler<CreateVisitorCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CreateVisitorCommand> _localizer;
        public CreateVisitorCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateVisitorCommand> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateVisitorCommand request, CancellationToken cancellationToken)
        {
           // TODO: Implement CreateVisitorCommandHandler method 
           var dto = _mapper.Map<VisitorDto>(request);
           var item = _mapper.Map<Visitor>(dto);
           // raise a create domain event
	       item.AddDomainEvent(new VisitorCreatedEvent(item));
           _context.Visitors.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

