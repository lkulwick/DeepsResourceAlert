using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ResourceAlert
{
	static class ResourceChecker
	{

		public static void AddAlertableResource(ThingDef resource, int alert_amount = 100)
		{
			Log.Message("Add Alertable Resource: " + resource.LabelCap + "amount: " + alert_amount);
			if (resource != null)
			{
				if (Alert_LowResource.alertableResources.ContainsKey(resource))
				{
					Alert_LowResource.alertableResources.Remove(resource);
					Messages.Message("Deep_ResourceAlert_ResourceChecker_KeyReplaced".Translate(resource.LabelCap, alert_amount), MessageTypeDefOf.NeutralEvent);
				}
				Alert_LowResource.alertableResources.Add(resource, alert_amount);
			}
		}

		public static void AddAlertableCategory(ThingCategoryDef resource, int alert_amount = 100)
		{
			Log.Message("Add Alertable Category: " + resource.LabelCap + "amount: " + alert_amount);
			if (resource != null)
			{
				if (Alert_LowResource.alertableCategories.ContainsKey(resource))
				{
					Alert_LowResource.alertableCategories.Remove(resource);
					Messages.Message("Deep_ResourceAlert_ResourceChecker_KeyReplaced".Translate(resource.LabelCap, alert_amount), MessageTypeDefOf.NeutralEvent);
				}
				Alert_LowResource.alertableCategories.Add(resource, alert_amount);
			}
		}

		public static void RemoveAlertableResource(ThingDef resource)
		{
			if (resource != null)
			{
				Alert_LowResource.alertableResources.Remove(resource);
			}
		}
		public static void RemoveAlertableCategory(ThingCategoryDef resource)
		{
			if (resource != null)
			{
				Alert_LowResource.alertableCategories.Remove(resource);
			}
		}
	}
}
