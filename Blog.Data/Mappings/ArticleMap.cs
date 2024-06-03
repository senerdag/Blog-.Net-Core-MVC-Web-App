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
	public class ArticleMap : IEntityTypeConfiguration<Article>
	{
		public void Configure(EntityTypeBuilder<Article> builder)
		{
			builder.HasData(new Article
			{
				Id = Guid.NewGuid(),
				Title = "Why Deftones so good?",
				Content = "Deftones Nisl pretium fusce id velit ut tortor pretium viverra. Sit amet risus nullam eget felis. Fames ac turpis egestas maecenas pharetra convallis. Dictumst vestibulum rhoncus est pellentesque elit ullamcorper dignissim cras tincidunt. Ornare arcu odio ut sem. Elit pellentesque habitant morbi tristique. Diam quam nulla porttitor massa id neque aliquam vestibulum morbi. Magnis dis parturient montes nascetur. A cras semper auctor neque vitae. Nunc lobortis mattis aliquam faucibus purus in massa tempor nec. Risus in hendrerit gravida rutrum. A arcu cursus vitae congue mauris rhoncus aenean vel elit. Vulputate ut pharetra sit amet aliquam id diam maecenas ultricies. Nullam vehicula ipsum a arcu cursus. Vivamus arcu felis bibendum ut. Tristique et egestas quis ipsum.",
				ViewCount = 15,
				
				CategoryId= Guid.Parse("1AEE4E85-738B-4C57-A50C-AD06BACF4B76"),
				
				ImageId= Guid.Parse("6CDFC8BE-3005-42F1-973C-3E91B987110D"),
				CreatedBy = "Admin Test",
				CreatedDate = DateTime.Now,
				IsDeleted=false,
				UserId = Guid.Parse("58307B4E-1D04-4558-B4FE-1DBFA3192662")
			},
			new Article
			{

				Id = Guid.NewGuid(),
				Title = "Why Slowdive so good?",
				Content = "Slowdive Sit amet risus nullam eget felis. Fames ac turpis egestas maecenas pharetra convallis. Dictumst vestibulum rhoncus est pellentesque elit ullamcorper dignissim cras tincidunt. Ornare arcu odio ut sem. Elit pellentesque habitant morbi tristique. Diam quam nulla porttitor massa id neque aliquam vestibulum morbi. Magnis dis parturient montes nascetur. A cras semper auctor neque vitae. Nunc lobortis mattis aliquam faucibus purus in massa tempor nec. Risus in hendrerit gravida rutrum. A arcu cursus vitae congue mauris rhoncus aenean vel elit. Vulputate ut pharetra sit amet aliquam id diam maecenas ultricies. Nullam vehicula ipsum a arcu cursus. Vivamus arcu felis bibendum ut. Tristique et egestas quis ipsum.",
				ViewCount = 15,
				CategoryId=Guid.Parse("3B8B3761-A1E7-4BC4-939B-39984FC65870"),
				ImageId= Guid.Parse("883917E7-53F3-4C8F-AA49-8D93A74C902A"),
				CreatedBy = "Admin Test",
				CreatedDate = DateTime.Now,
				IsDeleted = false,
                UserId = Guid.Parse("098C301B-A423-4E2E-A44F-4C6BCC74CE83")

            });


		}
	}
}
