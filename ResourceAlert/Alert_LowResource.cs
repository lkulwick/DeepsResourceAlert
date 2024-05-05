using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ResourceAlert
{
    public class Alert_LowResource : Alert
    {
        public static Dictionary<ThingDef, int> alertableResources = new Dictionary<ThingDef, int>();
		public static Dictionary<ThingCategoryDef, int> alertableCategories = new Dictionary<ThingCategoryDef, int>();
		public static HashSet<ThingDef> lowResources = new HashSet<ThingDef>();
		public static HashSet<ThingCategoryDef> lowCategories = new HashSet<ThingCategoryDef>();

		public Alert_LowResource()
        {
            Log.Message("Low Resources contructor");
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

        public override AlertReport GetReport()
        {
            //TODO: Check the performance impact of this:
            //if (Find.TickManager.TicksGame < 150000)
            //{
            //	return false;
            //}
            //Log.Message("Alert_LowResource get report");
            return this.CheckMapResources(Find.CurrentMap) != null;
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
            // Log.Message("Alert_LowResource populate");
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
                lowResourcesString += "\n" + resource.LabelCap + "Deep_ResourceAlert_LowResourceDescAvailable".Translate() + map.resourceCounter.GetCount(resource) + "Deep_ResourceAlert_LowResourceDescDesired".Translate() + alertableResources.TryGetValue(resource);
            }
			foreach (ThingCategoryDef resource in lowCategories)
			{
				lowResourcesString += "\n" + resource.LabelCap + "Deep_ResourceAlert_LowResourceDescAvailable".Translate() + map.resourceCounter.GetCountIn(resource) + "Deep_ResourceAlert_LowResourceDescDesired".Translate() + alertableCategories.TryGetValue(resource);
			}
			return lowResourcesString;
        }
    }

}
