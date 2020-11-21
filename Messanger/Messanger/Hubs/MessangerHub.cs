using Messanger.Contexts;
using Messanger.Dto;
using Messanger.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Messanger.Hubs
{
    public class MessangerHub : Hub
    {
        private readonly IDbRepository _repository;
        private DbRepository repository
        {
            get
            {
                return (DbRepository)this._repository;
            }
        }
        public MessangerHub(IDbRepository _repository)
        {
            this._repository = _repository;
        }

        //Отправка сообщения с клиента на сервер
        public async Task SendMessage(MessageDto messageDto)
        {
            //сохранение сообщения
            Message message = new Message();
            message.Id = Guid.NewGuid();
            message.UserId = messageDto.User.Id;
            message.InsertDate = DateTime.Now;
            message.Text = messageDto.Message;
            message.DialogId = messageDto.Dialog.Id;
            await repository.Add(message);
            await repository.SaveChangesAsync();

            //List<Message> fetchedMessage = repository.context.Messages
            //    .Where(msg => msg.Id == message.Id)
            //    .Include(message => message.User).Include(message => message.Dialog).ToList();

            Message fetchedMessage = repository.context.Messages
                .Where(msg => msg.Id == message.Id)
                .Include(message => message.User).Include(message => message.Dialog).First();

            //Отправка сообщения получателям
            MessageDto msgDto = new MessageDto(fetchedMessage);
            await Clients.Group(fetchedMessage.DialogId.ToString()).SendAsync("ReceiveDialogMessage", msgDto);
            //Clients.All.SendAsync("ReceiveOne", msgDto);
        }

        public async Task EnterGroup(DialogDto dialogDto)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, dialogDto.Id.ToString());
        }
    }
}
