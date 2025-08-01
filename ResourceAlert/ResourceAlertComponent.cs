using System.Collections.Generic;
using Verse;

namespace ResourceAlert
{
	/// <summary>
	/// Holds alert thresholds for resources and categories on a per-map basis.
	/// </summary>
	public class ResourceAlertComponent : MapComponent
	{

		// these will be saved/loaded automatically by Scribe_Collections.Look
		public Dictionary<ThingDef, int> alertableResources = new Dictionary<ThingDef, int>();
		public Dictionary<ThingCategoryDef, int> alertableCategories = new Dictionary<ThingCategoryDef, int>();

		public ResourceAlertComponent(Map map) : base(map) { }

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(
				ref alertableResources,
				"Deep_ResourceAlert_alertableResources",
				LookMode.Def,    // key is a Def
				LookMode.Value   // value is an int
			);
			Scribe_Collections.Look(
				ref alertableCategories,
				"Deep_ResourceAlert_alertableCategories",
				LookMode.Def,
				LookMode.Value
			);
			// ensure no null after loading
			if (alertableResources == null)
				alertableResources = new Dictionary<ThingDef, int>();
			if (alertableCategories == null)
				alertableCategories = new Dictionary<ThingCategoryDef, int>();

			DebugLog.Message($"Deep's Resource Alert: data loaded for map {map.GetUniqueLoadID()}.");
		}

		public override void MapGenerated()
		{
			base.MapGenerated();

			if (Find.CurrentMap.IsPlayerHome)
			{           
				// copy defaults into the fresh dictionaries
				foreach (var kv in ResourceAlertMod.Settings.defaultResourceThresholds)
					alertableResources[kv.Key] = kv.Value;
				foreach (var kv in ResourceAlertMod.Settings.defaultCategoryThresholds)
					alertableCategories[kv.Key] = kv.Value;
			}

		}
	}
}
