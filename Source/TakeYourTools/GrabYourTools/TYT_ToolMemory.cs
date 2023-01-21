using RimWorld;
using Verse;


namespace TakeYourTools
{
    public class TYT_ToolMemory : IExposable
    {
        #region Properties

        public Pawn pawn = null;

        private JobDef lastCheckedJob = null;
        
        private Thing previousEquipped = null;
        public Thing PreviousEquipped => previousEquipped;
        
        private bool? usingTool = false;
        public bool IsUsingTool => (usingTool.HasValue ? usingTool.Value : false);
        
        #endregion

        #region Methods
        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Defs.Look(ref lastCheckedJob, "lastCheckedJob");
            Scribe_Values.Look(ref usingTool, "usingTool");
            Scribe_References.Look(ref previousEquipped, "previousEquipped");
        }

        public bool UpdateJob(JobDef _job)
        {
            if (lastCheckedJob == null)
            {
                lastCheckedJob = _job;
                Log.Message($"TYT: TYT_ToolMemory - UpdateJob --> Init of the job in the memory");
                return true;
            }
            else if (lastCheckedJob != _job)
            {
                lastCheckedJob = _job;
                Log.Message($"TYT: TYT_ToolMemory - UpdateJob --> Update of the job {_job} in the memory");
                return true;
            }
            return false;            
        }

        public void UpdateUsingTool(Thing equipped, bool isUsingTool)
        {
            if (previousEquipped == null)
                previousEquipped = equipped;
            usingTool = isUsingTool;
        }
        #endregion
    }
}
