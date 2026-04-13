using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzansi_Builds.Models
{
    public class FriendRequest
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public bool IsAccepted { get; set; }
    }
}