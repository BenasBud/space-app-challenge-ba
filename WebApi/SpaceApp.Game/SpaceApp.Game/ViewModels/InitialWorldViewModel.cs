using System;
using System.Collections.Generic;
using SpaceApp.Game.Models;

namespace SpaceApp.Game.ViewModels
{
    public class InitialWorldViewModel
    {
        public Guid Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public GameObjectType GameObjectType { get; set; }
        public double Size { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class JoinRoomVm
    {
        public List<InitialWorldViewModel> OtherObjects { get; set; }
        public InitialWorldViewModel MyObject { get; set; }
    }
}
