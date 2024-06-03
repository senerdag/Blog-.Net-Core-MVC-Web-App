using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.Dtos.Users;
using Blog.Entity.Entities;
using Blog.Entity.Enums;
using Blog.Service.Extensions;
using Blog.Service.Helpers.Images;
using Blog.Service.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IImageHelper _imageHelper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IImageHelper imageHelper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _imageHelper = imageHelper;
            _contextAccessor = contextAccessor;
        }

        public async Task<IdentityResult> CreateUserAsync(UserAddDto userAddDto)
        {
            var map = _mapper.Map<AppUser>(userAddDto);

            map.UserName = userAddDto.Email;
            var result = await _userManager.CreateAsync(map, string.IsNullOrEmpty(userAddDto.Password) ? "" : userAddDto.Password);
            if (result.Succeeded)
            {
                var findRole = await _roleManager.FindByIdAsync(userAddDto.RoleId.ToString());
                await _userManager.AddToRoleAsync(map, findRole.ToString());
                return result;
            }
            else
            {
                return result;
            }
        }

        public async Task<(IdentityResult identityResult, string? email)> DeleteUserAsync(Guid userId)
        {
            var user = await GetAppUserByIdAsync(userId);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return (result,user.Email);
            }
            else { return (result, null); }
        }

        public async Task<List<AppRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();

        }

        public async Task<List<UserDto>> GetAllUsersWithRoleAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var map = _mapper.Map<List<UserDto>>(users);

            foreach (var item in map)
            {
                var findUser = await _userManager.FindByIdAsync(item.Id.ToString());
                var role = string.Join("", await _userManager.GetRolesAsync(findUser));
                item.Role = role;
            }

            return map;
        }

        public async Task<AppUser> GetAppUserByIdAsync(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<string> GetUserRoleAsync(AppUser user)
        {
            return string.Join("", await _userManager.GetRolesAsync(user));
        }

        public async Task<IdentityResult> UpdateUserAsync(UserUpdateDto userUpdateDto)
        {
            var user = await GetAppUserByIdAsync(userUpdateDto.Id);
            var userRole = await GetUserRoleAsync(user);


            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.RemoveFromRoleAsync(user, userRole);
                var findRole = await _roleManager.FindByIdAsync(userUpdateDto.RoleId.ToString());
                await _userManager.AddToRoleAsync(user, findRole.Name);
                return result;
            }
            else
                return result;
        }

        public async Task<UserProfilDto> GetUserProfileAsync()
        {
            var userId = _contextAccessor.HttpContext.User.GetLoggedInUserId();
            var getUserWithImage = await _unitOfWork.GetRepository<AppUser>().GetAsync(x => x.Id == userId, x => x.Image);
            var map = _mapper.Map<UserProfilDto>(getUserWithImage);
            map.Image.FileName = getUserWithImage.Image.FileName;
            return map;
        }

        private async Task<Guid> UploadImageForUser(UserProfilDto userProfilDto)
        {
            var userEmail = _contextAccessor.HttpContext.User.GetLoggedInEmail();
            var imageUpload = await _imageHelper.Upload($"{userProfilDto.FirstName}{userProfilDto.LastName}", userProfilDto.Photo, ImageType.User);
            Image image = new(imageUpload.FullName, userProfilDto.Photo.ContentType, userEmail);
            await _unitOfWork.GetRepository<Image>().AddAsync(image);
            return image.Id;
        }
        public async Task<bool> UserProfileUpdateAsyc(UserProfilDto userProfilDto)
        {
            if(userProfilDto.CurrentPassword==userProfilDto.NewPassword)
            {
                return false;
            }

            var userId = _contextAccessor.HttpContext.User.GetLoggedInUserId();
            var user = await GetAppUserByIdAsync(userId);
            var tempImageId = user.ImageId;



            var isVerified = await _userManager.CheckPasswordAsync(user, userProfilDto.CurrentPassword);
            if (isVerified && userProfilDto.NewPassword != null )
            {
                var result = await _userManager.ChangePasswordAsync(user, userProfilDto.CurrentPassword, userProfilDto.NewPassword);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.SignOutAsync();
                    await _signInManager.PasswordSignInAsync(user, userProfilDto.NewPassword, true, false);

                    _mapper.Map(userProfilDto, user);

                    if(userProfilDto.Photo!=null)
                    {
                        user.ImageId = await UploadImageForUser(userProfilDto);
                    }
                    else if (userProfilDto.Photo == null)
                    {
                        user.ImageId = tempImageId;
                    }


                    await _userManager.UpdateAsync(user);

                    await _unitOfWork.SaveAsync();

                    return true;

                }
                else
                {
                    return false;
                }
            }
            else if (isVerified )
            {
                await _userManager.UpdateSecurityStampAsync(user);
                _mapper.Map(userProfilDto, user);

                if (userProfilDto.Photo != null)
                {
                    user.ImageId = await UploadImageForUser(userProfilDto);
                }
                else if(userProfilDto.Photo == null)
                {
                    user.ImageId = tempImageId;
                }

                await _userManager.UpdateAsync(user);

                await _unitOfWork.SaveAsync();
                return true;
            }
            else
            {
                return false;

            }


           
        }
    }
}
