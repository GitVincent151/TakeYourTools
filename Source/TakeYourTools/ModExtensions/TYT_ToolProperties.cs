using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TakeYourTools
{
    public class TYT_ToolProperties : DefModExtension
    {

        public static readonly TYT_ToolProperties defaultValues = new TYT_ToolProperties();

        public List<StatModifier> baseWorkStatFactors;

        public List<string> defaultToolAssignmentTags;

        public float toolWearFactor = 1f;

    }
}
