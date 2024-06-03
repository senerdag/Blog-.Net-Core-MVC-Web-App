using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.Dtos.Articles;
using Blog.Entity.Dtos.Categories;
using Blog.Entity.Entities;
using Blog.Service.Extensions;
using Blog.Service.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.Services.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesNonDeleted()
        {

            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(x=>!x.IsDeleted);
            var map= _mapper.Map<List<CategoryDto>>(categories);

            return map;

        }
        public async Task CreateCategoryAsync(CategoryAddDto categoryAddDto)
        {
            var userId = _contextAccessor.HttpContext.User.GetLoggedInUserId();
            var userEmail = _contextAccessor.HttpContext.User.GetLoggedInEmail();
            Category category = new(categoryAddDto.Name,userEmail);
            await _unitOfWork.GetRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();
            
        }
        public async Task<Category> GetCategoryByGuid(Guid id)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetByGuidAsync(id);
            return category;
        }
        public async Task<string> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
        {
            var userEmail = _contextAccessor.HttpContext.User.GetLoggedInEmail();
            var category = await _unitOfWork.GetRepository<Category>().GetAsync(x => x.Id == categoryUpdateDto.Id && !x.IsDeleted);
            

            category.Name = categoryUpdateDto.Name;
            category.ModifiedBy = userEmail;
            category.ModifiedDate = DateTime.Now;



            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();

            return category.Name;
        }
        public async Task<string> SafeDeleteCategoryAsync(Guid categoryId)
        {
            var userEmail = _contextAccessor.HttpContext.User.GetLoggedInEmail();
            var category = await _unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryId);
            category.IsDeleted = true;
            category.DeletedDate = DateTime.Now;
            category.DeletedBy = userEmail;

            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);

            await _unitOfWork.SaveAsync();
            return category.Name;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesDeleted()
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(x => x.IsDeleted);
            var map = _mapper.Map<List<CategoryDto>>(categories);

            return map;
        }

        public async Task<string> UndoDeleteCategoryAsync(Guid categoryId)
        {
            var userEmail = _contextAccessor.HttpContext.User.GetLoggedInEmail();
            var category = await _unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryId);
            category.IsDeleted = false;
            category.DeletedDate = null;
            category.DeletedBy = null;

            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);

            await _unitOfWork.SaveAsync();
            return category.Name;
        }

        public async Task<List<CategoryDto>> Get5CategoriesNonDeleted()
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.IsDeleted && (x.Articles.Count!=0));
            categories = categories.TakeLast(5).ToList();
            var map = _mapper.Map<List<CategoryDto>>(categories);
            return map;
        }
    }
}
