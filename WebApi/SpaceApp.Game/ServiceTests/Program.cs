using SpaceApp.Game.Models;
using SpaceApp.Game.Services;
using System;
using System.Collections.Generic;
using SpaceApp.Game.Enums;

namespace ServiceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var name = PreProcessService.GenerateRoomName();
            Console.WriteLine(name);
            Console.ReadLine();
        }
    }
}
