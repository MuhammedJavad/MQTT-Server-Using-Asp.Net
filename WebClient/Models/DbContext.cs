using Microsoft.EntityFrameworkCore;

namespace WebClient.Models
{
    public class LampContext : DbContext
    {
        public LampContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var lampBuilder = builder.Entity<LampSourcing>();
                lampBuilder.Property(sourcing => sourcing.Status).IsRequired();
                lampBuilder.Property(sourcing => sourcing.Time).IsRequired();
                lampBuilder.HasKey(sourcing => sourcing.Id);
                
            base.OnModelCreating(builder);
        }
        
        public DbSet<LampSourcing> Lamp { get; set; }
    }
}