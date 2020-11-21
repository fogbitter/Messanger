using Messanger.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Messanger.Dto
{
    public class UserDto : BaseDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserDto()
        {

        }
        public UserDto(User user)
        {
            this.Id = user.Id;
            this.Name = user.Name;
            this.Email = user.Email;
            this.Password = user.Password;
        }
    }
}
