using DualJobData.BusinessLogic.Entities;
using DualJobData.BusinessLogic.Services.Interface;
using DualJobData.BusinessLogic.UnitOfWork;

namespace DualJobData.BusinessLogic.Services
{
    public class TestService(IUnitOfWork unitOfWork) : ITestService
    {
        public async Task Test()
        {
            using var uow = unitOfWork;
            using var userRepository = uow.UserRepository;
            var users = await userRepository.GetUsersByAcademicProgramIdAsync(1);
            var firstUser = users.First();
            firstUser.UserName = "Test";
            uow.BeginTransaction();
            await userRepository.AddAsync(firstUser);
            var saveTask = uow.SaveChanges();
            await Task.WhenAll(saveTask);
            uow.Commit();
        }
    }
}
