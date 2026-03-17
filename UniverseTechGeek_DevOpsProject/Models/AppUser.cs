// ── AppUser.cs ───────────────────────────────────────────────
// Place in: Models/AppUser.cs

using Microsoft.AspNetCore.Identity;

namespace Universetechgeek.Models
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
