using System;
using System.Collections.Generic;

namespace DatingApp.API.Models
{
    public class Help
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Art { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Characteristic { get; set; }
    }
}