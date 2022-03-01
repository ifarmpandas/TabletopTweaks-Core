﻿using Kingmaker.EntitySystem.Stats;
using TabletopTweaks.Core.Utilities;
using TabletopTweaks.Core.Wrappers;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewContent.Feats {
    static class NatureSoul {
        public static void AddNatureSoul() {
            var NatureSoul = FeatTools.CreateSkillFeat(modContext: TTTContext, "NatureSoul", StatType.SkillLoreNature, StatType.SkillPerception, bp => {
                bp.SetName("Nature's Soul");
                bp.SetDescription("You are innately in tune with nature and venerate the power and mystery of the natural world." +
                    "\nYou get a +2 bonus on all Lore (Nature) checks and Perception checks. " +
                    "If you have 10 or more ranks in one of these skills, the bonus increases to +4 for that skill.");
            });
            if (TTTContext.AddedContent.Feats.IsDisabled("NatureSoul")) { return; }
            FeatTools.AddAsFeat(NatureSoul);
        }
    }
}