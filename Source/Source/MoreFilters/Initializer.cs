using System;
using Verse;

namespace Locks2.MoreFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }

    [StaticConstructorOnStartup]
    public static class Initializer
    {
        static Initializer()
        {
            new HarmonyLib.Harmony("PirateBY.Locks2.MoreFilters").PatchAll();
        }
    }
}