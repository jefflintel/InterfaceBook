using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InterfaceBook.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public IdentityUser User { get; set; }
        public UserPost UserPost { get; set; }
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }
        public String Text { get; set; }
    }
}
