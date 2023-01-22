using HarmonyLib;
using System.Collections.Generic;
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
 //       public static TYT_Mod modInstance;
        public static TYT_Mod Instance; // Mod instance used to be sure the class instance is existing      
        public static TYT_ModSettings ModSettings; // Mod settings
        //public static List<TYT_ToolThing> ListofToolsInGame = new List<TYT_ToolThing>(); // List of tools in the game
        //public static TYT_ToolMemoryTracker ToolMemoryTracker => Current.Game.World.GetComponent<TYT_ToolMemoryTracker>(); // ToolMemoryTracker        
        #endregion

        #region Methods
        /// <summary>
        /// A mandatory constructor which resolves the reference to our settings.
        /// </summary>        
        public TYT_Mod(ModContentPack content) : base(content)
        {
            Log.Message($"TYT: TAKEYOURTOOLS MOD STARTED - HELLO ALL!"); 

            Instance = this;

            ModSettings = GetSettings<TYT_ModSettings>();
            //toolMemoryTracker = Current.Game.World.GetComponent<TYT_ToolMemoryTracker>();
        }

        ///// <summary>
        ///// Translate the setting category
        ///// </summary>
        //public override string SettingsCategory()
        //{
        //    return "TakeYourTools (TYT): ModSettingsCategory".Translate();
        //}

        /// <summary>
        /// Override the window for the mod settings
        /// </summary>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            ModSettings.DoWindowContents(inRect);
        }
        #endregion
    }
}
