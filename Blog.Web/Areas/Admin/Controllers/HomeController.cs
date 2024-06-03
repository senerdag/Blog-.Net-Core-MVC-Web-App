using Blog.Entity.Entities;
using Blog.Service.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IDashboardService _boardService;

        public HomeController(IArticleService articleService, IDashboardService boardService)
        {
            _articleService = articleService;
            _boardService = boardService;
        }

        public async Task<IActionResult> Index()
        {
            var articles=await _articleService.GetAllArticleWithCategoryNonDeletedAsync();
            


            return View(articles);
        }
        [HttpGet]
        public async Task<IActionResult> YearlyArticleCounts()
        {
            var count = await _boardService.GetYearlyArticleCount();
            return Json(JsonConvert.SerializeObject(count));
        }
    }
}
