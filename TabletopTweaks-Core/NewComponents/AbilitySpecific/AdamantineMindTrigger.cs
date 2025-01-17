﻿using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;

namespace TabletopTweaks.Core.NewComponents.AbilitySpecific {
    [TypeId("4e0c887660ff412fa3fe63524e475e90")]
    public class AdamantineMindTrigger : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleSavingThrow>, IRulebookHandler<RuleSavingThrow>, ISubscriber, IInitiatorRulebookSubscriber {
        public void OnEventAboutToTrigger(RuleSavingThrow evt) {
        }

        public void OnEventDidTrigger(RuleSavingThrow evt) {
            if (this.CheckConditions(evt) && base.Fact.MaybeContext != null) {
                MechanicsContext maybeContext = base.Fact.MaybeContext;
                using ((maybeContext != null) ? maybeContext.GetDataScope(base.Owner) : null) {
                    IFactContextOwner factContextOwner = base.Fact as IFactContextOwner;
                    if (factContextOwner != null) {
                        var NewSave = base.Context.TriggerRule(new RuleSavingThrow(evt.Reason.Context.MaybeCaster, SavingThrowType.Will, evt.DifficultyClass));
                        if (!NewSave.IsPassed) {
                            factContextOwner.RunActionInContext(this.Action, evt.Reason.Context.MaybeCaster);
                        } 
                    }
                }
            }
        }
        private bool CheckConditions(RuleSavingThrow evt) {
            return evt.IsPassed && evt.Reason?.Context.MaybeCaster != null && (evt.Reason?.Context.SpellDescriptor & this.Descriptor) != SpellDescriptor.None;
        }

        public SpellDescriptorWrapper Descriptor = SpellDescriptor.MindAffecting;
        public ActionList Action;
    }
}
