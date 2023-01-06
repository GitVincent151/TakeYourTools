﻿using System;
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
        public int workTicksDone = 0;
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
        public int WorkTicksToDegrade =>
           Mathf.FloorToInt((this.GetStatValue(TYT_StatToolsDefOf.ToolEstimatedLifespan) * GenDate.TicksPerDay) / MaxHitPoints);

        #endregion

        #region Constructor
        #endregion

        #region Properties
        /// <summary>
        /// Get baseWorkStatFactors according to wearFactorMultiplier 
        /// </summary>
        public IEnumerable<StatModifier> WorkStatFactors
        {
            get
            {
                foreach (StatModifier modifier in def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors)
                {
                    Log.Message($"TYT: WorkStatFactors-->{modifier.stat},{modifier.value}");

                    float newFactor = this.GetStatValue(TYT_StatToolsDefOf.ToolEffectivenessFactor);
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
        /// <summary>
        /// DefaultToolAssignmentTags: list of impacted jobs
        /// </summary>
        public IEnumerable<string> DefaultToolAssignmentTags
        {
            get
            {
                foreach (string modifier in def.GetModExtension<TYT_ToolProperties>().defaultToolAssignmentTags)
                {
                    Log.Message($"TYT: DefaultToolAssignmentTags-->{modifier}");
                    yield return modifier;
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
