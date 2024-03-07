using DualJobData.BusinessLogic.Entities;
using DualJobData.BusinessLogic.Repositories.Interfaces;
using DualJobData.BusinessLogic.Services.Interface;

namespace DualJobData.BusinessLogic.Services
{
    public class TestService(IUserRepository userRepository) : ITestService
    {
        public string Test()
        {
            return "service test call";
        }
    }
}
