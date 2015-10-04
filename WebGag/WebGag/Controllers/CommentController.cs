using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGag.DBContext;
using Microsoft.AspNet.Identity;
using WebGag.Models;
using System.IO;
using System.Text;
using System.Web.UI;

namespace WebGag.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        [ChildActionOnly]
        public ActionResult Index(string id)
        {
            if (id == null)
                return PartialView();

            return PartialView(getComment(id));            
        }

        private CommentsViewModel getComment(string id)
        {
            using (GagsDbContext model = new GagsDbContext())
            {
                var gagId = Guid.Parse(id);
                var gag = model.Gags.Include("Comments").Include("Comments.Owner").Where(x => x.Id == gagId).FirstOrDefault();
                if (gag == null)
                {
                    ViewBag.Messege = "no gag found";
                    return null;
                }
                var modelView = new CommentsViewModel()
                {
                    Comments = new List<Comment>(gag.Comments.OrderByDescending(x => x.Date)),
                    GagId = gagId
                };
                return modelView;
            }
        }
        [Authorize]
        [HttpPost]
        public JsonResult AddComment(AddCommentModel comment)
        {
            using (GagsDbContext model = new GagsDbContext())
            {
                var userId = new Guid(User.Identity.GetUserId());
                var user = model.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (user == null)
                {
                    user = new User()
                    {
                        Id = userId,
                        Nickname = User.Identity.Name,
                        DateOfBirth = DateTime.Now
                    };
                    model.Users.Add(user);
                }

                var g_gagId = Guid.Parse(comment.GagId);

                var gag = model.Gags.Include("Comments").Include("Comments.Owner").Where(x => x.Id == g_gagId).FirstOrDefault();
                if (gag != null)
                {

                    gag.Comments.Add(new Comment()
                    {
                        Id = Guid.NewGuid(),
                        Date = DateTime.Now,
                        Description = comment.Description,
                        Owner = user
                    });
                }
                model.SaveChanges();
            }
            var result = new JsonResult()
            {
                Data = getComment(comment.GagId)
            };
            return result;            
        }
    }
}