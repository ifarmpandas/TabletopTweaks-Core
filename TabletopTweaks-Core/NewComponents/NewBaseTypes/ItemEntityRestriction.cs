﻿using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Items;
using Kingmaker.UnitLogic;
using System;
using System.Linq;

namespace TabletopTweaks.Core.NewComponents.NewBaseTypes {
    [AllowedOn(typeof(BlueprintItemEquipment), false)]
    [TypeId("342e5233ad9042ad820cec4d62e6e7c3")]
    public abstract class ItemEntityRestriction : BlueprintComponent {
        /// <summary>
        /// Checks if the specified unit can equip the specified item.
        /// </summary>
        /// <param name="unit">
        /// Unit to check.
        /// </param>
        /// <param name="item">
        /// Item to check.
        /// </param>
        /// <returns>
        /// true if the specified unit can equip the specified item.
        /// </returns>
        public abstract bool CanBeEquippedBy(UnitDescriptor unit, ItemEntity item);

        [HarmonyPatch(typeof(ItemEntity), nameof(ItemEntity.CanBeEquippedInternal), new Type[] { typeof(UnitDescriptor) })]
        internal static class AbilityData_IsAvailableInSpellbook_QuickStudy_Patch {
            static void Postfix(ItemEntity __instance, UnitDescriptor owner, ref bool __result) {
                BlueprintItemEquipment blueprintItemEquipment = __instance.Blueprint as BlueprintItemEquipment;
                __result &= blueprintItemEquipment.GetComponents<ItemEntityRestriction>()
                    .Aggregate(true, (bool r, ItemEntityRestriction restriction) => r && restriction.CanBeEquippedBy(owner, __instance));
            }
        }
    }
}
