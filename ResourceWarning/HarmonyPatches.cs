using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResourceWarning
{
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("Deep.ResourceWarning");
			harmony.PatchAll();
		}
	}
}
