using HarmonyLib;
using Verse.AI;
using Verse;
using RimWorld;
using Unity.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeYourTools
{
    public class TYT_Patch_Pawn_JobTracker_Patches
    {
        [StaticConstructorOnStartup]
        public static class Pawn_JobTracker_Patches
        {
            [HarmonyPatch(typeof(Pawn_JobTracker))]
            [HarmonyPatch("StartJob")]

            public static class Pawn_JobTracker_StartJob
            {
                [HarmonyPostfix]
                public static void Postfix(Pawn_JobTracker __instance)
                {
                    Job __job = __instance?.curJob;
                    JobDriver __curDriver = __instance?.curDriver;
                    JobDef __curJobDef = __job?.def;
                    Pawn __pawn = __curDriver?.pawn;

                    if (__job != null && __curDriver != null && __curJobDef != null && __pawn != null)
                    {
                        

                        //if (__curDriver.pawn == null || __curDriver.pawn.Dead || __curDriver.pawn.equipment == null || __curDriver.pawn.inventory == null || !__curDriver.pawn.RaceProps.Humanlike)
                        if (__pawn.Dead || !__pawn.RaceProps.Humanlike)
                            return;

                        if (__pawn.Drafted)
                        {
                            // Init ToolMemoriesTracker
                            TYT_Mod.ToolMemoriesTracker.ClearMemory(__pawn);
                            return;
                        }

                        if (!__pawn.CanUseTools())
                            return;
                        
                        
                        Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_Patches --> 2 Job = {__job.def.defName} jobDriver = {__curDriver} pawn = {__pawn} JobDef = {__curJobDef}");
                        
                        //if (TYT_Mod.listofToolThinginGame.Contains())
                        
                        
                        // Look for better alternative tools to what the colonist currently has, based on what stats are relevant to the work types the colonist is assigned to
                        Log.Message($"TYT: TYT_ToolUtility - GetBestTool");


                        TYT_ToolThing tool = null;

                        List<Thing> usableTools = __pawn.GetAllUsableTools().ToList();
                        foreach (TYT_ToolThing curTool in usableTools.Cast<TYT_ToolThing>())
                        {
                            foreach (JobDef curToolJobDef in curTool.DefaultToolAssignmentTags)
                            {
                                if (curToolJobDef.defName == __curJobDef.defName)
                                {
                                    tool = curTool;
                                    Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Found tool --> label = {tool.Label}");

                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
