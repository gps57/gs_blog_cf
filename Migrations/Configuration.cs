namespace gs_blog_cf.Migrations
{
    using gs_blog_cf.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;


    internal sealed class Configuration : DbMigrationsConfiguration<gs_blog_cf.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(gs_blog_cf.Models.ApplicationDbContext context)
        {
            // syntax to remove database from within Package Manager Console:
            //          Update-Database -TargetMigration:0

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if(!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            // write some code that creates a user
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if(!context.Users.Any(u => u.Email == "glen@swiftbt.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "glen@swiftbt.com",
                    UserName = "glen@swiftbt.com",
                    FirstName = "Glen",
                    LastName = "Stewart",
                    DisplayName = "Glen"
                },  "Abc&123!");

                // grab the Id that just created by adding the above user
                var userId = userManager.FindByEmail("glen@swiftbt.com").Id;

                // now that I have created a user I want to assign the person to the specific role
                userManager.AddToRole(userId, "Admin");

                // Homework, write code that creates the user to occupy the Moderator Role
                // then assign that user to the moderator role.
            }

            if(!context.Users.Any(u => u.Email == "jasontwichell@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "jasontwichell@coderfoundry.com",
                    UserName = "jasontwichell@coderfoundry.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "Jason"
                }, "Abc&123!");

                userManager.AddToRole(
                    userManager.FindByEmail("jasontwichell@coderfoundry.com").Id,
                    "Moderator");
            }

            if (!context.Users.Any(u => u.Email == "arussell@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "arussell@coderfoundry.com",
                    UserName = "arussell@coderfoundry.com",
                    FirstName = "Andrew",
                    LastName = "Russell",
                    DisplayName = "Drew"
                }, "Abc&123!");

                userManager.AddToRole(
                    userManager.FindByEmail("arussell@coderfoundry.com").Id,
                    "Moderator");
            }

            // This adds a collection of blogs for testing purposes.
            for(var loop = 1; loop<= 10; loop++)
            {
                context.BlogPosts.AddOrUpdate(
                    b => b.Title,
                        new BlogPost
                        {
                            Title = $"Seeded Title {loop}",
                            Body = $"Seeded Body {loop}",
                            Abstract = $"Seeded Abstract {loop}",
                            Created = DateTime.Now
                        });
            }

        }  // end seed
    }
}
