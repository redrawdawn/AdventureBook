using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace AdventureBook.Models
{
    public class Adventure
    {
        public int Id { get; set; }
        
        [Required]
        [DisplayName("UserProfile")]
        public int UserProfileId { get; set; }
        
        public string Title { get; set; }
        
        public string Text { get; set; }
        
        public int Difficulty { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        public UserProfile UserProfile { get; set; }
    }
}
