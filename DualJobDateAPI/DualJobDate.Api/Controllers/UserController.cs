using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DualJobDate.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IServiceProvider serviceProvider,
    IMapper mapper,
    RoleManager<Role> roleManager, 
    IJwtAuthManager jwtAuthManager,
    IUtilService utilService)
    : ControllerBase
{
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string SpecialChars = "!@#$%^&*()_-+=[{]};:<>|./?";
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();


    [Authorize("AdminOrInstitution")]
    [HttpPut]
    [Route("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel model)
    {
        var adminUser = await userManager.GetUserAsync(User);
        int? institution = null;
        int? program = null;
        if (model.Role is UserTypeEnum.Company or UserTypeEnum.Student)
        {
            if (model.AcademicProgramId == null)
            {
                return BadRequest("AcademicProgram is mandatory for Student or Comapny");
            }
        }
        else
        {
            if (model.InstitutionId == null)
            {
                return BadRequest("Institution is mandatory for Student or Comapny");
            }
        }
        IQueryable<Institution> inst;
        IQueryable<AcademicProgram> ap;
        if (User.IsInRole("Admin"))
        {
            if (model.InstitutionId != null)
            {
                inst = utilService.GetInstitutionsAsync().Result.Where(x => x.Id == model.InstitutionId);
                if (inst == null)
                {
                    return NotFound("Institution not found");
                }
                institution = (int)model.InstitutionId;
            }else if (model.AcademicProgramId != null)
            {
                ap = utilService.GetAcademicProgramsAsync().Result.Where(x => x.Id == model.AcademicProgramId);
                if (ap == null)
                {
                    return NotFound("AcademicProgram not found");
                }

                program = (int)model.AcademicProgramId;
            }
            else
            {
                return BadRequest("InstitutionId and AcademicProgramId cannot be null!");
            }
        }
        else
        {
            if (model.AcademicProgramId != null)
            {
                ap = utilService.GetAcademicProgramsAsync().Result.Where(x => x.Id == model.AcademicProgramId && x.InstitutionId == adminUser.InstitutionId);
                if (ap == null)
                {
                    return NotFound("AcademicProgram not found");
                }
                program = (int)model.AcademicProgramId;
            }
            if (model.Role == UserTypeEnum.Admin || model.Role == UserTypeEnum.Institution)
                return Unauthorized("You're not authorized to register an Admin or Institution");
        }
        
        var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();

        if (string.IsNullOrEmpty(model.Email) || !EmailAddressAttribute.IsValid(model.Email))
            return BadRequest($"Email '{model.Email}' is invalid.");

        var user = new User
        {
            Email = model.Email,
            UserType = model.Role,
            IsNew = true,
            InstitutionId = institution ?? null,
            AcademicProgramId = program
        };

        await userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);

        var password = GeneratePassword();

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded) return BadRequest(result.Errors);
        var role = await roleManager.Roles.Where(r => r.UserTypeEnum == model.Role).SingleOrDefaultAsync();
        if (role == null) return NotFound("Role doesn't exist");
        await userManager.AddToRoleAsync(user, role.Name);

        return Ok($"User '{user.Email}' created successfully. ID: {user.Id}");
    }

    [Authorize("AdminOrInstitution")]
    [HttpPut("RegisterStudentsFromJson")]
    public async Task<ActionResult<Json>> RegisterStudentsFromJson([FromQuery] int academicProgramId, [FromBody] List<RegisterStudentUserFromJsonModel> registerStudentUserFromJsonModelList)
    {
        var errorMessages = new List<string>();
        var successMessages = new List<string>();

        var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();
        

        foreach (var registerStudentUser in registerStudentUserFromJsonModelList)
        {
            try
            {
                await RegisterUserFromJsonInformation(registerStudentUser, errorMessages, successMessages, userStore, academicProgramId, UserTypeEnum.Student);
            }
            catch (AuthenticationException exc)
            {
                return BadRequest(exc.Message);
            }
            catch (KeyNotFoundException exc)
            {
                return NotFound(exc.Message);
            }
            catch
            {
                errorMessages.Add($"Error for email {registerStudentUser.Email} occurred.");
            }
        }
        
        return new Json { SuccesfullyRegistered = successMessages, FailedToRegister = errorMessages };

    }
    
    [Authorize("AdminOrInstitution")]
    [HttpPut("RegisterCompaniesFromJson")]
    public async Task<ActionResult<Json>> RegisterCompaniesFromJson([FromQuery]int academicProgramId, [FromBody] List<RegisterCompanyUserFromJsonModel> registerCompanyUserFromJsonModelList)
    {
        var errorMessages = new List<string>();
        var successMessages = new List<string>();

        var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();
        
        
        foreach (var registerCompanyUser in registerCompanyUserFromJsonModelList)
        {
            try
            {
                var user = await RegisterUserFromJsonInformation(registerCompanyUser, errorMessages, successMessages, userStore, academicProgramId, UserTypeEnum.Company);

                if (user != null)
                {
                    await utilService.PutCompanyAsync(registerCompanyUser.CompanyName, (int)user.AcademicProgramId,
                        user.Id);
                }
            } catch (AuthenticationException exc)
            {
                return BadRequest(exc.Message);
            }
            catch (KeyNotFoundException exc)
            {
                return NotFound(exc.Message);
            }
            catch
            {
                errorMessages.Add($"Error for email {registerCompanyUser.Email} occurred.");
            }
        }
        
        return new Json { SuccesfullyRegistered = successMessages, FailedToRegister = errorMessages };

    }

    private async Task<User?> RegisterUserFromJsonInformation(IRegisterUserFromJsonModel registerUserFromJsonModel, ICollection<string> errorMessages, ICollection<string> successMessages, IUserStore<User> userStore, int academicProgramId, UserTypeEnum userType)
    {
        var currentUser = await userManager.GetUserAsync(User);
        int institution;
        int academicProgram = academicProgramId;
        var ap = await utilService.GetAcademicProgramsAsync().Result.FirstOrDefaultAsync(x => x.Id == academicProgram);
        if (ap == null)
        {
            throw new KeyNotFoundException("AcademicProgram not found");
        }

        if (string.IsNullOrEmpty(registerUserFromJsonModel.Email) ||
            !EmailAddressAttribute.IsValid(registerUserFromJsonModel.Email))
        {
            errorMessages.Add(
                $"Bad Request for email {registerUserFromJsonModel.Email}: Email '{registerUserFromJsonModel.Email}' is invalid.");
            return null;
        }

        var user = new User()
        {
            Email = registerUserFromJsonModel.Email,
            AcademicProgramId = academicProgram,
            IsNew = true,
            UserType = userType
        };

        await userStore.SetUserNameAsync(user, registerUserFromJsonModel.Email, CancellationToken.None);

        var password = GeneratePassword();

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            errorMessages.Add($"Bad Request for email {registerUserFromJsonModel.Email}: " +
                              String.Join(";", result.Errors));
            return null;
        }

        var role = await roleManager.Roles.Where(r => r.UserTypeEnum == userType)
            .SingleOrDefaultAsync();
        if (role == null)
        {
            errorMessages.Add($"Not found for email {registerUserFromJsonModel.Email}: Role doesn't exist");
        }

        await userManager.AddToRoleAsync(user, role.Name);

        if (errorMessages.IsNullOrEmpty())
        {
            successMessages.Add($"Registration for email {registerUserFromJsonModel.Email} succeeded.");
        }

        return user;
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult<JwtAuthResultViewModel>> Login(LoginModel model)
    {
        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid Username or Password");
        }
        var user = await userManager.FindByEmailAsync(model.Email);
        var jwtResult = await jwtAuthManager.GenerateTokens(user, DateTime.Now);
        return Ok(jwtResult);
    }

    [HttpPost]
    [Route("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest model)
    {
        var principal = jwtAuthManager.GetPrincipalFromToken(model.RefreshToken, true);
        if (principal == null) //Expression is NOT always false
        {
            return Unauthorized("Invalid Token");
        }
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid token - no user ID");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var newTokens = await jwtAuthManager.GenerateTokens(user, DateTime.Now);
        return Ok(newTokens);
    }
    
    [HttpPost]
    [Authorize]
    [Route("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (result.Succeeded)
        {
            user.IsNew = false;
            var userResult = await userManager.UpdateAsync(user);
            if (userResult.Succeeded) return Ok();
        }
        return BadRequest(result.Errors);
    }

    [HttpPost]
    [Authorize("AdminOrInstitution")]
    [Route("ResetPassword")]
    public async Task<ActionResult<CredentialsDto>> ResetPassword([FromQuery] string id)
    {
        var user = await userManager.FindByIdAsync(id);
        var adminUser = await userManager.GetUserAsync(User);
        if (User.IsInRole("Institution"))
        {
            if (user.UserType is UserTypeEnum.Admin or UserTypeEnum.Institution)
            {
                return Unauthorized("You can't change the password of an Admin User or Institution User");
            }
            if (user.AcademicProgram.InstitutionId != adminUser.InstitutionId)
            {
                return Unauthorized("No Permissions to change this user's name.");
            }
        }
        if (user is null) return NotFound();

        var code = await userManager.GeneratePasswordResetTokenAsync(user);

        var password = GeneratePassword();
        var result = await userManager.ResetPasswordAsync(user, code, password);
        if (result.Succeeded)
        {
            user.IsNew = true;
            var userResult = await userManager.UpdateAsync(user);
            if (userResult.Succeeded)
            {
                var credentials = new CredentialsDto
                {
                    Email = user.Email,
                    Password = password
                };
                return Ok(credentials);
            }
        }

        return BadRequest(result.Errors);
    }

    [Authorize(Policy = "AdminOrInstitution")]
    [HttpGet]
    [Route("GetAllUsers")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(
        [FromQuery] int? institutionId,
        [FromQuery] int? academicProgramId,
        [FromQuery] UserTypeEnum userType)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var query = userManager.Users.Include(u => u.Institution).Include(u => u.AcademicProgram).Include(u => u.Company).AsQueryable();
        if (User.IsInRole("Admin"))
        {
            if (institutionId.HasValue)
            {
                query = query.Where(u => u.AcademicProgram.InstitutionId == institutionId || u.InstitutionId == institutionId);
            }else if (academicProgramId.HasValue)
            {
                query = query.Where(u => u.AcademicProgramId == academicProgramId);
            }        
        }
        else
        {
                if (academicProgramId.HasValue)
                {
                    query = query.Include(u => u.AcademicProgram).Where(u => u.AcademicProgramId == academicProgramId && u.AcademicProgram.InstitutionId == user.InstitutionId);
                }
                else
                {
                    query = query.Include(u => u.AcademicProgram).Where(u => u.AcademicProgram.InstitutionId == user.InstitutionId);
                }

        }
        if (User.IsInRole("Admin"))
        {
            if (userType != UserTypeEnum.Default) query = query.Where(u => u.UserType == userType);
        }
        else
        {
            if (userType is UserTypeEnum.Admin or UserTypeEnum.Institution)
            {
                return Unauthorized("You don't have the rights to see other Users than Students or Companies");
            }
            if (userType != UserTypeEnum.Default) query = query.Where(u => u.UserType == userType);
        }

        var usersList = await query.ToListAsync(); ;

        var userResources = mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(usersList);
        return Ok(userResources);
    }


    [Authorize(Policy = "AdminOrInstitution")]
    [HttpGet]
    [Route("GetUser")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUser([FromQuery] string id)
    {
        var adminUser = await userManager.GetUserAsync(User);
        User? user;
        if (User.IsInRole("Admin"))
        {
            user = await userManager.Users.Where(u => u.Id == id).Include(u => u.Institution).Include(u => u.AcademicProgram).Include(u => u.Company).SingleOrDefaultAsync();
        }
        else
        {
            user = await userManager.Users.Where(u => u.Id == id).Include(u => u.Institution).Include(u => u.AcademicProgram).Include(u => u.Company).SingleOrDefaultAsync();
            if (user.AcademicProgram.InstitutionId != adminUser.InstitutionId || user.UserType == UserTypeEnum.Admin || user.UserType == UserTypeEnum.Institution)
            {
                return Unauthorized("You don't have the rights to see this user");
            }
        }
        if (user is null) return NotFound();
        var userResource = mapper.Map<User, UserDto>(user);
        return Ok(userResource);
    }


    [Authorize(Policy = "Institution")]
    [HttpDelete]
    [Route("DeleteUser")]
    public async Task<IActionResult> DeleteUser([FromQuery] string id)
    {
        var adminUser = await userManager.GetUserAsync(User);
        User? user;
        if (User.IsInRole("Admin"))
        {
            user = await userManager.Users.Where(u => u.Id == id).Include(u => u.Institution).Include(u => u.AcademicProgram).Include(u => u.Company).SingleOrDefaultAsync();
        }
        else
        {
            user = await userManager.Users.Where(u => u.Id == id).Include(u => u.Institution).Include(u => u.AcademicProgram).Include(u => u.Company).SingleOrDefaultAsync();
            if (user.AcademicProgram.InstitutionId != adminUser.InstitutionId || user.UserType == UserTypeEnum.Admin || user.UserType == UserTypeEnum.Institution)
            {
                return Unauthorized("You don't have the rights to delete this user");
            }
        }
        if (user is null) return NotFound();

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded) return Ok("User deleted successfully!");

        return BadRequest(result.Errors);
    }

    [HttpPost]
    [Authorize]
    [Route("DeleteUserWithPassword")]
    public async Task<IActionResult> DeleteUserWithPassword([FromBody] DeleteUserModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized("Wrong Credentials!");

        var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!checkPasswordResult.Succeeded) return Unauthorized("Wrong Credentials!");

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
            return Ok("User deleted successfully!");
        return BadRequest(result.Errors);
    }

    [Obsolete("Obsolete")]
    public static string GeneratePassword(int length = 12)
    {
        if (length < 4) throw new ArgumentException("Min 4 sign", nameof(length));

        var charCategories = new[]
        {
            LowerCase,
            UpperCase,
            Digits,
            SpecialChars
        };

        var data = new byte[length];
        using var crypto = new RNGCryptoServiceProvider();
        crypto.GetBytes(data);

        var passwordChars = new char[length];
        for (var i = 0; i < length; i++)
        {
            var categoryIndex = data[i] % charCategories.Length;
            var charIndex = data[i] % charCategories[categoryIndex].Length;

            passwordChars[i] = charCategories[categoryIndex][charIndex];
        }

        EnsureEachCategory(passwordChars, charCategories);

        return new string(passwordChars);
    }

    private static void EnsureEachCategory(char[] passwordChars, string[] charCategories)
    {
        var random = new Random();
        foreach (var t in charCategories)
            if (!passwordChars.Any(p => t.Contains(p)))
            {
                var replaceIndex = random.Next(passwordChars.Length);
                passwordChars[replaceIndex] = t[random.Next(t.Length)];
            }
    }
}

public class Json
{
    public List<string> SuccesfullyRegistered { get; set; }
    public List<string> FailedToRegister { get; set; }
}