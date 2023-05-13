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
        public string? Description { get; set; }
        public string? PurposeOfVisit { get; set; }
        public string? Documentation { get; set; }
        public DateTime? DateOfVisit { get; set; }
        public DateTime? Checkin { get; set; }
        public string? CheckinResult { get; set; }
        public DateTime? Checkout { get; set; }
        public string? CheckoutResult { get; set; }
        public VisitStatus Status { get; set; } = VisitStatus.New;
        public List<Photo>? Photo { get; set; }
    }
    public enum VisitStatus { 
        New,
        Pending,
        Approved,
        Done
    }
    public class Photo
    {
        public required string Name { get; set; }
        public decimal Size { get; set; }
        public required string Url { get; set; }
    }
}
