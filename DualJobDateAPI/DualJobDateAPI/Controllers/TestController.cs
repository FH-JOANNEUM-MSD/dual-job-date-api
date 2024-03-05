using DualJobData.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace DualJobDate.API.Controllers
{
    public class TestController
    {
        private readonly TestService _testService;

        public TestController(TestService testService)
        {
            _testService = testService;
        }

        public Action MyAction()
        {
            _testService.Test();
            return null;
        }
    }
}
