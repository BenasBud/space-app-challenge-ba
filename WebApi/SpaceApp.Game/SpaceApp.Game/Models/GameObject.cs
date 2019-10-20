using System;

namespace SpaceApp.Game.Models
{
    public class GameObject
    {
        public Guid GameObjectId { get; private set; }
        public Position Position { get; private set; }
        public double Size { get; private set; }
        public GameObjectType Type { get; private set; }
        public string Name { get; set; }

        private GameObject(Position position, GameObjectType type)
        {
            GameObjectId = Guid.NewGuid();
            Position = position;
            Type = type;
        }

        public static GameObject CreateScrap(Position position, double sizeCof, string name)
        {
            return new GameObject(position, GameObjectType.Scrap)
            {
                Size = sizeCof < 0.5 ? 8 : sizeCof * 16,
                Name = name
            };
        }

        public static GameObject CreateSatellite(Position position, double sizeCof, string name)
        {
            return new GameObject(position, GameObjectType.Satellite)
            {
                Size = sizeCof < 0.5 ? 64 : sizeCof * 124,
                Name = name
            };
        }

        public static GameObject CreatePlayer(Guid playerId, string name)
        {
            return new GameObject(new Position(), GameObjectType.Player)
            {
                Size = 32,
                GameObjectId = playerId,
                Name = name
            };
        }


        public void Digest(GameObject looser)
        {
            var sum = Math.PI * Size * Size + Math.PI * looser.Size * looser.Size * 0.2;
            Size = Math.Sqrt(sum / Math.PI);
        }

        public void Move(double x, double y)
        {
            Position.SetPosition(x, y);
        }
    }

    public enum GameObjectType
    {
        Scrap = 0,
        Player = 1,
        Satellite = 2
    }
}
