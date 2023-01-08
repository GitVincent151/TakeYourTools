using RimWorld;
using Verse;

namespace TakeYourTools
{
    public class TYT_StatTool : StatPart
    {
        #region Properties
        private readonly float noToolStatFactor = 0.3f; // 30% efficacity without tool
        private readonly float noToolStatFactorHardcore = -1f; // 
        private float NoToolStatFactorHardcore => (noToolStatFactorHardcore != -1f) ? noToolStatFactorHardcore : noToolStatFactor;
        #endregion


        public override string ExplanationPart(StatRequest req)
        {
            Log.Message($"TYT: TYT_StatTool - ExplanationPart");
            // The AI will cheat this system for now until tool generation gets figured out
            return req.Thing is Pawn pawn && pawn.CanUseTools()
                ? pawn.HasToolFor(parentStat, out TYT_ToolThing tool, out float statFactor)
                    ? tool.LabelCapNoCount + ": x" + statFactor.ToStringPercent()
                    : (string)("NoTool".Translate() + ": x" + NoToolStatFactor.ToStringPercent())
                : null;
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            Log.Message($"TYT: TYT_StatTool - TransformValue"); 
            if (req.Thing is Pawn pawn && pawn.CanUseTools())
            {
                if (pawn.HasToolFor(parentStat, out _, out float statFactor))
                {
                    val *= statFactor;
                }
                else
                {
                    val *= NoToolStatFactor;
                }
            }
        }

        public float NoToolStatFactor => TYT_ModSettings.reduceNoToolWorkEfficiency
            ? TYT_ModSettings.hardcoreMode ? NoToolStatFactorHardcore : noToolStatFactor
            : 1f;



    }
}
