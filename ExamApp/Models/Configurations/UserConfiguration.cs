using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamApp.Models.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(
                new User
                {
                    Id="1",
                    UserName = "Test",
                    PasswordHash = "Test123"
                },
                new User
                {
                    Id="2",
                    UserName = "Test2",
                    PasswordHash = "Test123"
                });
        }
    }
}
