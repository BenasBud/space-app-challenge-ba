using System;

namespace SpaceApp.Game.Models
{
    public class GameUserAction
    {
        public Guid UserId { get; set; }
        public Position Position { get; set; }

        public Guid? GameObjectId { get; set; }
    }
}
