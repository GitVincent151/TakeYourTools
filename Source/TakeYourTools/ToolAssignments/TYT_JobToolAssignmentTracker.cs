using Verse;

namespace TakeYourTools
{
    public class TYT_JobToolAssignmentTracker : ThingComp
    {
        private Pawn Pawn => (Pawn)parent;
        private TYT_JobToolAssignment curJobToolAssignment;
        public int nextToolOptimizeTick = -99999;
        public TYT_ToolForcedHandler forcedHandler;



        public TYT_JobToolAssignment CurrentJobToolAssignment
        {
            get
            {
                if (curJobToolAssignment == null)
                    curJobToolAssignment = Current.Game.GetComponent<TYT_JobToolAssignmentDatabase>().DefaultJobToolAssignment();
                return curJobToolAssignment;
            }
            set
            {
                curJobToolAssignment = value;
                nextToolOptimizeTick = Find.TickManager.TicksGame;
            }
        }

        public override void CompTick()
        {
            // If forced handler is somehow null, fix that
            if (forcedHandler == null)
                forcedHandler = new TYT_ToolForcedHandler();
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            forcedHandler = new TYT_ToolForcedHandler();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref nextToolOptimizeTick, "nextToolOptimizeTick", -99999);
            Scribe_Deep.Look(ref forcedHandler, "forcedHandler");
            Scribe_References.Look(ref curJobToolAssignment, "curJobToolAssignment");
        }


    }
}
