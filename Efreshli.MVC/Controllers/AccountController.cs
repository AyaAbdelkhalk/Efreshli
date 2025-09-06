using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Services.AuthServices;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.MVC.Controllers
{
    //[Authorize(UserRoles.Admin)]
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(IAuthService authService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password,false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                   var user=await _userManager.FindByEmailAsync(loginDto.Email);
                    if (user != null && _userManager.IsInRoleAsync(user, UserRoles.Admin).Result)
                    {

                        return RedirectToAction ( "index", "home");
                    }
                    else
                    {
                      await  _signInManager.SignOutAsync();
                     ModelState.AddModelError(string.Empty, "Access denied. Admin privileges required.");
                        return View(loginDto);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginDto);
                }
            
            }

            return View(loginDto);
        }
        [Authorize(UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> CreateAdmin(CreateAdminDto adminDto)
        {
            if(ModelState.IsValid)
            {
               var result =await _authService.CreateAdminAsync(adminDto);
                if (result.Succeeded)
                    return RedirectToAction("index", "User");
                foreach (var itr in result?.Errors)
                {
                    ModelState.AddModelError("", itr);
                    return View(adminDto);
                }
            }

            return View(adminDto);
        }



        public async Task<IActionResult> Logout()
        {
          await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
