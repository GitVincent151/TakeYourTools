using Verse;

namespace TakeYourTools
{
    /// <summary>
    /// Auto tool switcher
    /// </summary>
    public class TYT_ToolAssignment : IExposable, ILoadReferenceable
    {

        public TYT_ToolAssignment()
        {
        }

        public TYT_ToolAssignment(int uniqueId, string label)
        {
            this.uniqueId = uniqueId;
            this.label = label;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref uniqueId, "uniqueId");
            Scribe_Values.Look(ref label, "label");
            Scribe_Deep.Look(ref filter, "filter", new object[0]);
        }

        public string GetUniqueLoadID()
        {
            return "ToolAssignment_" + label + uniqueId.ToString();
        }

        public int uniqueId;
        public string label;
        public ThingFilter filter = new ThingFilter();

    }
}
