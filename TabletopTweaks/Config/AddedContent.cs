﻿
namespace TabletopTweaks.Config {
    class AddedContent {
        public bool CauldronWitchArchetype = true;
        public bool ElementalMasterArchetype = true;

        public void OverrideFixes(AddedContent userSettings) {
            CauldronWitchArchetype = userSettings.CauldronWitchArchetype;
            ElementalMasterArchetype = userSettings.ElementalMasterArchetype;
        }
    }
}
