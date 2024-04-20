using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AutoMapper;
using DualJobDate.BusinessLogic.Helper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using DualJobDate.BusinessObjects.Entities.Models;
using DualJobDate.BusinessObjects.Resources;
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
    [Authorize("AdminOrInstitution")]
    [HttpPut]
    [Route("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel model)
    {
        EmailAddressAttribute emailAddressAttribute = new();

        var user = new User
        {
            Email = model.Email,
            UserType = model.Role,
            IsNew = true,
        };

        if (User.IsInRole("Admin"))
        {
            if (model.InstitutionId is null || model.AcademicProgramId is null)
                return BadRequest("InstitutionId or AcademicProgramId cannot be null!");

            user.InstitutionId = model.InstitutionId.Value;
            user.AcademicProgramId = model.AcademicProgramId.Value;
        }
        else
        {
            var adminUser = await userManager.GetUserAsync(User);
            if (adminUser is null)
                return NotFound("User not found");

            if (model.Role == UserTypeEnum.Admin || model.Role == UserTypeEnum.Company)
                return Unauthorized("You're not authorized to register an Admin or Company");

            user.InstitutionId = adminUser.InstitutionId;
            user.AcademicProgramId = adminUser.AcademicProgramId;
        }

        if (string.IsNullOrEmpty(model.Email) || !emailAddressAttribute.IsValid(model.Email))
            return BadRequest($"Email '{model.Email}' is invalid.");

        var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();
        await userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
        var password = PasswordHelper.GeneratePassword();
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var role = await roleManager.Roles.Where(r => r.UserTypeEnum == model.Role).SingleOrDefaultAsync();
        if (role is null)
            return NotFound("Role doesn't exist");

        await userManager.AddToRoleAsync(user, role.Name ?? nameof(model.Role));

        return Ok($"User '{user.Email}' created successfully");
    }

    [HttpPost]
    [Route("Login")]
    public async Task<JwtAuthResultViewModel?> Login(LoginModel model)
    {
        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        if (!result.Succeeded)
            return null;

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user is null)
            return null;

        var jwtResult = await jwtAuthManager.GenerateTokens(user, DateTime.Now);
        return jwtResult;
    }

    [HttpPost]
    [Route("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest model)
    {
        var principal = jwtAuthManager.GetPrincipalFromToken(model.RefreshToken, true);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Invalid token - no user ID");

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound("User not found");

        var newTokens = await jwtAuthManager.GenerateTokens(user, DateTime.Now);
        return Ok(newTokens);
    }
    
    [HttpPost]
    [Authorize]
    [Route("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        user.IsNew = false;
        var userResult = await userManager.UpdateAsync(user);
        if (!userResult.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpPost]
    [Authorize("AdminOrInstitution")]
    [Route("ResetPassword")]
    public async Task<ActionResult<CredentialsResource>> ResetPassword([FromQuery] string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        var code = await userManager.GeneratePasswordResetTokenAsync(user);

        var password = PasswordHelper.GeneratePassword();
        var result = await userManager.ResetPasswordAsync(user, code, password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        user.IsNew = false;
        var userResult = await userManager.UpdateAsync(user);
        if (!userResult.Succeeded)
            return BadRequest(result.Errors);

        var credentials = new CredentialsResource
        {
            Email = user.Email ?? "No EMail Defined",
            Password = password
        };

        return Ok(credentials);
    }

    [Authorize(Policy = "AdminOrInstitution")]
    [HttpGet]
    [Route("GetAllUsers")]
    public async Task<ActionResult<IEnumerable<UserResource>>> GetAllUsers(
        [FromQuery] int? institutionId,
        [FromQuery] int? academicProgramId,
        [FromQuery] UserTypeEnum? userType = null)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var query = userManager.Users.Include(u => u.Company).AsQueryable();

        if (User.IsInRole("Admin") && institutionId.HasValue)
            query = query.Where(u => u.InstitutionId == institutionId);

        if (academicProgramId.HasValue)
            query = query.Where(u => u.AcademicProgramId == academicProgramId);

        if (userType.HasValue)
            query = query.Where(u => u.UserType == userType);

        var usersList = await query.ToListAsync();
        if (usersList.IsNullOrEmpty())
            return NotFound("No user found!");

        var userResources = mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(usersList);
        return Ok(userResources);
    }

    [Authorize(Policy = "AdminOrInstitution")]
    [HttpGet]
    [Route("GetUser")]
    public async Task<ActionResult<IEnumerable<UserResource>>> GetUser([FromQuery] string id)
    {
        var user = await userManager.Users
            .Include(u => u.Company)
            .SingleOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound("User not found");

        var userResource = mapper.Map<User, UserResource>(user);
        return Ok(userResource);
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete]
    [Route("DeleteUser")]
    public async Task<IActionResult> DeleteUser([FromQuery] string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound("User not found");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User deleted successfully!");
    }

    [HttpPost]
    [Authorize]
    [Route("DeleteUserWithPassword")]
    public async Task<IActionResult> DeleteUserWithPassword([FromBody] DeleteUserModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized("Wrong Credentials!");

        var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!checkPasswordResult.Succeeded)
            return Unauthorized("Wrong Credentials!");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User deleted successfully!");
    }
}