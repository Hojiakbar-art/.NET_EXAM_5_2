using Application.Identity;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000); 

        builder.Property(p => p.Price)
            .HasPrecision(18,2)
            .IsRequired();


        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);


        builder.HasIndex(p => new { p.CategoryId, p.Name })
            .IsUnique();

        

        
    }
}