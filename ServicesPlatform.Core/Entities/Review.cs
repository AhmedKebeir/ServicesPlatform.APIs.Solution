using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public class Review:BaseEntity
    {
        
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string? ImageUrl { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

        // اللي كتب التقييم
        public string AuthorId { get; set; }
        public ReviewAuthorType AuthorType { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }//finish
        public int TechnicianId { get; set; }
        public Technician Technician { get; set; }//fiinsh
        public int OrderId { get; set; }
        public Order Order { get; set; }//fiinsh


      



    }
}
