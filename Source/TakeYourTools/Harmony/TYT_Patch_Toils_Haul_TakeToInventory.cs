﻿using HarmonyLib;
using System;
using Verse;
using Verse.AI;

namespace TakeYourTools
{

    [HarmonyPatch(typeof(Toils_Haul))]
    [HarmonyPatch(nameof(Toils_Haul.TakeToInventory))]
    [HarmonyPatch(new Type[] { typeof(TargetIndex), typeof(Func<int>) })]
    public static class TYT_Patch_Toils_Haul_TakeToInventory
    {

        public static void Postfix(Toil __result, TargetIndex ind)
        {
            Action initAction = __result.initAction;
            __result.initAction = () =>
            {
                initAction();
                Pawn actor = __result.actor;
                Thing thing = actor.CurJob.GetTarget(ind).Thing;
                if (thing is TYT_ToolThing && actor.CanUseTools() && actor.inventory.Contains(thing))
                    if (actor.CurJob.playerForced)
                        actor.GetComp<TYT_JobToolAssignmentTracker>().forcedHandler.SetForced(thing, true);
            };
        }

    }

}
