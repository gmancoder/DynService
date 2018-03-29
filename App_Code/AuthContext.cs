using Microsoft.AspNet.Identity.EntityFramework;

namespace DynService_v3
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext()
            : base("AuthContext")
        {

        }
    }
}