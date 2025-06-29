﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;

namespace CyberHub.Models
{
    public class User : IdentityUser
    {

     
        //[Display(Name = "Display Name")]
        //[StringLength(100)]
        //public string? DisplayName { get; set; }

        [Display(Name = "Bio")]
        [StringLength(500)]
        public string? Bio { get; set; }

        [Display(Name = "Joined")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Last seen")]
        public DateTime? LastLogin { get; set; }

        public string? ProfilePictureUrl { get; set; }


        // les relations
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}
