using Verse;

namespace TakeYourTools
{
    public class TYT_JobToolAssignment : IExposable, ILoadReferenceable
    {

        #region Properties class TYT_JobToolAssignment
        public int uniqueId;
        public string label;
        public ThingFilter filter = new ThingFilter();
        #endregion

        #region Constructor class TYT_JobToolAssignment
        public TYT_JobToolAssignment()
        {
        }
        public TYT_JobToolAssignment(int uniqueId, string label)
        {
            this.uniqueId = uniqueId;
            this.label = label;
        }
        #endregion

        #region Method class TYT_JobToolAssignment
        public string GetUniqueLoadID()
        {
            return "PawnToolAssignment" + label + uniqueId.ToString();
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref uniqueId, "uniqueId");
            Scribe_Values.Look(ref label, "label");
            Scribe_Deep.Look(ref filter, "filter", new object[0]);
        }
        #endregion
    }
}
