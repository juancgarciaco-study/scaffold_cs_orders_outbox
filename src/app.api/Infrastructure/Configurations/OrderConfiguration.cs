// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using app.api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace app.api.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> modelBuilder)
    {

        modelBuilder.HasKey(ent => ent.Id);
        modelBuilder.Property(ent => ent.Id).ValueGeneratedOnAdd();

        modelBuilder.Property(ent => ent.CustomerName).HasMaxLength(100).IsRequired();

    }
}
