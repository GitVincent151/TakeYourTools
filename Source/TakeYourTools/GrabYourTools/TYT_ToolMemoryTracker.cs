using System;
using System.Collections.Generic;
using System.Linq;

using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;

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
            Log.Message($"TYT: TYT_ToolMemoryTracker - Constructor");


        }
        #endregion

        #region Methods
        /// <summary>
        /// Write data in save
        /// </summary>
        public override void ExposeData()
        {
            Log.Message($"TYT: TYT_ToolMemoryTracker - ExposeData --> Add existing memories from save in the memory tracker");
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                toolMemories = toolMemories.Where(memory => memory != null && memory.pawn != null && !memory.pawn.Dead && !memory.pawn.Destroyed && memory.pawn.Spawned).ToList();
            }
            Scribe_Collections.Look(ref toolMemories, "toolMemories", LookMode.Deep);
            CheckToolMemories();
        }

        /// <summary>
        /// Users somehow ending up with null memory and its unfeasible to test get their save
        /// </summary>
        private void CheckToolMemories()
        {
            if (toolMemories == null)
            {
                Log.Message($"TYT: TYT_ToolMemoryTracker - CheckToolMemories --> No memory list found in the game"); 
                toolMemories = new List<TYT_ToolMemory>();
            }
                
        }

        /// <summary>
        /// Get ToolMemory for a pawn
        /// </summary>
        public TYT_ToolMemory GetMemory(Pawn _pawn)
        {
            CheckToolMemories();

            TYT_ToolMemory toolMemory = toolMemories.Find(tm => tm != null && tm.pawn == _pawn);
            if (toolMemory == null)
            {
                toolMemory = new TYT_ToolMemory
                {
                    pawn = _pawn
                };
                toolMemories.Add(toolMemory);
                Log.Message($"TYT: TYT_ToolMemoryTracker - GetMemory --> Pawn nenory created for the pawn {_pawn.Name}");
            }
            return toolMemory;
        }        
        
        /// <summary>
        /// Clear ToolMemory for a pawn
        /// </summary>
        public void ClearMemory(Pawn pawn)
        {
            CheckToolMemories();

            Log.Message($"TYT: TYT_ToolMemoryTracker - ClearMemory for the pawn {pawn.Name}");
            TYT_ToolMemory toolMemory = toolMemories.Find(tm => tm != null && tm.pawn == pawn);
            if (toolMemory != null)
            {
                Log.Message($"TYT: TYT_ToolMemoryTracker - ClearMemory for the pawn {pawn.Name}");
                Thing previouslyEquipped = toolMemory.PreviousEquipped;
                if (previouslyEquipped != null && pawn.inventory != null && pawn.inventory.GetDirectlyHeldThings() != null && pawn.inventory.GetDirectlyHeldThings().Contains(previouslyEquipped))
                    TryEquipTool(pawn, previouslyEquipped as ThingWithComps, false);
                toolMemories.Remove(toolMemory);
            }
        }

        /// <summary>
        /// Look if a tool is available that is appropriate for the JobDef 
        /// </summary>
        public bool EquipAppropriateTool(Pawn pawn, JobDef _jobDef)
        {
                   
            if (pawn == null || _jobDef == null)
                return false;

            ThingOwner heldThingsOwner = pawn.inventory.GetDirectlyHeldThings();
            List<Thing> toolsHeld = heldThingsOwner.Where(thing => thing.def.thingClass == typeof(TYT_ToolThing)).ToList();
            foreach (TYT_ToolThing tool in toolsHeld)
            {
                if (HasAppropriatedToolsForJob(tool, _jobDef))
                {
                    Log.Message($"TYT: TYT_ToolMemoryTracker - EquipAppropriateTool --> Pawn {pawn.Name} takes tool {tool.Label} for JobDef {_jobDef}");
                    return TryEquipTool(pawn, tool as ThingWithComps);
                }
            }
            Log.Message($"TYT: TYT_ToolMemoryTracker - EquipAppropriateTool --> Pawn {pawn.Name} needs other tool for JobDef {_jobDef}");
            return false;
        }

        
        /// <summary>
        /// Check if the tool is appropriate for the JobDef
        /// </summary>
        public bool HasAppropriatedToolsForJob(TYT_ToolThing tool, JobDef _jobDef)
        {
            if (tool == null || _jobDef == null)
                return false;

            if (tool.DefaultToolAssignmentTags != null)
            {
                if (tool.DefaultToolAssignmentTags.Contains(_jobDef))
                {
                    Log.Message($"TYT: TYT_ToolMemoryTracker - HasAppropriatedToolsForJob --> the tool {tool.LabelShort} is appropriate for the job {_jobDef}");
                    return true;
                }
            }
            
            return false;
        }

        public static bool TryEquipTool(Pawn pawn, ThingWithComps tool, bool makeSound = true)
        {
            //Log.Message($"TYT: TYT_ToolMemoryTracker - TryEquipTool");
            if (pawn == null || tool == null)
                return false;

            ThingWithComps currentTool = pawn.equipment.Primary;

            bool transferSuccess = true;

            if (currentTool != null)
                transferSuccess = pawn.inventory.innerContainer.TryAddOrTransfer(currentTool);

            if (transferSuccess)
            {
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
                Log.Warning("TYT: Unable to transfer equipped tool to inventory");
            }

            return false;
        }

        public bool IsPawnUsingTool(Pawn pawn)
        {
            // Log.Message($"TYT: TYT_ToolMemoryTracker - IsPawnUsingTool for the pawn {pawn.Name}");
            return GetMemory(pawn)?.IsUsingTool ?? false;
        }

        #endregion
    }
}
