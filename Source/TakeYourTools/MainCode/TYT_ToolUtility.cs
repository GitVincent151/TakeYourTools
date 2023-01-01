using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace TakeYourTools
{

    public static class TYT_ToolUtility
    {

        /// <summary>
        /// Check if the Def is a tool
        /// </summary>
        /// <param name="def"></param>
        /// <param name="toolProps"></param>
        /// <returns>True is Def is a tool</returns>
        public static bool IsTool(this BuildableDef def, out TYT_ToolProperties toolProps)
        {
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
            Log.Message("GetToolOverrideReportText");
            List<StatModifier> statFactorList = tool.WorkStatFactors.ToList();
            //StuffPropsTool stuffPropsTool = tool.Stuff?.GetModExtension<StuffPropsTool>();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(stat.description);

            builder.AppendLine();
            builder.AppendLine(tool.def.LabelCap + ": " + tool.def.GetModExtension<TYT_ToolProperties>().baseWorkStatFactors.GetStatFactorFromList(stat).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));

            builder.AppendLine();
            /*builder.AppendLine(TYT_StatDefOf.ToolEffectivenessFactor.LabelCap + ": " +
                tool.GetStatValue(TYT_StatDefOf.ToolEffectivenessFactor).ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Factor));
            */
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
    }
}
