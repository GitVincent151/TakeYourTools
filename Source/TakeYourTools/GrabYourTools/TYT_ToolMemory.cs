using RimWorld;
using Verse;


namespace TakeYourTools
{
    public class TYT_ToolMemory : IExposable
    {
        #region Properties

        public Pawn pawnMemory = null;
        private JobDef lastJobDef = null;
        private Thing previousEquippedTool = null;
        #endregion

        #region Methods

        public Thing GetPreviousEquippedTool() => previousEquippedTool;

        /// <summary>
        /// Save/Load function
        /// https://spdskatr.github.io/RWModdingResources/saving-guide
        /// </summary>
        public void ExposeData()
        {
            Scribe_References.Look(ref pawnMemory, "pawn");
            Scribe_Defs.Look(ref lastJobDef, "lastCheckedJob");
            Scribe_References.Look(ref previousEquippedTool, "previousEquipped");
        }

        /// <summary>
        /// Update JobDef in the memory of the pawn
        /// </summary>
        public bool UpdateJobDef(JobDef jobDefOfPawn)
        {
            if (lastJobDef == null || lastJobDef != jobDefOfPawn)
            {
                lastJobDef = jobDefOfPawn;
                return true;
            }
            return false;            
        }

        /// <summary>
        /// Update previous equipped tool in the memory of the pawn
        /// </summary>
        public bool UpdatePreviousEquippedTool(Thing previousEquippedToolForPawn)
        {
            if (previousEquippedTool == null || previousEquippedTool != previousEquippedToolForPawn)
            {
                previousEquippedTool = previousEquippedToolForPawn;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reset the memory of the pawn
        /// </summary>
        public void ResetMemoryOfPawn()
        {
            previousEquippedTool = null;
            lastJobDef = null;
        }
        
        /*
        /// <summary>
        /// Store last equipped tool in the memory of the pawn and update the status UsingTool
        /// </summary>
        public void UpdateUsingTool(Thing equipped, bool isUsingTool)
        {
            if (previousEquippedTool == null)
                previousEquippedTool = equipped;
            UsingTool = isUsingTool;
        }
        */
        #endregion
    }
}
