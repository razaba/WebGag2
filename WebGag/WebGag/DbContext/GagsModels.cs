using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace WebGag.DBContext
{
    public class GagsDbContext : DbContext
    {
        public GagsDbContext(): base("GagContext") { }
        public DbSet<User> Users { get; set; }
        public DbSet<Gag> Gags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UsersLikes> UserLikes { get; set; }
    }
    public class UsersLikes
    {
        [Key]
        [Column(Order = 0)]
        public Guid GagId { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid UserId { get; set; }
    }
    public class Gag
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        public string Url { get; set; }
        public MediaType MediaType { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public User Owner { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public int Likes { get; set; }
    }

    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public User Owner { get; set; }
        public int Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

    public enum MediaType
    {
        PICTURE,
        VIDEO
    }
}