using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResourceAlert
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
			// Runs in your static init
			var tro = AccessTools.TypeByName("ToggleableReadouts.ToggleableReadoutsUtility");
			if (tro != null)
			{
				Log.Message("[ResourceAlert] Toggleable Readouts detected — enabling compatibility once methods are known.");
				// After you send me DoReadout* bodies, I'll fill in the precise patching here.
			}

			Harmony harmony = new Harmony("Deep.ResourceWarning");
            harmony.PatchAll();


			// Toggleable Readouts compatibility: patch their click handler if present
			try
			{
				var troType = AccessTools.TypeByName("ToggleableReadouts.ToggleableReadoutsUtility");
				if (troType != null)
				{
					var handleClicksMI = AccessTools.Method(
						troType, "HandleClicks",
						new[] { typeof(UnityEngine.Event), typeof(UnityEngine.EventType), typeof(UnityEngine.Rect), typeof(Verse.Def) });

					if (handleClicksMI != null)
					{
						harmony.Patch(handleClicksMI,
							postfix: new HarmonyMethod(typeof(ResourceAlert.ToggleableReadouts_Compat), nameof(ResourceAlert.ToggleableReadouts_Compat.HandleClicks_Postfix)));
						Log.Message("[ResourceAlert] Toggleable Readouts detected — compatibility enabled.");
					}
					else
					{
						Log.Warning("[ResourceAlert] Toggleable Readouts found, but HandleClicks method not found.");
					}
				}
			}
			catch (Exception e)
			{
				Log.Warning("[ResourceAlert] Failed to enable Toggleable Readouts compatibility: " + e);
			}


		}
	}
}
