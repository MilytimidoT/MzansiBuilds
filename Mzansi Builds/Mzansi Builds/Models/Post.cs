using System;
using System.Collections.Generic;

namespace Mzansi_Builds.Models
{
    public class Post
    {
        public string AuthorEmail { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public List<string> AttachmentPaths { get; set; }
        public string GithubLink { get; set; }

        // ✅ NEW
        public List<string> Likes { get; set; } = new List<string>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}