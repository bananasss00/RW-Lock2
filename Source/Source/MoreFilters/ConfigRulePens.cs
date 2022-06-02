using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Locks2;
using Locks2.Core;
using UnityEngine;
using Verse;

namespace Locks2.MoreFilters
{
    [HarmonyPatch(typeof(LockConfig.ConfigRuleAnimals), nameof(LockConfig.ConfigRuleAnimals.Allows))]
    [HotSwappable]
    public class ConfigRuleAnimals_Fix
    {
        [HarmonyPostfix]
        public static void Allows(Pawn pawn, ref bool __result)
        {
            if (__result == false)
                return;

            var pensFilter = Finder.currentConfig?.rules.OfType<ConfigRulePens>().FirstOrDefault();
            if (pensFilter == null || !pensFilter.enabled)
                return;

            __result = pensFilter.Allows(pawn);
            // Log.Warning($"{pawn} pens filter result {__result}");
        }
    }

    [HotSwappable]
    public class ConfigRulePens : LockConfig.IConfigRule
    {
        public override float Height => 54;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Allows(Pawn pawn)
        {
            if (!enabled) return false;
            
            var door = Finder.currentConfig?.door;
            if (door == null) return false;

            if (pawn.RaceProps.FenceBlocked && !door.def.building.roamerCanOpen && (!pawn.roping.IsRopedByPawn || !Allows(pawn.roping.RopedByPawn)))
            {
                return false;
            }

            return true;
        }

        public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan, Action notifySelectionEnded)
        {
            var before = enabled;
            Widgets.CheckboxLabeled(rect, "Locks2PensFilter".Translate(), ref enabled);
            if (before != enabled)
            {
                LockConfig.Notify_Dirty();
            }
        }

        public override LockConfig.IConfigRule Duplicate()
        {
            return new ConfigRulePens() { condition = condition, enabled = enabled };
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.enabled, "enabled", true, false);
        }

        public bool enabled = true;

    }
}
