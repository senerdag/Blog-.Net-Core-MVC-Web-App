using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.Dtos.Articles;
using Blog.Entity.Dtos.Users;
using Blog.Entity.Entities;
using Blog.Entity.Enums;
using Blog.Service.Helpers.Images;
using Blog.Service.Services.Abstractions;
using Blog.Web.ResultMessages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toast;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IImageHelper _imageHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public UserController(UserManager<AppUser> userManager, IMapper mapper, RoleManager<AppRole> roleManager, IToastNotification toast, SignInManager<AppUser> signInManager, IImageHelper imageHelper, IUnitOfWork unitOfWork, IUserService userService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _toast = toast;
            _signInManager = signInManager;
            _imageHelper = imageHelper;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _userService.GetAllUsersWithRoleAsync();
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var roles = await _userService.GetAllRolesAsync();
            return View(new UserAddDto { Roles = roles});
        }
        [HttpPost]
        public async Task<IActionResult> Add(UserAddDto userAddDto)
        {
            var map = _mapper.Map<AppUser>(userAddDto);
            var roles = await _userService.GetAllRolesAsync();

            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserAsync(userAddDto);
                if(result.Succeeded)
                {
                    
                    _toast.AddSuccessToastMessage(Message.Article.Add(userAddDto.Email));
                    return RedirectToAction("Index", "User", new { Area = "Admin" });
                }
                else
                {
                    foreach (var errors in result.Errors)
                    {
                        ModelState.AddModelError("", errors.Description);
                    }
                    return View(new UserAddDto { Roles = roles });
                }
            }
           
            return View(new UserAddDto { Roles = roles });
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid userId)
        {
            var user = await _userService.GetAppUserByIdAsync(userId);

            var roles = await _userService.GetAllRolesAsync();

            var map=_mapper.Map<UserUpdateDto>(user);
            map.Roles= roles;


            return View(map);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
        {
            var user = await _userService.GetAppUserByIdAsync(userUpdateDto.Id);
            if(user != null)
            {
                var roles = await _userService.GetAllRolesAsync(); 
                if(ModelState.IsValid)
                {
                    _mapper.Map(userUpdateDto, user);
                    user.UserName = userUpdateDto.Email;
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    var result = await _userService.UpdateUserAsync(userUpdateDto);
                    if(result.Succeeded)
                    {
                       
                        _toast.AddSuccessToastMessage(Message.Article.Update(userUpdateDto.Email));
                        return RedirectToAction("Index", "User", new { Area = "Admin" });
                    }
                    else
                    {
                        foreach (var errors in result.Errors)
                        {
                            ModelState.AddModelError("", errors.Description);
                        }
                        return View(new UserUpdateDto { Roles = roles });
                    }
                }

            }


            return NotFound();
        }
        public async Task<IActionResult> Delete(Guid userId)
        {
            

            var result = await _userService.DeleteUserAsync(userId);
                
            if(result.identityResult.Succeeded)
            {
                _toast.AddSuccessToastMessage(Message.Article.Delete(result.email));
                return RedirectToAction("Index", "User", new { Area = "Admin" });
            }
            else
            {
                foreach (var errors in result.identityResult.Errors)
                {
                    ModelState.AddModelError("", errors.Description);
                }

            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var profile = await _userService.GetUserProfileAsync();

            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfilDto userProfilDto)
        {
            if(userProfilDto.CurrentPassword==null)
            {
                _toast.AddErrorToastMessage("Something went wrong!");
                return View(await _userService.GetUserProfileAsync());
            }
            if (ModelState.IsValid)
            {
                var result = await _userService.UserProfileUpdateAsyc(userProfilDto);
                if(result)
                {
                    _toast.AddSuccessToastMessage("Profile is successfully updated!");
                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
                else
                {
                    var profile =await _userService.GetUserProfileAsync();

                    _toast.AddErrorToastMessage("Something went wrong!");
                    return View(profile);
                }
                
                
            }
            _toast.AddErrorToastMessage("Something went wrong!");
            return View(await _userService.GetUserProfileAsync());
        }
    }
}
