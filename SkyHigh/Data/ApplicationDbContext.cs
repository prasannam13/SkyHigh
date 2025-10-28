using Microsoft.EntityFrameworkCore;

using SkyHigh.Models;

using SkyHigh.Models;



namespace SkyHigh.Data

{

    public class ApplicationDbContext : DbContext

    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)

            : base(options) { }



        public DbSet<User> Users { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Passenger> Passengers { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Seat> Seats { get; set; }


        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  Handle SQLite string mapping if needed
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entity.GetProperties())
                    {
                        if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
                        {
                            property.SetColumnType("TEXT");
                        }
                    }
                }
            }

            //  Explicit table name 
            modelBuilder.Entity<ContactMessage>().ToTable("ContactMessages");
        }
    }
}
