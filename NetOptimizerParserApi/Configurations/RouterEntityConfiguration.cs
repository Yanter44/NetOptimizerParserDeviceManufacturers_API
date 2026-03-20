using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetOptimizerParserApi.Models.DbEntities;

namespace NetOptimizerParserApi.Configurations
{
    public class RouterEntityConfiguration : IEntityTypeConfiguration<RouterEntity>
    {
        public void Configure(EntityTypeBuilder<RouterEntity> builder)
        {
            builder.HasKey(r => r.Id);

            builder.OwnsMany(r => r.Ports, portBuilder =>
            {
                portBuilder.ToJson();
                portBuilder.Property(p => p.Type).HasConversion<string>();
            });

            builder.Property(r => r.Model).IsRequired().HasMaxLength(255);
            builder.Property(r => r.Vendor).IsRequired().HasMaxLength(100);
            builder.Property(r => r.AvveragePrice).HasPrecision(18, 2);
        }
    }
}
