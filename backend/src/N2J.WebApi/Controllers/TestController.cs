using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace N2J.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test")]
public class TestV1Controller : ControllerBase  // Different name
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "This is Version 1.0",
            features = new[] { "Basic CRUD", "Simple responses" },
            endpoint = "GET /api/v1/test"
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new
        {
            id = id,
            version = "1.0",
            data = $"Item {id} from v1"
        });
    }
}
