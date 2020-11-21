using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Messanger.Models
{
    public class DialogUserLink : BaseEntity
    {
        [Required]
        public Guid DialogId { get; set; }

        [ForeignKey("DialogId")]
        public Dialog Dialog { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]

        public User User { get; set; }

    }
}
