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
using DualJobDate.BusinessObjects.Resources;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DualJobDate.Api.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class UserController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
        IServiceProvider serviceProvider,
        IMapper mapper)
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
                AcademicProgramId = program,
                CompanyId = model.CompanyId
            };

            await userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);

            var password = "Password1!";

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(user, model.Role);
            
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
                return BadRequest();
            }

            //TODO Create own ChangePassword Method
            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] RegisterUserModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null || !(await userManager.IsEmailConfirmedAsync(user)))
            {
                return Ok();
            }

            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "User",
                new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
            return Ok();
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return BadRequest();
            }

            var result = await userManager.ResetPasswordAsync(user, model.ResetCode, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        [Route("GetAllUsers")]
        public Task<ActionResult<IEnumerable<UserResource>>> GetAllUsers()
        {
            var users = userManager.Users.ToList();
            var userResources = mapper.Map<List<User>, List<UserResource>>(users);
            return Task.FromResult<ActionResult<IEnumerable<UserResource>>>(Ok(userResources));
        }
        
        [Authorize(Policy = "Admin")]
        [HttpGet]
        [Route("GetUser/{UserId}")]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetAllUsers(string UserId)
        {
            var user = await userManager.FindByIdAsync(UserId);
            var userResource = mapper.Map<User, UserResource>(user);
            return Ok(userResource);
        }


        [Authorize(Policy = "Admin")]
        [HttpDelete]
        [Route("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
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
                return NotFound("Wrong Credentials!");
            }

            var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!checkPasswordResult.Succeeded)
            {
                return NotFound("Wrong Credentials!");
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

            var charCategories = new string[]
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