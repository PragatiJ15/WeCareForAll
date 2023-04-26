using Microsoft.AspNetCore.Identity;

namespace WeCareForAll.Models.Domain
{
    public class ApplicationUser :IdentityUser
    {
        public string? Name { get; set; }

    }
}
