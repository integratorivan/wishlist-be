using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Wishlist.Exception;

namespace Wishlist.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    /// <summary>
    /// Handle unobserved exceptions.
    /// </summary>
    /// <param name="configuration">Service configuration.</param>
    /// <returns>JSON response by error.</returns>
    [Route("/error")]
    [OpenApiIgnore]
    public IActionResult Error([FromServices] IConfiguration configuration)
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        int statusCode;
        string title;
        string fullError = configuration.GetSection("AllowFullError").Get<bool>() ? context.Error.ToString() : null;

        switch (context.Error)
        {
            case ArgumentException ae:
                statusCode = 400;
                title = ae.Message + $"(param: {ae.ParamName})";
                break;
            case UnauthorizedException unah:
                statusCode = 401;
                title = $"Unauthorized, {unah.Message}";
                break;
            case NotSupportedException nse:
                statusCode = 405;
                title = $"Method Not Allowed";
                break;
            case NotImplementedException nie:
                statusCode = 501;
                title = $"Not Implemented";
                break;
            default:
                statusCode = 500;
                title = "Internal Server Error";
                break;
        }

        return Problem(
            title: title,
            statusCode: statusCode,
            detail: fullError);
    }

    /// <summary>
    /// Get version app from file 'app_version.txt'
    /// </summary>
    [Route("/version")]
    [HttpGet]
    public async Task<string> GetVersion()
    {
        const string VERSION_FILE = "app_version.txt";

        if (System.IO.File.Exists(VERSION_FILE))
        {
            return await System.IO.File.ReadAllTextAsync(VERSION_FILE);
        }

        return null!;
    }
}