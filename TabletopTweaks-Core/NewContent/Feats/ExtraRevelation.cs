﻿using Kingmaker.Blueprints.Classes.Selection;
using TabletopTweaks.Core.Utilities;
using TabletopTweaks.Core.Wrappers;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewContent.Feats {
    static class ExtraRevelation {
        public static void AddExtraRevelation() {
            var OracleRevelationSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("60008a10ad7ad6543b1f63016741a5d2");

            var ExtraRevelation = FeatTools.CreateExtraSelectionFeat(modContext: TTTContext, "ExtraRevelation", OracleRevelationSelection, bp => {
                bp.SetName("Extra Revelation");
                bp.SetDescription("You gain one additional revelation. You must meet all of the prerequisites for this revelation." +
                    "\nYou can gain Extra Revelation multiple times.");
            });
            if (TTTContext.AddedContent.Feats.IsDisabled("ExtraRevelation")) { return; }
            FeatTools.AddAsFeat(ExtraRevelation);
        }
    }
}