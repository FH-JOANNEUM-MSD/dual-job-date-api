using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers
{
    [ApiController]
    [Authorize("Admin")]
    public class TestController(UserManager<User> userManager) : ControllerBase
    {
        [HttpGet("/test")]
        public async Task<ActionResult<User>> GetData()
        {
            return Ok(await userManager.GetUserAsync(User));
        }
    }
}
