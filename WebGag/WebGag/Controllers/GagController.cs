using WebGag.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebGag.Models;

namespace WebGag.Controllers
{
    public class GagController : Controller
    {
        const int PAGE_SIZE = 10;

        [Authorize(Roles ="Admin")]
        [HttpGet, ActionName("Delete")]
        public bool DeleteGag(string id)
        {
            using (GagsDbContext model = new GagsDbContext())
            {
                var gId = new Guid(id);
                var gag = model.Gags.Include("Comments").Where(x => x.Id == gId).FirstOrDefault();
                if (gag == null)
                {
                    return false;
                }
                model.Comments.RemoveRange(gag.Comments);
                model.Gags.Remove(gag);
                model.SaveChanges();
            }
            return true;
        }
        [Authorize]
        public string Like(string id)
        {
            Guid gagID;
            if( Guid.TryParse(id, out gagID))
            {
                using (GagsDbContext model = new GagsDbContext())
                {
                    var userId = new Guid(User.Identity.GetUserId());
                    var user = model.Users.Where(u => u.Id == userId).FirstOrDefault();
                    if (user == null)
                    {
                        user = new User()
                        {
                            Nickname = User.Identity.GetUserName(),
                            Id = userId,
                            DateOfBirth = DateTime.Now
                        };
                        model.Users.Add(user);
                    }
                    var isExist = model.UserLikes.Where(x => x.UserId == userId && x.GagId == gagID).FirstOrDefault();
                    if (isExist != null)
                    {
                        return "already liked";
                    }
                    else
                    {
                        var gag = model.Gags.Where(x => x.Id == gagID).FirstOrDefault();
                        if (gag == null)
                        {
                            return "gag not found";
                        }
                        model.UserLikes.Add(new UsersLikes()
                        {
                            GagId = gagID,
                            UserId = userId
                        });
                        gag.Likes++;

                        model.SaveChanges();
                        return gag.Likes.ToString();
                    }
                }
            }
            return string.Empty;
        }
        public ActionResult Index(bool isHot = false, int page = 1)
        {
            ViewBag.isHot = isHot;
            ViewBag.page = page;
            using (GagsDbContext model = new GagsDbContext())
            {
                var gags = loadGags(model, isHot, page);
                return CreateViewModelContext(gags, model);
            }
        }

        public ActionResult Search(string text, int page = 1, bool Gags = false, bool Comments = false, bool Commentiers = false, bool Owner = false)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return RedirectToAction("Index", "Gag");
            }

            ViewBag.text = text;
            ViewBag.page = page;
            using (GagsDbContext model = new GagsDbContext())
            {
                var searchGags = model.Gags.Include("Owner").Include("Comments").Include("Comments.Owner").
                            Where(x =>(Gags && x.Title.Contains(text))||
                                      (Gags && x.Description.Contains(text)) ||
                                      (Owner && x.Owner.Nickname.Contains(text) )||
                                      ((Comments || Commentiers)&& x.Comments.Any(y => (Comments && y.Description.Contains(text)) ||
                                        (Commentiers && y.Owner.Nickname.Contains(text))))).
                            OrderByDescending(x => x.UploadDate).
                            Skip(PAGE_SIZE * (page - 1)).
                            Take(PAGE_SIZE).ToArray();
                return CreateViewModelContext(searchGags, model);
            }
        }

        private IEnumerable<Gag> loadGags(GagsDbContext model, bool isHot, int page)
        {
            var gags = model.Gags.Include("Owner").Include("Comments");
            var orderdGags = gags.OrderByDescending(x => x.UploadDate);
            if (isHot)
            {
                orderdGags = orderdGags.OrderByDescending(x => x.Likes).OrderByDescending(x=> x.Comments.Count);
            }
            return orderdGags.Skip(PAGE_SIZE * (page - 1)).Take(PAGE_SIZE).ToArray();                
        }

        private ActionResult CreateViewModelContext(IEnumerable<Gag> gags, GagsDbContext model)
        {
            bool isUserLogged = true;
            Guid userId = Guid.Empty;

            var userStringId = User.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(userStringId))
            {
                isUserLogged = false;
            }
            else
            {
                userId = new Guid(userStringId);
            }
            var gagViewModel = new List<GagViewModel>(10);
            foreach (var gag in gags)
            {
                var isLiked = !isUserLogged || model.UserLikes.Any(x => x.GagId == gag.Id && x.UserId == userId);
                gagViewModel.Add(
                    new GagViewModel(gag)
                    {
                        Liked = isLiked
                    });
            }
            return View(gagViewModel);
        }
    }
}