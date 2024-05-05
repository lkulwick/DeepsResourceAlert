using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ResourceAlert
{
    public class ResourceAlertSettings : GameComponent
    {
        public ResourceAlertSettings(Game game)
        {

        }

        public ResourceAlertSettings()
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Log.Message("Deep's Resource Alert: data loaded");
            Scribe_Collections.Look(ref Alert_LowResource.alertableResources, "Deep_ResourceAlert_alertableResources", LookMode.Def, LookMode.Value);
        }
    }
}
