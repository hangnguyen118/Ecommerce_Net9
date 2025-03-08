using EcommerceWebsite.EntityModels;
using EcommerceWebsite.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebsite.DataContext.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "adminnet@gmail.com",
                    Email = "adminnet@gmail.com",
                    Name = "Nguyen Thi Dieu Hang",                    
                    PhoneNumber = "0324567890",
                    StreetAddress = "110/9/4A TCH",
                    State = "district 12",
                    City = "Ho Chi Minh",
                    PostalCode = "12345",
                }, "Hangnguyen@1108").GetAwaiter().GetResult();
                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "adminnet@gmail.com");
                user.EmailConfirmed = true;
                _db.SaveChanges();
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();                
            }
            return;
        }
    }
}
