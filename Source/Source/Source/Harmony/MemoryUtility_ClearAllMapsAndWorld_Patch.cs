using HarmonyLib;
using Locks2.Core;
using Verse.Profile;

namespace Locks2.Harmony
{
    [HarmonyPatch(typeof(MemoryUtility), nameof(MemoryUtility.ClearAllMapsAndWorld))]
    public class MemoryUtility_ClearAllMapsAndWorld_Patch
    {
        public static void Prefix()
        {
            Extensions.ClearCaches();
            LockConfig.ClearCaches();
        }
    }
}