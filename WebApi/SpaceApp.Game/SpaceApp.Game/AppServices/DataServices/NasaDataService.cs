using System.Collections.Generic;
using System.Linq;
using SpaceApp.Game.Enums;
using SpaceApp.Game.Models;
using SpaceApp.Game.Services;

namespace SpaceApp.Game.AppServices.DataServices
{

    public class NasaDataService
    {
        public IEnumerable<SpaceObjectViewModel> GetSpaceScrap()
        {
            var data = DataGatheringService.GetDebrisFromNasa();

            var result = PreProcessService.GetParsedObjects(data, SpaceObjectType.Debris);

            return result;
        }

        public List<SpaceObjectViewModel> GetSpaceSatellites()
        {
            var data = DataGatheringService.GetSatelitesFromNasa();

            var result = PreProcessService.GetParsedObjects(data, SpaceObjectType.Satelite);
            return result.ToList();
        }
    }
}
