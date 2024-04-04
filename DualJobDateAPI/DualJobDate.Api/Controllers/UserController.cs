using AutoMapper;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Resources;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DualJobDate.Api.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class UserController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
        IServiceProvider serviceProvider,
        IMapper mapper, RoleManager<Role> roleManager)
        : ControllerBase
    {
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
                {
                    return BadRequest("InstitutionId or AcademicProgramId cannot be null!");
                }
                institution = (int)model.InstitutionId;
                program = (int)model.AcademicProgramId;
            }
            else
            {
                institution = adminUser.InstitutionId;
                program = adminUser.AcademicProgramId;
                if (model.Role == UserTypeEnum.Admin || model.Role == UserTypeEnum.Company)
                {
                    return Unauthorized("You're not authorized to register an Admin or Company");
                }
            }
            var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();
            
            if (string.IsNullOrEmpty(model.Email) || !EmailAddressAttribute.IsValid(model.Email))
            {
                return BadRequest($"Email '{model.Email}' is invalid.");
            }

            var user = new User
            {
                Email = model.Email,
                UserType = UserTypeEnum.Admin,
                IsNew = true,
                InstitutionId = institution,
                AcademicProgramId = program
            };
            
            await userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);

            var password = GeneratePassword(12);

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            var role = await roleManager.Roles.Where(r => r.UserTypeEnum == model.Role).SingleOrDefaultAsync();
            if (role == null)
            {
                return NotFound("Role doesn't exist");
            }
            await userManager.AddToRoleAsync(user, role.Name);
            
            return Ok($"User '{user.Email}' created successfully");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login([FromBody] LoginModel login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
        {

            var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
            var isPersistent = (useCookies == true) && (useSessionCookies != true);
            signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
            }

            // The signInManager already produced the needed response in the form of a cookie or bearer token.
            return TypedResults.Empty;
        }

        [HttpPost]
        [Route("Refresh")]
        public async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> RefreshToken
        ([FromBody] RefreshRequest refreshRequest)
        {
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
            var refreshTokenProtector = optionsMonitor.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
            var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

            // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
            if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
                DateTime.Now >= expiresUtc ||
                await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not User user)

            {
                return TypedResults.Challenge();
            }

            var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
            return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);

        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
            {
                return Unauthorized();
            }

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                user.IsNew = false;
                var userResult = await userManager.UpdateAsync(user);
                if (userResult.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Authorize("AdminOrInstitution")]
        [Route("ResetPassword")]
        public async Task<ActionResult<CredentialsResource>> ResetPassword([FromQuery] string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound();
            }

            var code = await userManager.GeneratePasswordResetTokenAsync(user);

            var password = GeneratePassword(12);
            var result = await userManager.ResetPasswordAsync(user, code, password);
            if (result.Succeeded)
            {
                user.IsNew = false;
                var userResult = await userManager.UpdateAsync(user);
                if (userResult.Succeeded)
                {
                    var credentials = new CredentialsResource
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
        public async Task<ActionResult<IEnumerable<UserResource>>> GetAllUsers(
            [FromQuery] int? institutionId, 
            [FromQuery] int? academicProgramId,
            [FromQuery] UserTypeEnum userType)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var users = new List<User>();

            IQueryable<User> query = userManager.Users.Include(u => u.Company).AsQueryable();

            if (User.IsInRole("Admin") && institutionId.HasValue)
            {
                query = query.Where(u => u.InstitutionId == institutionId);
            }
            else if (academicProgramId.HasValue)
            {
                query = query.Where(u => u.AcademicProgramId == academicProgramId);
            }
            else
            {
                return BadRequest("Invalid request parameters or insufficient permissions.");
            }

            if (userType != UserTypeEnum.Default)
            {
                query = query.Where(u => u.UserType == userType);
            }

            var usersList = await query.ToListAsync();

            if (usersList.IsNullOrEmpty())
            {
                return NotFound("No user found!");
            }
    
            var userResources = mapper.Map<IEnumerable<User>, IEnumerable<UserResource>>(usersList);
            return Ok(userResources);
        }

        
        [Authorize(Policy = "AdminOrInstitution")]
        [HttpGet]
        [Route("GetUser")]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetUser([FromQuery] string userId)
        {
            var user = await userManager.Users.Where(u => u.Id == userId).Include(u => u.Company).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound("User not found");
            }
            var userResource = mapper.Map<User, UserResource>(user);
            return Ok(userResource);
        }


        [Authorize(Policy = "Admin")]
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound();
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("User deleted successfully!");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("DeleteUserWithPassword")]
        public async Task<IActionResult> DeleteUserWithPassword([FromBody] DeleteUserModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Wrong Credentials!");
            }

            var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!checkPasswordResult.Succeeded)
            {
                return Unauthorized("Wrong Credentials!");
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("User deleted successfully!");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_-+=[{]};:<>|./?";

        [Obsolete("Obsolete")]
        public static string GeneratePassword(int length = 12)
        {
            if (length < 4) throw new ArgumentException("Min 4 sign", nameof(length));

            var charCategories = new []
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
            {
                if (!passwordChars.Any(p => t.Contains(p)))
                {
                    var replaceIndex = random.Next(passwordChars.Length);
                    passwordChars[replaceIndex] = t[random.Next(t.Length)];
                }
            }
        }
    }
}