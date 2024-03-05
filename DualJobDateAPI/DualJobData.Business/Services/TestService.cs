using DualJobData.BusinessLogic.Entities;
using DualJobData.BusinessLogic.Repositories.Interfaces;
using DualJobDateAPI.Repository;
using DualJobDateAPI.Repository.Interfaces;

namespace DualJobData.BusinessLogic.Services
{
    public class TestService
    {
        private readonly IUserRepository _userRepository;

        public TestService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Test()
        {
            var user = new User();
            user.TenantId = 1;
            user.StationId = 1;
            _userRepository.Add(user);
        }
    }
}
