using System.Text.Json.Serialization;

namespace quest_web.Models
{
    public class UserDetails
    {
        public string Username { get; set; }

        [JsonIgnore] public string Password { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }
    }
}