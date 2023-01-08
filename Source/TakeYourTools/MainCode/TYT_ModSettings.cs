//using SettingsHelper;
using UnityEngine;
using Verse;

namespace TakeYourTools
{
    public class TYT_ModSettings : ModSettings
    {
        #region Properties
        public static bool hardcoreMode = false;
        public static bool reduceNoToolWorkEfficiency = false;
        public static bool toolMapGen = true;
        public static bool toolLimit = true;
        private static float toolDegradationFactor = 1f;
        public static bool toolOptimization = true;
        public static float ToolDegradationFactor => Mathf.Pow(toolDegradationFactor, (toolDegradationFactor < 1f) ? 1 : 2);
        public static bool ToolDegradation => toolDegradationFactor > 0f;
        #endregion

        #region Methods
        /// <summary>
        /// Interface for the mod settings
        /// </summary>
        public void DoWindowContents(Rect wrect)
        {
            Listing_Standard options = new Listing_Standard();
            Color defaultColor = GUI.color;
            options.Begin(wrect);

            GUI.color = defaultColor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            options.Gap();

            // Same GUI colour as Merciless
            GUI.color = new Color(1f, 0.2f, 0.2f);
            options.CheckboxLabeled("Settings_HardcoreMode".Translate(), ref hardcoreMode, "Settings_HardcoreMode_Tooltip".Translate());
            GUI.color = defaultColor;
            options.Gap();
            options.CheckboxLabeled("Settings_ReduceNoToolWorkEfficiency".Translate(), ref reduceNoToolWorkEfficiency, "Settings_ReduceNoToolWorkEfficiency_Tooltip".Translate());
            options.Gap();
            options.CheckboxLabeled("Settings_ToolMapGen".Translate(), ref toolMapGen, "Settings_ToolMapGen_Tooltip".Translate());
            options.Gap();
            options.CheckboxLabeled("Settings_ToolLimit".Translate(), ref toolLimit, "Settings_ToolLimit_Tooltip".Translate());
            options.Gap();
            options.SliderLabeled("Settings_ToolDegradationRate".Translate(), toolDegradationFactor, 0f, 2f);
            /*
            options.AddLabeledSlider("Settings_ToolDegradationRate".Translate(), ref toolDegradationFactor, 0f, 2f,
                rightAlignedLabel: ToolDegradationFactor.ToStringByStyle(ToStringStyle.FloatTwo, ToStringNumberSense.Factor), roundTo: 0.01f);
            */
            options.Gap();
            options.CheckboxLabeled("Settings_ToolOptimization".Translate(), ref toolOptimization, "Settings_ToolOptimization_Tooltip".Translate());
            options.End();

            // Get the new settings
            Mod.GetSettings<TYT_ModSettings>().Write();

        }

        /// <summary>
        /// Save Data
        /// </summary>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref hardcoreMode, "hardcoreMode", false);
            Scribe_Values.Look(ref reduceNoToolWorkEfficiency, "reduceNoToolWorkEfficiency", false);
            Scribe_Values.Look(ref toolMapGen, "toolMapGen", true);
            Scribe_Values.Look(ref toolLimit, "toolLimit", true);
            Scribe_Values.Look(ref toolDegradationFactor, "toolDegradationFactor", 1f);
            Scribe_Values.Look(ref toolOptimization, "toolOptimization", true);
        }
        #endregion
    }
}
