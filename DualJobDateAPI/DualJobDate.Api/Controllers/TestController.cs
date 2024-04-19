using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers;

[ApiController]
[Authorize]
public class DataController(ITestService myService, UserManager<User> userManager) : ControllerBase
{
    [HttpGet("/test")]
    public async Task<ActionResult<User>> GetData()
    {
        return Ok(await userManager.GetUserAsync(User));
    }
}