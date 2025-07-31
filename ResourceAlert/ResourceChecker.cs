using ResourceAlert;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;


//Dictionary<ThingDef, int> alertableResources = ResourceAlertManager.Resources;
//Dictionary<ThingCategoryDef, int> alertableCategories = ResourceAlertManager.Categories;

namespace ResourceAlert
{
    static class ResourceChecker
    {

        public static void AddAlertableResource(ThingDef resource, int alert_amount = 100)
        {
            DebugLog.Message("Add Alertable Resource: " + resource.LabelCap + "amount: " + alert_amount);
            if (resource != null)
            {
                if (ResourceAlertManager.Resources.ContainsKey(resource))
                {
					ResourceAlertManager.Resources.Remove(resource);
                    Messages.Message("Deep_ResourceAlert_ResourceChecker_KeyReplaced".Translate(resource.LabelCap, alert_amount), MessageTypeDefOf.NeutralEvent);
                }
				ResourceAlertManager.Resources.Add(resource, alert_amount);
            }
        }

        public static void AddAlertableCategory(ThingCategoryDef resource, int alert_amount = 100)
        {
            DebugLog.Message("Add Alertable Category: " + resource.LabelCap + "amount: " + alert_amount);
            if (resource != null)
            {
                if (ResourceAlertManager.Categories.ContainsKey(resource))
                {
					ResourceAlertManager.Categories.Remove(resource);
                    Messages.Message("Deep_ResourceAlert_ResourceChecker_KeyReplaced".Translate(resource.LabelCap, alert_amount), MessageTypeDefOf.NeutralEvent);
                }
				ResourceAlertManager.Categories.Add(resource, alert_amount);
            }
        }

        public static void RemoveAlertableResource(ThingDef resource)
        {
            if (resource != null)
            {
				ResourceAlertManager.Resources.Remove(resource);
            }
        }
        public static void RemoveAlertableCategory(ThingCategoryDef resource)
        {
            if (resource != null)
            {
				ResourceAlertManager.Categories.Remove(resource);
            }
        }
    }
}
