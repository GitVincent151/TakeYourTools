using RimWorld;
using Verse;


namespace TakeYourTools
{
    public class TYT_ToolMemory : IExposable
    {
        #region Properties

        public Pawn pawn = null;

        private SkillDef lastCheckedSkill = null;
        
        private Thing previousEquipped = null;
        public Thing PreviousEquipped => previousEquipped;
        
        private bool? usingTool = false;
        public bool IsUsingTool => (usingTool.HasValue ? usingTool.Value : false);
        
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
