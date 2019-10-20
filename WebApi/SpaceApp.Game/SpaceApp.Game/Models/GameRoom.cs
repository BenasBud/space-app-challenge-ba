using System;
using System.Collections.Generic;

namespace SpaceApp.Game.Models
{
    public class GameRoom : Room
    {
        public Game Game { get; set; } = new Game();
        public HighScore HighScore { get; set; } = new HighScore();

        public GameRoom(string roomName) : base(roomName)
        {
        }

        public GameRoom(Guid roomId, string roomName) : base(roomId, roomName)
        {

        }

        public Room GetRoom()
        {
            return new Room(RoomId, RoomName);
        }

        public void RemoveUser(Guid userId)
        {
            Game.Remove(userId);
        }
    }

    public class Room
    {
        public Guid RoomId { get; private set; }
        public string RoomName { get; private set; }
        public Room(string roomName)
        {
            RoomId = Guid.NewGuid();
            RoomName = roomName;
        }
        public Room(Guid roomId, string roomName)
        {
            RoomId = roomId;
            RoomName = roomName;
        }
    }
}
