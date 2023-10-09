using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using System.Linq;

namespace TabletopTweaks.Core.NewComponents {
    public class AddAdditionalSpellDamage : EntityFactComponentDelegate,
        IRulebookHandler<RuleDealDamage>,
        IInitiatorRulebookHandler<RuleDealDamage>,
        ISubscriber, IInitiatorRulebookSubscriber {

        public bool CheckWeaponType;
        public bool CheckAbilityType;
        public AbilityType m_AbilityType;
        public bool CheckSpellDescriptor;
        public SpellDescriptorWrapper SpellDescriptorsList;
        public bool CheckSpellParent;
        public bool NotZeroDamage;
        public bool CheckDamageDealt;
        public CompareOperation.Type CompareType;
        public ContextValue TargetValue;
        public bool CheckEnergyDamageType;
        public DamageEnergyType EnergyType;
        public bool ApplyToAreaEffectDamage;
        public bool TargetKilledByThisDamage;
        public bool IgnoreDamageFromThisFact = true;
        public bool EnemyOnly;
        public BlueprintWeaponTypeReference m_WeaponType;
        public BlueprintAbilityReference[] m_AbilityList;
        public DamageTypeDescription m_DamageType;
        public ContextDiceValue m_DamageValue;

        public BlueprintWeaponType WeaponType => m_WeaponType?.Get();
        public ReferenceArrayProxy<BlueprintAbility, BlueprintAbilityReference> AbilityList => m_AbilityList;

        public void OnEventAboutToTrigger(RuleDealDamage evt) {
            if ((IgnoreDamageFromThisFact && evt.Reason.Fact == base.Fact)
                || (CheckWeaponType && evt.DamageBundle.Weapon?.Blueprint.Type != WeaponType)
                || (CheckAbilityType && evt.Reason.Ability?.Blueprint.Type != m_AbilityType)
                || (CheckSpellDescriptor && evt.Reason.Ability == null)
                || !evt.Reason.Ability.Blueprint.SpellDescriptor.HasFlag((SpellDescriptor)SpellDescriptorsList)
                || (!ApplyToAreaEffectDamage && evt.SourceArea)
                || (CheckEnergyDamageType && evt.DamageBundle
                    .Aggregate(false, (acc, dmg) => acc || (dmg.Type == DamageType.Energy && ((EnergyDamage)dmg).EnergyType == EnergyType)))
                || (EnemyOnly && evt.Initiator.IsEnemy(evt.Target))
            ) {
                return;
            }

            if (CheckDamageDealt || NotZeroDamage) {
                var fakeDamage = evt.DamageBundle.First.Copy();
                fakeDamage.CopyFrom(evt.DamageBundle.First);
                var fakeRule = new RuleDealDamage(evt.Initiator, evt.Target, fakeDamage) {
                    SourceAbility = base.Context.SourceAbility,
                    SourceArea = base.Context.AssociatedBlueprint as BlueprintAbilityAreaEffect,
                    IsFake = true
                };
                var total = base.Context.TriggerRule(new RuleCalculateDamage(fakeRule))
                    .CalculatedDamage.Sum(x => (x.Source.Type == DamageType.Direct) ? x.FinalValue : 0);
                if (NotZeroDamage && total > 0) { return; }
                if (CheckDamageDealt && !(CompareType.CheckCondition(total, TargetValue.Calculate(base.Fact.MaybeContext)))) { return; }
            }

            var damage = this.m_DamageType?.CreateDamage(
                dice: new DiceFormula(m_DamageValue.DiceCountValue.Calculate(base.Context), m_DamageValue.DiceType),
                bonus: m_DamageValue.BonusValue.Calculate(base.Context)
            );
            if (damage == null) { return; }
            damage.SourceFact = this.Fact;
            evt.Add(damage);
        }

        public void OnEventDidTrigger(RuleDealDamage evt) {

        }

    }
}
