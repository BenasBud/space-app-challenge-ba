using Microsoft.AspNetCore.Mvc;
using SpaceApp.Game.AppServices;
using SpaceApp.Game.Models;
using SpaceApp.Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using SpaceApp.Game.AppServices.DataServices;

namespace SpaceApp.Game.Controllers
{

    [Route("")]
    [ApiController]
    public class GameController : ControllerBase
    {

        private readonly NasaDataService _nasaDataService;
        private readonly GameEngine _gameEngine;

        public GameController(NasaDataService nasaDataService, GameEngine gameEngine)
        {
            _nasaDataService = nasaDataService;
            _gameEngine = gameEngine;
        }


        [HttpPost("api/Game/StartGame")]
        public ActionResult StartGame(Guid roomId)
        {
            var room = _gameEngine.GameRooms.FirstOrDefault(x => x.RoomId == roomId);
            if (room is null)
            {
                return BadRequest("Room does not exist");
            }
            //undone: from room
            var spaceScrap = _nasaDataService.GetSpaceScrap();
            if (spaceScrap != null)
            {
                room.Game.Load(spaceScrap.Select(scrap => GameObject.CreateScrap(new Position(scrap.X, scrap.Y), scrap.SizeCoeficient, scrap.Name)).ToList());
            }

            var satellite = _nasaDataService.GetSpaceSatellites();
            if (satellite != null)
            {
                room.Game.Load(satellite.Select(satell => GameObject.CreateSatellite(new Position(satell.X, satell.Y), satell.SizeCoeficient, satell.Name)).ToList());
            }

            return Ok();
        }
        [HttpPost("api/Game/LeaveGame")]
        public ActionResult<IEnumerable<string>> LeaveGame(Guid roomId)
        {
            return new string[] { "value1", "value2" };
        }
        [HttpGet("api/Game/GetHighScore")]
        public ActionResult<IEnumerable<string>> GetHighScore()
        {
            return new string[] { "value1", "value2" };
        }
    }
}