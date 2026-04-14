using System;

namespace Mzansi_Builds.Models
{
    // Simple model used by DataService. Keep public properties for XML serialization.
    public class User
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfileImagePath { get; set; }

        // Additional UI/profile fields
        public string Bio { get; set; }
        public DateTime JoinedAt { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int PostsCount { get; set; }

        // Parameterless constructor required for XmlSerializer
        public User() { }
    }
}