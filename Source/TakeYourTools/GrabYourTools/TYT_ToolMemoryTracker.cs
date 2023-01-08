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
        public TYT_ToolMemoryTracker(World world) : base(world)
        {

        }
        private List<TYT_ToolMemory> toolMemories = new List<TYT_ToolMemory>();
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
            if (toolMemories == null)
                toolMemories = new List<TYT_ToolMemory>();
        }

        /// <summary>
        /// Get ToolMemory for a pawn
        /// </summary>
        public TYT_ToolMemory GetMemory(Pawn pawn)
        {
            CheckToolMemories();
            TYT_ToolMemory toolMemory = toolMemories.Find(tm => tm != null && tm.pawn == pawn);
            if (toolMemory == null)
            {
                Log.Message($"TYT: TYT_ToolMemoryTracker - GetMemory init");
                toolMemory = new TYT_ToolMemory();
                toolMemory.pawn = pawn;
                toolMemories.Add(toolMemory);
            }
            return toolMemory;
        }

        /// <summary>
        /// Clear ToolMemory for a pawn
        /// </summary>
        public void ClearMemory(Pawn pawn)
        {
            CheckToolMemories();
            TYT_ToolMemory toolMemory = toolMemories.Find(tm => tm != null && tm.pawn == pawn);
            if (toolMemory != null)
            {
                Log.Message($"TYT: TYT_ToolMemoryTracker - ClearMemory");
                Thing previouslyEquipped = toolMemory.PreviousEquipped;
                if (previouslyEquipped != null && pawn.inventory != null && pawn.inventory.GetDirectlyHeldThings() != null && pawn.inventory.GetDirectlyHeldThings().Contains(previouslyEquipped))
                    TryEquipTool(pawn, previouslyEquipped as ThingWithComps, false);
                toolMemories.Remove(toolMemory);
            }
        }

        /// <summary>
        /// Identify 
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static bool EquipAppropriateTool(Pawn pawn, SkillDef skill)
        {
            Log.Message($"TYT: TYT_ToolMemoryTracker - EquipAppropriateTool");            
            if (pawn == null || skill == null)
                return false;

            ThingOwner heldThingsOwner = pawn.inventory.GetDirectlyHeldThings();
            List<Thing> toolsHeld = heldThingsOwner.Where(thing => thing.def.IsTool()).ToList();
            foreach (Thing tool in toolsHeld)
            {
                if (HasReleventStatModifiers(tool, skill))
                {
                    return TryEquipTool(pawn, tool as ThingWithComps);
                }
            }
            return false;
        }

        public static bool HasReleventStatModifiers(Thing tool, SkillDef skill)
        {
            if (tool == null)
                return false;

            List<StatModifier> statModifiers = tool.def.equippedStatOffsets;
            if (skill != null && statModifiers != null)
            {
                Log.Message($"TYT: TYT_ToolMemoryTracker - Found relevantSkills...");
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
                if (tool.holdingOwner != null)
                {
                    tool.holdingOwner.Remove(tool);
                }
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
            //Log.Message($"TYT: TYT_ToolMemoryTracker - IsPawnUsingTool {GetMemory(pawn)?.IsUsingTool}");
            return GetMemory(pawn)?.IsUsingTool ?? false;
        }

        #endregion
    }
}
