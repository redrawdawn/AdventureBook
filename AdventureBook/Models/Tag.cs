using System.ComponentModel.DataAnnotations;

namespace AdventureBook.Models
{
    public class Tag
    {
        int Id { get; set; }

        [Required]
        string Name { get; set; }
    }
}
