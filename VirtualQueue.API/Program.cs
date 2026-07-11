using Microsoft.EntityFrameworkCore;
using VirtualQueue.Api.Middleware;
using VirtualQueue.Application.Common.Interfaces;
using VirtualQueue.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// Dependency Injection registrations
// -----------------------------------------------------------------------

// DbContext is registered as Scoped by AddDbContext (the correct default -
// one instance per HTTP request; sharing it across requests would allow
// data to leak between unrelated requests, and using Transient would
// break EF Core's unit-of-work/change-tracking model).
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Api and Application code depend on the interface, never on
// ApplicationDbContext directly - this is what lets us unit test
// against a different IApplicationDbContext (e.g., EF Core InMemory)
// without touching Postgres.
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Virtual Queue API",
        Version = "v1",
        Description = "Public queue-joining and business dashboard API for local businesses."
    });
});

var app = builder.Build();

// -----------------------------------------------------------------------
// Middleware pipeline - order matters here:
// 1. Exception handling first, so it can catch errors from everything after it.
// 2. Swagger/HTTPS/dev-only tooling.
// 3. Routing, then Authorization (auth isn't implemented yet in Sprint 1,
//    but the placeholder is kept so the pipeline shape doesn't change later).
// 4. Controller endpoints last.
// -----------------------------------------------------------------------

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Virtual Queue API v1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposed for WebApplicationFactory-based integration tests later.
public partial class Program { }
