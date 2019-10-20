using SpaceApp.Game.Enums;

namespace SpaceApp.Game.Models
{
    public class SpaceObjectViewModel
    {
        public double X { get; set; }

        public double Y { get; set; }

        public SpaceObjectType Type { get; set; }

        public double SizeCoeficient { get; set; }

        public string Name { get; set; }

    }
}
