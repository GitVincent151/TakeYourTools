using System.Collections.Generic;
using Verse;
using static HarmonyLib.Code;

namespace TakeYourTools
{
    public class TYT_ToolForcedHandler : IExposable
    {
        #region Properties
        private List<Thing> forcedTools = new List<Thing>(); //List of tools forced
        //public List<Thing> ForcedTools => forcedTools; //Pointer on the list
        #endregion

        #region Methods
        public void SetForced(Thing tool, bool forced)
        {
            Log.Message($"TYT: TYT_ToolForcedHandler - SetForced {forced}");
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
                Log.Message($"TYT: TYT_ToolForcedHandler - Tool was forced while Destroyed: {tool}");
                if (forcedTools.Contains(tool))
                    forcedTools.Remove(tool);
                return false;
            }
            Log.Message($"TYT: TYT_ToolForcedHandler - Tool forced status");
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
