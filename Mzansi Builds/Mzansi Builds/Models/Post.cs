using System;
using System.Collections.Generic;

namespace Mzansi_Builds.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public List<string> AttachmentPaths { get; set; } = new List<string>();
        public string GithubLink { get; set; }

        // New: like support
        public int LikesCount { get; set; }
        public List<string> LikedBy { get; set; } = new List<string>();

        public Post()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
    }
}