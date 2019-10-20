
using Newtonsoft.Json.Linq;
using SpaceApp.Game.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SpaceApp.Game.Services
{
    public static class DataGatheringService
    {
        static readonly string debrisDataLink = "https://celestrak.com/NORAD/elements/cosmos-2251-debris.txt";
        static readonly string sateliteDataLink = "http://data.ivanstanojevic.me/api/tle";

        public static List<SpaceObject> GetDebrisFromNasa() //change type to model class name
        {

            var result = GetFileViaHttp(debrisDataLink);
            string str = Encoding.UTF8.GetString(result);
            string[] lines = str.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var spaceObjects = new List<SpaceObject>(lines.Length);

            for (int i = 0; i < lines.Length; i += 3)
            {
                var spaceObject = new SpaceObject
                {
                    Name = lines[i],
                    Line1 = lines[i + 1],
                    Line2 = lines[i + 2]
                };

                spaceObjects.Add(spaceObject);
            }

            return spaceObjects;
        }

        public static List<SpaceObject> GetSatelitesFromNasa()
        {
            string response;
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString(sateliteDataLink);
            }

            JObject responseObject = JObject.Parse(response);
            List<SpaceObject> satelites = new List<SpaceObject>(1000);
            foreach (dynamic obj in (JArray)responseObject["member"])
            {
                satelites.Add(new SpaceObject { Name = (string)obj["name"], Line1 = (string)obj["line1"].ToString(), Line2 = (string)obj["line2"].ToString() });
            }
            return satelites;
        }

        public static byte[] GetFileViaHttp(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadData(url);
            }
        }
    }
}
