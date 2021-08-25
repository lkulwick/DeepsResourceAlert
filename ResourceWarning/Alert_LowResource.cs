using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class Alert_LowResource : Alert
	{
		public Alert_LowResource()
		{
		}

		public Alert_LowResource(ThingDef resource, int resource_limit)
		{
			this.defaultLabel = "LowResource".Translate();
			this.defaultPriority = AlertPriority.Medium;
			this.resource = resource;
			this.resource_limit = resource_limit;
		}


        public override TaggedString GetExplanation()
		{
			Map map = this.MapWithLowFood();
			if (map == null)
			{
				return "";
			}
			int available_resource_amount = map.resourceCounter.GetCount(resource);
			return "LowResourceDesc".Translate(resource.defName, available_resource_amount.ToString(), resource_limit.ToString());
		}

		public override AlertReport GetReport()
		{
			if (Find.TickManager.TicksGame < 150000)
			{
				return false;
			}
			return this.MapWithLowFood() != null;
		}

		private Map MapWithLowFood()
		{
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				Map map = maps[i];
				if (map.IsPlayerHome && map.mapPawns.AnyColonistSpawned)
				{
					if (map.resourceCounter.GetCount(resource) < resource_limit)
					{
						return map;
					}
				}
			}
			return null;
		}

		public override string GetLabel()
		{
			return "Deep_ResourceWarning_LowResources".Translate();
		}

		private ThingDef resource;
		private int resource_limit;
	}

}
