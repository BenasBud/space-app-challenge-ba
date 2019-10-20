using Microsoft.AspNetCore.Mvc;
using SpaceApp.Game.AppServices.DataServices;
using SpaceApp.Game.Models;
using SpaceApp.Game.Services;
using SpaceApp.Game.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceApp.Game.Controllers
{
    [Route("")]
    [ApiController]
    [Produces("application/json")]
    public class RoomsController : ControllerBase
    {
        private readonly GameEngine _gameEngine;
        private readonly NasaDataService _nasaDataService;

        public RoomsController(GameEngine gameEngine, NasaDataService nasaDataService)
        {
            _gameEngine = gameEngine;
            _nasaDataService = nasaDataService;
        }
        [HttpGet("api/Rooms/GetRooms")]
        public ActionResult<IEnumerable<Room>> GetRooms()
        {
            var rooms = _gameEngine.GetRooms();
            return Ok(rooms);
        }

        /// <summary>
        /// Create initial room and populate scaps
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        [HttpPost("api/Rooms/CreateRoom")]
        public ActionResult<Room> CreateRoom()
        {

            var roomName = PreProcessService.GenerateRoomName();
            var gameRoom = new GameRoom(roomName);

            var spaceScrap = _nasaDataService.GetSpaceScrap();
            if (spaceScrap is null)
            {
                return BadRequest("There is no scrap to cleanup. Well done.");
            }

            gameRoom.Game.Load(spaceScrap.Select(scrap => 
                GameObject.CreateScrap(new Position(scrap.X, scrap.Y), scrap.SizeCoeficient, scrap.Name)).ToList());

            _gameEngine.AddNewRoom(gameRoom);

            return Ok(gameRoom.GetRoom());
        }
        /// <summary>
        /// start if room created
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/Rooms/JoinRoom")]
        public ActionResult<IEnumerable<string>> JoinRoom(JoinRoomModel model)
        {
            //_gameEngine.JoinUserToRoom(model.RoomId, model.UserName);
            // subscribe to hub
            return Ok();
        }

        [HttpGet("api/Rooms/GetScoreBoard")]
        public ActionResult<IEnumerable<EndGameInfo>> GetScoreBoard(Guid roomId)
        {
            var gameRoom = _gameEngine.GameRooms.Single(x => x.RoomId == roomId);
            var users = gameRoom.Game.GameObjects.Where(x => x.Type == GameObjectType.Player).OrderBy(x => x.Size);
            var scoreBoard = new List<EndGameInfo>();
            foreach (var user in users)
            {
                scoreBoard.Add(new EndGameInfo { UserId = user.Name, Score = user.Size });
            }
            return scoreBoard;
        }
    }
}