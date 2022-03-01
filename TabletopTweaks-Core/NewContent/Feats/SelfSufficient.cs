﻿using Kingmaker.EntitySystem.Stats;
using TabletopTweaks.Core.Utilities;
using TabletopTweaks.Core.Wrappers;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewContent.Feats {
    static class SelfSufficient {
        public static void AddSelfSufficient() {
            // Icon: Spell Focus? Alertness?
            var SelfSufficent = FeatTools.CreateSkillFeat(modContext: TTTContext, "Self-Sufficient", StatType.SkillLoreNature, StatType.SkillLoreReligion, bp => {
                bp.SetName("Self-Sufficient");
                bp.SetDescription("You know how to get along in the wild and how to effectively treat wounds." +
                    "\nYou get a +2 bonus on Lore (Nature) and " +
                    "Lore (Religion) skill checks. If you have 10 or more ranks in one of these skills," +
                    " the bonus increases to +4 for that skill.");
            });
            if (TTTContext.AddedContent.Feats.IsDisabled("Self-Sufficient")) { return; }
            FeatTools.AddAsFeat(SelfSufficent);
        }
    }
}