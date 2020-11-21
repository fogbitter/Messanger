using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messanger.Dto
{
    public class CreateDialogDto
    {
        public DialogDto Dialog { get; set; }
        public List<UserDto> Users { get; set; }
    }
}
