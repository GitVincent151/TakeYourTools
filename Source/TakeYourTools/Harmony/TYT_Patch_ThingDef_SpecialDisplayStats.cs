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
/*
        public static IEnumerable<StatDrawEntry> Postfix(IEnumerable<StatDrawEntry> __result, ThingDef __instance, StatRequest req)
        {
            Log.Message($"TYT: TYT_Patch_ThingDef_SpecialDisplayStats - Postfix"); 
            foreach (StatDrawEntry r in __result)
            {
                yield return r;
            }
            // Tool def
            if (req.Thing == null && __instance.IsTool(out TYT_ToolProperties toolProperties))
            {

                foreach (StatModifier modifier in toolProperties.baseWorkStatFactors)
                {
                    yield return new StatDrawEntry(TYT_StatCategoryDefOf.ToolStatCategoryDef,
                        modifier.stat.LabelCap,
                        modifier.value.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Factor), modifier.stat.description, 1);
                }

                // Stuff
                if (__instance.IsStuff && __instance.GetModExtension<TYT_StuffProps>() is TYT_StuffProps stuffProps)
                {
                    Log.Message($"TYT: TYT_Patch_ThingDef_SpecialDisplayStats - To be done Vincent");
                    foreach (StatModifier modifier in stuffProps.wearFactorMultiplier)
                    {
                        yield return new StatDrawEntry(TYT_StatCategoryDefOf.ToolStatCategoryDef,
                            modifier.stat.LabelCap,
                            modifier.value.ToStringByStyle(ToStringStyle.FloatTwo, ToStringNumberSense.Factor), modifier.stat.description, 1);
                    }
                }

            }

        }
*/
    }


}
