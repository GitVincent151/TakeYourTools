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
    public class ToolThing : ThingWithComps
    {
        public IEnumerable<StatModifier> WorkStatFactors
        {
            get
            {
                foreach (StatModifier modifier in def.GetModExtension<ToolProperties>().baseWorkStatFactors)
                {
                    //float newFactor = modifier.value * this.GetStatValue(ST_StatDefOf.ToolEffectivenessFactor);

                    /*if (Stuff?.GetModExtension<StuffPropsTool>()?.toolStatFactors.NullOrEmpty() == false)
                        foreach (StatModifier modifier2 in Stuff?.GetModExtension<StuffPropsTool>()?.toolStatFactors)
                            if (modifier2.stat == modifier.stat)
                                newFactor *= modifier2.value;
                    */
                    Log.Message($"WorkStatFactors-->{modifier.stat},{modifier.value}");
                    yield return new StatModifier
                    {
                        stat = modifier.stat,
                        //value = newFactor
                        value = modifier.value
                    };
                }
            }
        }
        public IEnumerable<string> DefaultToolAssignmentTags
        {
            get
            {
                foreach (string modifier in def.GetModExtension<ToolProperties>().defaultToolAssignmentTags)
                {
                    Log.Message($"DefaultToolAssignmentTags-->{modifier}");
                    yield return modifier;
                }
            }

        }
    }
}
