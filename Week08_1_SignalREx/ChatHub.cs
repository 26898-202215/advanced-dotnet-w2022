using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Week08_1_SignalREx.Models;

namespace Week08_1_SignalREx
{
    public class ChatHub:Hub
    {
        public async Task SendAllMessages(string userName, string textMessage)
        {
            var message = new ChatMessage
            {
                UserName = userName,
                Message = textMessage,
                TimeStamp = DateTimeOffset.Now
            };

            await Clients.All.SendAsync("ReceiveMessage",message.UserName, message.TimeStamp,message.Message);   

        }
    }
}
