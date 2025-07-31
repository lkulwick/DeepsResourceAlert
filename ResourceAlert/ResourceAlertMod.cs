using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ResourceAlert
{
	/// <summary>
	/// Centralizes loading/saving of alertable resources and categories.
	/// Remove any duplicate ResourceAlertSettings classes to avoid conflicts.
	/// </summary>
	public class ResourceAlertModSettings : ModSettings
	{

		public override void ExposeData()
		{
			base.ExposeData();

			// Load or save the list of alertable resources
			Scribe_Collections.Look(
				ref Alert_LowResource.alertableResources,
				"Deep_ResourceAlert_alertableResources",
				LookMode.Def,
				LookMode.Value);

			// Load or save the list of alertable categories
			Scribe_Collections.Look(
				ref Alert_LowResource.alertableCategories,
				"Deep_ResourceAlert_alertableCategories",
				LookMode.Def,
				LookMode.Value);

			// Initialize dictionaries if missing
			if (Alert_LowResource.alertableResources == null)
				Alert_LowResource.alertableResources = new Dictionary<ThingDef, int>();
			else
			{
				// Remove entries with missing ThingDef (null keys)
				var nullKeys = Alert_LowResource.alertableResources.Keys
					.Where(k => k == null)
					.ToList();
				foreach (var key in nullKeys)
					Alert_LowResource.alertableResources.Remove(key);
			}

			if (Alert_LowResource.alertableCategories == null)
				Alert_LowResource.alertableCategories = new Dictionary<ThingCategoryDef, int>();
			else
			{
				// Remove entries with missing ThingCategoryDef (null keys)
				var nullCatKeys = Alert_LowResource.alertableCategories.Keys
					.Where(k => k == null)
					.ToList();
				foreach (var key in nullCatKeys)
					Alert_LowResource.alertableCategories.Remove(key);
			}
		}
	}
}
