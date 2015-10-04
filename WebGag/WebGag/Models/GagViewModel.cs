using WebGag.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGag.Models
{
    public class AddCommentModel
    {
        public string GagId { get; set; }
        public string Description { get; set; }
    }
    public class CommentsViewModel
    {
        public Guid GagId { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class GagViewModel
    {
        public GagViewModel()
        {

        }
        public GagViewModel(Gag gag) 
        {
            this.Id = gag.Id;
            this.LastUpdateDate = gag.LastUpdateDate;
            this.Likes = gag.Likes;
            this.MediaType = gag.MediaType;
            this.Owner = gag.Owner;
            this.Title = gag.Title;
            this.Description = gag.Description;
            this.UploadDate = gag.UploadDate;
            this.Url = gag.Url;
            this.Comment = gag.Comments.Count;
        }
        public int Comment { get; set; }
        public int Likes { get; set; }
        public MediaType MediaType { get; set; }
        public User Owner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public Guid Id{ get; set; }
        public bool Liked { get; set; }
    }
}