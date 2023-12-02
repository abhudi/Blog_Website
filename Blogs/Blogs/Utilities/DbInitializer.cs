﻿using Blogs.Data;
using Blogs.Models;
using Microsoft.AspNetCore.Identity;

namespace Blogs.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext context,
                                UserManager<ApplicationUser>userManager,
                                RoleManager<IdentityRole>roleManager) 
        {
            _context = context;
            _userManager = userManager; 
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAutor)).GetAwaiter().GetResult();
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin"
                },"Admin@0011").Wait();

                var appUser = _context.ApplicationUsers!.FirstOrDefault(x => x.Email == "admin@gmail.com");
                if(appUser != null)
                {
                    _userManager.AddToRoleAsync(appUser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
                }
                var listOfPages = new List<Page>()
                {
                    new Page()
                    {
                        Title = "About Us",
                        Slug = "about"
                    },
                    new Page()
                {
                    Title = "Contact Us",
                    Slug = "contact"
                },
                    new Page()
                {
                    Title = "Privacy Policy",
                    Slug = "privacy"
                }
            };

                _context.Pages!.AddRange(listOfPages);
                _context.SaveChanges();
            }
        }
    }
}
