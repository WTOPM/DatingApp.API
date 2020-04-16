using System;

namespace DatingApp.API.Dtos
{
    public class HelpForCreationDto
    {
        public int ProductId { get; set; }
        public int RecipientId { get; set; }
        public DateTime HelpSent { get; set; }
        public string Content { get; set; }
        public HelpForCreationDto()
        {
            HelpSent = DateTime.Now;
        }

    }
}