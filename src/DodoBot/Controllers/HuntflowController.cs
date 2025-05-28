using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DodoBot.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HuntflowController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> GetDodoStreamAsync(object t)
    {
        return Ok();
    }
}