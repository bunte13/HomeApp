using HomeApp.Data;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace HomeApp.Models
{
    public class SeedData
    {

        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<HomeAppUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            HomeAppUser user = await UserManager.FindByEmailAsync("admin@HomeApp.com");
            if (user == null)
            {
                var User = new HomeAppUser();
                User.Email = "admin@HomeApp.com";
                User.UserName = "admin@HomeApp.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }
        
            public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new HomeAppContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<HomeAppContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Item.Any() || context.Review.Any() || context.Category.Any() || context.CategoryItem.Any() || context.Supplier.Any())
                {
                    return; // DB has been seeded
                }
                context.Supplier.AddRange(
                    new Supplier
                    {
                        Name = "Lorme",
                        Country = "Slovenia",
                        Established = 1990,
                        LogoUrl = "https://logovectordl.com/wp-content/uploads/2021/03/lorme-logo-vector.png"
                    },
                    new Supplier
                    {
                        Name = "Bormioli",
                        Country = "Spain",
                        Established = 1995,
                        LogoUrl = "https://upload.wikimedia.org/wikipedia/en/4/42/Logo_Bormioli_Rocco_Group.jpg"
                    }

                    );
               
                context.SaveChanges();

                context.Item.AddRange(
                   new Item
                   {
                       Name = "glass",
                       Quantity = 12,
                       SupplierId = 1,
                       PictureJPG = "https://www.corecatering.co.za/wp-content/uploads/2019/04/WIN0018_WINE_FORTIUS_510ML_MAIN1.jpg",
                       Description = "a special class for your fine wine dining "
                   },
                   new Item
                   {
                       Name = "Fork",
                       Quantity = 32,
                       SupplierId = 1,
                       PictureJPG = "https://www.hoffmaster.com/media/catalog/product/cache/5e5ab4b510a8ac0e5c717a412505503e/m/i/mini_metallic_fork_883314_1.png",
                       Description = "a special fork for your fine dining "
                   });
                context.SaveChanges();

                context.Review.AddRange(
                    new Review
                    {
                        ItemId = 1,
                        AppUser ="Bunte",
                        Comment ="such a great deal",
                        Rating = 9
                    },
                    new Review
                    {
                        ItemId = 2,
                        AppUser = "Dame",
                        Comment = "very cost efficient",
                        Rating = 10
                    });
                context.SaveChanges();
                context.Category.AddRange(
                   new Category
                   {
                       CatgoryName = "Bathroom"
                   },
                   new Category
                   {
                       CatgoryName = "Kitchen"
                   },
                   new Category
                   {
                       CatgoryName = "Garden"
                   });
                context.SaveChanges();

                context.CategoryItem.AddRange(
                    new CategoryItem
                    {
                        ItemId = 1,
                        CategoryId = 1
                    },
                    new CategoryItem
                    {
                        ItemId = 2,
                        CategoryId = 2
                    });
                context.SaveChanges();

               
                context.UserItem.AddRange(
                    new UserItem
                    {
                        AppUser = "user 1",
                        ItemId = 1
                    },
                    new UserItem
                    {
                        AppUser = "user 2",
                        ItemId = 2
                    });
               
                context.SaveChanges();

            }

        }

    }
}
