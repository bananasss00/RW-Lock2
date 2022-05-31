// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection.Emit;
// using HarmonyLib;
// using Locks2;
// using Locks2.Core;
// using Verse;
//
// namespace ConfigRuleDrons
// {
//     [AttributeUsage(AttributeTargets.Class)]
//     public class RuleInject : Attribute
//     {
//         public Type injectAfter;
//         public RuleInject(Type injectAfter)
//         {
//             this.injectAfter = injectAfter;
//         }
//     }
//
//     [StaticConstructorOnStartup]
//     public static class ConfigRuleInjector
//     {
//         private static List<Type> _ruleInjectAll;
//
//         static ConfigRuleInjector()
//         {
//             if (!LoadedModManager.RunningModsListForReading.Any(x => x.Name.Equals("Misc. Robots")))
//                 return;
//
//             ConfigRuleDrons.Init();
//
//             _ruleInjectAll = GenTypes.AllTypes.Where(m => m.TryGetAttribute<RuleInject>(out _)).ToList();
//
//             var h = new Harmony("pirateby.locks2.newrules.sk");
//
//             h.Patch(AccessTools.Method(typeof(Locks2.Base), nameof(Locks2.Base.ResetSettings)),
//                 transpiler: new HarmonyMethod(typeof(ConfigRuleInjector), nameof(Base_ResetSettings_Transpiler)));
//
//             h.Patch(AccessTools.Method(typeof(Locks2.Core.LockConfig), nameof(Locks2.Core.LockConfig.Initailize)),
//                 prefix: new HarmonyMethod(typeof(ConfigRuleInjector), nameof(LockConfig_Initailize_Prefix)),
//                 postfix: new HarmonyMethod(typeof(ConfigRuleInjector), nameof(LockConfig_Initailize_Postfix)));
//         }
//
//         private static void InsertAfterItem<T>(this List<T> list, T afterItem, T item)
//         {
//             if (afterItem != null)
//             {
//                 var idx = list.IndexOf(afterItem) + 1;
//                 if (idx < list.Count)
//                     list.Insert(idx, item);
//                 else
//                     list.Add(item);
//             }
//             else
//             {
//                 list.Add(item);
//                 Log.Warning($"[Locks2ConfigRuleInjector] Can't find rule injectAfter for type: {afterItem}");
//             }
//         }
//
//         #region LockConfig_Initailize
//         private static void LockConfig_Initailize_Prefix(ref bool __state)
//         {
//             __state = Finder.settings.defaultRules.Count > 0;
//         }
//         /*
//         public void Initailize()
//         {
//             rules = new List<IConfigRule>();
//             if (Finder.settings.defaultRules.Count > 0)
//             {
//                 foreach (var type in Finder.settings.defaultRules)
//                 {
//                     rules.Add(Activator.CreateInstance(type) as IConfigRule);
//                 }
//             }
//             else
//             {
//                 rules.Add(new ConfigRuleAnimals());
//                 rules.Add(new ConfigRuleColonists());
//                 rules.Add(new ConfigRuleGuests());
//                 rules.Add(new ConfigRuleIgnorDrafted());
//             }
//         }
//         */
//         private static void LockConfig_Initailize_Postfix(LockConfig __instance, bool __state)
//         {
//             if (!__state) // not used default rules
//             {
//                 var rules = __instance.rules;
//                 foreach (var t in _ruleInjectAll)
//                 {
//                     var ruleInject = t.TryGetAttribute<RuleInject>();
//                     var ruleInstance = Activator.CreateInstance(t) as LockConfig.IConfigRule;
//                     var item = rules.FirstOrDefault(r => r.GetType() == ruleInject.injectAfter);
//                     rules.InsertAfterItem(item, ruleInstance);
//                 }
//             }
//         }
//         #endregion #region LockConfig_Initailize
//
//         #region Base_ResetSettings
//         private static void Base_ResetSettings_BeforeWriteSettings(Locks2.Base instance)
//         {
//             var defRules = instance.settings.defaultRules;
//             foreach (var t in _ruleInjectAll)
//             {
//                 var ruleInject = t.TryGetAttribute<RuleInject>();
//                 var item = defRules.FirstOrDefault(r => r == ruleInject.injectAfter);
//                 defRules.InsertAfterItem(item, t);
//             }
//         }
//
//         private static IEnumerable<CodeInstruction> Base_ResetSettings_Transpiler(IEnumerable<CodeInstruction> instructions)
//         {
//             var defaultRules = AccessTools.Field(typeof(Locks2.Settings), nameof(Locks2.Settings.defaultRules));
//             foreach (var ci in instructions)
//             {
//                 yield return ci;
//                 if (ci.opcode == OpCodes.Stfld && ci.operand == defaultRules)
//                 {
//                     /* Add after assign:
//                         settings.defaultRules = new List<Type>() {
//                             typeof(ConfigRuleAnimals),
//                             typeof(ConfigRuleColonists),
//                             typeof(ConfigRuleGuests),
//                             typeof(ConfigRuleIgnorDrafted)
//                         };
//                      */
//                     yield return new CodeInstruction(OpCodes.Ldarg_0);
//                     yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ConfigRuleInjector), nameof(Base_ResetSettings_BeforeWriteSettings)));
//                 }
//             }
//         }
//         #endregion Base_ResetSettings
//     }
// }