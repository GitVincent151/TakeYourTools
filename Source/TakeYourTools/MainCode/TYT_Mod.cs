using HarmonyLib;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Verse;

namespace TakeYourTools
{
    /// <summary>
    /// StaticConstructorOnStartup vs inheriting from Mod
    /// Subclasses of the Mod class are loaded extremely early during launch.As such, it is less suitable for most purposes(most notably: using Defs or anything that uses Defs).
    /// Classes with the StaticConstructorOnStartup are initialised just before the Main Menu is shown.Classes with this annotation are loaded in the main thread: a requirement for loading any Texture, Material, Shader, Graphic, GameObject or MaterialPropertyBlock.
    /// Inheriting from Mod is necessary if you want Mod Settings, or if you need to run really early in the loading process.
    /// For the exact order of initialisation, look up Verse.PlayDataLoader.DoPlayLoad.
    /// </summary>
    public class TYT_Mod : Mod
    {
        #region Properties
        public static TYT_ModSettings settings;
        public static TYT_Mod modInstance;

        #endregion

        #region Overide the interface for the mod settings 
        /// <summary>
        /// Init the TYT_Mod class for the mod settings
        /// </summary>        
        public TYT_Mod(ModContentPack content) : base(content)
        {
            GetSettings<TYT_ModSettings>();
        }
        /// <summary>
        /// Translate the setting category
        /// </summary>
        public override string SettingsCategory()
        {
            return "TakeYourTools (TYT): ToolsSettingsCategory".Translate();
        }
        /// <summary>
        /// Override the window for the mod settings
        /// </summary>
        /// <param name="inRect"></param>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            GetSettings<TYT_ModSettings>().DoWindowContents(inRect);
        }
        #endregion

    }
}
