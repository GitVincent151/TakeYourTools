using System;
using System.Collections.Generic;
using System.Linq;

using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.Sound;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.GridBrushBase;

namespace TakeYourTools
{
    public class TYT_ToolMemoryTracker : WorldComponent
    {
        #region Properties
        private static List<TYT_ToolMemory> toolMemories = new List<TYT_ToolMemory>();

        #endregion

        #region Constructor
        public TYT_ToolMemoryTracker(World world) : base(world)
        {
            // Log.Message($"TYT: TYT_ToolMemoryTracker - Constructor");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load data in save
        /// </summary>
        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                toolMemories = toolMemories.Where(memory => memory != null && memory.pawnMemory != null && !memory.pawnMemory.Dead && !memory.pawnMemory.Destroyed && memory.pawnMemory.Spawned).ToList();
                // Log.Message($"TYT: TYT_ToolMemoryTracker - ExposeData --> Save existing memories for Pawns still existing");
            }
            Scribe_Collections.Look(ref toolMemories, "toolMemories", LookMode.Deep);
        }

        /// <summary>
        /// Get ToolMemory for a pawn
        /// </summary>
        private TYT_ToolMemory GetMemory(Pawn pawn)
        {
            if (pawn == null)
                return null;

            TYT_ToolMemory toolMemory = toolMemories.Find(tm => tm != null && tm.pawnMemory == pawn);
            if (toolMemory == null)
            {
                toolMemory = new TYT_ToolMemory
                {
                    pawnMemory = pawn
                };
                toolMemories.Add(toolMemory);
                // Log.Message($"TYT: TYT_ToolMemoryTracker - GetMemory --> Pawn menory created for the pawn {toolMemory.pawnMemory.LabelShort}");
            }

            return toolMemory;
        }

        /// <summary>
        /// Update JobDef for a pawn 
        /// </summary>
        public bool UpdateJobDef(Pawn pawn, JobDef jobDef)
        {
            if (pawn == null || jobDef == null)
                return false;

            return GetMemory(pawn).UpdateJobDef(jobDef);
        }

        /// <summary>
        /// Check if pawn is using a tool TYT_ToolThing
        /// </summary>
        public bool IsPawnUsingTool(Pawn pawn)
        {
            if (pawn == null)
                return false;

            if (pawn.equipment.Primary != null && pawn.equipment.Primary.def.thingClass == typeof(TYT_ToolThing))
            {
                // Log.Message($"TYT: TYT_ToolMemoryTracker - IsPawnUsingTool --> Pawn {pawn.LabelShort} is using a tool TYT_ToolThing {pawn.equipment.Primary.def.defName}");
                return true;
            }
            return false;
        }
        /*
        /// <summary>
        /// Restore previous equipped Thing
        /// </summary>
        public void RestorePreviousEquippedTool(Pawn pawn)
        {
            if (pawn == null)
                return;

            TYT_ToolMemory toolMemory = GetMemory(pawn);
            if (toolMemory.GetPreviousEquippedTool() != null && pawn.inventory != null && pawn.inventory.GetDirectlyHeldThings() != null && pawn.inventory.GetDirectlyHeldThings().Contains(toolMemory.GetPreviousEquippedTool()))
            {
                // Log.Message($"TYT: TYT_ToolMemoryTracker - RestorePreviousEquippedTool --> Pawn {pawn.LabelShort} will try to take the ThingWithComps {toolMemory.GetPreviousEquippedTool().LabelShort}");
                if (TryEquipTool(pawn, toolMemory.GetPreviousEquippedTool() as ThingWithComps, false))
                    toolMemory.ResetMemoryOfPawn();
            }
        }
        */
        
        /// <summary>
        /// Look if a tool is available in the inventory of the pawn that is appropriate for the JobDef 
        /// </summary>
        public bool EquipAppropriateTool(Pawn pawn, JobDef _jobDef)
        {
            if (pawn == null || _jobDef == null)
                return false;

            foreach (TYT_ToolThing tool in pawn.inventory.GetDirectlyHeldThings().Where(thing => thing.def.thingClass == typeof(TYT_ToolThing)).ToList().Cast<TYT_ToolThing>())
            {
                if (HasAppropriatedToolsForJobDef(tool, _jobDef))
                {
                    // Log.Message($"TYT: TYT_ToolMemoryTracker - EquipAppropriateTool --> Pawn {pawn.LabelShort} takes tool {tool.Label} for JobDef {_jobDef}");
                    return TryEquipTool(pawn, tool as ThingWithComps);
                }
            }
            return false;
        }
        
