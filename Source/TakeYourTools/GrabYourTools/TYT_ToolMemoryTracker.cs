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
        private List<TYT_ToolMemory> toolMemories = new List<TYT_ToolMemory>();

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
            Log.Message($"TYT: TYT_ToolMemoryTracker - CheckToolMemories");
            if (toolMemories == null)
                toolMemories = new List<TYT_ToolMemory>();
        }

        /// <summary>
        /// Get ToolMemory for a pawn
        /// </summary>
        public TYT_ToolMemory GetMemory(Pawn _pawn)
        {
            CheckToolMemories();

            Log.Message($"TYT: TYT_ToolMemoryTracker - GetMemory");
            TYT_ToolMemory toolMemory = toolMemories.Find(tm => tm != null && tm.pawn == _pawn);
            if (toolMemory == null)
            {
                Log.Message($"TYT: TYT_ToolMemoryTracker - GetMemory for the pawn {_pawn.Name}");
                toolMemory = new TYT_ToolMemory
                {
                    pawn = _pawn
                };
                toolMemories.Add(toolMemory);
                Log.Message($"TYT: TYT_ToolMemoryTracker - pawn nenory created for the pawn {_pawn.Name}");
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
        /// Identify 
        /// </summary>
        public bool EquipAppropriateTool(Pawn pawn, JobDef _jobDef)
        {
            Log.Message($"TYT: TYT_ToolMemoryTracker - EquipAppropriateTool for the pawn {pawn.Name}");            
            if (pawn == null || _jobDef == null)
                return false;

            ThingOwner heldThingsOwner = pawn.inventory.GetDirectlyHeldThings();
            List<Thing> toolsHeld = heldThingsOwner.Where(thing => thing.def.IsTool()).ToList();
            foreach (Thing tool in toolsHeld)
            {
                if (HasReleventStatModifiers(tool, _jobDef))
                {
                    return TryEquipTool(pawn, tool as ThingWithComps);
                }
            }
            return false;
        }

        public bool HasReleventStatModifiers(Thing tool, JobDef _jobDef)
        {
            Log.Message($"TYT: TYT_ToolMemoryTracker - HasReleventStatModifiers - start");

            if (tool == null)
                return false;

            //List<StatModifier> statModifiers = tool.def.equippedStatOffsets;
            List<JobDef> jobDefList = tool.def.GetModExtension<TYT_ToolProperties>().defaultToolAssignmentTags;

            if (_jobDef != null && jobDefList != null)
            {
                

                if (jobDefList.Contains(_jobDef))
                {
                    Log.Message($"TYT: TYT_ToolMemoryTracker - Found relevantSkills for the tool {tool.LabelShort}");
                    return true;
                }
                else
                {
                    Log.Message($"TYT: TYT_ToolMemoryTracker - Didn´t found relevantSkills for the tool {tool.LabelShort}");
                }

              
                /*
                foreach (JobDef _job in jobDefList)
                    
                foreach (StatModifier statModifier in statModifiers)
                {
                    List<SkillNeed> skillNeedOffsets = statModifier.stat.skillNeedOffsets;
                    List<SkillNeed> skillNeedFactors = statModifier.stat.skillNeedFactors;
                    if (skillNeedOffsets != null)
                    {
                        Log.Message($"TYT: TYT_ToolMemoryTracker - Found skillNeedOffsets...");
                        foreach (SkillNeed skillNeed in skillNeedOffsets)
                        {
                            if (skill == skillNeed.skill)
                            {
                                //Logger.MessageFormat(this, "{0} has {1}, relevant to {2}", tool.Label, statModifier.stat.label, skillNeed.skill);
                                return true;
                            }
                        }
                    }
                    if (skillNeedFactors != null)
                    {
                        foreach (SkillNeed skillNeed in skillNeedFactors)
                        {
                            if (skill == skillNeed.skill)
                            {
                                //Logger.MessageFormat(this, "{0} has {1}, relevant to {2}", tool.Label, statModifier.stat.label, skillNeed.skill);
                                return true;
                            }
                        }
                    }
                }
                */
            }
            
            return false;
        }

        public static bool TryEquipTool(Pawn pawn, ThingWithComps tool, bool makeSound = true)
        {
            Log.Message($"TYT: TYT_ToolMemoryTracker - TryEquipTool");
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
            Log.Message($"TYT: TYT_ToolMemoryTracker - IsPawnUsingTool for the pawn {pawn.Name}");
            return GetMemory(pawn)?.IsUsingTool ?? false;
        }

        #endregion
    }
}
