using HarmonyLib;
using Verse.AI;
using Verse;
using RimWorld;
using Unity.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace TakeYourTools
{
    public class TYT_Patch_Pawn_JobTracker_Patches
    {
        public static JobDef __curJobDef = null;
        public static Pawn __pawn = null;
        public static TYT_ToolMemory memory = null;
        public static TYT_ToolMemoryTracker ToolMemoriesTracker => Current.Game.World.GetComponent<TYT_ToolMemoryTracker>();

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
                    //if (TYT_Mod.Instance == null)
                    //{
                    //    Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Mod instance not existing");
                    //    return;
                    //}

                    __curJobDef = __instance?.curJob?.def;
                    __pawn = __instance?.curDriver?.pawn;

                    if (__curJobDef != null
                        && __pawn != null
                        && __pawn.Dead != true
                        && __pawn.RaceProps.Humanlike == true
                        )
                    {
                        Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Pawn {__pawn} will start JobDef {__curJobDef}");

                        if (__pawn.Drafted)
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Init Memory of the Pawn {__pawn}");
                            ToolMemoriesTracker.ClearMemory(__pawn);
                            return;
                        }

                        memory = ToolMemoriesTracker.GetMemory(__pawn);
                        Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> {__pawn} has tool {__pawn.equipment.Primary.def} that is of the type {__pawn.equipment.Primary.def.thingClass}");
                        if (!memory.UpdateJob(__curJobDef))
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Job was not updated, we keep the tool");
                            return;
                        }

                        // Don't do it if this job uses weapons (i.e. hunting)
                        if (__curJobDef == JobDefOf.AttackStatic || __curJobDef == JobDefOf.AttackMelee)
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Using Weapons, we do nothing");
                            memory.UpdateUsingTool(null, false);

                        }
                        // Check currently equipped item
                        //else if (__pawn.equipment.Primary != null && TYT_Mod.ToolMemoriesTracker.HasReleventStatModifiers(__pawn.equipment.Primary, __curJobDef))
                        //else v
                        else if (
                            //TYT_Mod.Instance != null
                            __pawn.equipment.Primary != null
                        //&& 
                            && __pawn.equipment.Primary.def is ThingDef
                            && __pawn.equipment.Primary.def.thingClass == typeof(TYT_ToolThing)
                            && ToolMemoriesTracker.HasAppropriatedToolsForJob((TYT_ToolThing)__pawn.equipment.Primary, __curJobDef)
                            )
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Check currently equipped item {__pawn.equipment.Primary}");
                            memory.UpdateUsingTool(null, true);
                        }
                        // Try and find something else in inventory
                        else
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> EquipAppropriateTools");
                            //memory.UpdateUsingTool(__pawn.equipment.Primary, TYT_Mod.ToolMemoriesTracker.EquipAppropriateWeapon(__pawn, __curJobDef));
                        }
                        Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> End");
                        //    }
                        //    else
                        //    {
                        //        Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent7"); 
                        //        TYT_Mod.ToolMemoriesTracker.ClearMemory(__pawn);
                        //        Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob Vincent8");
                        //    }

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
