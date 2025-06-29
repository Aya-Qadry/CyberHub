﻿using System.ComponentModel.DataAnnotations;

namespace CyberHub.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Post date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



        [Required]
        public string AuthorId { get; set; } = string.Empty;

        [Required]
        public int PostId { get; set; }

        //replies
        public int? ParentCommentId { get; set; }

        public virtual User Author { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
        public virtual Comment? ParentComment { get; set; }
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

    }
}
