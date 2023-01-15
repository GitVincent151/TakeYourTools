using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Verse;
using Verse.AI;

namespace TakeYourTools
{

    public class TYT_JobToolAssignmentDatabase : GameComponent
    {

        private bool initialized = false;
        private List<TYT_JobToolAssignment> JobToolAssignments = new List<TYT_JobToolAssignment>();
        public List<TYT_JobToolAssignment> AllJobToolAssignments => JobToolAssignments;
                
        /// <summary>
        /// Constructor
        /// </summary>
        public TYT_JobToolAssignmentDatabase(Game game)
        {
        }
        
        public TYT_JobToolAssignment DefaultJobToolAssignment() => JobToolAssignments.Count == 0 ? MakeNewJobToolAssignment() : JobToolAssignments[0];

        public override void FinalizeInit()
        {
            if (!initialized)
            {
                GenerateStartingJobToolAssignments();
                initialized = true;
            }
        }

        private void GenerateStartingJobToolAssignments()
        {

            Log.Message($"TYT: TYT_JobToolAssignmentDatabase - GenerateStartingJobToolAssignments");
            /*
            TYT_JobToolAssignment staAnything = MakeNewJobToolAssignment();
            staAnything.label = "Tool Assignment is anything".Translate();

            TYT_JobToolAssignment staConstructor = MakeNewJobToolAssignment();
            staConstructor.label = "Tool Assignment is Constructor".Translate();
            staConstructor.filter.SetDisallowAll(null, null);

            TYT_JobToolAssignment staMiner = MakeNewJobToolAssignment();
            staMiner.label = "Tool Assignment is Miner".Translate();
            staMiner.filter.SetDisallowAll(null, null);

            TYT_JobToolAssignment staPlantWorker = MakeNewJobToolAssignment();
            staPlantWorker.label = "Tool Assignment is PlantWorker".Translate();
            staPlantWorker.filter.SetDisallowAll(null, null);
            */
            foreach (ThingDef tDef in DefDatabase<ThingDef>.AllDefs)
            {
                TYT_ToolProperties toolProperties = tDef.GetModExtension<TYT_ToolProperties>();
                if (toolProperties == null)
                    continue;
                else
                {
                    foreach (JobDef toolAssignmentTags in toolProperties.defaultToolAssignmentTags)
                    {
                        Log.Message($"TYT: TYT_JobToolAssignmentDatabase - GenerateStartingJobToolAssignments for defaultToolAssignmentTags {toolAssignmentTags}");
                        TYT_JobToolAssignment jobToolAssignment = MakeNewJobToolAssignment();
                        jobToolAssignment.label = toolAssignmentTags.ToString().Translate();
                        jobToolAssignment.filter.SetAllow(tDef, true);
                    }
                    /*
                    if (toolProps.defaultJobToolAssignmentTags.Contains("Constructor"))
                        staConstructor.filter.SetAllow(tDef, true);
                    if (toolProps.defaultJobToolAssignmentTags.Contains("Miner"))
                        staMiner.filter.SetAllow(tDef, true);
                    if (toolProps.defaultJobToolAssignmentTags.Contains("PlantWorker"))
                        staPlantWorker.filter.SetAllow(tDef, true);
                    */
                }
            }
            /*
            TYT_JobToolAssignment staNothing = MakeNewJobToolAssignment();
            staNothing.label = "Tool Assignment is Nothing".Translate();
            staNothing.filter.SetDisallowAll(null, null);
            */
        }
        public TYT_JobToolAssignment MakeNewJobToolAssignment()
        {
            Log.Message($"TYT: TYT_JobToolAssignment - MakeNewJobToolAssignment");
            int uniqueId = JobToolAssignments.Any() ? JobToolAssignments.Max(a => a.uniqueId) + 1 : 1;
            TYT_JobToolAssignment jobToolAssignment = new TYT_JobToolAssignment(uniqueId, $"{"TYT_JobToolAssignment".Translate()} {uniqueId}");
            jobToolAssignment.filter.SetAllow(TYT_ThingCategoryDefOf.BaseTools, true);
            JobToolAssignments.Add(jobToolAssignment);
            return jobToolAssignment;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref initialized, "initialized", false);
            Scribe_Collections.Look(ref JobToolAssignments, "JobToolAssignments", LookMode.Deep, new object[0]);
        }

        public AcceptanceReport TryDelete(TYT_JobToolAssignment pawnToolAssignment)
        {
            Log.Message($"TYT: TYT_JobToolAssignment - TryDelete");
            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive)
                if (pawn.TryGetComp<TYT_PawnToolAssignmentTracker>()?.GetCurrentJobToolAssignment() == pawnToolAssignment)
                    return new AcceptanceReport("JobToolAssignmentInUse".Translate(pawn));
            foreach (Pawn pawn2 in PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead)
                if (pawn2.TryGetComp<TYT_PawnToolAssignmentTracker>() is TYT_PawnToolAssignmentTracker pawnToolAssignmentTracker &&
                    pawnToolAssignmentTracker.GetCurrentJobToolAssignment() == pawnToolAssignment)
                    pawnToolAssignmentTracker.SetCurrentJobToolAssignment(null);
            JobToolAssignments.Remove(pawnToolAssignment);
            return AcceptanceReport.WasAccepted;
        }
    }

}
