﻿using Blog.Entity.Dtos.Categories;
using Blog.Entity.Entities;
using Microsoft.AspNetCore.Http;

namespace Blog.Entity.Dtos.Articles
{
    public class ArticleUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CategoryId { get; set; }
        public Image Image { get; set; }
        public IFormFile? Photo { get; set; }
        public IList<CategoryDto> Categories { get; set; }
    }
}
