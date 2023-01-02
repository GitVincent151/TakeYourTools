using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using static UnityEngine.Random;

namespace TakeYourTools
{

    public static class TYT_ToolUtility
    {

        public static List<StatDef> ToolStats { get; } = DefDatabase<StatDef>.AllDefsListForReading.Where(s => s.StatsForTool()).ToList();
        //public static List<WorkGiverDef> SurvivalToolWorkGivers { get; } = DefDatabase<WorkGiverDef>.AllDefsListForReading.Where(w => w.HasModExtension<WorkGiverExtension>()).ToList();
        public static bool StatsForTool(this StatDef stat)
        {
            Log.Message($"TYT: TYT_ToolUtility - StatsForTool StatDef {stat.ToStringSafe()}");
            if (!stat.parts.NullOrEmpty())
                foreach (StatPart part in stat.parts)
                {
                    
                    Log.Message($"TYT: TYT_ToolUtility - StatsForTool stat.parts {part.GetType().FullName}"); 
                    if (part?.GetType() == typeof(TYT_StatDefOf))
                    {
                        Log.Message($"TYT: TYT_ToolUtility - StatsForTool found TYT_StatDefOf {part.GetType().FullName}");
                        return true;
                    }
                        
                }
            return false;
            /*
            if (stat.category.defName == "PawnWork")
                return true;
            */
            /*
            if (!stat.parts.NullOrEmpty())
                foreach (StatPart part in stat.parts)
                    if (part?.GetType() == typeof(statDef))
                        return true;
            return false;
            */

        }
        

        /// <summary>
        /// Check if the Def is a tool
        /// </summary>
        /// <param name="def"></param>
        /// <param name="toolProps"></param>
        /// <returns>True is Def is a tool</returns>
        public static bool IsTool(this BuildableDef def, out TYT_ToolProperties toolProps)
        {
            Log.Message($"TYT: TYT_ToolUtility - IsTool");
            toolProps = def.GetModExtension<TYT_ToolProperties>();
            return def.IsTool();
        }
        public static bool IsTool(this BuildableDef def) =>
            def is ThingDef tDef && tDef.thingClass == typeof(TYT_ToolProperties) && tDef.HasModExtension<TYT_ToolProperties>();

        /// <summary>
        /// Generate report for tool properties
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="stat"></param>
        /// <returns>String with the report</returns>
        public static string GetToolOverrideReportText(TYT_ToolThing tool, StatDef stat)
        {
            Log.Message($"TYT: TYT_ToolUtility - GetToolOverrideReportText");
            List<StatModifier> statFactorList = tool.WorkStatFactors.ToList();
            //StuffPropsTool stuffPropsTool = tool.Stuff?.GetModExtension<StuffPropsTool>();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(stat.description);

            builder.AppendLine();
            builder.AppendLine(tool.def.LabelCap + ": " + tool.def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));

