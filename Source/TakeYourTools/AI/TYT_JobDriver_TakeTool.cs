using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using UnityEngine;
using static UnityEngine.GridBrushBase;
using Verse.Sound;

namespace TakeYourTools
{
    public class TYT_JobDriver_TakeTool : JobDriver
    {
        protected virtual DesignationDef RequiredDesignation => null;

        //reserve the tool, so other pawns can't take it. This is OUR tool!
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Log.Message($"TYT: TYT_JobDriver_TakeTool - TryMakePreToilReservations --> Job TakeTools");
            //return this.pawn.Reserve(this.job.GetTarget(TargetIndex.A), this.job, 1, -1, null);
            //return true;

            var target = job.GetTarget(TargetIndex.A);
            if (target.IsValid)
            {
                var pawn = this.pawn;
                var target2 = target;
                var job = this.job;
                if (!pawn.Reserve(target2, job, 1, -1, null, errorOnFailed))
                {
                    return false;
                }
            }
            pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.A), job, 1, -1, null);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Init();
            yield return Toils_JobTransforms.MoveCurrentTargetIntoQueue(TargetIndex.A);
            var initExtractTargetFromQueue = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(TargetIndex.A, (RequiredDesignation == null) ? null : new Func<Thing, bool>((Thing t) => Map.designationManager.DesignationOn(t, RequiredDesignation) != null));
            yield return initExtractTargetFromQueue;
            yield return Toils_JobTransforms.SucceedOnNoTargetInQueue(TargetIndex.A);
            yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A, true);
            var gotoThing = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).JumpIfDespawnedOrNullOrForbidden(TargetIndex.A, initExtractTargetFromQueue);
            if (RequiredDesignation != null)
            {
                gotoThing.FailOnThingMissingDesignation(TargetIndex.A, RequiredDesignation);
            }
            yield return gotoThing;
            
            
            
            
            Log.Message($"TYT: TYT_JobDriver_TakeTool - MakeNewToils --> Job TakeTools"); 
            
            // These are the fail conditions. If at any time during this toil the tool becomes unavailable, our toil ends.
            //this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            //this.FailOnDowned(TargetIndex.A);
            //this.FailOnNotCasualInterruptible(TargetIndex.A);
            //this.FailOnBurningImmobile(TargetIndex.A);

            // These are all vanilla Toils, which is easy to use.
            // Go to the target
           yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);

            /*
            yield return Toils_General.Do(delegate
            {
                //Pawn petter = this.pawn;
                //Pawn pettee = (Pawn)this.pawn.CurJob.targetA.Thing;
                //pettee.interactions.TryInteractWith(petter, InteractionDefOf.Nuzzle);
                Log.Message($"Take the tool");
                TryEquipTool(pawn, (Pawn)this.pawn.CurJob.targetA.Thing as ThingWithComps);
            });
            */

            // Take tool
            yield return Toils_Haul.TakeToInventory(TargetIndex.A, 1);
            Log.Message($"TYT: TYT_JobDriver_TakeTool - MakeNewToils --> Done");


        }




    }
}
