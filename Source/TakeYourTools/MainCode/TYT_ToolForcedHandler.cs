using System.Collections.Generic;
using Verse;

namespace TakeYourTools
{
    public class TYT_ToolForcedHandler : IExposable
    {
        #region Properties class TYT_ToolForcedHandler
        private List<Thing> forcedTools = new List<Thing>(); //List of tools forced
        public List<Thing> ForcedTools => forcedTools; //Pointer on the list
        #endregion

        #region Methods class TYT_ToolForcedHandler
        public void SetForced(Thing tool, bool forced)
        {
            if (forced && !forcedTools.Contains(tool))
                forcedTools.Add(tool);
            else if (!forced && forcedTools.Contains(tool))
                forcedTools.Remove(tool);

        }
        public void Reset() => forcedTools.Clear();
        public bool IsForced(Thing tool)
        {
            if (tool.Destroyed)
            {
                Log.Error($"Tool was forced while Destroyed: {tool}");
                if (forcedTools.Contains(tool))
                    forcedTools.Remove(tool);
                return false;
            }
            return forcedTools.Contains(tool);
        }
        public bool AllowedToAutomaticallyDrop(Thing tool) => !IsForced(tool);
        public bool SomethingForced => !forcedTools.NullOrEmpty();
        public void ExposeData()
        {
            Scribe_Collections.Look(ref forcedTools, "forcedTools", LookMode.Reference);
        }
        #endregion
    }
}
