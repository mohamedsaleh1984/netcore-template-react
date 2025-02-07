namespace AuthWebApi.Extensions
{
    public static class CorsExtension
    {
        public static void AddCorsExtension(this IServiceCollection services)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins, policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // Allow only React frontend
                          .AllowAnyMethod()                     // Allow GET, POST, PUT, DELETE, etc.
                          .AllowAnyHeader();                    // Allow any headers
                });
            });
        }
    }
}
