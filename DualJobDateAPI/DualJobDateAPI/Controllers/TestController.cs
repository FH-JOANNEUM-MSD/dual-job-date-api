using DualJobData.BusinessLogic.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers
{
    public class DataController(ITestService myService) : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult GetData()
        {
            try
            {
                var data = myService.Test();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
