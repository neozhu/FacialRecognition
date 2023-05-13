using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Domain.Entities
{
    public class Visitor:BaseAuditableEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Interviewee { get; set; }
        public string? Description { get; set; }
        public string? PurposeOfVisit { get; set; }
        public string? Documentation { get; set; }
        public DateTime? DateOfVisit { get; set; }
        public VisitStatus Status { get; set; } = VisitStatus.New;
        public List<Photo>? Photos { get; set; } = null!;
        public List<VisitHistory>? VisitHistories { get; set; } = null!;
    }
    
    public class Photo
    {
        public required string Name { get; set; }
        public decimal Size { get; set; }
        public required string Url { get; set; }
    }
    public class VisitHistory
    {
        public DateTime? CheckDateTime { get; set; }
        public string? RecognitionResult { get; set; }
        public string? TakePhoto { get; set; }
        public string? Tracking { get; set; }
        public string? Signature { get; set; }
    }
}
