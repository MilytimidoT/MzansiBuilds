using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzansi_Builds.Models
{
    public class Post
    {
        public string AuthorEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public IEnumerable<string> AttachmentPaths { get; set; }
        public string GithubLink { get; set; }
    }
}