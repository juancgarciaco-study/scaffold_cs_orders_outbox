// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace app.api.Infrastructure.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> modelBuilder)
    {

        modelBuilder.HasKey(ent => ent.Id);

        modelBuilder.Property(ent => ent.Id).ValueGeneratedOnAdd();

        //  // only for concrete types
        // modelBuilder.OwnsOne(p => p.JsonContent, content => {content.ToJson(); })

        modelBuilder.Property(ent => ent.JsonContent).HasColumnType("jsonb");


    }
}
