using HarmonyLib;
using Verse.AI;
using Verse;
using RimWorld;

namespace TakeYourTools
{
    public class TYT_Patch_Pawn_JobTracker_Patches
    {
        [StaticConstructorOnStartup]
        public static class Pawn_JobTracker_Patches
        {
            /*
            [HarmonyPatch(typeof(JobDriver))]
            [HarmonyPatch("GetFinalizerJob")]
            */

            [HarmonyPatch(typeof(Pawn_JobTracker))]
            [HarmonyPatch("StartJob")]

            public static class Pawn_JobTracker_StartJob
            {
                [HarmonyPostfix]
                public static void Postfix(Pawn_JobTracker __instance)
                {
                    if (__instance == null)
                        return;

                    Job __job = __instance.curJob;
                    JobDriver __curDriver = __instance.curDriver;
                    Pawn __pawm = __instance.curDriver.pawn;
                    Log.Message($"TYT: TYT_Patch_Pawn_JobTracker_Patches - Pawn_JobTracker_Patches --> Job = {__job} jobDriver = {__curDriver} pawn = {__pawm}");

                    /*
                    __instance.AddPreInitAction(delegate
                    {
                        Pawn pawn = __instance.GetActor();
                        JobDef activeJobDef = pawn.CurJob?.def;
                        WorkGiverDef activeWorkGiverDef = pawn.CurJob?.workGiverDef;
                        SkillDef activeSkill = pawn.CurJob?.RecipeDef?.workSkill;

                        if (pawn == null || pawn.Dead || pawn.equipment == null || pawn.inventory == null || !pawn.RaceProps.Humanlike)
                        {
                            Log.Message($"TYT: TYT_Patch_GrabYourTools - Toil_Constructor AddPreInitAction Pawn = {__instance.actor} JobDef = {activeJobDef} WorkGiverDef = {activeJobDef} activeSkill = {activeSkill} autre = {__instance}"); 
                            return;
                        }

                        if (pawn.Drafted)
                        {
                            // Init ToolMemoriesTracker
                            TYT_Mod.ToolMemoriesTracker.ClearMemory(pawn);
                            return;
                        }


                        
                        Log.Message($"TYT: TYT_Patch_GrabYourTools - Toil_Constructor AddPreInitAction Pawn = {__instance.actor} JobDef = {activeJobDef} WorkGiverDef = {activeJobDef} activeSkill = {activeSkill} autre = {__instance}");

                        if (__instance.activeSkill != null && __instance.activeSkill() != null)
                            activeSkill = __instance.activeSkill();

                        if (activeSkill != null)
                        {
                            TYT_ToolMemory memory = TYT_Mod.ToolMemoriesTracker.GetMemory(pawn);

                            if (!memory.UpdateSkill(activeSkill))
                                return;

                            // Don't do it if this job uses weapons (i.e. hunting)
                            if (activeSkill == SkillDefOf.Shooting || activeSkill == SkillDefOf.Melee)
                            {
                                memory.UpdateUsingTool(null, false);
                            }
                            // Check currently equipped item
                            else if (pawn.equipment.Primary != null && TYT_ToolMemoryTracker.HasReleventStatModifiers(pawn.equipment.Primary, activeSkill))
                            {
                                memory.UpdateUsingTool(null, true);
                            }
                            // Try and find something else in inventory
                            else
                            {
                                memory.UpdateUsingTool(pawn.equipment.Primary, TYT_ToolMemoryTracker.EquipAppropriateTool(pawn, activeSkill));
                            }
                        }
                        else
                        {
                            TYT_Mod.ToolMemoriesTracker.ClearMemory(pawn);
                        }
                    });
                    */
                }
            }
        }
    }
}
