using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Dtos;
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
    IJwtAuthManager jwtAuthManager)
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
        int institution;
        int program;
        if (User.IsInRole("Admin"))
        {
            if (model.InstitutionId == null || model.AcademicProgramId == null)
                return BadRequest("InstitutionId or AcademicProgramId cannot be null!");
            institution = (int)model.InstitutionId;
            program = (int)model.AcademicProgramId;
        }
        else
        {
            institution = adminUser.InstitutionId;
            program = adminUser.AcademicProgramId;
            if (model.Role == UserTypeEnum.Admin || model.Role == UserTypeEnum.Company)
                return Unauthorized("You're not authorized to register an Admin or Company");
        }

        var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();

        if (string.IsNullOrEmpty(model.Email) || !EmailAddressAttribute.IsValid(model.Email))
            return BadRequest($"Email '{model.Email}' is invalid.");

        var user = new User
        {
            Email = model.Email,
            UserType = model.Role,
            IsNew = true,
            InstitutionId = institution,
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
        var users = new List<User>();

        var query = userManager.Users.Include(u => u.Institution).Include(u => u.AcademicProgram).Include(u => u.Company).AsQueryable();

        if (User.IsInRole("Admin") && institutionId.HasValue)
            query = query.Where(u => u.InstitutionId == institutionId);
        else if (academicProgramId.HasValue)
            query = query.Where(u => u.AcademicProgramId == academicProgramId);
        else
            return BadRequest("Invalid request parameters or insufficient permissions.");

        if (userType != UserTypeEnum.Default) query = query.Where(u => u.UserType == userType);

        var usersList = await query.ToListAsync();

        if (usersList.IsNullOrEmpty()) return NotFound("No user found!");

        var userResources = mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(usersList);
        return Ok(userResources);
    }


    [Authorize(Policy = "AdminOrInstitution")]
    [HttpGet]
    [Route("GetUser")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUser([FromQuery] string id)
    {
        var user = await userManager.Users.Where(u => u.Id == id).Include(u => u.Institution).Include(u => u.AcademicProgram).Include(u => u.Company).SingleOrDefaultAsync();
        if (user == null) return NotFound("User not found");
        var userResource = mapper.Map<User, UserDto>(user);
        return Ok(userResource);
    }


    [Authorize(Policy = "Admin")]
    [HttpDelete]
    [Route("DeleteUser")]
    public async Task<IActionResult> DeleteUser([FromQuery] string id)
    {
        var user = await userManager.FindByIdAsync(id);
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