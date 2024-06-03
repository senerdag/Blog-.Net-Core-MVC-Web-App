using AutoMapper;
using Blog.Entity.Dtos.Users;
using Blog.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.ViewComponents
{
    public class _DashboardHeadViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public _DashboardHeadViewComponent(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var map= _mapper.Map<UserDto>(loggedUser);

            var role= string.Join("",await _userManager.GetRolesAsync(loggedUser));

            map.Role = role;
            return View(map);
        }
    }
}
