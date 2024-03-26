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
using System.Text;
using DualJobDate.BusinessObjects.Resources;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using Microsoft.AspNetCore.Authentication;

namespace DualJobDate.Api.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class UserController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
        IServiceProvider serviceProvider,
        IMapper mapper,
        IEmailHelper emailHelper,
        IJwtHelper jwtHelper)
        : ControllerBase
    {
        private static readonly EmailAddressAttribute EmailAddressAttribute = new();

        [HttpPut]
        [Route("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel model)
        {
            var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();
            var emailStore = (IUserEmailStore<User>)userStore;
            

            if (string.IsNullOrEmpty(model.Email) || !EmailAddressAttribute.IsValid(model.Email))
            {
                return BadRequest($"Email '{model.Email}' is invalid.");
            }

            var user = new User
            {
                Email = model.Email,
                UserType = UserTypeEnum.Admin,
                BirthDate = new DateTime(),
                IsNew = true,
                FirstName = "root",
                LastName = "root",
            };

            await userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);

            //TODO
            var password = "Password1!";

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(user, "Admin");

            emailHelper.SendEmailAsync(user.Email, password);
            return Ok($"ApplicationUser '{user.Email}' created successfully");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            const string unauthorizedMessage = "Wrong Email or Password";

            if (user?.Email is null)
            {
                return Unauthorized(unauthorizedMessage);
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                await userManager.AccessFailedAsync(user);
                return Unauthorized(unauthorizedMessage);
            }

            var token = jwtHelper.GenerateJwtToken(user.Id, user.UserType.ToString());
            await userManager.ResetAccessFailedCountAsync(user);

            return Ok(new { Token = token });
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("Refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
            var refreshTokenProtector = optionsMonitor.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
            var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

            if (refreshTicket?.Properties.ExpiresUtc is not { } expiresUtc ||
                DateTime.Now >= expiresUtc ||
                await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)
            {
                return Unauthorized();
            }

            var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
            return Ok(newPrincipal);
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
                return Ok();
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
                return NotFound();
            }

            var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!checkPasswordResult.Succeeded)
            {
                return BadRequest();
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
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