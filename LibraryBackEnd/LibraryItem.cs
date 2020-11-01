using System;
using System.ComponentModel.DataAnnotations;

namespace Library
{
    public class LibraryItem
    {
        public int Id { get; set; }
        
        public int CategoryId { get; set; }
        [Required]
        public Category Category { get; set; }
        [MaxLength(500)]
        [Required]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Author { get; set; }

        public int? Pages { get; set; }

        public int? RunTimeMinutes { get; set; }

        public bool IsBorrowable { get; set; }
        [MaxLength(500)]
        public string Borrower { get; set; }

        public DateTime? BorrowDate { get; set; }
        [MaxLength(500)]
        [Required]
        public string Type { get; set; }
    }
}
