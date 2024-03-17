using DualJobDate.BusinessLogic.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers
{
    public class UserController(IUserService myService) : ControllerBase
    {
        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await myService.GetUserById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
