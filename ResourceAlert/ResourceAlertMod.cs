using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;

namespace ResourceAlert
{
	//public class DefaultAlertSettings : ModSettings
	//{
	//	// default per-map thresholds, keyed by ThingDef
	//	public Dictionary<ThingDef, int> defaultResourceThresholds
	//		= new Dictionary<ThingDef, int>();

	//	// default per-map thresholds, keyed by ThingCategoryDef
	//	public Dictionary<ThingCategoryDef, int> defaultCategoryThresholds
	//		= new Dictionary<ThingCategoryDef, int>();

	//	public override void ExposeData()
	//	{
	//		base.ExposeData();
	//		// Scribe out those two dictionaries
	//		Scribe_Collections.Look(
	//			ref defaultResourceThresholds,
	//			"defaultResourceThresholds",
	//			LookMode.Def,   // key is a Def
	//			LookMode.Value  // value is an int
	//		);
	//		Scribe_Collections.Look(
	//			ref defaultCategoryThresholds,
	//			"defaultCategoryThresholds",
	//			LookMode.Def,
	//			LookMode.Value
	//		);
	//	}
	//}

	public class ResourceAlertMod : Mod
    {

        public ResourceAlertMod(ModContentPack content) : base(content)
        {
        }

        public override string SettingsCategory()
        {
            return "Deep Resource Alert";
        }

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);


			if (listing.ButtonText("Show Tracked Resources"))
			{
				SoundDefOf.Click.PlayOneShot(SoundInfo.OnCamera());
				Find.WindowStack.Add(new TrackedResourcesWindow());
			}

			listing.End();
		}

	}
}
