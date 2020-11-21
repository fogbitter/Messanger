using Messanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messanger.Dto
{
    public class DialogDto : BaseDto
    {
        public string Name { get; set; }
        public DialogDto()
        {

        }
        public DialogDto(Dialog dialog)
        {
            this.Id = dialog.Id;
            this.Name = dialog.Name;
        }
    }
}
