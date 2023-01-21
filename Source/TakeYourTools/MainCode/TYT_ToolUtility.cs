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
        public static List<StatDef> StatDefForPawnWork { get; } = DefDatabase<StatDef>.AllDefsListForReading.Where(s => s.StatRelevantForPawnWork()).ToList();
        public static bool StatRelevantForPawnWork(this StatDef stat)
        {
            if (!stat.parts.NullOrEmpty())
                foreach (StatPart part in stat.parts)
                {
                    
                    Log.Message($"TYT: TYT_ToolUtility - Looking for StatRelevantForPawnWork");
                    if (part?.GetType() == StatCategoryDefOf.PawnWork.GetType())
                    {
                        Log.Message($"TYT: TYT_ToolUtility - StatRelevantForPawnWork found stat.parts {part.GetType().FullName}");
                        return true;
                    }
                        
                }
            return false;
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
            def is ThingDef tDef && tDef.thingClass == typeof(TYT_ToolProperties);
        //&& tDef.HasModExtension<TYT_ToolProperties>();

        /// <summary>
        /// Generate report for tool properties
        /// </summary>
        public static string GetToolOverrideReportText(TYT_ToolThing tool, StatDef stat)
        {
            TYT_StuffProps stuffProps = tool.Stuff?.GetModExtension<TYT_StuffProps>();
            float finalValue = 0f;
            float value = 0f;
            StringBuilder builder = new StringBuilder();

            // Description
            builder.AppendLine(stat.description);
            builder.AppendLine();
            builder.AppendLine();

            // Stat
            value = tool.def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors.GetStatFactorFromList(stat);
            finalValue = value;
            builder.AppendLine(stat.LabelForFullStatListCap + " (" + tool.def.LabelCap + "): " + value.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));

            // Tool quality factor
            builder.AppendLine();
            value = tool.GetStatValue(TYT_StatToolsDefOf.StatQualityFactor);
            finalValue *= value;
            builder.AppendLine(TYT_StatToolsDefOf.StatQualityFactor.LabelCap + ": " + value.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));

            // Stuff wear factor
            builder.AppendLine();
            if (stuffProps != null)
            {
                value = stuffProps.wearFactorMultiplier;
                finalValue *= value; 
                builder.AppendLine("Stuff".Translate() + " (" + tool.Stuff.LabelCap + "): " + value.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
            }

            // Final value
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("StatsReport_FinalValue".Translate() + ": " + finalValue.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
            return builder.ToString();
        }

        /// <summary>
        /// Generate report for stuff properties
        /// </summary>
        public static string GetStuffOverrideReportText(TYT_ToolThing tool, StatDef stat)
        {
            Log.Message($"TYT: TYT_ToolUtility - GetStuffOverrideReportText");
            //List<StatModifier> statFactorList = tool.WorkStatFactors.ToList();
            TYT_StuffProps stuffProps = tool.Stuff?.GetModExtension<TYT_StuffProps>();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(stat.description);
            Log.Message($"TYT: TYT_ToolUtility - GetToolOverrideReport_stat.description {stat.description}");

            builder.AppendLine();
            builder.AppendLine(tool.def.LabelCap + ": " + tool.def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
            Log.Message($"TYT: TYT_ToolUtility - GetToolOverrideReport_stat.description {tool.def.LabelCap + ": " + tool.def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor)}");

            builder.AppendLine();
            builder.AppendLine(TYT_StatToolsDefOf.StatQualityFactor.LabelCap + ": " +
                tool.GetStatValue(TYT_StatToolsDefOf.StatQualityFactor).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
            Log.Message($"TYT: TYT_ToolUtility - GetToolOverrideReport_GetStatValueo {TYT_StatToolsDefOf.StatQualityFactor.LabelCap + ": " + tool.GetStatValue(TYT_StatToolsDefOf.StatQualityFactor).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor)}");

            if (stuffProps != null)
            {
                builder.AppendLine();
                builder.AppendLine(tool.def.LabelCap + ": " + tool.def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
                builder.AppendLine("StatsReport_Material".Translate() + " (" + tool.Stuff.LabelCap + "): " + stuffProps.wearFactorMultiplier.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
                Log.Message($"TYT: TYT_ToolUtility - GetToolOverrideReportText Result {builder.ToString()}");
            }
            /*
                        if (stuffProps != null && stuffProps.wearFactorMultiplier != 1f)
                        {
                            builder.AppendLine();
                            builder.AppendLine(tool.def.LabelCap + ": " + tool.def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
                            builder.AppendLine("StatsReport_Material".Translate() + " (" + tool.Stuff.LabelCap + "): " +
                                stuffProps.wearFactorMultiplier.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
                        }
            */            /*
                        if (stuffProps != null && stuffProps.wearFactorMultiplier.GetStatFactorFromList(stat) != 1f)
                        {
                            builder.AppendLine();
                            builder.AppendLine(tool.def.LabelCap + ": " + tool.def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
                            builder.AppendLine("StatsReport_Material".Translate() + " (" + tool.Stuff.LabelCap + "): " +
                                stuffProps.wearFactorMultiplier.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
                        }*/
            /*           else
                       {
                           Log.Message($"TYT: TYT_ToolUtility - GetToolOverrideReportText stuffProps ={stat.description}");
                       }
            */
            builder.AppendLine();
            builder.AppendLine("StatsReport_FinalValue".Translate() + ": " + tool.WorkStatFactors.ToList().GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
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
        public static IEnumerable<TYT_ToolThing> BestToolsFor(Pawn pawn)
        {
            Log.Message($"TYT: TYT_ToolUtility - BestToolsFor"); 
            foreach (StatDef stat in StatDefForPawnWork)
            {
                TYT_ToolThing tool = pawn.GetBestTool(stat);
                if (tool != null)
                {
                    yield return tool;
                }
            }
        }
        /*
        public static List<StatDef> AssignedToolRelevantWorkGiversStatDefs(this Pawn pawn)
        {
            List<StatDef> resultList = new List<StatDef>();
            foreach (WorkGiver giver in pawn.AssignedToolRelevantWorkGivers())
                foreach (StatDef stat in giver.def.GetModExtension<WorkGiverExtension>().requiredStats)
                    if (!resultList.Contains(stat))
                        resultList.Add(stat);
            return resultList;
        }

        public static bool NeedsSurvivalTool(this Pawn pawn, SurvivalTool tool)
        {
            foreach (StatDef stat in pawn.AssignedToolRelevantWorkGiversStatDefs())
                if (StatUtility.StatListContains(tool.WorkStatFactors.ToList(), stat))
                    return true;
            return false;
        }
        */
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
        public static bool IsUnderToolCarryLimitFor(this int count, Pawn pawn) => !TYT_ModSettings.toolLimit || count < pawn.GetStatValue(TYT_StatToolsDefOf.ToolCarryCapacity);
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

        public static Job DequipAndTryStoreTool(this Pawn pawn, Thing tool, bool enqueueCurrent = true)
        {
            if (pawn.CurJob != null && enqueueCurrent)
                pawn.jobs.jobQueue.EnqueueFirst(pawn.CurJob);

            Zone_Stockpile pawnPosStockpile = Find.CurrentMap.zoneManager.ZoneAt(pawn.PositionHeld) as Zone_Stockpile;
            if ((pawnPosStockpile == null || !pawnPosStockpile.settings.filter.Allows(tool)) &&
                StoreUtility.TryFindBestBetterStoreCellFor(tool, pawn, pawn.Map, StoreUtility.CurrentStoragePriorityOf(tool), pawn.Faction, out IntVec3 c))
            {
                Job haulJob = new Job(JobDefOf.HaulToCell, tool, c)
                {
                    count = 1
                };
                pawn.jobs.jobQueue.EnqueueFirst(haulJob);
            }

            return new Job(TYT_JobDefOf.DropSurvivalTool, tool);
        }
        /*
        public static List<StatDef> AssignedToolRelevantWorkGiversStatDefs(this Pawn pawn)
        {
            List<StatDef> resultList = new List<StatDef>();
            foreach (WorkGiver giver in pawn.AssignedToolRelevantWorkGivers())
                foreach (StatDef stat in giver.def.GetModExtension<WorkGiverExtension>().requiredStats)
                    if (!resultList.Contains(stat))
                        resultList.Add(stat);
            return resultList;
        }

        public static bool NeedsSurvivalTool(this Pawn pawn, TYT_ToolThing tool)
        {
            foreach (StatDef stat in pawn.AssignedToolRelevantWorkGiversStatDefs())
                if (StatUtility.StatListContains(tool.WorkStatFactors.ToList(), stat))
                    return true;
            return false;
        }
        */
    }
}
