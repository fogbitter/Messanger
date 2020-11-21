using Messanger.Contexts;
using Messanger.Dto;
using Messanger.Hubs;
using Messanger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messanger.Controllers
{
    [Route("api/messanger")]
    [ApiController]
    public class MessangerController : ControllerBase
    {
        private readonly IHubContext<MessangerHub> hubContext;
        private readonly IDbRepository _repository;
        private DbRepository repository => (DbRepository)_repository;

        public MessangerController(IHubContext<MessangerHub> hubContext, IDbRepository _repository)
        {
            this.hubContext = hubContext;
            this._repository = _repository;
        }

        //Аналог SignalR
        [Route("send")]
        [HttpPost]
        public IActionResult SendRequest([FromBody] MessageDto msg)
        {
            this.hubContext.Clients.All.SendAsync("ReceiveOne", msg.User.Id, msg.Message);
            return Ok();
        }

        //Авторизация
        [Route("loginUser")]
        [HttpPost]
        public IActionResult LoginUserRequest([FromBody] UserDto userDto)
        {
            List<Dialog> dialogs = repository.context.Dialogs.ToList();
            //Запрос к бд, поиск пользователь
            User sameUser = this.repository.GetFirst<User>(x => x.Email == userDto.Email && x.Password == userDto.Password);
            if (sameUser == null)
            {
                return NotFound("Неверный логин или пароль.");
            }
            UserDto userDtoResponse = new UserDto(sameUser);
            return Ok(userDtoResponse);
        }

        //Регистраций пользователя
        [Route("signupuser")]
        [HttpPost]
        public async Task<IActionResult> SignupUserRequest([FromBody] UserDto userDto)
        {
            User sameUser = this.repository.GetFirst<User>(x => x.Email == userDto.Email);
            if (sameUser != null)
            {
                return BadRequest("");
            }
            User user = new User();
            user.Name = userDto.Name;
            user.Email = userDto.Email;
            user.Password = userDto.Password;
            user.Id = Guid.NewGuid();
            await repository.Add<User>(user);
            await repository.SaveChangesAsync();

            UserDto userDtoResponse = new UserDto(user);
            return Ok(userDtoResponse);
        }

        //Создание диалога
        [Route("createDialog")]
        [HttpPost]
        public async Task<IActionResult> CreateDialogRequest([FromBody] CreateDialogDto createDialogDto)
        {
            if (createDialogDto.Dialog == null
                || createDialogDto.Dialog.Name == null
                || createDialogDto.Users == null
                || createDialogDto.Users.Count == 0)
            {
                return BadRequest("Не верная команда.");
            }
            Dialog dialog = new Dialog();
            dialog.Id = Guid.NewGuid();
            dialog.Name = createDialogDto.Dialog.Name;
            List<DialogUserLink> links = new List<DialogUserLink>();

            foreach(UserDto userDto in createDialogDto.Users)
            {
                User sameUser = this.repository.GetFirst<User>(x => x.Name == userDto.Name);
                if (sameUser == null)
                {
                    return BadRequest($"Пользователь {userDto.Name} не найден.");
                }
                DialogUserLink link = new DialogUserLink();
                link.Id = Guid.NewGuid();
                link.DialogId = dialog.Id;
                link.UserId = sameUser.Id;
                links.Add(link);
            }

            await repository.Add(dialog);
            await repository.AddRange(links);
            await repository.SaveChangesAsync();

            DialogDto dialogDto = new DialogDto(dialog);
            return Ok(dialogDto);
        }

        //Получение сообщений в диалоге
        [Route("getMessages")]
        [HttpPost]
        public IActionResult GetMessagesRequest([FromBody] DialogDto DialogDto)
        {
            List<Message> messages = repository.context.Messages
                .Where(message => message.DialogId == DialogDto.Id)
                .Include(message => message.User).Include(message=>message.Dialog).OrderBy(message=>message.InsertDate).ToList();
            List<MessageDto> messagesDto = new List<MessageDto>();
            foreach(Message msg in messages)
            {
                messagesDto.Add(new MessageDto(msg));
            }
            return Ok(messagesDto);
        }

        //Получение всех диалогов
        [Route("getDialogs")]
        [HttpPost]
        public IActionResult GetDialogsRequest([FromBody] UserDto userDto)
        {
            var dialogLinks = repository.context.DialogUserLinks
                .Where(dialogUserLink => dialogUserLink.UserId == userDto.Id)
                .Select(dul => new { User = dul.User, Dialog = dul.Dialog })
                .ToList();
            List<DialogDto> dialogsDto = new List<DialogDto>();
            foreach (var link in dialogLinks)
            {
                dialogsDto.Add(new DialogDto(link.Dialog));
            }
            return Ok(dialogsDto);
        }
    }
}
