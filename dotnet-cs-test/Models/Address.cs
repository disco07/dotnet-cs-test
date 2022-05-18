using System;

namespace quest_web.Models
{
    public class Address
    {
        public int id { get; set; }
        public string road { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public int User { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime updatedDate { get; set; }
    }
}