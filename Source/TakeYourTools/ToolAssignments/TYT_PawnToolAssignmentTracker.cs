using Verse;

namespace TakeYourTools
{
    public class TYT_PawnToolAssignmentTracker : ThingComp
    {
        #region Properties
        private Pawn pawn => (Pawn)parent;
        private TYT_JobToolAssignment curJobToolAssignment;
        public int nextToolOptimizeTick = -99999;
        public TYT_ToolForcedHandler forcedHandler;

        #endregion
        #region Methods
        #endregion

        #region Constructor
        public override void Initialize(CompProperties props)
        {
            Log.Message($"TYT: TYT_PawnToolAssignmentTracker - Initialize {pawn.GetUniqueLoadID()}");
            // Called once when the ThingComp is instantiated, and called once during loading. Most commonly used to initialise the comp props, but functionally similar to PostMake() so equally suitable for setting up default values.
            base.Initialize(props);
            forcedHandler = new TYT_ToolForcedHandler();
        }
        #endregion

        #region Methods
        public TYT_JobToolAssignment GetCurrentJobToolAssignment()
        {
            Log.Message($"TYT: TYT_PawnToolAssignmentTracker - GetCurrentJobToolAssignment"); 
            if (curJobToolAssignment == null)
                curJobToolAssignment = Current.Game.GetComponent<TYT_JobToolAssignmentDatabase>().DefaultJobToolAssignment();
            return curJobToolAssignment;
        }
        public void SetCurrentJobToolAssignment(TYT_JobToolAssignment value)
        {
            Log.Message($"TYT: TYT_PawnToolAssignmentTracker - SetCurrentJobToolAssignment"); 
            curJobToolAssignment = value;
            nextToolOptimizeTick = Find.TickManager.TicksGame;
        }
        public override void CompTick()
        {
            // If forced handler is somehow null, fix that
            if (forcedHandler == null)
                forcedHandler = new TYT_ToolForcedHandler();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref nextToolOptimizeTick, "nextToolOptimizeTick", -99999);
            Scribe_Deep.Look(ref forcedHandler, "forcedHandler");
            Scribe_References.Look(ref curJobToolAssignment, "curJobToolAssignment");
        }
        #endregion

    }
}
