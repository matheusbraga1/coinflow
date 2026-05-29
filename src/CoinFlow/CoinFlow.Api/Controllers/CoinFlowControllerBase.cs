using Microsoft.AspNetCore.Mvc;

namespace CoinFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoinFlowControllerBase : ControllerBase
{
}
