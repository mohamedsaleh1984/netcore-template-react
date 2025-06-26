namespace AuthWebApi.Extensions
{
    public static class CorsExtension
    {
        public static void AddCorsExtension(this IServiceCollection services)
        {
            const string ALLOW_DEVELOPMENT_CORS_ORIGINS_POLICY = "AllowDevelopmentSpecificOrigins";
            const string LOCAL_DEVELOPMENT_URL = "http://localhost:3000";
            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy(ALLOW_DEVELOPMENT_CORS_ORIGINS_POLICY, policy =>
                {
                    policy.WithOrigins(LOCAL_DEVELOPMENT_URL) // Allow only React frontend
                          .AllowAnyMethod()                     // Allow GET, POST, PUT, DELETE, etc.
                          .AllowAnyHeader()                    // Allow any headers
                          .AllowCredentials();
                });
            });
        }
    }
}
