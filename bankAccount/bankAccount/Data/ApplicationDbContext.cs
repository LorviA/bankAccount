using bankAccount.Models;
using Microsoft.EntityFrameworkCore;

namespace bankAccount.Data
{
    public class ApplicationDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("Accounts");
            modelBuilder.Entity<Transaction>().ToTable("Transactions");

            modelBuilder.Entity<Account>()
                .Property(a => a.Version)
                .IsRowVersion()
                .IsConcurrencyToken();

            modelBuilder.Entity<Account>()
               .HasIndex(a => a.OwnerId)
                .HasMethod("HASH");

           modelBuilder.Entity<Transaction>()
                .HasIndex(t => new { t.AccountId, t.Time });

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.Time)
                .HasMethod("GIST");
        }

    }
}
