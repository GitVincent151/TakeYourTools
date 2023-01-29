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
        private static JobDef __curJobDef = null;
        private static Pawn __pawn = null;
        private static TYT_ToolMemoryTracker ToolMemoryTracker => Current.Game.World.GetComponent<TYT_ToolMemoryTracker>();

        [StaticConstructorOnStartup]
        public static class Pawn_JobTracker_Patches
        {
            [HarmonyPatch(typeof(Pawn_JobTracker))]
            [HarmonyPatch("StartJob", MethodType.Normal)]

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
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Pawn {__pawn.LabelShort} has been called for combat and will not take the tools");
                            // Restore the last previous tool
                            //ToolMemoryTracker.RestorePreviousEquippedTool(__pawn);
                            return;
                        }

                        Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Pawn {__pawn.LabelShort} will start JobDef {__curJobDef}");

                        // This job is already activ by the pawn
                        if (!ToolMemoryTracker.UpdateJobDef(__pawn, __curJobDef))
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> JobDef {__curJobDef} for Pawn {__pawn.LabelShort} was not updated, we won´t change anything");
                            // return;
                        }
                        /*
                        // Don't do it if this job uses weapons (i.e. hunting)
                        else if (__curJobDef == JobDefOf.AttackStatic || __curJobDef == JobDefOf.AttackMelee)
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Pawn {__pawn.LabelShort} has JobDef {__curJobDef}, we do nothing");

                        }
                        */
                        // Check if currently equipped item is appropriate for the JobDef
                        else if (
                            __pawn.equipment.Primary != null
                            && __pawn.equipment.Primary.def is ThingDef
                            && __pawn.equipment.Primary.def.thingClass == typeof(TYT_ToolThing)
                            && ToolMemoryTracker.HasAppropriatedToolsForJobDef((TYT_ToolThing)__pawn.equipment.Primary, __curJobDef)
                            )
                        {
                            //Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> {__pawn.LabelShort} has tool {__pawn.equipment.Primary.def} that is appropriate for {__curJobDef}");
                        }
                        // Try and find something else in inventory
                        else if (
                            ToolMemoryTracker.EquipAppropriateTool(__pawn, __curJobDef)
                            )
                        {
                            //Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Pawn {__pawn.LabelShort} will be equipped with {__pawn.equipment.Primary}");
                        }
                        // Check if an other tool is available on the map
                        else if (
                            ToolMemoryTracker.SearchAppropriateTool(__pawn, __curJobDef)
                            )
                        {
                            Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Pawn {__pawn.LabelShort} will search for a tool appropriate for {__curJobDef}");

                        }

                        // Restore the last previous tool
                        else
                        {
                            //Unequip or drop tool
                            //Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_StartJob --> Restore previous equipped tool {__pawn.LabelShort}");
                            //ToolMemoryTracker.RestorePreviousEquippedTool(__pawn);
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
                    if (!__result && ToolMemoryTracker.IsPawnUsingTool(___pawn))
                        __result = true;
                }
            }
        }

    }
}
