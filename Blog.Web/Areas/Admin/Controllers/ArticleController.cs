using AutoMapper;
using Blog.Entity.Dtos.Articles;
using Blog.Entity.Entities;
using Blog.Service.Extensions;
using Blog.Service.Services.Abstractions;
using Blog.Web.Const;
using Blog.Web.ResultMessages;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IValidator<Article> _validator;
        private readonly IToastNotification _toast;

        public ArticleController(IArticleService articleService, ICategoryService categoryService, IMapper mapper, IValidator<Article> validator, IToastNotification toast)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _mapper = mapper;
            _validator = validator;
            _toast = toast;
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.SuperAdmin},{RoleConsts.Admin},{RoleConsts.User} ")]
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticleWithCategoryNonDeletedAsync();
            return View(articles);
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.SuperAdmin},{RoleConsts.Admin} ")]

        public async Task<IActionResult> Add()
        {
            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            return View(new ArticleAddDto { Categories = categories});
        }
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.SuperAdmin},{RoleConsts.Admin} ")]

        public async Task<IActionResult> Add(ArticleAddDto articleAddDto)
        {
            var map = _mapper.Map<Article>(articleAddDto);
            var result = await _validator.ValidateAsync(map);
           
            if(result.IsValid)
            {
                await _articleService.CreateArticleAsync(articleAddDto);
                _toast.AddSuccessToastMessage(Message.Article.Add(articleAddDto.Title));
                return RedirectToAction("Index", "Article", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelState(this.ModelState);

                var categories = await _categoryService.GetAllCategoriesNonDeleted();
                return View(new ArticleAddDto { Categories = categories });
            }

            
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.SuperAdmin},{RoleConsts.Admin} ")]

        public async Task<IActionResult> Update(Guid articleId)
        {
            var articles = await _articleService.GetArticleWithCategoryNonDeletedAsync(articleId);
            var categories = await _categoryService.GetAllCategoriesNonDeleted();

            var articleUpdateDto = _mapper.Map<ArticleUpdateDto>(articles);
            articleUpdateDto.Categories = categories;


            return View(articleUpdateDto);
        }
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.SuperAdmin},{RoleConsts.Admin} ")]
        public async Task<IActionResult> Update(ArticleUpdateDto articleUpdateDto)
        {
            var map = _mapper.Map<Article>(articleUpdateDto);
            var result = await _validator.ValidateAsync(map);

            if (result.IsValid)
            {
                var title= await _articleService.UpdateArticleAsync(articleUpdateDto);
                _toast.AddSuccessToastMessage(Message.Article.Update(title));
                return RedirectToAction("Index", "Article", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelState(this.ModelState) ;
                var categories = await _categoryService.GetAllCategoriesNonDeleted();

                articleUpdateDto.Categories = categories;


                return View(articleUpdateDto);
            }
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.SuperAdmin},{RoleConsts.Admin} ")]
        public async Task<IActionResult> Delete(Guid articleId)
        {
           var title= await _articleService.SafeDeleteArticleAsync(articleId);
            _toast.AddSuccessToastMessage(Message.Article.Delete(title));
            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.SuperAdmin},{RoleConsts.Admin} ")]

        public async Task<IActionResult> DeletedArticles()
        {
            var articles = await _articleService.GetAllArticleWithCategoryDeletedAsync();
            return View(articles);
        }
        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.SuperAdmin},{RoleConsts.Admin} ")]
        public async Task<IActionResult> Undelete(Guid articleId)
        {
            var title = await _articleService.UndoDelete(articleId);
            _toast.AddSuccessToastMessage(Message.Article.UndoDelete(title));
            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }


    }
}
