using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace ResourceAlert
{
    public class ResourceAlertModSettings : ModSettings
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(
                ref Alert_LowResource.alertableResources,
                "Deep_ResourceAlert_alertableResources",
                LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(
                ref Alert_LowResource.alertableCategories,
                "Deep_ResourceAlert_alertableCategories",
                LookMode.Def, LookMode.Value);

            if (Alert_LowResource.alertableResources == null)
                Alert_LowResource.alertableResources = new Dictionary<ThingDef, int>();
            if (Alert_LowResource.alertableCategories == null)
                Alert_LowResource.alertableCategories = new Dictionary<ThingCategoryDef, int>();
        }
    }

    public class ResourceAlertMod : Mod
    {
        private ResourceAlertModSettings settings;

        public ResourceAlertMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<ResourceAlertModSettings>();
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
                Find.WindowStack.Add(new TrackedResourcesWindow());
        }
    }
}
