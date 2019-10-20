using Microsoft.AspNetCore.SignalR;
using SpaceApp.Game.AppServices.MovementServices;
using SpaceApp.Game.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceApp.Game.Communication
{
    public class GameMethods
    {
        public const string UserChangedPosition = "User_Position_Changed";
        public const string UserConnected = "User_Connected";
    }

    public class GameHub : Hub
    {
        private static readonly Dictionary<Guid, string> _userConnections = new Dictionary<Guid, string>();

        private readonly GameChangerService _gameChangerService;
        private readonly IHubContext<GameHub> _hub;

        public GameHub(IHubContext<GameHub> hub, GameChangerService gameChangerService)
        {
            _hub = hub;
            _gameChangerService = gameChangerService;
        }

        private Guid UserId => (Guid)Context.Items["userId"];
        private Guid RoomId => (Guid)Context.Items["roomId"];

        public override async Task OnConnectedAsync()
        {
            var userId = Guid.NewGuid();
            Context.Items.Add("userId", userId);

            await base.OnConnectedAsync();
            _userConnections.Add(userId, Context.ConnectionId);
        }

        public void Set_Username(string username)
        {
            Context.Items.Add("username", username);
        }

        public void Join_Room(Guid roomId)
        {
            Context.Items.Add("roomId", roomId);
        }

        public async Task Start_Game()
        {
            var data = _gameChangerService.JoinRoom(RoomId, UserId, Context.Items["username"].ToString());

            await _hub.Clients.Group(RoomId.ToString()).SendAsync("User_Connected", data.MyObject);
            await _hub.Groups.AddToGroupAsync(Context.ConnectionId, RoomId.ToString());
            await _hub.Clients.Client(Context.ConnectionId).SendAsync("initialize", data.OtherObjects);
        }

        public async Task User_Digest(Guid looserId)
        {
            var looser = _gameChangerService.Digest(RoomId, UserId, looserId, out GameObject winner);
            if (looser is null)
            {
                return;
            }

            if (looser.Type == GameObjectType.Player && _userConnections.TryGetValue(looserId, out var connectionId))
            {
                await _hub.Clients.Client(connectionId).SendAsync("User_GameOver", winner.Name);
                await _hub.Groups.RemoveFromGroupAsync(connectionId, RoomId.ToString());
            }

            await _hub.Clients.Group(RoomId.ToString()).SendAsync("User_Digested", looserId);
            await _hub.Clients.Group(RoomId.ToString()).SendAsync("User_Resize", UserId, winner.Size);

        }

        public async Task User_Moved(GameUserAction action)
        {
            action.UserId = UserId;
            await _hub.Clients.AllExcept(Context.ConnectionId).SendAsync(GameMethods.UserChangedPosition, action);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            await _hub.Clients.Group(RoomId.ToString()).SendAsync("User_Disconnected", UserId);

            _gameChangerService.RemoveFromRoom(UserId, RoomId);
            _userConnections.Remove(UserId);

            _gameChangerService.CleanupIfNeeded(RoomId);
        }
    }
}
