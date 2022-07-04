using Lessons.ClientCardApi.Abstraction.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lessons.ClientCardApi.Data.AuthContext.AuthDbContext
{
    public sealed class AuthContext : IdentityDbContext<UserAuth>
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}