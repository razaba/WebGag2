using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGag.DBContext;
using WebGag.Models;

namespace WebGag.Controllers
{
    public class StatsController : Controller
    {
        // GET: Stats
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult TheMostActiveUser()
        {
            using (GagsDbContext model = new GagsDbContext())
            {
                var userLikes = model.UserLikes. 
                    Join(model.Users, ul=>ul.UserId, u=> u.Id,(ul, u)=> new  {user =u, likes=ul }).
                    GroupBy(x => x.user.Id).OrderByDescending(x => x.Count());
                var userComments = model.Comments.Include("Owner").GroupBy(x => x.Owner.Id).OrderByDescending(x => x.Count());
                var userGags = model.Gags.Include("Owner").GroupBy(x => x.Owner.Id).OrderByDescending(x => x.Count());

                Dictionary<string, int> users = new Dictionary<string, int>();
                var arr = userLikes.ToArray();
                foreach (var user in userLikes)
                {
                    var name = user.First().user.Nickname;
                    if (!users.ContainsKey(name))
                    {
                        users[name] = 0;
                    }
                    users[name] += user.Count();
                }

                foreach (var user in userComments)
                {
                    var name = user.First().Owner.Nickname;
                    if (!users.ContainsKey(name))
                    {
                        users[name] = 0;
                    }
                    users[name] += user.Count();
                }
                foreach (var user in userGags)
                {
                    var name = user.First().Owner.Nickname;
                    if (!users.ContainsKey(name))
                    {
                        users[name] = 0;
                    }
                    users[name] += user.Count();
                }
                var topUsers = users.OrderByDescending(x => x.Value).Take(10).Select(x => new { Name = x.Key, Value = x.Value });                
                var context = new List<StatsModel>(10);
                foreach (var user in topUsers)
                {
                    context.Add(new StatsModel()
                    {
                        Name = user.Name,
                        Value = user.Value
                    });
                }
                JsonResult result = new JsonResult() { Data = context, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return result;
            }
        }
        public JsonResult TheMostLikeGags()
        {
            using (GagsDbContext model = new GagsDbContext())
            {
                var top10 = model.Gags.OrderByDescending(x => x.Likes).Take(10);
                var context = new List<StatsModel>(10);
                foreach (var gags in top10)
                {
                    context.Add(new StatsModel()
                    {
                        Name = gags.Title,
                        Value = gags.Likes
                    });
                }
                JsonResult result = new JsonResult() { Data = context, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return result;

            }
        }
        public JsonResult MostFunnyUsers()
        {
            using (GagsDbContext model = new GagsDbContext())
            {
                var allGags = model.Gags.Include("Owner");
                ConcurrentDictionary<Guid, int> counters = new ConcurrentDictionary<Guid, int>();
                foreach (var gag in allGags)
                {
                    if (!counters.ContainsKey(gag.Owner.Id))
                        counters[gag.Owner.Id] = 0;
                    counters[gag.Owner.Id]++;
                }
                var top10 = counters.OrderByDescending(x => x.Value).Take(10);
                var context = new List<StatsModel>(10);
                foreach (var user in top10)
                {
                    context.Add(new StatsModel()
                    {
                        Name = model.Users.Where(x => x.Id == user.Key).First().Nickname,
                        Value = user.Value
                    });
                }
                JsonResult result = new JsonResult() { Data = context, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return  result;
                
            }
        }
    }
}