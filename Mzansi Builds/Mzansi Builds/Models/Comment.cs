using System;

namespace Mzansi_Builds.Models
{
    public class Comment
    {
        public string AuthorEmail { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
