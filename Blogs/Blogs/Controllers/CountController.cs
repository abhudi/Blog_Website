using AspNetCoreHero.ToastNotification.Abstractions;
using Blogs.Data;
using Blogs.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogs.Controllers
{
    public class CountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        public CountController(ApplicationDbContext context, INotyfService notification)
        {
            _context = context;
            _notification = notification;
        }
        
        
    }
}
