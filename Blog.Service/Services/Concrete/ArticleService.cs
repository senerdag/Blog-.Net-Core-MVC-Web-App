using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.Dtos.Articles;
using Blog.Entity.Entities;
using Blog.Entity.Enums;
using Blog.Service.Extensions;
using Blog.Service.Helpers.Images;
using Blog.Service.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.Services.Concrete
{
	public class ArticleService : IArticleService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IImageHelper _imageHelper;


        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor, IImageHelper imageHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _imageHelper = imageHelper;
        }

        public async Task<ArticleListDto> GetAllByPagingAsync(Guid? categoryId, int currentPage=1, int pageSize=3, bool isAscending=false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;

            var articles = categoryId == null ? await _unitOfWork.GetRepository<Article>().GetAllAsync(a => !a.IsDeleted, a => a.Category, i => i.Image,u=>u.User) : await _unitOfWork.GetRepository<Article>().GetAllAsync(a => a.CategoryId == categoryId && !a.IsDeleted, x => x.Category, i => i.Image, u => u.User);

            var sortedArticles = isAscending ? articles.OrderBy(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList() : articles.OrderByDescending(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new ArticleListDto
            {
                Articles=sortedArticles,
                CategoryId=categoryId==null?null:categoryId.Value,
                CurrentPage=currentPage,
                IsAscending=isAscending,
                PageSize=pageSize,
                TotalCount=articles.Count
                

            };

        }
        public async Task CreateArticleAsync(ArticleAddDto articleAddDto)
        {
            //var userId =  Guid.Parse("58307B4E-1D04-4558-B4FE-1DBFA3192662");

            var userId= _contextAccessor.HttpContext.User.GetLoggedInUserId();
            var userEmail= _contextAccessor.HttpContext.User.GetLoggedInEmail();

            var imageUpload = await _imageHelper.Upload(articleAddDto.Title,articleAddDto.Photo,ImageType.Post);
            Image image = new(imageUpload.FullName,articleAddDto.Photo.ContentType,userEmail);
            await _unitOfWork.GetRepository<Image>().AddAsync(image);

            var article = new Article(articleAddDto.Title, articleAddDto.Content, userId,articleAddDto.CategoryId, image.Id,userEmail);
           
            await _unitOfWork.GetRepository<Article>().AddAsync(article);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<ArticleDto>> GetAllArticleWithCategoryNonDeletedAsync()
		{
			
			var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(x=>!x.IsDeleted,x=>x.Category);
            var map = _mapper.Map<List<ArticleDto>>(articles);
			return map;
		}

        public async Task<ArticleDto> GetArticleWithCategoryNonDeletedAsync(Guid articleId)
        {
            var article = await _unitOfWork.GetRepository<Article>().GetAsync(x=>x.Id==articleId && !x.IsDeleted,x=>x.Category,i=>i.Image,u=>u.User);
            var map = _mapper.Map<ArticleDto>(article);
            return map;
        }

        public async Task<string> UpdateArticleAsync(ArticleUpdateDto articleUpdateDto)
        {
            var userEmail = _contextAccessor.HttpContext.User.GetLoggedInEmail();
            var article = await _unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == articleUpdateDto.Id && !x.IsDeleted, x => x.Category, i=>i.Image);
           
            if(articleUpdateDto.Photo!=null)
            {
                _imageHelper.Delete(article.Image.FileName);

                var imageUpload = await _imageHelper.Upload(articleUpdateDto.Title, articleUpdateDto.Photo,ImageType.Post);
                Image image = new(imageUpload.FullName, articleUpdateDto.Photo.ContentType, userEmail);
                await _unitOfWork.GetRepository<Image>().AddAsync(image);

                article.ImageId = image.Id;
            }

            article.Title = articleUpdateDto.Title;
            article.Content = articleUpdateDto.Content;
            article.CategoryId= articleUpdateDto.CategoryId; 
            article.ModifiedDate= DateTime.Now;
            article.ModifiedBy = userEmail;



            await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await _unitOfWork.SaveAsync();


            return article.Title;
        }

        public async Task<string> SafeDeleteArticleAsync(Guid articleId)
        {
            var userEmail = _contextAccessor.HttpContext.User.GetLoggedInEmail();
            var article= await _unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);
            article.IsDeleted = true;
            article.DeletedDate = DateTime.Now;
            article.DeletedBy = userEmail;

            await _unitOfWork.GetRepository<Article>().UpdateAsync(article);

            await _unitOfWork.SaveAsync();
            return article.Title;

        }
        public async Task<List<ArticleDto>> GetAllArticleWithCategoryDeletedAsync()
        {

            var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(x => x.IsDeleted, x => x.Category);
            var map = _mapper.Map<List<ArticleDto>>(articles);
            return map;
        }

        public async Task<string> UndoDelete(Guid articleId)
        {
            var userEmail = _contextAccessor.HttpContext.User.GetLoggedInEmail();
            var article = await _unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);
            article.IsDeleted = false;
            article.DeletedDate = null;
            article.DeletedBy = null;

            await _unitOfWork.GetRepository<Article>().UpdateAsync(article);

            await _unitOfWork.SaveAsync();
            return article.Title;
        }

        public async Task<ArticleListDto> SearchAsync(string keyWord, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;

            var articles =   await _unitOfWork.GetRepository<Article>().GetAllAsync(a => !a.IsDeleted &&(a.Title.Contains(keyWord)|| a.Content.Contains(keyWord) || a.Category.Name.Contains(keyWord)), a => a.Category, i => i.Image, u => u.User) ;

            var sortedArticles = isAscending ? articles.OrderBy(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList() : articles.OrderByDescending(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new ArticleListDto
            {
                Articles = sortedArticles,
                
                CurrentPage = currentPage,
                IsAscending = isAscending,
                PageSize = pageSize,
                TotalCount = articles.Count


            };
        }
        public async Task<ArticleListDto> SearchCategoriesAsync(string keyWord, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;

            var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(a => !a.IsDeleted && ( a.Category.Name.Contains(keyWord)), a => a.Category, i => i.Image, u => u.User);

            var sortedArticles = isAscending ? articles.OrderBy(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList() : articles.OrderByDescending(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new ArticleListDto
            {
                Articles = sortedArticles,

                CurrentPage = currentPage,
                IsAscending = isAscending,
                PageSize = pageSize,
                TotalCount = articles.Count


            };
        }
    }
}
