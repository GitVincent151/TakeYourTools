using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TakeYourTools
{

    /// <summary>
    /// Initialization of the mod TakeYourTools (static class)
    /// The [StaticConstructorOnStartup] annotation does what it says on the tin.
    /// At startup, it runs the Static Constructor of any class with that annotation.
    /// What you need to know: It's a method without a return type, has the same name as the class and doesn't take any arguments.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class TYT_StaticConstructorClass
    {
        #region Properties

        // Instance of the class
        //private TYT_StaticConstructorClass _instance;
        //public TYT_StaticConstructorClass Instance => _instance;

        // Patch for "Combat Extended"
        public static bool UsingCombatExtended => ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name == "Combat Extended");

        // ToolMemoryTracker and Patch   
        public static TYT_ToolMemoryTracker ToolMemoriesTracker => Current.Game.World.GetComponent<TYT_ToolMemoryTracker>();

        #endregion

        #region Constructor
        static TYT_StaticConstructorClass()
        {
            // Hello
            Log.Message("TYT: Mod TakeYourTools started");

        }
        #endregion
    }
}
