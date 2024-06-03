using Blog.Data.UnitOfWorks;
using Blog.Service.Services.Abstractions;
using Blog.Web.Models;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Diagnostics;

namespace Blog.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IArticleService _articleService;
        private readonly IToastNotification _toast;

        public HomeController(ILogger<HomeController> logger, IArticleService articleService, IToastNotification toast)
        {
            _logger = logger;
            _articleService = articleService;
            _toast = toast;
          
        }
        [HttpGet]
        public async Task<IActionResult> Index(Guid? categoryId, int currentPage=1,int pageSize=3,bool isAscending=false)
		{
			var articles = await _articleService.GetAllByPagingAsync(categoryId,currentPage,pageSize,isAscending);
			return View(articles);
		}
		[HttpGet]
        public async Task<IActionResult> Search(string keyWord, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {

            var articles = await _articleService.SearchAsync(keyWord, currentPage, pageSize, isAscending);
            if (keyWord == null || keyWord.Length == 0)
            {
                _toast.AddWarningToastMessage("Something went wrong.");
                return RedirectToAction("Index");
            }
            else if (articles.TotalCount==0)
            {
                _toast.AddWarningToastMessage($"There is no article for {keyWord}.");
                return RedirectToAction("Index");
            }
                return View(articles);
        }
        [HttpGet]
        public async Task<IActionResult> SearchCategories(string keyWord, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {

            var articles = await _articleService.SearchCategoriesAsync(keyWord, currentPage, pageSize, isAscending);
            if (keyWord == null || keyWord.Length == 0)
            {
                _toast.AddWarningToastMessage("Something went wrong.");
                return RedirectToAction("Index");
            }
            else if (articles.TotalCount == 0)
            {
                _toast.AddWarningToastMessage($"There is no article for {keyWord}.");
                return RedirectToAction("Index");
            }
            return View(articles);
        }
        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
        public async Task<IActionResult> Detail(Guid id)
        {
            var article  = await _articleService.GetArticleWithCategoryNonDeletedAsync(id);
            return View(article);

        }
	}
}
