using AspNetCoreHero.ToastNotification.Abstractions;
using Blogs.Data;
using Blogs.Models;
using Blogs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogs.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingController(ApplicationDbContext context, INotyfService notification, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _notification = notification;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = _context.Settings!.ToList();
            if(settings.Count > 0)
            {
                var vm = new SettingVM()
                {
                    Id = settings[0].Id,
                    SiteName = settings[0].SiteName,
                    Title = settings[0].Title,
                    ShortDescription = settings[0].ShortDescription,
                    ThumbnailUrl = settings[0].ThumbnailUrl,
                    FacebookUrl = settings[0].FacebookUrl,
                    GithubUrl = settings[0].GithubUrl,
                    TwitterUrl = settings[0].TwitterUrl,
                };
                return View(vm);
            }
            var setting = new Setting()
            {
                SiteName = "Demo Name"
            };
            await _context.Settings!.AddAsync(setting);
            await _context.SaveChangesAsync();
            var createdSettings = _context.Settings!.ToList();
            var createdVm = new SettingVM()
            {
                Id = createdSettings[0].Id,
                SiteName = createdSettings[0].SiteName,
                Title = createdSettings[0].Title,
                ShortDescription = createdSettings[0].ShortDescription,
                ThumbnailUrl = createdSettings[0].ThumbnailUrl,
                FacebookUrl = createdSettings[0].FacebookUrl,
                GithubUrl = createdSettings[0].GithubUrl,
                TwitterUrl = createdSettings[0].TwitterUrl,
            };
            return View(createdVm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SettingVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var setting = await _context.Settings!.FirstOrDefaultAsync
                (x => x.Id == vm.Id);
            if (setting == null) 
            {
                _notification.Error("Something Went Wrong");
                 return View(vm); 
            }
            setting.SiteName = vm.SiteName; 
            setting.Title = vm.Title;
            setting.ShortDescription = vm.ShortDescription; 
            setting.FacebookUrl = vm.FacebookUrl;
            setting.GithubUrl = vm.GithubUrl;
            setting.TwitterUrl = vm.TwitterUrl;
            setting.ThumbnailUrl = vm.ThumbnailUrl; 
            
            if(vm.Thumbnail != null)
            {
                setting.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Setting Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }
        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + " " + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream filestream = System.IO.File.Create(filePath))
            {
                file.CopyTo(filestream);
            }
            return uniqueFileName;
        }
    }
}
