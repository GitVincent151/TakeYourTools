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
                    if (__instance == null)
                        return;

                    Job __job = __instance?.curJob;
                    JobDriver __curDriver = __instance?.curDriver;
                    JobDef __curJobDef = __job?.def;
                    Pawn __pawn = __curDriver?.pawn;

                    if (__job != null
                        && __curDriver != null
                        && __curJobDef != null
                        && __pawn != null
                        && __pawn.Dead != true
                        && __pawn.RaceProps.Humanlike == true
                        )
                    {
                        
                        if (__pawn.Drafted)
                        {
                            // Init tool memory for new pawn
                            TYT_Mod.ToolMemoriesTracker.ClearMemory(__pawn);
                            return;
                        }

                        if (!__pawn.CanUseTools())
                            return;
                        
                        Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Pawn = {__pawn} will start JobDef = {__curJobDef}");

                        if (__curJobDef != null)
                        {
                            TYT_ToolMemory memory = TYT_Mod.ToolMemoriesTracker.GetMemory(__pawn);
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent");

                            if (!memory.UpdateJob(__curJobDef))
                            {
                                Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent0"); 
                                return;
                            }
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - UpdateJob true");
                            // Don't do it if this job uses weapons (i.e. hunting)
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - AttackStatic {JobDefOf.AttackStatic}");
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - __curJobDef {__curJobDef}");



                            //if (__curJobDef.defName == JobDefOf.AttackStatic.defName || __curJobDef.defName == JobDefOf.AttackMelee.defName)
                            if (__curJobDef == JobDefOf.Equip)
                            {
                                Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent2");     
                                memory.UpdateUsingTool(null, false);

                            }
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent20");
                            // Check currently equipped item
                            //else if (__pawn.equipment.Primary != null && TYT_Mod.ToolMemoriesTracker.HasReleventStatModifiers(__pawn.equipment.Primary, __curJobDef))
                            //else v
                            if (TYT_Mod.Instance != null
                                && __pawn.equipment.Primary != null
                                && TYT_Mod.ToolMemoriesTracker.HasReleventStatModifiers(__pawn.equipment.Primary, __curJobDef))
                            {
                                Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent3"); 
                                memory.UpdateUsingTool(null, true);
                                Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent4");
                            }
                            // Try and find something else in inventory
                            else
                            {
                                Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent5");
                                //memory.UpdateUsingTool(__pawn.equipment.Primary, TYT_Mod.ToolMemoriesTracker.EquipAppropriateWeapon(__pawn, __curJobDef));
                            }
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent6");
                        }
                        else
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent7"); 
                            TYT_Mod.ToolMemoriesTracker.ClearMemory(__pawn);
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent8");
                        }

                        /*

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
                        */
                    }

                }
            }
        }
    }
}
