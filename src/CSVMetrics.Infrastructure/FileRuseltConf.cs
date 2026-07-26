using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CSVMetrics.Domain.Entities;

namespace CSVMetrics.Infrastructure;

public class FileResultConf : IEntityTypeConfiguration<FileResult>
{
    public void Configure(EntityTypeBuilder<FileResult> builder)
    {
        builder.ToTable("Results");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(260);
        builder.HasIndex(x => x.FileName).IsUnique();
        builder.HasIndex(x => x.StartDate);
        builder.HasIndex(x => x.AvgValue);
        builder.HasIndex(x => x.AvgExecutionTime);
    }
}