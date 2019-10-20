using Microsoft.AspNetCore.Mvc;
using SpaceApp.Game.Enums;
using SpaceApp.Game.Models;
using SpaceApp.Game.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceApp.Game.Controllers
{
    public class SpaceObjectsController : ControllerBase
    {
        [HttpGet("api/SpaceObjects/Debris")]
        public ActionResult<List<SpaceObjectViewModel>> GetDebris()
        {
            var data = DataGatheringService.GetDebrisFromNasa();

            var result = PreProcessService.GetParsedObjects(data,SpaceObjectType.Debris);

            return Ok(result);
        }

        [HttpGet("api/SpaceObjects/Satelites")]
        public ActionResult<List<SpaceObjectViewModel>> GetSatelites()
        {
            var data =  DataGatheringService.GetSatelitesFromNasa();

            var result = PreProcessService.GetParsedObjects(data, SpaceObjectType.Satelite);

            return Ok(result);
        }
    }
}
