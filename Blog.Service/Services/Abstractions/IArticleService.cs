using Blog.Entity.Dtos.Articles;
using Blog.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.Services.Abstractions
{
	public interface IArticleService
	{
		Task<List<ArticleDto>> GetAllArticleWithCategoryNonDeletedAsync();	
		Task CreateArticleAsync(ArticleAddDto articleAddDto);

		Task<ArticleDto> GetArticleWithCategoryNonDeletedAsync(Guid articleId);

		Task<string> UpdateArticleAsync(ArticleUpdateDto articleUpdateDto);
		Task<string> SafeDeleteArticleAsync(Guid articleId);
		Task<string> UndoDelete(Guid articleId);
		Task<List<ArticleDto>> GetAllArticleWithCategoryDeletedAsync();
		Task<ArticleListDto> GetAllByPagingAsync(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false);
        Task<ArticleListDto> SearchAsync(string keyWord, int currentPage = 1, int pageSize = 3, bool isAscending = false);
		Task<ArticleListDto> SearchCategoriesAsync(string keyWord, int currentPage = 1, int pageSize = 3, bool isAscending = false);

    }
}
