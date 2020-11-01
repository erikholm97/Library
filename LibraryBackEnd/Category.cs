using System.ComponentModel.DataAnnotations;

namespace Library
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(500)]
        [Required]
        public string CategoryName { get; set; }

    }
}