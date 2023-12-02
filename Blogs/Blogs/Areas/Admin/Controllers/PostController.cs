using AspNetCoreHero.ToastNotification.Abstractions;
using Blogs.Data;
using Blogs.Models;
using Blogs.Utilities;
using Blogs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Blogs.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public PostController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostEnvironment,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notification = notyfService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            var listOfPosts = new List<Post>();
            var _loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(_loggedInUser!);
            if (loggedInUserRole[0] == WebsiteRoles.WebsiteAdmin)
            {
                listOfPosts = await _context.Posts!.Include(x => x.ApplicationUser).ToListAsync();
            }
            else
            {
                 listOfPosts = await _context.Posts!.Include(x => x.ApplicationUser).Where(x=>x.ApplicationUser!.Id==_loggedInUser!.Id).ToListAsync();

            }
            var listOfPostsVM = listOfPosts.Select(x => new PostVM()

            {
                Id = x.Id,
                Title = x.Title,
                CreatedDate = x.CreatedDate,
                ThumbnailUrl = x.ThumbnailUrl,
                AuthorName = x.ApplicationUser!.FirstName + " " + x.ApplicationUser.LastName
            }).ToList();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(await listOfPostsVM.OrderByDescending(x=>x.CreatedDate).ToPagedListAsync(pageNumber, pageSize));
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePostVM());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }

            //get logged in user id
            var _loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

            var post = new Post();
            post.Title = vm.Title;
            post.Description = vm.Description;
            post.ShortDescription = vm.ShortDescription;
            post.ApplicationUserId = _loggedInUser!.Id;

            if(post.Title!= null)
            {
               
                    string slug = vm.Title!.Trim();
                    slug = slug.Replace(" ", " ");
                    post.Slug = slug + "" + Guid.NewGuid();
                
            }

            if(vm.Thumbnail != null)
            {
               post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
           await _context.Posts!.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post Created Successfully");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            if (loggedInUserRole[0] == WebsiteRoles.WebsiteAdmin || loggedInUser?.Id == post?.ApplicationUserId)
            {
                _context.Posts!.Remove(post!);
                await _context.SaveChangesAsync();
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index","Post", new {area = "Admin"});
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult>  Edit(int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x =>x.Id == id);
            if(post == null)
            {
                _notification.Error("page not found");
                return View();
            }
            var _loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(_loggedInUser!);
            if (loggedInUserRole[0] != WebsiteRoles.WebsiteAdmin && _loggedInUser!.Id != post.ApplicationUserId)
            {
                _notification.Error("You are not authorized");
                return RedirectToAction("Index");
            }

            var vm = new CreatePostVM()
            {
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Description = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,
          
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if(post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            post.Title = vm.Title;
            post.ShortDescription = vm.ShortDescription;
            post.Description = vm.Description;

            if(vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Post Updated Successfully");
            return RedirectToAction("Index","Post", new {area = "Admin"});
        }
        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + " " + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using(FileStream filestream = System.IO.File.Create(filePath))
            {
                file.CopyTo(filestream);
            }
            return uniqueFileName;
        }
    }
}
