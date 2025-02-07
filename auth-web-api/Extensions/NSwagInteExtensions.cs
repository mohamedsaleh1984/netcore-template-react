using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag;
using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.TypeScript;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AuthWebApi.Extensions
{
    public static class NSwagInteExtensions
    {
        public static void AddSwager(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSwaggerDocument();
        }

        public static void UseNSwag(this IApplicationBuilder app)
        {
            //Get Current Assembly
            var name = Assembly.GetExecutingAssembly().GetName();
            app.UseOpenApi(x =>
            {
                x.PostProcess = (doc, request) =>
                {
                    doc.Info.Title = name.Name;
                    doc.Info.Version = "v1.0";
                };

                x.Path = "/swagger/v1/swagger.json";

            });
            app.UseSwaggerUi3(options =>
            {
                options.CustomStylesheetPath = "/swagger-ui/theme-feeling-blue.css";
            });
            app.Use(CreatePath("/_/ts", GenerateTypeScriptClient));
        }
        private static async Task<string> GenerateTypeScriptClient(HttpContext context)
        {
            var document = await GetOpenApiDocument(context);

            return new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                ClassName = "ApiClient",
                OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator(),
                HttpClass = HttpClass.HttpClient,
                InjectionTokenType = InjectionTokenType.InjectionToken,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings = {

                    TypeStyle = TypeScriptTypeStyle.Interface,
                }
            }).GenerateFile();
        }
        private static async Task<OpenApiDocument> GetOpenApiDocument(HttpContext context)
        {
            var url = context.Request.GetDisplayUrl().Replace(context.Request.Path, "/swagger/v1/swagger.json");
            return await OpenApiDocument.FromUrlAsync(url);
        }
        private static Func<HttpContext, Func<Task>, Task> CreatePath(string path, Func<HttpContext, Task<string>> response)
        {
            return async (context, next) =>
            {
                if (context.Request.Path != path)
                {
                    await next();
                    return;
                }

                context.Response.Clear();
                await context.Response.WriteAsync(await response(context));
            };
        }
    }
}
