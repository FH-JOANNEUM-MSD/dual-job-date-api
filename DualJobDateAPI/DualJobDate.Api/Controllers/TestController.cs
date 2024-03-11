using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers
{
    public class DataController(ITestService myService) : ControllerBase
    {
        [HttpGet("test")]
        public async Task<IActionResult> GetData()
        {
            try
            {
                await myService.Test();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
