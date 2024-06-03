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
	public class ImageMap : IEntityTypeConfiguration<Image>
	{
		public void Configure(EntityTypeBuilder<Image> builder)
		{
			builder.HasData(new Image
			{
				
					Id = Guid.Parse("6CDFC8BE-3005-42F1-973C-3E91B987110D"),
					FileName = "images/testimages",
					FileType = "jpg",
					CreatedBy = "Admin Test",
					CreatedDate = DateTime.Now,
					IsDeleted = false,
				
			},
			new Image
			{
				
					Id = Guid.Parse("883917E7-53F3-4C8F-AA49-8D93A74C902A"),
					FileName = "slowdiveimages/testimages",
					FileType = "png",
					CreatedBy = "Admin Test",
					CreatedDate = DateTime.Now,
					IsDeleted = false,
				
			});
		}
	}
}
