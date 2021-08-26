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
        // public static Dictionary<ThingDef, int> AlertableResources = new Dictionary<ThingDef, int>();

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

        public static void RemoveAlertableResource(ThingDef resource)
        {
            if (resource != null)
            {
                Alert_LowResource.alertableResources.Remove(resource);
            }
        }

    }
}
