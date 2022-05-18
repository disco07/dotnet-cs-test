using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace quest_web.Models
{
    public enum UserRole
    {
        ROLE_USER = 0,
        ROLE_ADMIN = 1
    }

    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        [Column(TypeName = "nvarchar(255)")] public UserRole Role { get; set; }

        public DateTime Creation_Date { get; set; }
        public DateTime Updated_Date { get; set; }
    }
}