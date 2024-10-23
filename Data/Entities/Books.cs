using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Data.Entities
{
    public class Books
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        [StringLength(50)]
        public string Genre { get; set; }

        [StringLength(20)]
        public string? ISBN { get; set; }

        public int PublishedYear { get; set; }

        [Range(0, int.MaxValue)]
        public int CopiesAvailable { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalCopies { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(100)]
        public string? Publisher { get; set; }

        public string Image { get; set; }

        public DateTime? DateAdded { get; set; }

        public bool IsAvailable { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PenaltyAmount { get; set; }

        public DateTime? DueDate { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    }
}
