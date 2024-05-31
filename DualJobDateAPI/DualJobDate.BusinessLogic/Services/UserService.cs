using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DualJobDate.BusinessLogic.Services;

public class UserService(
    UserManager<User> userManager,
    IUtilService utilService,
    IPasswordGenerator generator,
    IUserStore<User> userStore,
    RoleManager<Role> roleManager) : IUserService
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();

    public async Task<User> CreateAsync(User user)
    {
        var inst = utilService.GetInstitutionsAsync().Result.Where(x => x.Id == user.InstitutionId);
        if (inst == null) throw new ArgumentOutOfRangeException("Institution not found");

        var ap = utilService.GetAcademicProgramsAsync(null).Result.Where(x => x.Id == user.AcademicProgramId);
        if (ap == null)  throw new ArgumentOutOfRangeException("AcademicProgram not found");

        if (string.IsNullOrEmpty(user.Email) || !EmailAddressAttribute.IsValid(user.Email))
            throw new ArgumentException($"Email '{user.Email}' is invalid.");

        await userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
        var password = generator.GeneratePassword();

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded) throw new ArgumentException(result.Errors.First().Description);


        var role = await roleManager.Roles.Where(r => r.UserTypeEnum == user.UserType).SingleOrDefaultAsync();
        if (role == null) throw new ArgumentOutOfRangeException("Role doesn't exist");
        await userManager.AddToRoleAsync(user, role.Name);

        return user;

    }
}
