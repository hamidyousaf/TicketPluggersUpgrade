using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TP.Upgrade.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ApiBaseController
    {
        [Route("/error")]
        public IActionResult Error()
        {
            Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            return Problem();
        }
    }
}
