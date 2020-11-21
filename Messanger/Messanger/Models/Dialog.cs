using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Messanger.Models
{
    public class Dialog : BaseEntity
    {
        public string Name { get; set; }
        List<Message> Messages { get; set; }
        List<DialogUserLink> DialogUserLinks { get; set; }
    }
}
