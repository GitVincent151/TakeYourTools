using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace TakeYourTools
{
    /// <summary>
    /// JobGivers are found either in the ThinkTree or in a DutyDef.
    /// The ThinkTree defines main colonist behaviour and most jobs in here are short:
    /// the ThinkTree is there for stateless decisions, not for tasks with a lot of state;
    /// it's expected that things will be cancelled with little-to-no notification.
    /// </summary>
    public class TYT_JobGiver_TakeTheTool : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Log.Message($"TYT: TYT_JobGiver_TakeTheTool - TryGiveJob --> ");
            throw new NotImplementedException();
        }


    }
}
