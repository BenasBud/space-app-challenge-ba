using Newtonsoft.Json.Linq;
using SpaceApp.Game.Enums;
using SpaceApp.Game.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Zeptomoby.OrbitTools;

namespace SpaceApp.Game.Services
{
    public static class PreProcessService
    {
        private static readonly double MapEdgeLength = 5000;
        static Random rnd = new Random();
        
        public static JObject RoomNames;

        static PreProcessService()
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\StaticData\roomNameOptions.json";
            RoomNames = JObject.Parse(File.ReadAllText(path));
        }
        public static IEnumerable<SpaceObjectViewModel> GetParsedObjects(IEnumerable<SpaceObject> spaceObjects, SpaceObjectType type )
        {
            List<SpaceObjectViewModel> spaceObjectsResults= new List<SpaceObjectViewModel>(10000);

            foreach (var spaceObject in spaceObjects)
            {
                Tle tle = new Tle(spaceObject.Name, spaceObject.Line1, spaceObject.Line2);
                var viewModel = SpaceObjectToViewModel(tle);
                viewModel.Type = type;
                spaceObjectsResults.Add(viewModel);
            }
            var maxX = spaceObjectsResults.Select(x => x.X).Max();
            var maxY = spaceObjectsResults.Select(x => x.Y).Max();
            var maxZ = spaceObjectsResults.Select(x => x.SizeCoeficient).Max();
            var minX = spaceObjectsResults.Select(x => x.X).Min();
            var minY = spaceObjectsResults.Select(x => x.Y).Min();
            var minZ = spaceObjectsResults.Select(x => x.SizeCoeficient).Min();
            

            foreach (var spaceObject in spaceObjectsResults)
            {
                spaceObject.X = ((MapEdgeLength*2) / (maxX - minX)) * (spaceObject.X - minX)- MapEdgeLength;
                spaceObject.Y = ((MapEdgeLength*2) / (maxY - minY)) * (spaceObject.Y - minY)- MapEdgeLength;
                spaceObject.SizeCoeficient = Math.Abs((1 / (maxZ - minZ)) * (spaceObject.SizeCoeficient - minZ) - 1);
            }

            return spaceObjectsResults;
        }

        public static SpaceObjectViewModel SpaceObjectToViewModel(Tle tle)
        {
            Satellite sat = new Satellite(tle);
            Eci eci = sat.PositionEci(0);
            return new SpaceObjectViewModel { X = eci.Position.X, Y = eci.Position.Y, SizeCoeficient= eci.Position.Z, Name = tle.Name };
        }
        public static string GenerateRoomName()
        {


            int galaxyIndex = rnd.Next(1, 23);
            int moonPlanetIndex = rnd.Next(1, 164);
            var galaxy = RoomNames["galaxies"][galaxyIndex];
            var moonPlanet = RoomNames["planetsAndMoons"][moonPlanetIndex];
            return galaxy.ToString().ToUpper() + "-" + moonPlanet.ToString().ToUpper();
            }
        }
}
