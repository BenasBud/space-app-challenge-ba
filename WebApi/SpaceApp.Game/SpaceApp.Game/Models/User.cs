using System;

namespace SpaceApp.Game.Models
{
    public class User
    {
        static Random rnd = new Random();
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Position Position { get; set; }
        public long TotalScore { get; set; }
        public User(string userName)
        {
            UserId = Guid.NewGuid();
            UserName = userName;
            GetRandStartPosition();
        }
        public User(Guid userId, string userName)
        {
            UserId = userId;
            UserName = userName;
            GetRandStartPosition();
        }

        public void GetRandStartPosition()
        {
            var minusX = rnd.Next(0, 1);
            var minusY = rnd.Next(0, 1);
            var X = minusX == 1 ? rnd.Next(3000, 4999) * (-1) : rnd.Next(3000, 4999);
            var Y = minusY == 1 ? rnd.Next(3000, 4999) * (-1) : rnd.Next(3000, 4999);
            Position = new Position(X, Y);
        }
    }
}
