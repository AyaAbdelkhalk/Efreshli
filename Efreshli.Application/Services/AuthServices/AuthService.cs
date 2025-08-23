using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.EmailService;
using Efreshli.Application.Services.RoleService;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Efreshli.Domain.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Efreshli.Application.Resources.SharedResourcesKeys;

namespace Efreshli.Application.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleService _roleService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings,
            IEmailService emailService,
            RoleManager<IdentityRole> roleManager,
            IRoleService roleService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
            _roleService = roleService;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<Response<string>> RegisterAsync(RegisterDto model)
        {
            ApplicationUser createdUser = null;
            bool userCreated = false;
            bool roleAssigned = false;

            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return ResponseHandler.BadRequest<string>("User with this email already exists");
                }

                // Ensure roles exist
                await _roleService.EnsureRolesExistAsync();

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CreatedBy = model.Email,
                    CreatedDate = DateTime.UtcNow,
                    EmailConfirmed = false
                };

                // Create user
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResponseHandler.ValidationError<string>(errors);
                }

                userCreated = true;
                createdUser = user;

                // Assign Customer role
                var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.Customer);
                if (!roleResult.Succeeded)
                {
                    var roleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                    throw new InvalidOperationException($"Failed to assign user role: {string.Join(", ", roleErrors)}");
                }

                roleAssigned = true;

                // Generate email confirmation token
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));

                // Send confirmation email
                await _emailService.SendConfirmationEmailAsync(user.Email,encodedToken, user.FirstName);

                return ResponseHandler.Success("", "Registration successful. Please check your email to confirm your account.");
            }
            catch (Exception ex)
            {
                // Cleanup in reverse order of creation
                try
                {
                    if (userCreated && createdUser != null)
                    {
                        // Only delete if user was created but something else failed
                        if (roleAssigned)
                        {
                            // Remove role first (optional, but cleaner)
                            await _userManager.RemoveFromRoleAsync(createdUser, UserRoles.Customer);
                        }

                        await _userManager.DeleteAsync(createdUser);
                        _logger.LogInformation("Rolled back user creation due to failure: {Email}", model.Email);
                    }
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Failed to cleanup user during registration failure: {Email}", model.Email);
                    // Don't throw - we want to preserve the original exception
                }

                _logger.LogError(ex, "Registration failed for email: {Email}", model.Email);

                // Return user-friendly error message
                return ex switch
                {
                    InvalidOperationException => ResponseHandler.ValidationError<string>(ex.Message),
                    _ => ResponseHandler.BadRequest<string>($"Registration failed: {ex.Message}")
                };
            }
        }
        public async Task<Response<string>> LoginAsync(LoginDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return ResponseHandler.Unauthorized<string>("Invalid email or password");
                }

                // Check if email is confirmed
                if (!user.EmailConfirmed)
                {
                    return ResponseHandler.BadRequest<string>("Please confirm your email before logging in. Check your inbox or request a new confirmation email.");
                }

                // Use PasswordSignInAsync which handles the password check internally
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        return ResponseHandler.BadRequest<string>("Account locked out. Please try again later.");
                    }
                    if (result.IsNotAllowed)
                    {
                        return ResponseHandler.BadRequest<string>("Login not allowed. Please confirm your email.");
                    }

                    return ResponseHandler.Unauthorized<string>("Invalid email or password");
                }

                var tokenResult = await GenerateJwtTokenAsync(user);
                if (!tokenResult.Succeeded)
                {
                    return ResponseHandler.BadRequest<string>(tokenResult.Message);
                }

                return ResponseHandler.Success(tokenResult.Data, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {Email}", model.Email);
                return ResponseHandler.BadRequest<string>($"Login failed: {ex.Message}");
            }
        }
        

        public async Task<Response<bool>> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return ResponseHandler.Success(true, "If the email exists, a password reset link has been sent");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                // Send email with reset token
                await _emailService.SendPasswordResetEmailAsync(user.Email, encodedToken);

                return ResponseHandler.Success(true, "Password reset email sent");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<bool>($"Password reset failed: {ex.Message}");
            }
        }

        public async Task<Response<bool>> ResetPasswordAsync(ResetPasswordDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return ResponseHandler.NotFound<bool>("User not found");
                }

                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResponseHandler.ValidationError<bool>(errors);
                }

                return ResponseHandler.Success(true, "Password reset successfully");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<bool>($"Password reset failed: {ex.Message}");
            }
        }

        public async Task<Response<string>> CreateAdminAsync(CreateAdminDto model)
        {
            try
            {
                // Check if admin role exists
                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                }

                var adminUser = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CreatedBy = "System", // or current admin user
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(adminUser, model.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResponseHandler.ValidationError<string>(errors);
                }

                // Assign Admin role
                var roleResult = await _userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(adminUser);
                    var roleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                    return ResponseHandler.ValidationError<string>($"Failed to assign admin role: {string.Join(", ", roleErrors)}");
                }

                var tokenResult = await GenerateJwtTokenAsync(adminUser);
                if (!tokenResult.Succeeded)
                {
                    return ResponseHandler.BadRequest<string>(tokenResult.Message);
                }

                return ResponseHandler.Created(tokenResult.Data, "Admin user created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin creation failed for email: {Email}", model.Email);

                return ResponseHandler.BadRequest<string>($"Admin creation failed: {ex.Message}");
            }
        }

        private async Task<Response<string>> GenerateJwtTokenAsync(ApplicationUser user)
        {
            try
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                         new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                         ClaimValueTypes.Integer64),
                new Claim("fullName", user.FullName ?? "")
            };

                // Add user roles to claims
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return ResponseHandler.Success(tokenString);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<string>($"Token generation failed: {ex.Message}");
            }
        }
        //public async Task<Response<bool>> ConfirmEmailAsync(string email, string token)
        //{
        //    try
        //    {
        //        var user = await _userManager.FindByEmailAsync(email);
        //        if (user == null)
        //        {
        //            return ResponseHandler.NotFound<bool>("User not found");
        //        }

        //        if (user.EmailConfirmed)
        //        {
        //            return ResponseHandler.BadRequest<bool>("Email is already confirmed");
        //        }

        //        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        //        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        //        if (!result.Succeeded)
        //        {
        //            var errors = result.Errors.Select(e => e.Description).ToList();
        //            return ResponseHandler.ValidationError<bool>($"Email confirmation failed: {string.Join(", ", errors)}");
        //        }

        //        return ResponseHandler.Success(true, "Email confirmed successfully. You can now login.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Email confirmation failed for email: {Email}", email);
        //        return ResponseHandler.BadRequest<bool>($"Email confirmation failed: {ex.Message}");
        //    }
        //}
        public async Task<Response<bool>> ConfirmEmailAsync(string email, string token)
        {
            try
            {
                // Decode URL parameters
                var decodedEmail = WebUtility.UrlDecode(email);
                var decodedToken = WebUtility.UrlDecode(token);

                var user = await _userManager.FindByEmailAsync(decodedEmail);
                if (user == null)
                {
                    return ResponseHandler.NotFound<bool>("User not found");
                }

                if (user.EmailConfirmed)
                {
                    return ResponseHandler.BadRequest<bool>("Email is already confirmed");
                }

                // Decode base64 token
                var decodedBytes = WebEncoders.Base64UrlDecode(decodedToken);
                var originalToken = Encoding.UTF8.GetString(decodedBytes);

                var result = await _userManager.ConfirmEmailAsync(user, originalToken);

                if (!result.Succeeded)
                {
                    // Check for specific errors
                    if (result.Errors.Any(e => e.Code.Contains("InvalidToken")))
                    {
                        return ResponseHandler.BadRequest<bool>("Invalid or expired confirmation token. Please request a new confirmation email.");
                    }

                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResponseHandler.ValidationError<bool>($"Confirmation failed: {string.Join(", ", errors)}");
                }

                return ResponseHandler.Success(true, "Email confirmed successfully!");
            }
            catch (FormatException)
            {
                return ResponseHandler.BadRequest<bool>("Invalid token format. Please use the link from your email.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email confirmation failed for email: {Email}", email);
                return ResponseHandler.BadRequest<bool>($"Confirmation failed: {ex.Message}");
            }
        }
        public async Task<Response<bool>> ResendConfirmationEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Don't reveal that the user doesn't exist for security
                    return ResponseHandler.Success(true, "If the email exists, a confirmation email has been sent");
                }

                if (user.EmailConfirmed)
                {
                    return ResponseHandler.BadRequest<bool>("Email is already confirmed");
                }

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));

                await _emailService.SendConfirmationEmailAsync(user.Email, encodedToken, user.FirstName);

                return ResponseHandler.Success(true, "Confirmation email sent. Please check your inbox.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resend confirmation email to: {Email}", email);
                return ResponseHandler.BadRequest<bool>($"Failed to resend confirmation email: {ex.Message}");
            }
        }
    }
}