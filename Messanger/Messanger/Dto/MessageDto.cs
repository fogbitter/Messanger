using Messanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messanger.Dto
{
    public class MessageDto : BaseDto
    {
        public UserDto User { get; set; }
        public string Message { get; set; }
        public DialogDto Dialog { get; set; }
        public DateTime Date { get; set; }

        public MessageDto()
        {

        }

        public MessageDto(Message message)
        {
            this.Id = message.Id;
            this.User = new UserDto(message.User);
            this.Dialog = new DialogDto(message.Dialog);
            this.Message = message.Text;
            this.Date = message.InsertDate;
        }
    }
}
