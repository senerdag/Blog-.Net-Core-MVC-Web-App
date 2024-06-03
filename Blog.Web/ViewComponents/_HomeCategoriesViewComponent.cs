using Blog.Service.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.ViewComponents
{
    public class _HomeCategoriesViewComponent:ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public _HomeCategoriesViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.Get5CategoriesNonDeleted();



            return View(categories);

           
        }
    }
}
