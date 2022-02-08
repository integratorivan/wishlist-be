using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Whishlist API",
        Description = "Backend for wishlist API",
        Contact = new OpenApiContact
        {
            Name = "Our Contact",
            Url = new Uri("https://wishlist.com/contact")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "wl v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandler("/error");
app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseResponseCompression();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        ResultStatusCodes = new Dictionary<HealthStatus, int>()
        {
            [HealthStatus.Healthy] = 200,
            [HealthStatus.Degraded] = 400,
            [HealthStatus.Unhealthy] = 500,
        },
    });
});

app.Run();