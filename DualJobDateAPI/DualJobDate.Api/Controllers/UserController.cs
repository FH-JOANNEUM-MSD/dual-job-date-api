using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using AutoMapper;
using DualJobDate.API.Models;
using DualJobDate.API.Resources;
using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DualJobDate.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IServiceProvider serviceProvider,
    IMapper _mapper,
    IEmailSender _emailSender)
    : ControllerBase
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();


    [HttpPut]
    [Route("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel model, string role)
    {
        var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();
        var emailStore = (IUserEmailStore<User>)userStore;
        var email = model.Email;

        if (string.IsNullOrEmpty(email) || !EmailAddressAttribute.IsValid(email))
        {
            return BadRequest($"Email '{email}' is invalid.");
        }

        var user = new User { Email = model.Email };
        await userStore.SetUserNameAsync(user, email, CancellationToken.None);
        const string password = "Password!1";
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await userManager.AddToRoleAsync(user, role);
        
        _emailSender.SendEmailAsync(user.Email, password);
        return Ok($"User '{user.Email}' created successfully");
    }
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
    {
        var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
        var isPersistent = (useCookies == true) && (useSessionCookies != true);
        signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent, true);
        
        if (!result.Succeeded)
        {
            return Unauthorized("Wrong Email or Password");
        }

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        return Ok();
    }
    
    [HttpPost]
    [Route("Refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest refreshRequest)
    { 
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
        var refreshTokenProtector = optionsMonitor.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
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
        if (user == null) return BadRequest("Benutzer nicht gefunden.");

        var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (result.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }
    [HttpPost]
    [Route("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] RegisterUserModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
        {
            return Ok();
        }

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action("ResetPassword", "User", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
        //await _emailSender.SendEmailAsync(model.Email, "Passwort zurücksetzen",
        //    $"Bitte setzen Sie Ihr Passwort zurück, indem Sie <a href='{callbackUrl}'>hier klicken</a>.");

        return Ok();
    }

    [HttpPost]
    [Route("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("Ein Fehler ist aufgetreten.");
        }

        var result = await userManager.ResetPasswordAsync(user, model.ResetCode, model.NewPassword);
        if (result.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }
    
    [Authorize(Policy = "Admin")]
    [HttpGet]
    [Route("GetAllUsers")]
    public Task<ActionResult<IEnumerable<UserResource>>> GetAllUsers()
    {
        var users = userManager.Users.ToList();
        var userResources = _mapper.Map<List<User>, List<UserResource>>(users);
        return Task.FromResult<ActionResult<IEnumerable<UserResource>>>(Ok(userResources));
    }

    
    [Authorize(Policy = "Admin")]
    [HttpDelete]
    [Route("DeleteUser/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
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
    
    [HttpPost]
    [Route("DeleteUserWithPassword")]
    public async Task<IActionResult> DeleteUserWithPassword([FromBody] DeleteUserModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Benutzer nicht gefunden.");
        }

        var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
        if (!checkPasswordResult.Succeeded)
        {
            return BadRequest("Falsches Passwort.");
        }

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Ok("Benutzer erfolgreich gelöscht.");
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

    // Generiert ein Passwort der angegebenen Länge mit mindestens einem Zeichen aus jeder Kategorie
    public static string GeneratePassword(int length = 12)
    {
        if (length < 4) throw new ArgumentException("Länge muss mindestens 4 Zeichen betragen.", nameof(length));

        string[] charCategories = new string[]
        {
            LowerCase,
            UpperCase,
            Digits,
            SpecialChars
        };

        byte[] data = new byte[length];
        using (var crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetBytes(data);
        }

        char[] passwordChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            int categoryIndex = data[i] % charCategories.Length;
            int charIndex = data[i] % charCategories[categoryIndex].Length;

            passwordChars[i] = charCategories[categoryIndex][charIndex];
        }

        // Stellen Sie sicher, dass das Passwort mindestens ein Zeichen aus jeder Kategorie enthält
        EnsureEachCategory(passwordChars, charCategories);

        return new string(passwordChars);
    }

    private static void EnsureEachCategory(char[] passwordChars, string[] charCategories)
    {
        Random random = new Random();
        for (int i = 0; i < charCategories.Length; i++)
        {
            if (!passwordChars.Any(p => charCategories[i].Contains(p)))
            {
                // Ersetzt ein zufälliges Zeichen im Passwort durch ein Zeichen aus der fehlenden Kategorie
                int replaceIndex = random.Next(passwordChars.Length);
                passwordChars[replaceIndex] = charCategories[i][random.Next(charCategories[i].Length)];
            }
        }
    }
}