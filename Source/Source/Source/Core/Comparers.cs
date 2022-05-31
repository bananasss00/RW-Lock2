using System.Collections.Generic;
using Verse;

namespace Locks2.Core
{
    // HashSet.Contains use Equals function(class Pawn not override this method, so after loadMap same pawns not equal!!!)
    public class PawnComparer : IEqualityComparer<Pawn>
    {
        public static readonly PawnComparer Instance = new PawnComparer();

        public bool Equals(Pawn x, Pawn y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.thingIDNumber == y.thingIDNumber;
        }

        public int GetHashCode(Pawn obj)
        {
            return obj.thingIDNumber;
        }
    }

	public class GameConditionDefComparer : IEqualityComparer<GameConditionDef>
    {
        public static readonly GameConditionDefComparer Instance = new GameConditionDefComparer();

        public bool Equals(GameConditionDef x, GameConditionDef y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.shortHash == y.shortHash;
        }

        public int GetHashCode(GameConditionDef obj)
        {
            return obj.GetHashCode();
        }
    }
}