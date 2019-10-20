using System;

namespace SpaceApp.Game.Models
{
    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Position()
        {
            X = new Random().Next(-1 * Constants.World.XMax, Constants.World.XMax);
            Y = new Random().Next(-1 * Constants.World.YMax, Constants.World.YMax);
        }

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void SetPosition(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
