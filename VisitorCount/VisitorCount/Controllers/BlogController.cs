using Microsoft.AspNetCore.Mvc;
using VisitorCount.Models;

namespace VisitorCount.Controllers
{
    public class BlogController : Controller
    {
        private static List<BlogPost> blogPosts = new List<BlogPost>
    {
        new BlogPost { Id = 1, Title = "Post 1", Content = "Content 1", IsHidden = false },
        new BlogPost { Id = 2, Title = "Post 2", Content = "Content 2", IsHidden = false },
        // Add more posts as needed
    };
        public ActionResult Index()
        {
            return View(blogPosts);
        }
    }
    [HttpPost]
    public JsonResult ToggleVisibility(int postId)
    {
        var post = blogPosts.FirstOrDefault(p => p.Id == postId);
        if (post != null)
        {
            post.IsHidden = !post.IsHidden;
            return Json(new { success = true, isHidden = post.IsHidden });
        }

        return Json(new { success = false });
    }
}