        /// <summary>
        /// Check if the tool is appropriate for the JobDef
        /// </summary>
        public bool HasAppropriatedToolsForJobDef(TYT_ToolThing tool, JobDef _jobDef)
        {
            if (tool == null || _jobDef == null)
                return false;

            if (tool.DefaultToolAssignmentTags != null)
            {
                if (tool.DefaultToolAssignmentTags.Contains(_jobDef))
                {
                    // Log.Message($"TYT: TYT_ToolMemoryTracker - HasAppropriatedToolsForJob --> the tool {tool.LabelShort} is appropriate for the job {_jobDef}");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Look if a tool is available in the inventory of the pawn that is appropriate for the JobDef 
        /// </summary>
        public bool SearchAppropriateTool(Pawn pawn, JobDef _jobDef)
        {
            if (pawn == null || _jobDef == null)
                return false;

            ThingOwner thingOwner = Find.CurrentMap.GetDirectlyHeldThings();
            Log.Message($"TYT: TYT_ToolMemoryTracker - SearchAppropriateTool --> Pawn {pawn.LabelShort} will search tool for JobDef {_jobDef}");
            foreach (TYT_ToolThing tool in thingOwner.Where(thing => thing.def.thingClass == typeof(TYT_ToolThing)).ToList().Cast<TYT_ToolThing>())
            {
                if (
                    HasAppropriatedToolsForJobDef(tool, _jobDef)
                    //&& !(tool.InUse)
                    && pawn.CanReserveAndReach(tool, PathEndMode.OnCell, Danger.None, 1, -1, null, false)
                    )
                {
                    Log.Message($"TYT: TYT_ToolMemoryTracker - SearchAppropriateTool --> Pawn {pawn.LabelShort} found the tool {tool.Label} for JobDef {_jobDef}");

                    Job jobNew = new Job(DefDatabase<JobDef>.GetNamed("TakeTool"), tool);
                    pawn.Reserve(tool, jobNew, 1, -1, null);
                    //pawn.jobs.jobQueue.EnqueueLast(jobNew);
                    pawn.jobs.jobQueue.EnqueueFirst(jobNew);

                    //return TryEquipTool(pawn, tool as ThingWithComps);
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Try to equip the pawn with the tool
        /// </summary>
        private bool TryEquipTool(Pawn pawn, ThingWithComps tool, bool makeSound = true)
        {
            if (pawn == null || tool == null)
                return false;

            ThingWithComps currentTool = pawn.equipment.Primary;

            bool transferSuccess = true;

            if (currentTool != null)
                transferSuccess = pawn.inventory.innerContainer.TryAddOrTransfer(currentTool);

            if (transferSuccess)
            {
                //TYT_ToolMemory toolMemory = GetMemory(pawn);
                //toolMemory.UpdatePreviousEquippedTool(currentTool);
                if (tool.stackCount > 1)
                {
                    tool = (ThingWithComps)tool.SplitOff(1);
                }
                tool.holdingOwner?.Remove(tool);
                pawn.equipment.AddEquipment(tool);
                if (makeSound)
                    tool.def.soundInteract?.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
                return true;
            }
            else
            {
                Log.Message($"TYT: TYT_ToolMemoryTracker - TryEquipTool --> Unable to transfer equipped tool for Pawn {pawn.Name} to inventory");

                //if pawns are heavily encumbered, they need to haul.
                if (MassUtility.EncumbrancePercent(pawn) >= 0.90f)
                {
                    Job haul = HaulAIUtility.HaulToStorageJob(pawn, currentTool);
                    pawn.inventory.UnloadEverything = true;
                    return true;
                }
                
            }

            return false;
        }

        #endregion
    }
}
