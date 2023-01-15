using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Jobs;
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
        // Patch for "Combat Extended"
        public static bool UsingCombatExtended => ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.Name == "Combat Extended");
        #endregion
        
        #region Constructor
        static TYT_StaticConstructorClass()
        {

            // Needed?
            Log.Message($"TYT: TYT_StaticConstructorClass --> Checking Def");

            /*
            // Add ToolAssignmentTracker property to all appropriate pawns
            foreach (ThingDef tDef in DefDatabase<ThingDef>.AllDefs.Where(t => t.race?.Humanlike == true))
            {
                               
                Log.Message($"TYT: TYT_StaticConstructorClass --> Generating Def for CompProperties");
                
                
                if (tDef.comps == null)
                {
                    tDef.comps = new List<CompProperties>();
                }
                tDef.comps.Add(new CompProperties(typeof(TYT_PawnToolAssignmentTracker)));
            }
            */

        }
        #endregion
    }
}
