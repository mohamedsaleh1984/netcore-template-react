using AuthWebApi.Models;

namespace AuthWebApi.DbContext
{
    public static class DbContext
    {
        public static List<User> Users = new List<User>
                {
                    new User { Username = "admin", Email = "admin@example.com", Role = "Admin",Password="password" },
                    new User { Username = "user", Email = "user@example.com", Role = "User",Password="password" }
                };
    }

}
