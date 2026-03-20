using Microsoft.EntityFrameworkCore;
using NetOptimizerParserApi.Configurations;
using NetOptimizerParserApi.Models.DbEntities;

namespace NetOptimizerParserApi.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<CommutatorEntity> CommutatorsTable { get; set; }
        public DbSet<RouterEntity> RoutersTable { get; set; }
        public DbSet<PcEntity> PcTable { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
