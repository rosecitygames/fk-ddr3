using IndieDevTools.Commands;
using IndieDevTools.Maps;

namespace IndieDevTools.Demo.CrabBattle
{
    public class Explode : AbstractCommand
    {
        IMapElement mapElement = null;
        IExplodable explodable = null;

        protected override void OnStart()
        {
            explodable.Explode();
            mapElement.RemoveFromMap();
            Complete();
        }

        public static ICommand Create(IMapElement mapElement, IExplodable explodable)
        {
            return new Explode
            {
                mapElement = mapElement,
                explodable = explodable
            };
        }
    }
}
