using Blog.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Entity.Entities
{
    public class AppUser : IdentityUser<Guid>, IEntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }


        public Guid ImageId { get; set; } = Guid.Parse("bf18cd9e-03d9-484b-ad98-74f1dcf3f138");
        public Image Image { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
