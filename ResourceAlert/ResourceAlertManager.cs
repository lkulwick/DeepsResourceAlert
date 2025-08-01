using System.Collections.Generic;
using Verse;

namespace ResourceAlert
{
	/// <summary>
	/// Helper to fetch any of our per-map dictionaries without repeating GetComponent everywhere.
	/// </summary>
	public static class ResourceAlertManager
	{
		// central point of access to the component
		private static ResourceAlertComponent Comp =>
			Find.CurrentMap?.GetComponent<ResourceAlertComponent>();

		// your existing dictionaries:
		public static Dictionary<ThingDef, int> Resources =>
			Comp?.alertableResources;

		public static Dictionary<ThingCategoryDef, int> Categories =>
			Comp?.alertableCategories;

		// in future, just add more:
		// public static Dictionary<SomeKey, SomeValue> NewDict =>
		//     Comp.newDictField;
	}
}
