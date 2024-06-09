using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;

namespace DualJobDate.BusinessLogic.Services;

public class TestService(IUnitOfWork unitOfWork) : ITestService
{
    public async Task Test()
    {
        using var uow = unitOfWork;
        using var institutionRepository = uow.InstitutionRepository;
        var institution = new Institution
        {
            Id = 1
        };
        uow.BeginTransaction();
        await institutionRepository.AddAsync(institution);
        var saveTask = uow.SaveChanges();
        await Task.WhenAll(saveTask);
        uow.Commit();
    }
}