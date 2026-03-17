using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Universetechgeek.Models;

namespace Universetechgeek.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Review> Reviews { get; set; } = default!;
        public DbSet<Rating> Ratings { get; set; } = default!;
        public DbSet<WatchlistItem> WatchlistItems { get; set; } = default!;
    }
}