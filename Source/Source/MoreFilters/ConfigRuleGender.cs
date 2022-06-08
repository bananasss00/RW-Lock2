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
    public class ConfigRuleGender : LockConfig.IConfigRule
    {
        public override float Height => 80;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Allows(Pawn pawn)
        {
            if (!enabled) return false;
            switch (pawn.gender)
            {
                case Gender.Female:
                    return female;
                case Gender.Male:
                    return male;
                case Gender.None:
                    return male && female;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void DoContent(IEnumerable<Pawn> pawns, Rect rect, Action notifySelectionBegan, Action notifySelectionEnded)
        {
            var before = enabled;
            Widgets.CheckboxLabeled(rect.TopPartPixels(enabled ? 25 : 54), "Locks2GenderFilter".Translate(), ref enabled);
            if (enabled)
            {
                rect = rect.TopPartPixels(20);
                rect.position += new Vector2(0, 25);
                rect.size = new Vector2(rect.size.x, 25);
                Text.Font = GameFont.Tiny;

                var beforeFemale = female;
                Widgets.CheckboxLabeled(rect, "Female".Translate(), ref female);
                if (beforeFemale != female)
                {
                    LockConfig.Notify_Dirty();
                }

                rect.position += new Vector2(0, 25);
                var beforeMale = male;
                Widgets.CheckboxLabeled(rect, "Male".Translate(), ref male);
                if (beforeMale != male)
                {
                    LockConfig.Notify_Dirty();
                }
            }

            if (before != enabled)
            {
                LockConfig.Notify_Dirty();
            }
        }

        public override LockConfig.IConfigRule Duplicate()
        {
            return new ConfigRuleGender() { condition = condition, enabled = enabled, male = male, female = female};
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.enabled, "enabled", true, false);
            Scribe_Values.Look<bool>(ref this.male, "male", true, false);
            Scribe_Values.Look<bool>(ref this.female, "female", true, false);
        }

        public bool enabled = true;
        public bool male = true;
        public bool female = true;
    }
}
