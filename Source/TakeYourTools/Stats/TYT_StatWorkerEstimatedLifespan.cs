using RimWorld;
using System.Text;
using UnityEngine;
using Verse;

namespace TakeYourTools
{
    public class TYT_StatWorkerEstimatedLifespan : StatWorker
    {

        public static int BaseWearInterval =>
            Mathf.RoundToInt(GenDate.TicksPerHour * ((TYT_ToolsSettings.hardcoreMode) ? 0.67f : 1f)); // Once per hour of continuous work, or ~40 mins with hardcore

        public override bool ShouldShowFor(StatRequest req) =>
            ((BuildableDef)req.Def).IsTool() && TYT_ToolsSettings.ToolDegradation;

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            TYT_ToolThing tool = req.Thing as TYT_ToolThing;
            return GetBaseEstimatedLifespan(tool, req.Def as BuildableDef);
        }
        private float GetBaseEstimatedLifespan(TYT_ToolThing tool, BuildableDef def)
        {
            TYT_ToolProperties toolProperties = def.GetModExtension<TYT_ToolProperties>() ?? TYT_ToolProperties.defaultValues;
            TYT_StuffProps stuffProps = tool.Stuff?.GetModExtension<TYT_StuffProps>() ?? TYT_StuffProps.defaultValues;

            if (!((ThingDef)def).useHitPoints)
                return float.PositiveInfinity;
            Log.Message($"TYT: TYT_StatWorkerEstimatedLifespan - GetBaseEstimatedLifespan {BaseWearInterval} * {tool.MaxHitPoints}");
            return GenDate.TicksToDays(Mathf.RoundToInt((BaseWearInterval * tool.MaxHitPoints)));

            // Vincent return GenDate.TicksToDays(Mathf.RoundToInt((BaseWearInterval * tool.MaxHitPoints) / stuffProps.wearFactorMultiplier));
        }
        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            TYT_ToolThing tool = req.Thing as TYT_ToolThing;
            return $"{"StatsReport_BaseValue".Translate()}: {GetBaseEstimatedLifespan(tool, req.Def as BuildableDef).ToString("F1")}";
        }
        public override void FinalizeValue(StatRequest req, ref float val, bool applyPostProcess)
        {
            val /= TYT_ToolsSettings.ToolDegradationFactor;
            base.FinalizeValue(req, ref val, applyPostProcess);
        }
        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            StringBuilder finalBuilder = new StringBuilder();
            finalBuilder.AppendLine($"{"Settings_ToolDegradationRate".Translate()}: " +
                $"{(1 / TYT_ToolsSettings.ToolDegradationFactor).ToStringByStyle(ToStringStyle.FloatTwo, ToStringNumberSense.Factor)}");
            finalBuilder.AppendLine();
            finalBuilder.AppendLine(base.GetExplanationFinalizePart(req, numberSense, finalVal));
            return finalBuilder.ToString();
        }
    }
}
