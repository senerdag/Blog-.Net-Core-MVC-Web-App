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
    public class UserRoleMap : IEntityTypeConfiguration<AppUserRole>
    {
        public void Configure(EntityTypeBuilder<AppUserRole> builder)
        {
            // Primary key
            builder.HasKey(r => new { r.UserId, r.RoleId });

            // Maps to the AspNetUserRoles table
            builder.ToTable("AspNetUserRoles");

            builder.HasData(new AppUserRole
            {
               
                UserId= Guid.Parse("58307B4E-1D04-4558-B4FE-1DBFA3192662"),
                RoleId= Guid.Parse("937EFD37-7F98-4E42-8108-0151D943A824")
            },
            new AppUserRole
            {
                UserId = Guid.Parse("098C301B-A423-4E2E-A44F-4C6BCC74CE83"),
                RoleId = Guid.Parse("3CA9D9B6-A76E-4CE7-BB80-290790EEA31C")
            });
        }
    }
}
