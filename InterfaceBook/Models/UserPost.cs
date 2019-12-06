using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InterfaceBook.Models
{
    public class UserPost
    {
        [Key]
        public int UserPostId { get; set; }
        public IdentityUser User { get; set; }
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }
        public String Text { get; set; }
        public List<Comment> Comments { get; set; }

        [NotMapped]
        public int? CommentCount { get; set; }

        [NotMapped]
        public bool? Editable { get; set; }
    }
}
