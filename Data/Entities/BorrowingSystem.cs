using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Data.Entities
{
    public class BorrowingSystem

    {
        [Key]
        public int Id { get; set; }
        public int? bookId { get; set; }

        [ForeignKey("bookId")]
        public Books BookFk { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser UserFk { get; set; }
        public DateTime BorrowingDate { get; set; }
        public DateTime ? ReturnDate { get; set; }
        public float Penalty { get; set; }
        public bool IsReteurned { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        

    }
}
