using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Locks2.Core;
using RimWorld;
using UnityEngine;
using Verse;

namespace Locks2.MoreFilters
{
    public class ConfigRuleHediffs : LockConfig.IConfigRule
    {
        private static IEnumerable<HediffDef> hediffDefs;
        public HashSet<HediffDef> blackSet = new HashSet<HediffDef>(HediffDefComparer.Instance);
        public bool enabled = true;

        private readonly List<HediffDef> removalHediffs = new List<HediffDef>();

        public override float Height => (enabled ? blackSet.Count * 25 + 75f : 54) + 15;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Allows(Pawn pawn)
        {
            if (!enabled) return false;
            if (pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayer)) return false;
            var hediffSet = pawn.health?.hediffSet;
            if (hediffSet == null) return false;

            foreach (var hediff in hediffSet.hediffs)
            {
                if (blackSet.Contains(hediff.def))
                    return false;
            }

            return true;
        }

        public override LockConfig.IConfigRule Duplicate()
        {
            return new ConfigRuleHediffs { enabled = enabled, blackSet = new HashSet<HediffDef>(blackSet, HediffDefComparer.Instance) };
        }

        public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan,
            Action notifySelectionEnded)
        {
            if (hediffDefs == null) hediffDefs = DefDatabase<HediffDef>.AllDefs;
            var before = enabled;
            Text.Font = GameFont.Small;
            Widgets.CheckboxLabeled(rect.TopPartPixels(25), "Locks2HediffsFilter".Translate(), ref enabled);
            Text.Font = GameFont.Tiny;
            if (enabled)
            {
                Widgets.Label(rect.TopPartPixels(50).BottomPartPixels(25),
                    "Locks2HediffsFilterBlacklist".Translate());
                var rowRect = rect.TopPartPixels(75).BottomPartPixels(25);
                removalHediffs.Clear();
                foreach (var def in blackSet)
                {
                    if (Widgets.ButtonText(rowRect, def.label))
                    {
                        LockConfig.Notify_Dirty();
                        removalHediffs.Add(def);
                    }

                    rowRect.y += 25;
                }
                foreach (var def in removalHediffs) blackSet.Remove(def);
                if (Widgets.ButtonText(rowRect, "+"))
                {
                    Find.CurrentMap.reachability.ClearCache();
                    notifySelectionBegan.Invoke();
                    DoExtraContent(def =>
                    {
                        blackSet.Add(def as HediffDef);
                        LockConfig.Notify_Dirty();
                    }, hediffDefs.Where(def => !blackSet.Contains(def)), notifySelectionEnded);
                }
            }

            if (before != enabled)
            {
                LockConfig.Notify_Dirty();
                Find.CurrentMap.reachability.ClearCache();
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enabled, "enabled", true);
            Scribe_Collections.Look(ref blackSet, "blackSet", LookMode.Def);
            if (blackSet == null) blackSet = new HashSet<HediffDef>(HediffDefComparer.Instance);
        }

        private void DoExtraContent(Action<Def> onSelection, IEnumerable<HediffDef> defs, Action notifySelectionEnded)
        {
            ITab_Lock.currentSelector = new Selector_DefSelection(defs, onSelection, true, notifySelectionEnded);
        }
    }
}