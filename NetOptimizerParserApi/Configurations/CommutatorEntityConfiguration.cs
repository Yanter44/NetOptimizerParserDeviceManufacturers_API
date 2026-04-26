using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetOptimizerParserApi.Models.DbEntities;

namespace NetOptimizerParserApi.Configurations
{
    public class CommutatorEntityConfiguration : IEntityTypeConfiguration<CommutatorEntity>
    {
        public void Configure(EntityTypeBuilder<CommutatorEntity> builder)
        {

            builder.HasKey(c => c.Id);

            builder.OwnsMany(c => c.Ports, portBuilder =>
            {
                portBuilder.ToJson();
                portBuilder.Property(p => p.Type).HasConversion<string>();

            });

            builder.Property(c => c.Model).IsRequired().HasMaxLength(255);

            builder.Property(c => c.Vendor).IsRequired().HasMaxLength(100);

            builder.Property(c => c.AveragePrice).HasPrecision(18, 2);

            builder.Property(c => c.SwitchRoleType).HasConversion<string>();
        }
    }
}
