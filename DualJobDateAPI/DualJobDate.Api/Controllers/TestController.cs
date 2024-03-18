﻿using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers
{
    [ApiController]
    [Authorize]
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
