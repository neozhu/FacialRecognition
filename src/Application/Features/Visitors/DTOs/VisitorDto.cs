// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Enums;
using System.ComponentModel;
namespace CleanArchitecture.Blazor.Application.Features.Visitors.DTOs;

[Description("Visitors")]
public class VisitorDto:IMapFrom<Visitor>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Visitor, VisitorDto>().ReverseMap();
    }
     [Description("Id")]
    public int Id {get;set;} 
    [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Interviewee")]
    public string? Interviewee {get;set;} 
    [Description("Comments")]
    public string? Description {get;set;} 
    [Description("Purpose Of Visit")]
    public string? PurposeOfVisit {get;set;} 
    [Description("Documentation")]
    public string? Documentation {get;set;} 
    [Description("Date Of Visit")]
    public DateTime? DateOfVisit {get;set;}
    [Description("Status")]
    public VisitStatus Status { get; set; } = VisitStatus.New;
    [Description("Photo")]
    public List<Photo>? Photos { get; set; }
    [Description("VisitHistories")]
    public List<VisitHistory>? VisitHistories { get; set; }

    public string? ExternalId { get; set; }

}

