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

        // Parameterless constructor required for XmlSerializer
        public User() { }
    }
}