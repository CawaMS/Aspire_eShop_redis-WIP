using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Data;

namespace Store.Data
{
    public class UserContext(DbContextOptions<UserContext> options) : IdentityDbContext<StoreUser>(options)
    {
    }
}
