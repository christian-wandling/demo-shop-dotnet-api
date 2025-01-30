using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Api.Common;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public abstract class ApiController : ControllerBase
{
    protected ActionResult ToActionResult(Result result) => result.ToActionResult(this);
    protected ActionResult<T> ToActionResult<T>(Result<T> result) => result.ToActionResult(this);
}
