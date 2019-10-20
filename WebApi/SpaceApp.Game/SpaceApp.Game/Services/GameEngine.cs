using SpaceApp.Game.Models;
using SpaceApp.Game.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceApp.Game.Services
{
    public class GameEngine
    {
        public List<GameRoom> GameRooms { get; set; } = new List<GameRoom>();

        internal void AddNewRoom(GameRoom gameRoom)
        {
            GameRooms.Add(gameRoom);
        }

        internal IEnumerable<Room> GetRooms()
        {
            var rooms = GameRooms.Select(x => new Room(x.RoomId, x.RoomName));
            return rooms;
        }

        internal List<InitialWorldViewModel> GetRoomData(Guid roomId)
        {
            var room = GameRooms.FirstOrDefault(r => r.RoomId == roomId);
            var objects = room?.Game.GameObjects.Select(x => new InitialWorldViewModel
            {
                Id = x.GameObjectId,
                X = x.Position.X,
                Y = x.Position.Y,
                GameObjectType = x.Type,
                Size = x.Size,
                Name = x.Name,
                Description = "Smth"
            }).ToList();

            return objects ?? new List<InitialWorldViewModel>();
        }

        internal GameObject JoinUserToRoom(Guid roomId, Guid userId, string userName)
        {
            var room = GameRooms.Find(x => x.RoomId == roomId);
            var newObject = GameObject.CreatePlayer(userId, userName);
            room?.Game.GameObjects.Add(newObject);

            return newObject;
        }

        internal void RemoveFromRoom(Guid userId, Guid roomId)
        {
            var room = GameRooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room is null)
            {
                return;
            }
            room.RemoveUser(userId);
        }
    }
}