            builder.AppendLine();
            builder.AppendLine(TYT_StatDefOf.ToolEffectivenessFactor.LabelCap + ": " +
                tool.GetStatValue(TYT_StatDefOf.ToolEffectivenessFactor).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
            
            /*if (stuffPropsTool != null && stuffPropsTool.toolStatFactors.GetStatFactorFromList(stat) != 1f)
            {
                builder.AppendLine();
                builder.AppendLine("StatsReport_Material".Translate() + " (" + tool.Stuff.LabelCap + "): " +
                    stuffPropsTool.toolStatFactors.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
            }
            */
            builder.AppendLine();
            builder.AppendLine("StatsReport_FinalValue".Translate() + ": " + statFactorList.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
            return builder.ToString();
        }

        /// <summary>
        /// Check if the pawn can use tools
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns>True if pawn can use tools</returns>
        public static bool CanUseTools(this Pawn pawn) =>
            pawn.RaceProps.intelligence >= Intelligence.ToolUser && pawn.Faction == Faction.OfPlayer && (pawn.equipment != null || pawn.inventory != null) && pawn.TraderKind == null;
        public static bool CanUseTools(this Pawn pawn, ThingDef def)
        {
            Log.Message($"TYT: TYT_ToolUtility - CanUseTools"); 
            TYT_ToolProperties toolProperties = def.GetModExtension<TYT_ToolProperties>();
            if (toolProperties == null)
            {
                Log.Error($"TYT: TYT_ToolUtility - Tried to check if {def} is a usable tool but has null tool properties");
                return false;
            }
            foreach (StatModifier modifier in toolProperties.baseWorkStatFactors)
                if (modifier.stat?.Worker?.IsDisabledFor(pawn) == false)
                    return true;
            return false;
        }

        /// <summary>
        /// Best tool for the pawn
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns>return the toolThing</returns>
        public static IEnumerable<TYT_ToolThing> BestToolsFor(Pawn pawn)
        {
            Log.Message($"TYT: TYT_ToolUtility - BestToolsFor"); 
            foreach (StatDef stat in ToolStats)
            {
                TYT_ToolThing tool = pawn.GetBestTool(stat);
                if (tool != null)
                {
                    yield return tool;
                }
            }
        }
        public static TYT_ToolThing GetBestTool(this Pawn pawn, StatDef stat)
        {
            Log.Message($"TYT: TYT_ToolUtility - GetBestTool"); 
            if (!pawn.CanUseTools())
                return null;

            TYT_ToolThing tool = null;
            float statFactor = stat.GetStatPart<TYT_StatTool>().NoToolStatFactor;

            List<Thing> usableTools = pawn.GetAllUsableTools().ToList();
            foreach (TYT_ToolThing curTool in usableTools.Cast<TYT_ToolThing>())
                foreach (StatModifier modifier in curTool.WorkStatFactors)
                    if (modifier.stat == stat && modifier.value > statFactor)
                    {
                        tool = curTool;
                        statFactor = modifier.value;
                    }

            if (tool != null)
                LessonAutoActivator.TeachOpportunity(TYT_ModConceptDefOf.UsingTeachOpportunity, OpportunityType.Important);

            return tool;
        }
        public static IEnumerable<Thing> GetAllUsableTools(this Pawn pawn) => pawn.equipment?.GetDirectlyHeldThings().Where(t => t.def.IsTool()).Concat(pawn.GetUsableHeldTools());
        public static IEnumerable<Thing> GetUsableHeldTools(this Pawn pawn)
        {
            List<Thing> heldTools = pawn.GetHeldTools().ToList();
            return heldTools.Where(t => heldTools.IndexOf(t).IsUnderToolCarryLimitFor(pawn));
        }
        public static bool IsUnderToolCarryLimitFor(this int count, Pawn pawn) => !TYT_ToolsSettings.toolLimit || count < pawn.GetStatValue(TYT_StatDefOf.ToolCarryCapacity);
        /// <summary>
        /// Check if the pawn has the tool
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="tool"></param>
        /// <returns>return true if pawn has a tool</returns>
        public static bool HasTool(this Pawn pawn, ThingDef tool) => pawn.GetHeldTools().Any(t => t.def == tool);
        public static bool HasToolFor(this Pawn pawn, StatDef stat) => pawn.GetBestTool(stat) != null;
        public static bool HasToolFor(this Pawn pawn, StatDef stat, out TYT_ToolThing tool, out float statFactor)
        {
            Log.Message($"TYT: TYT_ToolUtility - HasToolFor");
            tool = pawn.GetBestTool(stat);
            statFactor = tool?.WorkStatFactors.ToList().GetStatFactorFromList(stat) ?? -1f;
            return tool != null;
        }
        public static IEnumerable<Thing> GetHeldTools(this Pawn pawn) => pawn.inventory?.innerContainer.Where(t => t.def.IsTool());

        public static bool CanRemoveExcessTools(this Pawn pawn) => !pawn.Drafted && !pawn.IsFormingCaravan() && !pawn.IsCaravanMember() && pawn.CurJobDef?.casualInterruptible != false && !pawn.IsBurning() && !(pawn.carryTracker?.CarriedThing is TYT_ToolThing);
        /*
        public static bool CanRemoveExcessTools(this Pawn pawn) => !pawn.Drafted && !pawn.IsWashing() && !pawn.IsFormingCaravan() && !pawn.IsCaravanMember() && pawn.CurJobDef?.casualInterruptible != false && !pawn.IsBurning() && !(pawn.carryTracker?.CarriedThing is TYT_ToolThing);
        private static bool IsWashing(this Pawn pawn)
        {
            if (!ModCompatibilityCheck.DubsBadHygiene)
                return false;
            return pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed("Washing"));
        }
        */

    }
}
