using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ResourceWarning
{
    static class ResourceChecker
    {
		public static Dictionary<ThingDef, int> AlertableResources = new Dictionary<ThingDef, int>();

        public static void AddAlertableResource(ThingDef resource, int alert_amount = 100)
        {
			Log.Message("Add Alertable Resource: " + resource.defName + "amount: " + alert_amount);
			if (resource != null)
            {
				Log.Message("Add Alertable Resource. Its not null: " + resource.defName + "amount: " + alert_amount);
				AlertableResources.Add(resource, alert_amount);
				Log.Message("Added");
			}
        }

		public static void RemoveAlertableResource(ThingDef resource)
        {
			if (resource != null)
            {
				AlertableResources.Remove(resource);
            }
        }
		
		public static void Check()
        {
			CheckResources(AlertableResources);
        }
		public static bool CheckResources(Dictionary<ThingDef, int> resources)
        {
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				Map map = maps[i];
				if (map.IsPlayerHome && map.mapPawns.AnyColonistSpawned)
				{
					CheckResourcesForMap(resources, map);
				}
			}
			return true;
        }

		private static bool CheckResourcesForMap(Dictionary<ThingDef, int> resources, Map map)
        {
			int iterations = 0;
			foreach (KeyValuePair<ThingDef, int> resource in resources)
            {
				int resource_stock = map.resourceCounter.GetCount(resource.Key);
				if (resource_stock < resource.Value)
				{
					CallAlert(resource);
				}
				iterations++;
			}

			return false;
		}

        private static void CallAlert(KeyValuePair<ThingDef, int> resource)
        {
            throw new NotImplementedException();
        }
    }
}
