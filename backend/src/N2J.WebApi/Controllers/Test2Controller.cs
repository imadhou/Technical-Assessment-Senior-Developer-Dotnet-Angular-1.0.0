
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;


namespace N2J.WebApi.Controllers;  // Add this namespace


[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/test")]
public class TestV2Controller : ControllerBase  // Different name
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "This is Version 2.0 - Enhanced!",
            features = new[] { "Advanced filtering", "Pagination", "Audit trail" },
            endpoint = "GET /api/v2/test",
            newFeature = "This is only available in v2"
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new
        {
            id = id,
            version = "2.0",
            data = $"Item {id} from v2",
            metadata = new { created = DateTime.UtcNow, modified = DateTime.UtcNow }
        });
    }

    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        return Ok(new
        {
            message = "This endpoint only exists in v2",
            totalCount = 150,
            analytics = "Available in v2 only"
        });
    }
}