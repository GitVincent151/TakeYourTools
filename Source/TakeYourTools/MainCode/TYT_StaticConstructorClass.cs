using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace TakeYourTools
{

    [StaticConstructorOnStartup]
    public static class TYT_StaticConstructorClass
    {

        /// <summary>
        /// Initialization of the mod (static class), PawnDef will get an instance of the class TYT_JobToolAssignmentTracker
        /// </summary>
        static TYT_StaticConstructorClass()
        {
            Log.Message($"TYT: TYT_StaticConstructor - MOD STARTED - HELLO ALL!");
            /*
            // Add validator to ThingSetMakerDef
            Log.Message($"TYT: Add validator to ThingSetMakerDef");
            TYT_ThingSetMakerDefOf.MapGen_AncientRuinsTools.root.fixedParams.validator = (ThingDef t) => t.IsWithinCategory(TYT_ThingCategoryDefOf.BaseTools_Crafted);
            Log.Message($"TYT: Added validator to ThingSetMakerDef");
            */

            /*
            if (ModCompatibilityCheck.MendAndRecycle)
            {
                ResolveMendAndRecycleRecipes();
            }
            */

            //ResolveSmeltingRecipeUsers();
            // CheckStuffForStuffPropsTool();

            // Add ToolAssignmentTracker to all appropriate pawns
            foreach (ThingDef tDef in DefDatabase<ThingDef>.AllDefs.Where(t => t.race?.Humanlike == true))
            {
                if (tDef.comps == null)
                {
                    Log.Message($"TYT: TYT_StaticConstructor - Initialize the list of properties for components");
                    tDef.comps = new List<CompProperties>();
                }
                Log.Message($"TYT: TYT_StaticConstructor - Add ToolAssignmentTracker to {tDef.defName}");
                tDef.comps.Add(new CompProperties(typeof(TYT_PawnToolAssignmentTracker)));
            }
        }

        /*
        private static void ResolveMendAndRecycleRecipes()
        {
            bool categoryMatch = false;
            foreach (RecipeDef recipe in DefDatabase<RecipeDef>.AllDefs.Where(r => r.defName.Contains("SurvivalTool") && r.workerClass != typeof(RecipeWorker)))
            {
                categoryMatch = false;
                foreach (ThingDef thing in DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.thingClass == typeof(SurvivalTool)))
                {
                    if (recipe.IsIngredient(thing))
                    {
                        categoryMatch = true;
                        break;
                    }
                }

                if (!categoryMatch)
                {
                    recipe.recipeUsers.Clear();
                }

            }
        }
        */
        /*
        private static void ResolveSmeltingRecipeUsers()
        {
            foreach (ThingDef benchDef in DefDatabase<ThingDef>.AllDefs.Where(t => t.IsWorkTable))
            {
                if (benchDef.recipes != null)
                {
                    if (benchDef.recipes.Contains(ST_RecipeDefOf.SmeltWeapon))
                    {
                        benchDef.recipes.Add(ST_RecipeDefOf.SmeltSurvivalTool);
                    }

                    if (benchDef.recipes.Contains(ST_RecipeDefOf.DestroyWeapon))
                    {
                        benchDef.recipes.Add(ST_RecipeDefOf.DestroySurvivalTool);
                    }
                }
            }
        }
        */
        /*
        private static void CheckStuffForStuffPropsTool()
        {
            StringBuilder stuffBuilder = new StringBuilder();
            stuffBuilder.AppendLine("Checking all stuff for StuffPropsTool modExtension...");
            stuffBuilder.AppendLine();
            StringBuilder hasPropsBuilder = new StringBuilder("Has props:\n");
            StringBuilder noPropsBuilder = new StringBuilder("Doesn't have props:\n");

            List<StuffCategoryDef> toolCats = new List<StuffCategoryDef>();
            foreach (ThingDef tool in DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.IsSurvivalTool()))
            {
                if (!tool.stuffCategories.NullOrEmpty())
                {
                    foreach (StuffCategoryDef category in tool.stuffCategories)
                    {
                        if (!toolCats.Contains(category))
                        {
                            toolCats.Add(category);
                        }
                    }
                }
            }

            foreach (ThingDef stuff in DefDatabase<ThingDef>.AllDefsListForReading.Where(
                (ThingDef t) =>
                {
                    if (!t.IsStuff)
                    {
                        return false;
                    }

                    bool retVal = false;
                    foreach (StuffCategoryDef stuffCat in t.stuffProps.categories)
                    {
                        if (toolCats.Contains(stuffCat))
                        {
                            retVal = true;
                            break;
                        }
                    }

                    return retVal;
                }))
            {

                string newLine = $"{stuff} ({stuff.modContentPack.Name})";
                if (stuff.HasModExtension<StuffPropsTool>())
                {
                    hasPropsBuilder.AppendLine(newLine);
                }
                else
                {
                    noPropsBuilder.AppendLine(newLine);
                }
            }

            stuffBuilder.Append(hasPropsBuilder);
            stuffBuilder.AppendLine();
            stuffBuilder.Append(noPropsBuilder);
            Log.Message(stuffBuilder.ToString());
        }
        */
    }


}
