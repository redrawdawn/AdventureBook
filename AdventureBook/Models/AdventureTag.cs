using System.ComponentModel.DataAnnotations;

namespace AdventureBook.Models
{
    public class AdventureTag
    {
        int Id { get; set; }

        [Required]
        int AdventureId { get; set; }

        [Required]
        int TagId { get; set; }
    }
}
