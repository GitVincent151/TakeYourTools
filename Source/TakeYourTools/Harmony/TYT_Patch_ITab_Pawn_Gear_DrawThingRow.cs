using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace TakeYourTools
{
/*
    [HarmonyPatch(typeof(ITab_Pawn_Gear))]
    [HarmonyPatch("DrawThingRow")]
    public static class TYT_Patch_ITab_Pawn_Gear_DrawThingRow
    {

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Log.Message($"TYT: TYT_Patch_ITab_Pawn_Gear_DrawThingRow - Transpiler");
            List<CodeInstruction> instructionList = instructions.ToList();

            bool done = false;

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

                // If equipment is a tool, adjust its label in a similar fashion to how apparel labels are adjusted (though using a helper method)
                if (!done && instruction.opcode == OpCodes.Stloc_S && ((LocalBuilder)instruction.operand).LocalIndex == 5)
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 5); // text
                    yield return new CodeInstruction(OpCodes.Ldarg_3); // thing
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(ITab_Pawn_Gear), "SelPawnForGear").GetGetMethod(true)); // this.SelPawnForGear
                    instruction = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TYT_Patch_ITab_Pawn_Gear_DrawThingRow), "AdjustDisplayedLabel")); // AdjustDisplayedLabel(ref text, thing, this.SelPawnForGear)
                    done = true;
                }

                yield return instruction;
            }
        }

        public static void AdjustDisplayedLabel(ref string originalLabel, Thing thing, Pawn pawn)
        {
            Log.Message($"TYT: TYT_Patch_ITab_Pawn_Gear_DrawThingRow - AdjustDisplayedLabel"); 
            if (thing is TYT_ToolThing tool)
            {
                Log.Message($"TYT: AdjustDisplayedLabel {tool}");
                // Forced
                if (pawn.GetComp<TYT_PawnToolAssignmentTracker>() is TYT_PawnToolAssignmentTracker jobToolAssignmentTracker && jobToolAssignmentTracker.forcedHandler.IsForced(tool))
                    originalLabel += $", {"ApparelForcedLower".Translate()}";

                // In use

            }
        }

    }
*/
}
