using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CSVMetrics.Domain.Entities;

namespace CSVMetrics.Infrastructure;

public class MeasurementValueConfiguration : IEntityTypeConfiguration<MeasurementValue>
{
    public void Configure(EntityTypeBuilder<MeasurementValue> builder)
    {
        builder.ToTable("Values");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(260); //Win max path length
        builder.HasIndex(x => new { x.FileName, x.Date }); //Kapec cool tema, srazu sorted
    }
}