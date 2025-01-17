﻿using HarmonyLib;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewEvents {
    public interface IStatBonusCalculatedHandler : IGlobalSubscriber {

        void StatBonusCalculated(ref int value, StatType stat, ModifierDescriptor descriptor, Buff source);

        private class EventTriggers {

            [HarmonyPatch(typeof(AddStatBonus), nameof(AddStatBonus.OnTurnOn))]
            static class AddStatBonus_Idealize_Patch {
                static readonly MethodInfo Modifier_AddModifierUnique = AccessTools.Method(typeof(ModifiableValue), "AddModifierUnique", new Type[] {
                typeof(int),
                typeof(EntityFactComponent),
                typeof(ModifierDescriptor)
            });
                static readonly MethodInfo EventTriggers_AddEvent = AccessTools.Method(
                    typeof(EventTriggers),
                    nameof(EventTriggers.CallEvent),
                    new Type[] { typeof(int), typeof(AddStatBonus) }
                );
                //Add Idealize calculations
                static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

                    var codes = new List<CodeInstruction>(instructions);
                    int target = FindInsertionTarget(codes);
                    //Utilities.ILUtils.LogIL(TTTContext, codes);
                    codes.InsertRange(target, new CodeInstruction[] {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, EventTriggers_AddEvent)
                    });
                    //Utilities.ILUtils.LogIL(TTTContext, codes);
                    return codes.AsEnumerable();
                }
                private static int FindInsertionTarget(List<CodeInstruction> codes, int startingIndex = 0) {
                    int target = -1;
                    for (int i = startingIndex; i < codes.Count; i++) {
                        //Find where the modifier is added and grab the load of the value varriable
                        //if (codes[i].opcode == OpCodes.Ldloc_0) { target = i + 1; }
                        if (codes[i].opcode == OpCodes.Stloc_0) {
                            target = i;
                        }
                    }
                    if (target < 0) {
                        TTTContext.Logger.Log("ADD STAT IDEALIZE PATCH - AddStatBonus: COULD NOT FIND TARGET");
                    }
                    return target;
                }
                /*
                private static int FindInsertionTarget(List<CodeInstruction> codes) {
                    int target = 0;
                    for (int i = 0; i < codes.Count; i++) {
                        //Find where the modifier is added and grab the load of the value varriable
                        if (codes[i].opcode == OpCodes.Ldloc_0) { target = i + 1; }
                        if (codes[i].Calls(Modifier_AddModifierUnique)) {
                            return target;
                        }
                    }
                    TTTContext.Logger.Log("ADD STAT IDEALIZE PATCH - AddStatBonus: COULD NOT FIND TARGET");
                    return -1;
                }
                */
            }

            [HarmonyPatch(typeof(AddContextStatBonus), nameof(AddContextStatBonus.OnTurnOn))]
            static class AddContextStatBonus_Idealize_Patch 
                {
                static readonly MethodInfo Modifier_AddModifier = AccessTools.Method(typeof(ModifiableValue), "AddModifier", new Type[] {
                typeof(int),
                typeof(EntityFactComponent),
                typeof(ModifierDescriptor)
            });
                static readonly MethodInfo EventTriggers_AddEvent = AccessTools.Method(
                    typeof(EventTriggers),
                    nameof(EventTriggers.CallEvent),
                    new Type[] { typeof(int), typeof(AddContextStatBonus) }
                );
                //Add Idealize calculations
                static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

                    var codes = new List<CodeInstruction>(instructions);
                    int target = FindInsertionTarget(codes);
                    //Utilities.ILUtils.LogIL(TTTContext, codes);
                    codes.InsertRange(target, new CodeInstruction[] {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, EventTriggers_AddEvent)
                    });
                    /*
                    target = FindInsertionTarget(codes, target);
                    codes.InsertRange(target, new CodeInstruction[] {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, EventTriggers_AddEvent)
                    });
                    */
                    //Utilities.ILUtils.LogIL(TTTContext, codes);
                    return codes.AsEnumerable();
                }
                private static int FindInsertionTarget(List<CodeInstruction> codes, int startingIndex = 0) {
                    int target = -1;
                    for (int i = startingIndex; i < codes.Count; i++) {
                        //Find where the modifier is added and grab the load of the value varriable
                        //if (codes[i].opcode == OpCodes.Ldloc_0) { target = i + 1; }
                        if (codes[i].opcode == OpCodes.Stloc_0) {
                            target = i;
                        }
                    }
                    if (target < 0) {
                        TTTContext.Logger.Log("ADD STAT IDEALIZE PATCH - AddContextStatBonus: COULD NOT FIND TARGET");
                    }
                    return target;
                }
                /*
                private static int FindInsertionTarget(List<CodeInstruction> codes, int startingIndex = 0) {
                    int target = startingIndex;
                    for (int i = startingIndex; i < codes.Count; i++) {
                        //Find where the modifier is added and grab the load of the value varriable
                        if (codes[i].opcode == OpCodes.Ldloc_0) { target = i + 1; }
                        if (codes[i].Calls(Modifier_AddModifier) && target != startingIndex) {
                            return target;
                        }
                    }
                    TTTContext.Logger.Log("ADD STAT IDEALIZE PATCH - AddContextStatBonus: COULD NOT FIND TARGET");
                    return -1;
                }
                */
            }

            [HarmonyPatch(typeof(AddGenericStatBonus), nameof(AddStatBonus.OnTurnOn))]
            static class AddGenericStatBonus_Idealize_Patch {
                static readonly MethodInfo Modifier_AddModifierUnique = AccessTools.Method(typeof(ModifiableValue), "AddModifierUnique", new Type[] {
                typeof(int),
                typeof(EntityFactComponent),
                typeof(ModifierDescriptor)
            });
                static readonly MethodInfo EventTriggers_AddEvent = AccessTools.Method(
                    typeof(EventTriggers),
                    nameof(EventTriggers.CallEvent),
                    new Type[] { typeof(int), typeof(AddGenericStatBonus) }
                );
                //Add Idealize calculations
                static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

                    var codes = new List<CodeInstruction>(instructions);
                    int target = FindInsertionTarget(codes);
                    //Utilities.ILUtils.LogIL(TTTContext, codes);
                    codes.InsertRange(target, new CodeInstruction[] {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, EventTriggers_AddEvent)
                    });
                    //Utilities.ILUtils.LogIL(TTTContext, codes);
                    return codes.AsEnumerable();
                }
                private static int FindInsertionTarget(List<CodeInstruction> codes, int startingIndex = 0) {
                    int target = -1;
                    for (int i = startingIndex; i < codes.Count; i++) {
                        //Find where the modifier is added and grab the load of the value varriable
                        //if (codes[i].opcode == OpCodes.Ldloc_0) { target = i + 1; }
                        if (codes[i].opcode == OpCodes.Stloc_0) {
                            target = i;
                        }
                    }
                    if (target < 0) {
                        TTTContext.Logger.Log("ADD STAT IDEALIZE PATCH - AddGenericStatBonus: COULD NOT FIND TARGET");
                    }
                    return target;
                }
                /*
                private static int FindInsertionTarget(List<CodeInstruction> codes) {
                    int target = 0;
                    for (int i = 0; i < codes.Count; i++) {
                        //Find where the modifier is added and grab the load of the value varriable
                        if (codes[i].opcode == OpCodes.Ldloc_0) { target = i + 1; }
                        if (codes[i].Calls(Modifier_AddModifierUnique)) {
                            return target;
                        }
                    }
                    TTTContext.Logger.Log("ADD STAT IDEALIZE PATCH - AddGenericStatBonus: COULD NOT FIND TARGET");
                    return -1;
                }
                */
            }

            [HarmonyPatch(typeof(AddStatBonusAbilityValue), nameof(AddStatBonusAbilityValue.OnTurnOn))]
            static class AddStatBonusAbilityValue_Idealize_Patch {
                static readonly MethodInfo Modifier_AddModifierUnique = AccessTools.Method(typeof(ModifiableValue), "AddModifierUnique", new Type[] {
                typeof(int),
                typeof(EntityFactComponent),
                typeof(ModifierDescriptor)
            });
                static readonly MethodInfo EventTriggers_AddEvent = AccessTools.Method(
                    typeof(EventTriggers),
                    nameof(EventTriggers.CallEvent),
                    new Type[] { typeof(int), typeof(AddStatBonusAbilityValue) }
                );
                static readonly MethodInfo ContextValue_Calculate = AccessTools.Method(
                    typeof(ContextValue),
                    nameof(ContextValue.Calculate),
                    new Type[] { typeof(MechanicsContext) }
                );
                //Add Idealize calculations
                static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

                    var codes = new List<CodeInstruction>(instructions);
                    int target = FindInsertionTarget(codes);
                    //Utilities.ILUtils.LogIL(TTTContext, codes);
                    codes.InsertRange(target, new CodeInstruction[] {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, EventTriggers_AddEvent)
                    });
                    //Utilities.ILUtils.LogIL(TTTContext, codes);
                    return codes.AsEnumerable();
                }
                private static int FindInsertionTarget(List<CodeInstruction> codes) {
                    int target = 0;
                    for (int i = 0; i < codes.Count; i++) {
                        //Find where the modifier is added and grab the load of the value varriable
                        if (codes[i].Calls(ContextValue_Calculate)) { target = i + 1; }
                        if (codes[i].Calls(Modifier_AddModifierUnique)) {
                            return target;
                        }
                    }
                    TTTContext.Logger.Log("ADD STAT IDEALIZE PATCH - AddGenericStatBonus: COULD NOT FIND TARGET");
                    return -1;
                }
            }

            private static int CallEvent(int value, AddStatBonus component) {
                return CallEvent(value, component.Stat, component.Descriptor, component.Fact as Buff);
            }
            private static int CallEvent(int value, AddContextStatBonus component) {
                return CallEvent(value, component.Stat, component.Descriptor, component.Fact as Buff);
            }
            private static int CallEvent(int value, AddGenericStatBonus component) {
                return CallEvent(value, component.Stat, component.Descriptor, component.Fact as Buff);
            }
            private static int CallEvent(int value, AddStatBonusAbilityValue component) {
                return CallEvent(value, component.Stat, component.Descriptor, component.Fact as Buff);
            }
            private static int CallEvent(int value, StatType stat, ModifierDescriptor descriptor, Buff source) {
                if (source == null) { return value; }
                EventBus.RaiseEvent<IStatBonusCalculatedHandler>(h => h.StatBonusCalculated(ref value, stat, descriptor, source));
                return value;
            }
        }
    }
}
