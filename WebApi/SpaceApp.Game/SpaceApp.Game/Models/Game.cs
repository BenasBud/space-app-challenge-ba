using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceApp.Game.Models
{
    public class Game
    {
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        public void Load(List<GameObject> gameObjects)
        {
            GameObjects = gameObjects;
        }

        public GameObject DigestObject(Guid winner, Guid looser, out GameObject winnerObject1)
        {
            var winnerObject = GameObjects.FirstOrDefault(x => x.GameObjectId == winner);
            var looserObject = GameObjects.FirstOrDefault(x => x.GameObjectId == looser);

            if (winnerObject is null || looserObject is null)
            {
                winnerObject1 = null;
                return null;
            }

            winnerObject.Digest(looserObject);
            
            GameObjects.Remove(looserObject);
            winnerObject1 = winnerObject;
            return looserObject;
        }

        public Task MoveObject(Guid gameObjectId, double x, double y)
        {
            var gameObject = GameObjects.FirstOrDefault(gObject => gObject.GameObjectId == gameObjectId);
            gameObject?.Move(x, y);
            return Task.CompletedTask;
        }

        public void Remove(Guid objectId)
        {
            var objectToRemove = GameObjects.FirstOrDefault(x => x.GameObjectId == objectId);
            if (objectToRemove != null)
            {
                GameObjects.Remove(objectToRemove);
            }
        }
    }
}
