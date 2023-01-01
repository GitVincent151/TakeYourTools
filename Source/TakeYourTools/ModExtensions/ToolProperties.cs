using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TakeYourTools
{
    public class ToolProperties : DefModExtension
    {

        public static readonly ToolProperties defaultValues = new ToolProperties();

        public List<StatModifier> baseWorkStatFactors;

        //[NoTranslate]
        public List<string> defaultToolAssignmentTags;

    }
}
