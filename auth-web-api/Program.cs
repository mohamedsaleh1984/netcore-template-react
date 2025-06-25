using AuthWebApi.Extensions;  
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(x => { }); 
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(); 
builder.Logging.AddConsole();
builder.Services.AddJwtExtension();
builder.Services.AddSwager();
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCorsExtension();


var app = builder.Build();
app.UseHttpLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseNSwag();
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for  production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.Run();
