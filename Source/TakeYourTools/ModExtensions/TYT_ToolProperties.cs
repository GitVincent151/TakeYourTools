using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace TakeYourTools
{
    public class TYT_ToolProperties : DefModExtension
    {

        public static readonly TYT_ToolProperties defaultValues = new TYT_ToolProperties();

        public List<StatModifier> baseWorkStatFactors;

        public List<JobDef> defaultToolAssignmentTags;

        public float toolWearFactor = 1f;

    }
}
