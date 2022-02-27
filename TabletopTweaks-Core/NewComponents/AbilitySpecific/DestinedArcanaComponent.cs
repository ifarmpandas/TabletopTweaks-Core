﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using UnityEngine;

namespace TabletopTweaks.Core.NewComponents.AbilitySpecific {
    /// <summary>
    /// When a spell is cast, applies a buff based on the spell level of the cast spell.
    /// </summary>
    [TypeId("080a6418d97c4bd48cd37851b9cfe89e")]
    public class DestinedArcanaComponent : UnitFactComponentDelegate,
        IInitiatorRulebookHandler<RuleCastSpell>,
        IRulebookHandler<RuleCastSpell>, ISubscriber,
        IInitiatorRulebookSubscriber {

        public void OnEventAboutToTrigger(RuleCastSpell evt) {
            if (evt.Spell != null && evt.Spell.Spellbook != null && evt.Spell.Blueprint.Type == AbilityType.Spell && evt.Spell.Blueprint.Range == AbilityRange.Personal) {
                int level = evt.Context.SpellLevel - 1;
                if (level > 8 || level < 0) { return; }
                ApplyBuff(evt.Context, level);
            }
        }

        private void ApplyBuff(MechanicsContext mechanicsContext, int buff) {
            _ = Owner.AddBuff(Buffs[buff].Get(), mechanicsContext, new Rounds(1).Seconds);
        }

        public void OnEventDidTrigger(RuleCastSpell evt) {

        }
        /// <summary>
        /// Buffs applied when a spell is cast. The index of the buff corresponds to the spell level + 1.
        /// </summary>
        [SerializeField]
        public BlueprintBuffReference[] Buffs;
    }
}
