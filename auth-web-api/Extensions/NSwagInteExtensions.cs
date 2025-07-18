﻿using Microsoft.AspNetCore.Http.Extensions;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag;
using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation.Processors.Security;
using System.Reflection;

namespace AuthWebApi.Extensions
{
    public static class NSwagInteExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            // Support Authorization for Generated Client.
            services.AddSwaggerDocument(doc =>
            {
                doc.AddSecurity("Bearer", new OpenApiSecurityScheme()
                {
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "JWT Authorization Header using the Bearer schema.",
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                });
                doc.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
            });

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
            app.UseSwaggerUi(options =>
            {
                options.CustomStylesheetPath = "/swagger-ui/theme-feeling-blue.css";
            });


            app.Use(CreatePath("/_/ts", GenerateTypeScriptClient));
        }

        private static async Task<string> GenerateTypeScriptClient(HttpContext context)
        {
            var document = await GetOpenApiDocument(context);


            string fileContent = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                ClassName = "ApiClient",
                OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator(),
                HttpClass = HttpClass.HttpClient,
                InjectionTokenType = InjectionTokenType.InjectionToken,
                GenerateClientInterfaces = true,
                GenerateDtoTypes = true,
                TypeScriptGeneratorSettings = {
                    TypeStyle = TypeScriptTypeStyle.Interface,
                },
                UseTransformOptionsMethod = true,
                Template = TypeScriptTemplate.Fetch,
                ClientBaseClass = "BaseClient",

            }).GenerateFile();

            List<string> strings = fileContent.Split('\n').ToList();
            strings[9] = @"import { BaseClient } from ""./BaseClient"";";


            fileContent = string.Join("\n", strings);

            return fileContent;
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
