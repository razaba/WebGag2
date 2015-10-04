using WebGag.DBContext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGag.Models;
using Microsoft.AspNet.Identity;

namespace WebGag.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(UploadGagViewModel gag)
        {
            if (gag.Media != null && gag.Media.ContentLength > 0)
                try
                {
                    var id = Guid.NewGuid();
                    var fileName = Path.GetFileName(gag.Media.FileName);
                    var names = fileName.Split('.');
                    var ext = names.Last().ToLower();
                    var isDefined = false;
                    var type = MediaType.VIDEO;
                    if (ext.Equals("jpg") || ext.Equals("png") || ext.Equals("gif"))
                    {
                        isDefined = true;
                        type = MediaType.PICTURE;
                    }
                    if (ext.Equals("mp4"))
                    {
                        isDefined = true;
                    }
                    if (!isDefined)
                    {
                        ViewBag.Message = "Unrecognized File!";
                        return View();
                    }

                    fileName = id.ToString() + "." + ext;
                    string path = Path.Combine(Server.MapPath("~/Gags/"), fileName);

                    gag.Media.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                    using (var gagContext = new GagsDbContext())
                    {
                        var userId = new Guid(User.Identity.GetUserId());
                        var user = gagContext.Users.Where(u => u.Id == userId).FirstOrDefault();
                        if (user == null)
                        {
                            user = new User()
                            {
                                Nickname = User.Identity.GetUserName(),
                                Id = userId,
                                DateOfBirth = DateTime.Now
                            };
                            gagContext.Users.Add(user);

                        }
                        gagContext.Gags.Add(new Gag()
                        {
                            Description = gag.Description,
                            Title = gag.Title,
                            Url = "~/Gags/" + fileName,
                            Id = id,
                            LastUpdateDate = DateTime.Now,
                            UploadDate = DateTime.Now,
                            MediaType = type,
                            Owner = user
                            
                        });
                        gagContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }
    }
}