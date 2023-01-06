using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using static UnityEngine.GridBrushBase;

namespace TakeYourTools
{
    public class TYT_ToolThing : ThingWithComps
    {
        #region Properties
        /// <summary>
        /// GetModExtension
        /// </summary>
        public List<StatModifier> BaseWorkStatFactors => this.def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors;
        public List<String> DefaultToolAssignmentTags => this.def.GetModExtension<TYT_ToolProperties>().defaultToolAssignmentTags;
        public float ToolWearFactor => this.def.GetModExtension<TYT_ToolProperties>().toolWearFactor;

        /// <summary>
        ///  ToolDegradation
        /// </summary>
        public int workTicksDone = 0;
        public int WorkTicksToDegrade => Mathf.FloorToInt((this.GetStatValue(TYT_StatToolsDefOf.StatEstimatedLifespan) * GenDate.TicksPerDay) / MaxHitPoints);

        /// <summary>
        /// Pawn holding the tool
        /// </summary>
        public Pawn HoldingPawn
        {
            get
            {
                if (ParentHolder is Pawn_EquipmentTracker eq)
                    return eq.pawn;
                if (ParentHolder is Pawn_InventoryTracker inv)
                    return inv.pawn;
                return null;
            }
        }

        /// <summary>
        /// Tool cab be used & BestTool
        /// </summary>
        public bool InUse
        {
            get
            {
                return HoldingPawn != null
                       && HoldingPawn.CanUseTools()
                       && HoldingPawn.CanUseTools(def)
                       && TYT_ToolUtility.BestToolsFor(HoldingPawn).Contains(this);
            }
        }


        #endregion

        #region Properties
        /// <summary>
        /// Change the WorkStatFactors according to StatQualityFactor and StatWearFactor
        /// </summary>
        public IEnumerable<StatModifier> WorkStatFactors
        {
            get
            {
                foreach (StatModifier modifier in def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors)
                {
                    Log.Message($"TYT: WorkStatFactors-->{modifier.stat},{modifier.value}");

                    float newFactor = this.GetStatValue(TYT_StatToolsDefOf.StatQualityFactor);
                    Log.Message($"TYT: WorkStatFactors---> ToolEffectivenessFactor-->{newFactor.ToString()}");

                    Log.Message($"TYT: WorkStatFactors-->{modifier.stat},{modifier.value * newFactor}");
                    yield return new StatModifier
                    {
                        stat = modifier.stat,
                        value = modifier.value * newFactor
                    };
                }
            }
        }

        #endregion

        #region Methods
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            Log.Message($"TYT: TYT_ToolThing - SpecialDisplayStats");
            foreach (StatModifier modifier in WorkStatFactors)
            {
                yield return new StatDrawEntry(TYT_StatCategoryDefOf.ToolStatCategoryDef,
                    modifier.stat.LabelCap,
                    modifier.value.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor),
                    TYT_ToolUtility.GetToolOverrideReportText(this, modifier.stat), 1);
            }
            Log.Message($"TYT: TYT_ToolThing - SpecialDisplayStats_out");

        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref workTicksDone, "workTicksDone", 0);
        }
        #endregion
    }
}
