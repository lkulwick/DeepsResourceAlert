using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ResourceAlert
{
    public class Alert_LowResource : Alert, IExposable
    {
        public static Dictionary<ThingDef, int> alertableResources = new Dictionary<ThingDef, int>();
        public static HashSet<ThingDef> lowResources = new HashSet<ThingDef>();

        public Alert_LowResource()
        {
            //Log.Message("Alert_LowResource contructor called.");
            this.defaultLabel = "Deep_ResourceAlert_Alert_LowResource".Translate();
            this.defaultPriority = AlertPriority.Medium;
            // TODO: populate dict with saved data
        }

        public override TaggedString GetExplanation()
        {
            Map map = this.MapWithLowResource();
            if (map == null)
            {
                return "";
            }
            string explaination = lowResources.Count == 0 ? "" : CombinedLowResourcesString(map);
            return explaination;
            //return "LowResourceDesc".Translate(resource.LabelCap, available_resource_amount.ToString(), resource_limit.ToString());
        }

        public override AlertReport GetReport()
        {
            //Log.Message("Alert_LowResource get report 1");
            //if (Find.TickManager.TicksGame < 150000)
            //{
            //	return false;
            //}
            //Log.Message("Alert_LowResource get report");
            return this.MapWithLowResource() != null;
        }




        private Map MapWithLowResource()
        {
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                Map map = maps[i];
                if (map.IsPlayerHome && map.mapPawns.AnyColonistSpawned)
                {
                    if (this.PopulateLowResources(map))
                    {
                        return map;
                    }
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
            // TODO: instead of clear, manage hashset by adding and removing
            bool foundLowResource = false;
            Log.Message("Alert_LowResource populate");
            foreach (KeyValuePair<ThingDef, int> entry in alertableResources)
            {
                if (map.resourceCounter.GetCount(entry.Key) < entry.Value)
                {
                    lowResources.Add(entry.Key);
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
            return lowResourcesString;
        }

        public void ExposeData()
        {
            Log.Message("loading alertableresources");
            Scribe_Collections.Look(ref alertableResources, "Deep_ResourceAlert_alertableResources", LookMode.Def, LookMode.Value);
        }

    }

}
