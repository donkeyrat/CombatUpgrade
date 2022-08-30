using Landfall.TABS;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CombatUpgrade {

	public class CUMain {

        public CUMain()
        {
            var db = LandfallUnitDatabase.GetDatabase();
            var upgradedUnits = combatUpgrade.LoadAllAssets<UnitBlueprint>().ToList();
            var unitList = db.UnitList.ToList().FindAll(x => ((UnitBlueprint)x).UnitBase.name == "Humanoid_1 Prefabs_VB");
            var unitsToUpgrade = new List<IDatabaseEntity>(unitList);
            foreach (var u in unitList)
            {
                var unit = (UnitBlueprint)u;
                var foundUnit = upgradedUnits.Find(x => x.name == unit.name + "_2.0");
                if (foundUnit)
                {
                    DeepCopyUnit(unit, foundUnit);
                    unitsToUpgrade.Remove(unit);
                }
            }

            foreach (var u in unitsToUpgrade)
            {
                var unit = (UnitBlueprint)u;
                if (unit.sizeMultiplier > 3f)
                {
                    continue;
                }
                unit.animationMultiplier = 1f;
                unit.movementSpeedMuiltiplier /= 1.5f;
                unit.stepMultiplier = 1f;
                unit.UnitBase = combatUpgrade.LoadAsset<GameObject>("Humanoid_2.0");
                var newMoves = unit.objectsToSpawnAsChildren.ToList();
                string chosenHover = "2.0Hover";
                if (unit.sizeMultiplier >= 1.1f) chosenHover = "2.0Hover_MonkeyKing";
                if (unit.sizeMultiplier > 1.2f) chosenHover = "2.0Hover_Knight";
                if (unit.sizeMultiplier > 1.3f) chosenHover = "2.0Hover_Jarl";
                if (unit.sizeMultiplier > 1.4f) chosenHover = "2.0Hover_Captain";
                newMoves.Add(combatUpgrade.LoadAsset<GameObject>(chosenHover));
                unit.objectsToSpawnAsChildren = newMoves.ToArray();
            }
        }

        public void DeepCopyUnit(UnitBlueprint unit1, UnitBlueprint unit2)
        {
            unit1.health = unit2.health;
            unit1.m_props = unit2.m_props;
            unit1.animationMultiplier = unit2.animationMultiplier;
            unit1.balanceMultiplier = unit2.balanceMultiplier;
            unit1.costTweak = unit2.costTweak;
            unit1.damageMultiplier = unit2.damageMultiplier;
            unit1.dragMultiplier = unit2.dragMultiplier;
            unit1.forceCost = unit2.forceCost;
            unit1.m_propData = unit2.m_propData;
            unit1.massMultiplier = unit2.massMultiplier;
            unit1.sizeMultiplier = unit2.sizeMultiplier;
            unit1.scaleWeapons = unit2.scaleWeapons;
            unit1.stepMultiplier = unit2.stepMultiplier;
            unit1.turningData = unit2.turningData;
            unit1.turnSpeed = unit2.turnSpeed;
            unit1.balanceForceMultiplier = unit2.balanceForceMultiplier;
            unit1.movementSpeedMuiltiplier = unit2.movementSpeedMuiltiplier;
            unit1.LeftWeapon = unit2.LeftWeapon;
            unit1.RightWeapon = unit2.RightWeapon;
            unit1.objectsToSpawnAsChildren = unit2.objectsToSpawnAsChildren;
            unit1.minSizeRandom = unit2.minSizeRandom;
            unit1.maxSizeRandom = unit2.maxSizeRandom;
            unit1.UnitBase = unit2.UnitBase;
        }

        public static AssetBundle combatUpgrade = AssetBundle.LoadFromMemory(Properties.Resources.combatupgrade);
    }
}
