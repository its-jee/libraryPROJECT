using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Data.Entities
{
    public class Reviews
    {
        [Key]
        public int Id { get; set; }
        public  int? bookId { get; set; } 

        [ForeignKey("bookId")]
        public Books BookFk { get; set; }
        public string UserId { get; set; }

        public string Descrptions { get; set; }

    }
}
