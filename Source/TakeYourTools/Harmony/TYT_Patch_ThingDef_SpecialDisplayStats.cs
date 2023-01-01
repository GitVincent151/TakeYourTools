using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TakeYourTools
{

    [HarmonyPatch(typeof(ThingDef))]
    [HarmonyPatch(nameof(ThingDef.SpecialDisplayStats))]
    public static class TYT_Patch_ThingDef_SpecialDisplayStats
    {

        public static IEnumerable<StatDrawEntry> Postfix(IEnumerable<StatDrawEntry> __result, ThingDef __instance, StatRequest req)
        {
            foreach (StatDrawEntry r in __result)
            {
                yield return r;
            }
            // Tool def
            if (req.Thing == null && __instance.IsTool(out TYT_ToolProperties tProps))
            {

                foreach (StatModifier modifier in tProps.baseWorkStatFactors)
                {
                    yield return new StatDrawEntry(TYT_StatCategoryDefOf.ToolStatCategoryDef,
                        modifier.stat.LabelCap,
                        modifier.value.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor), modifier.stat.description, 1);
                }
            }
            /*
            // Stuff
            if (__instance.IsStuff && __instance.GetModExtension<StuffPropsTool>() is StuffPropsTool sPropsTool)
            {
                foreach (StatModifier modifier in sPropsTool.toolStatFactors)
                {
                    yield return new StatDrawEntry(ST_StatCategoryDefOf.SurvivalToolMaterial,
                        modifier.stat.LabelCap,
                        modifier.value.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor), modifier.stat.description, 1);
                }
            }
            */
        }

    }

}
