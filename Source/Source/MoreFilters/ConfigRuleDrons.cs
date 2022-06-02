using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Locks2.Core;
using UnityEngine;
using Verse;

namespace Locks2.MoreFilters
{
    // [RuleInject(typeof(LockConfig.ConfigRuleGuests))]
    public class ConfigRuleDrons : LockConfig.IConfigRule
    {
        public override float Height => 54;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Allows(Pawn pawn)
        {
            if (!enabled) return false;
            return kindDefNamePrefixes.Any(prefix => pawn.kindDef.defName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan, Action notifySelectionEnded)
        {
            var before = enabled;
            Widgets.CheckboxLabeled(rect, "Locks2DronsFilter".Translate(), ref enabled);
            if (before != enabled)
            {
                LockConfig.Notify_Dirty();
            }
        }

        public override LockConfig.IConfigRule Duplicate()
        {
            return new ConfigRuleDrons() { condition = condition, enabled = enabled };
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.enabled, "enabled", true, false);
        }

        private readonly string[] kindDefNamePrefixes = new string[] { "AIRobot_", "RPP_Bot_" };

        public bool enabled = true;

    }
}
