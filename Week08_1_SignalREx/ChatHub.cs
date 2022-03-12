using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Week08_1_SignalREx.Models;

namespace Week08_1_SignalREx
{
    public class ChatHub:Hub
    {
        private readonly AppDbContext _dbContext;
        private static ConcurrentDictionary<string, ChatRoom> chatRooms = new ConcurrentDictionary<string, ChatRoom>();

        private static ConcurrentDictionary<string, string> userNames = new ConcurrentDictionary<string, string>();


        public ChatHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "Chat Hub", DateTimeOffset.Now, "Welcome to Chat Hub!");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            string userLeftName = "Unknown User";
            userNames.TryRemove(Context.ConnectionId, out userLeftName);
            await Clients.All.SendAsync("ReceiveMessage", "Chat Hub", DateTimeOffset.UtcNow, $"{userLeftName} left the conversation");

            await base.OnDisconnectedAsync(ex);
        }
        
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

        public async Task JoinRoom(string roomName)
        {
            roomName = roomName.ToLower();
            string currentConnectionId = Context.ConnectionId;

            if (!chatRooms.ContainsKey(roomName))
            {
                ChatRoom newRoom = new ChatRoom() {
                    RoomName = roomName,
                    ConnectionIds = new List<string>()
                };
                newRoom.ConnectionIds.Add(currentConnectionId);
                chatRooms.TryAdd(roomName, newRoom);

            }
            else
            {
                ChatRoom existingRoom = new ChatRoom();
                chatRooms.TryGetValue(roomName, out existingRoom);
                existingRoom.ConnectionIds.Add(currentConnectionId);

                chatRooms.TryAdd(roomName, existingRoom);
            }

            await Groups.AddToGroupAsync(currentConnectionId,roomName);
            await Clients.Caller.SendAsync("ReceiveMessage", "Chat Hub", DateTimeOffset.Now, $"You joined room: {roomName}!");
        }

        public async Task SendMessage(string roomname, string userName, string textMessage)
        {
            if (!userNames.ContainsKey(Context.ConnectionId))
            {
                userNames.TryAdd(Context.ConnectionId, userName);
            }


            var message = new ChatMessage
            {
                UserName = userName,
                Message = textMessage,
                TimeStamp = DateTimeOffset.Now
            };

            

            await Clients.Group(roomname.ToLower()).SendAsync("ReceiveMessage", message.UserName, message.TimeStamp, message.Message);
        }
    }
}
