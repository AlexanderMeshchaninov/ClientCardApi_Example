using Lessons.ClientCardApi.Abstraction.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Lessons.ClientCardApi.Data.Context.Context
{
    public sealed class ApplicationContext : DbContext
    {
        public DbSet<CreditCardInfoModel> CreditCardInfo { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Номер кредитной карты уникальный в бд
            modelBuilder.Entity<CreditCardInfoModel>()
                 .HasIndex(x => x.CreditCardNumber)
                 .IsUnique();
        }
    }
}