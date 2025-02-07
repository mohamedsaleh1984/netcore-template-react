using AuthWebApi.Extensions;
using AuthWebApi.Helper;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(x => { }); 
builder.Services.AddControllers();
builder.Services.AddSingleton<TokenHelper>();
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
