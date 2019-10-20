using System;

namespace SpaceApp.Game.ViewModels
{
    public class JoinRoomModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid RoomId { get; set; }
    }
}
