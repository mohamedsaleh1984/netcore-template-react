
using AuthWebApi.Models;
using Microsoft.EntityFrameworkCore;


namespace AuthWebApi 
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }


}
