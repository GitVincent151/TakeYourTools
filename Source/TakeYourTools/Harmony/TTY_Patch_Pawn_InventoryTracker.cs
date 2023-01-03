using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;
using static UnityEngine.Random;

namespace TakeYourTools
{

    public static class TYT_Patch_Pawn_InventoryTracker
    {

        [HarmonyPatch(typeof(Pawn_InventoryTracker), nameof(Pawn_InventoryTracker.FirstUnloadableThing), MethodType.Getter)]
        public static class FirstUnloadableThing
        {

            public static void Postfix(Pawn_InventoryTracker __instance, ref ThingCount __result)
            {
                Log.Message($"TYT: TYT_Patch_Pawn_InventoryTracker - FirstUnloadableThing");
                if (__result.Thing is TYT_ToolThing tool && tool.InUse)
                {
                    bool foundNewThing = false;
                    // Had to iterate through because a lambda expression in this case isn't possible
                    for (int i = 0; i < __instance.innerContainer.Count; i++)
                    {
                        Thing newThing = __instance.innerContainer[i];
                        if (newThing as TYT_ToolThing == null || !((TYT_ToolThing)newThing).InUse)
                        {
                            __result = new ThingCount(newThing, newThing.stackCount);
                            foundNewThing = true;
                            break;
                        }
                    }
                    if (!foundNewThing)
                        __result = default;
                }
            }

        }

        [HarmonyPatch(typeof(Pawn_InventoryTracker), nameof(Pawn_InventoryTracker.InventoryTrackerTickRare))]
        public static class InventoryTrackerTickRare
        {

            public static void Postfix(Pawn_InventoryTracker __instance)
            {
                Log.Message($"TYT: TYT_Patch_Pawn_InventoryTracker - InventoryTrackerTickRare");
                if (TYT_ToolsSettings.toolLimit)
                {
                    Pawn pawn = __instance.pawn;
                    if (pawn.CanUseTools() && pawn.GetHeldTools().Count() > pawn.GetStatValue(TYT_StatDefOf.ToolCarryCapacity) && pawn.CanRemoveExcessTools())
                    {
                        Log.Message($"TYT: TYT_Patch_Pawn_InventoryTracker - GetHeldTools");
                        Thing tool = pawn.GetHeldTools().Last();
                        //Job job = pawn.DequipAndTryStoreTool(tool);
                        //pawn.jobs.StartJob(job, JobCondition.InterruptForced, cancelBusyStances: false);
                    }
                }
            }

        }

        [HarmonyPatch(typeof(Pawn_InventoryTracker), nameof(Pawn_InventoryTracker.Notify_ItemRemoved))]
        public static class Notify_ItemRemoved
        {

            public static void Postfix(Pawn_InventoryTracker __instance, Thing item)
            {
                Log.Message($"TYT: TYT_Patch_Pawn_InventoryTracker - Notify_ItemRemoved");
                if (item is TYT_ToolThing && __instance.pawn.TryGetComp<TYT_PawnToolAssignmentTracker>() is TYT_PawnToolAssignmentTracker assignmentTracker)
                {
                    assignmentTracker.forcedHandler.SetForced(item, false);
                }

            }

        }

    }

}
