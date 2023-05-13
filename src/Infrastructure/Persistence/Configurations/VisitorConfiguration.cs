// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class VisitorConfiguration : IEntityTypeConfiguration<Visitor>
{
    public void Configure(EntityTypeBuilder<Visitor> builder)
    {
        builder.Property(e => e.Photos)
            .HasConversion(
                v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
                v => JsonSerializer.Deserialize<List<Photo>>(v, DefaultJsonSerializerOptions.Options),
                new ValueComparer<List<Photo>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
        builder.Property(e => e.VisitHistories)
           .HasConversion(
               v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
               v => JsonSerializer.Deserialize<List<VisitHistory>>(v, DefaultJsonSerializerOptions.Options),
               new ValueComparer<List<VisitHistory>>(
                   (c1, c2) => c1.SequenceEqual(c2),
                   c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                   c => c.ToList()));
        //builder.OwnsMany(x => x.Photos, builder => { builder.ToJson(); });
        //builder.OwnsMany(x => x.VisitHistories, builder => { builder.ToJson(); });
        builder.Property(t => t.Status).HasConversion<string>();
        builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
        builder.Ignore(e => e.DomainEvents);
    }
}


