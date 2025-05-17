
using Microsoft.AspNetCore.Identity;

namespace Demirbaslar.API.Models
{
    public class AppUsers : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
