using BepInEx;

namespace CombatUpgrade {

    [BepInPlugin("teamgrad.combatupgrade", "Combat Upgrade", "1.0.0")]
    public class CULauncher : BaseUnityPlugin {

        public CULauncher() { CUBinder.UnitGlad(); }
    }
}