using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ResourceAlert
{
    public class Alert_LowResource : Alert
    {
        //public static Dictionary<ThingDef, int> alertableResources = new Dictionary<ThingDef, int>();
        //public static Dictionary<ThingCategoryDef, int> alertableCategories = new Dictionary<ThingCategoryDef, int>();


        // 
        //public static Dictionary<ThingDef, int> criticalResources = new Dictionary<ThingDef, int>();
        //public static Dictionary<ThingCategoryDef, int> criticalCategories = new Dictionary<ThingCategoryDef, int>();


        public static HashSet<ThingDef> lowResources = new HashSet<ThingDef>();
        public static HashSet<ThingCategoryDef> lowCategories = new HashSet<ThingCategoryDef>();

        public Alert_LowResource()
        {
            DebugLog.Message("Low Resources contructor");
            this.defaultLabel = "Deep_ResourceAlert_Alert_LowResource".Translate();
            this.defaultPriority = AlertPriority.Medium;
        }

        public override TaggedString GetExplanation()
        {
            Map map = Find.CurrentMap;
            if (map == null)
            {
                return "";
            }
            string explaination = (lowResources.Count + lowCategories.Count) == 0 ? "" : CombinedLowResourcesString(map);
            return explaination;
        }

		private const int CheckInterval = 250;
		private int lastCheckedTick = -CheckInterval;
        private bool lastReport = false;
        private string lastMap = null;

		public override AlertReport GetReport()
        {
			Map currentMap = Find.CurrentMap;
			if (currentMap == null)
				return false;

			int ticksNow = Find.TickManager.TicksGame;
            if (currentMap.GetUniqueLoadID() != lastMap ||
				ticksNow - lastCheckedTick >= CheckInterval)
            {
                lastCheckedTick = ticksNow;
				lastReport = CheckMapResources(currentMap) != null;
				lastMap = currentMap.GetUniqueLoadID();
			}
            return lastReport;
        }

        private Map CheckMapResources(Map map)
        {
            if (map.IsPlayerHome && map.mapPawns.AnyColonistSpawned)
            {
                if (this.PopulateLowResources(map))
                {
                    return map;
                }
            }
            return null;
        }

        public override string GetLabel()
        {
            return "Deep_ResourceAlert_Alert_LowResource".Translate();
        }

        private bool PopulateLowResources(Map map)
        {
            lowResources.Clear();
            lowCategories.Clear();
            // TODO: instead of clear, manage hashset by adding and removing
            bool foundLowResource = false;
			// DebugLog.Message("Alert_LowResource populate");

			var resourceAlertMapComponent = Find.CurrentMap.GetComponent<ResourceAlertComponent>();

            Dictionary<ThingDef, int> alertableResources = ResourceAlertManager.Resources;
			Dictionary<ThingCategoryDef, int> alertableCategories = ResourceAlertManager.Categories;

			foreach (KeyValuePair<ThingDef, int> entry in alertableResources)
            {
                if (map.resourceCounter.GetCount(entry.Key) < entry.Value)
                {
                    lowResources.Add(entry.Key);
                    foundLowResource = true;
                }
            }
            foreach (KeyValuePair<ThingCategoryDef, int> entry in alertableCategories)
            {
                if (map.resourceCounter.GetCountIn(entry.Key) < entry.Value)
                {
                    lowCategories.Add(entry.Key);
                    foundLowResource = true;
                }
            }
            return foundLowResource;
        }
        private string CombinedLowResourcesString(Map map)
        {
            string lowResourcesString = "Deep_ResourceAlert_LowResourceDescGeneric".Translate();
            foreach (ThingDef resource in lowResources)
            {
                lowResourcesString += "\n" + resource.LabelCap + "Deep_ResourceAlert_LowResourceDescAvailable".Translate() +
                    map.resourceCounter.GetCount(resource) + 
                    "Deep_ResourceAlert_LowResourceDescDesired".Translate() +
                    ResourceAlertManager.Resources.TryGetValue(resource);
            }
            foreach (ThingCategoryDef resource in lowCategories)
            {
                lowResourcesString += "\n" + resource.LabelCap + "Deep_ResourceAlert_LowResourceDescAvailable".Translate() +
                    map.resourceCounter.GetCountIn(resource) +
                    "Deep_ResourceAlert_LowResourceDescDesired".Translate() +
                    ResourceAlertManager.Categories.TryGetValue(resource);
            }
            return lowResourcesString;
        }
    }

}
