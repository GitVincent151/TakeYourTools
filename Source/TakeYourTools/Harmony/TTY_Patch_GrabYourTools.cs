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
                    __curJobDef = __instance?.curJob?.def;
                    __pawn = __instance?.curDriver?.pawn;

                    if (__curJobDef != null
                        && __pawn != null
                        && __pawn.Dead != true
                        && __pawn.RaceProps.Humanlike == true
                        )
                    {
                        if (__pawn.Drafted)
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Pawn {__pawn} has been called for combat and will not take the tools");
                            // Vincent --> Drop the tools
                            ToolMemoriesTracker.ClearMemory(__pawn);
                            return;
                        }

                        memory = ToolMemoriesTracker.GetMemory(__pawn);
                        Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Pawn {__pawn} will start JobDef {__curJobDef}");

                        // This job is already activ by the pawn
                        if (!memory.UpdateJob(__curJobDef))
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Job was not updated, we won´t change anything");
                            return;
                        }

                        // Don't do it if this job uses weapons (i.e. hunting)
                        if (__curJobDef == JobDefOf.AttackStatic || __curJobDef == JobDefOf.AttackMelee)
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Using Weapons, we do nothing");
                            memory.UpdateUsingTool(null, false);

                        }
                        // Check currently equipped item
                        else if (
                            __pawn.equipment.Primary != null
                            && __pawn.equipment.Primary.def is ThingDef
                            && __pawn.equipment.Primary.def.thingClass == typeof(TYT_ToolThing)
                            && ToolMemoriesTracker.HasAppropriatedToolsForJob((TYT_ToolThing)__pawn.equipment.Primary, __curJobDef)
                            )
                        {
                            // Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> {__pawn} has tool {__pawn.equipment.Primary.def} that is of the type {__pawn.equipment.Primary.def.thingClass}");
                            memory.UpdateUsingTool(null, true);
                        }
                        // Try and find something else in inventory
                        else
                        {
                            // Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> EquipAppropriateTools");
                            memory.UpdateUsingTool(__pawn.equipment.Primary, ToolMemoriesTracker.EquipAppropriateTool(__pawn, __curJobDef));
                        }
                        // Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> End");
                    }
                    
                }
            }
        }

        //PawnRenderer
        [StaticConstructorOnStartup]
        public static class Pawn_JovRenderer_Patches
        {
            [HarmonyPatch(typeof(PawnRenderer))]
            [HarmonyPatch("CarryWeaponOpenly", MethodType.Normal)]
            public static class PawnRenderer_CarryWeaponOpenly
            {
                [HarmonyPostfix]
                public static void Postfix(ref bool __result, Pawn ___pawn)
                {
                    if (!__result && ToolMemoriesTracker.IsPawnUsingTool(___pawn))
                        __result = true;
                }
            }
        }
    }
}
