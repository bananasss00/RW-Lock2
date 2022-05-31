using Verse;

namespace Locks2.MoreFilters
{
    [StaticConstructorOnStartup]
    public static class Initializer
    {
        static Initializer()
        {
            new HarmonyLib.Harmony("PirateBY.Locks2.MoreFilters").PatchAll();
        }
    }
}