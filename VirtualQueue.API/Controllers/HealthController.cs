using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualQueue.Application.Common.Interfaces;
using VirtualQueue.Infrastructure.Persistence;

namespace VirtualQueue.Api.Controllers;

/// <summary>
/// Not a business feature - a smoke-test endpoint proving the DI graph,
/// EF Core wiring, and middleware pipeline all work together. Sprint 2
/// adds BusinessesController, ServicesController, and QueueController
/// with the actual CRUD/queue features.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ApplicationDbContext _concreteDbContext;

    // Two constructor params look redundant at first glance, but they serve
    // different purposes: IApplicationDbContext is what business logic
    // depends on everywhere else in the app; ApplicationDbContext (concrete)
    // is only needed here because Database.CanConnectAsync() is an
    // EF Core-specific capability that intentionally isn't leaked onto the
    // Application-layer interface. Both resolve to the same Scoped instance,
    // so this doesn't create two DbContexts per request.
    public HealthController(IApplicationDbContext dbContext, ApplicationDbContext concreteDbContext)
    {
        _dbContext = dbContext;
        _concreteDbContext = concreteDbContext;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "healthy", timestampUtc = DateTime.UtcNow });
    }

    [HttpGet("db")]
    public async Task<IActionResult> CheckDatabase(CancellationToken cancellationToken)
    {
        var canConnect = await _concreteDbContext.Database.CanConnectAsync(cancellationToken);
        return Ok(new { databaseReachable = canConnect });
    }
}
