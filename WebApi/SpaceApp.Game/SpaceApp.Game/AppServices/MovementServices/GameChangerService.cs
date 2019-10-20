using SpaceApp.Game.Models;
using SpaceApp.Game.Services;
using SpaceApp.Game.ViewModels;
using System;
using System.Linq;

namespace SpaceApp.Game.AppServices.MovementServices
{
    public class GameChangerService
    {
        private readonly GameEngine _gameEngine;

        public GameChangerService(GameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }

        public GameObject Digest(Guid roomId, Guid winner, Guid looser, out GameObject winnerObject)
        {
            var room = _gameEngine.GameRooms.FirstOrDefault(x => x.RoomId == roomId);
            if (room is null)
            {
                winnerObject = null;
                return null;
            }


            var looserObject = room.Game.DigestObject(winner, looser, out GameObject winnerObject1);
            winnerObject = winnerObject1;
            return looserObject;
        }

        public void Move(Guid roomId, Guid player, double x, double y)
        {
            var room = _gameEngine.GameRooms.FirstOrDefault(r => r.RoomId == roomId);
            room?.Game.MoveObject(player, x, y);
        }

        public JoinRoomVm JoinRoom(Guid roomId, Guid userId, string userName)
        {
            var objects = _gameEngine.GetRoomData(roomId);
            var myObject = _gameEngine.JoinUserToRoom(roomId, userId, userName);

            return new JoinRoomVm
            {
                OtherObjects = objects,
                MyObject = new InitialWorldViewModel
                {
                    Id = myObject.GameObjectId,
                    X = myObject.Position.X,
                    Y = myObject.Position.Y,
                    GameObjectType = myObject.Type,
                    Size = myObject.Size,
                    Name = myObject.Name,
                    Description = "Smth"
                }
            };
        }

        public void CleanupIfNeeded(Guid roomId)
        {
            var room = _gameEngine.GameRooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room is null)
            {
                return;
            }

            if (room.Game.GameObjects.Any(x => x.Type == GameObjectType.Player))
            {
                return;
            }

            _gameEngine.GameRooms.Remove(room);
        }

        public void RemoveFromRoom(Guid userId, Guid roomId)
        {
            _gameEngine.RemoveFromRoom(userId, roomId);
        }
    }
}
