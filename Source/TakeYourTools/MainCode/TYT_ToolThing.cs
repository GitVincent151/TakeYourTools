using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TakeYourTools
{
    public class TYT_ToolThing : ThingWithComps
    {
        #region Properties class TYT_ToolThing
        #endregion

        #region Constructor class TYT_ToolThing
        #endregion


        #region Properties class TYT_ToolThing
        /// <summary>
        /// WorkStatFactors: list of stat modifiers
        /// </summary>
        public IEnumerable<StatModifier> WorkStatFactors
        {
            get
            {
                foreach (StatModifier modifier in def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors)
                {
                    Log.Message($"TYT: WorkStatFactors-->{modifier.stat},{modifier.value}");
                    yield return new StatModifier
                    {
                        stat = modifier.stat,
                        value = modifier.value
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

        #region Methods class TYT_ToolThing
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            Log.Message($"TYT: SpecialDisplayStats");
            foreach (StatModifier modifier in WorkStatFactors)
            {
                yield return new StatDrawEntry(TYT_StatCategoryDefOf.ToolStatCategoryDef,
                    modifier.stat.LabelCap,
                    modifier.value.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor),
                    TYT_ToolUtility.GetToolOverrideReportText(this, modifier.stat), 1);
            }

        }
        #endregion
    }
}
