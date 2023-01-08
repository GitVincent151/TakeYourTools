using RimWorld;
using Verse;


namespace TakeYourTools
{
    public class TYT_ToolMemory : IExposable
    {
        #region Properties
        public Pawn pawn = null;
        private SkillDef lastCheckedSkill = null;
        private bool? usingTool = false;
        private Thing previousEquipped = null;
        public bool IsUsingTool => (usingTool.HasValue ? usingTool.Value : false);
        public Thing PreviousEquipped => previousEquipped;
        #endregion
        #region Methods
        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Defs.Look(ref lastCheckedSkill, "lastCheckedSkill");
            Scribe_Values.Look(ref usingTool, "usingTool");
            Scribe_References.Look(ref previousEquipped, "previousEquipped");
        }
        public bool UpdateSkill(SkillDef skill)
        {
            if (lastCheckedSkill != skill)
            {
                lastCheckedSkill = skill;
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
