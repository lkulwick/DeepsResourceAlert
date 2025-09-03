// Adds our Hotkey+click behavior on Toggleable Readouts' rows without touching their visuals.
using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResourceAlert
{
	// No attributes here; we patch it manually from HarmonyPatches.cs
	internal static class ToggleableReadouts_Compat
	{
		// Signature must match ToggleableReadoutsUtility.HandleClicks(Event, EventType, Rect, Def)
		// Postfix runs after their right-click handling; we only act on our hotkey + left click.
		public static void HandleClicks_Postfix(Event eventCurrent, EventType eventType, Rect rect, Def def)
		{
			try
			{
				// Only consider left mouse; their code uses right mouse for context menu
				if (eventCurrent == null || eventCurrent.button != 0)
					return;

				// Require our keybinding (Deep_ResourceAlert_SetResource) to be pressed
				var kb = ResourceAlertKeyBindingDefOf.Deep_ResourceAlert_SetResource;
				if (kb == null || !(kb.KeyDownEvent || kb.IsDownEvent))
					return;

				// Use an invisible button over the same rect; true = play nice with mouseover logic
				if (!Widgets.ButtonInvisible(rect, true))
					return;

				// If our window is not already open, open it for the clicked def
				if (!Find.WindowStack.TryRemove(typeof(SetResourcesWindow), doCloseSound: true))
				{
					if (def is ThingDef td)
					{
						Find.WindowStack.Add(new SetResourcesWindow(td));
					}
					else if (def is ThingCategoryDef tc)
					{
						Find.WindowStack.Add(new SetResourcesWindow(tc));
					}
				}

				// Consume the click so it doesn't double-trigger any other left-click logic
				eventCurrent.Use();
			}
			catch (Exception e)
			{
				Log.Warning("[ResourceAlert] TR compatibility click handler failed: " + e);
			}
		}
	}
}
