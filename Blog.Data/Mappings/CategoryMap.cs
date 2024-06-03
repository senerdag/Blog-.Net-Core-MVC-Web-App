using Blog.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Mappings
{
	public class CategoryMap : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.HasData(new Category
			{

				Id = Guid.Parse("1AEE4E85-738B-4C57-A50C-AD06BACF4B76"),
				Name = "Bands",
				CreatedBy = "Admin Test",
				CreatedDate = DateTime.Now,
				IsDeleted = false,

			},
			new Category
			{
				Id = Guid.Parse("3B8B3761-A1E7-4BC4-939B-39984FC65870"),
				Name = "Slowdive",
				CreatedBy = "Admin Test",
				CreatedDate = DateTime.Now,
				IsDeleted = false



			});


		}
	}
}
