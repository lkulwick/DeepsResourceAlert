// WindowAccessPatch.cs (safer transpilers)
// Replaces fragile Ldloc_* assumptions with rect-load cloning around GUI.DrawTexture.
// Comments in English.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResourceAlert
{
	public static class ResourceRightClick
	{
		public static bool open = true;
		public static List<Def> PinnedDefCache = new List<Def>();
		private static readonly string pintrans = Translator.Translate("PinnedRes");

		// ------------------------------
		// DoCategory: open SetResourcesWindow for a ThingCategory on hotkey + click
		// ------------------------------
		// --- inside ResourceRightClick ---

		[HarmonyPatch(typeof(Listing_ResourceReadout), "DoCategory")]
		public static class Patch_DoCategory_Safer
		{
			private static readonly MethodInfo DrawTextureMI = AccessTools.Method(
				typeof(GUI), nameof(GUI.DrawTexture), new[] { typeof(Rect), typeof(Texture) });

			private static readonly MethodInfo ClickerMI = AccessTools.Method(
				typeof(Patch_DoCategory_Safer), nameof(Clicker));

			// Called when user holds our keybinding and clicks the row rect
			private static void Clicker(TreeNode_ThingCategory node, Rect rect)
			{
				if ((ResourceAlertKeyBindingDefOf.Deep_ResourceAlert_SetResource.KeyDownEvent
					 || ResourceAlertKeyBindingDefOf.Deep_ResourceAlert_SetResource.IsDownEvent)
					&& Widgets.ButtonInvisible(rect, true))
				{
					DebugLog.Message($"[ResourceAlert] Category click: {node.catDef.defName} rect={rect}");
					if (!Find.WindowStack.TryRemove(typeof(SetResourcesWindow), true))
					{
						DebugLog.Message($"[ResourceAlert] Opening SetResourcesWindow for category {node.catDef.defName}");
						Find.WindowStack.Add(new SetResourcesWindow(node.catDef));
					}
				}
			}

			/// <summary>
			/// Robust IL: save original arguments to locals, invoke Clicker(node, rect), then restore args for DrawTexture.
			/// This avoids cloning any possibly by-ref or instance-producing instruction sequences.
			/// </summary>
			[HarmonyTranspiler]
			[HarmonyPriority(Priority.VeryLow)]
			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
			{
				var code = new List<CodeInstruction>(instructions);

				// Declare locals: Texture tempTex; Rect tempRect;
				LocalBuilder texLocal = generator.DeclareLocal(typeof(Texture));  // ref type
				LocalBuilder rectLocal = generator.DeclareLocal(typeof(Rect));    // value type

				bool injected = false;
				for (int i = 0; i < code.Count; i++)
				{
					var ci = code[i];

					if (ci.Calls(DrawTextureMI))
					{
						// At this point the stack is [..., rect, texture] for the DrawTexture call.
						// We insert before the call:
						//   stloc.s tempTex
						//   stloc.s tempRect
						//   ldarg.1                      // node
						//   ldloc.s tempRect             // rect
						//   call Clicker
						//   ldloc.s tempRect             // rect
						//   ldloc.s tempTex              // texture
						yield return new CodeInstruction(OpCodes.Stloc_S, texLocal);
						yield return new CodeInstruction(OpCodes.Stloc_S, rectLocal);
						yield return new CodeInstruction(OpCodes.Ldarg_1);
						yield return new CodeInstruction(OpCodes.Ldloc_S, rectLocal);
						yield return new CodeInstruction(OpCodes.Call, ClickerMI);
						yield return new CodeInstruction(OpCodes.Ldloc_S, rectLocal);
						yield return new CodeInstruction(OpCodes.Ldloc_S, texLocal);

						injected = true;
					}

					// Emit original instruction (including the call)
					yield return ci;
				}

				if (!injected)
					Log.Warning("[ResourceAlert] Patch_DoCategory_Safer: DrawTexture call not found; no injection performed.");
			}
		}

		// ---------------------------------------------

		[HarmonyPatch(typeof(Listing_ResourceReadout), "DoThingDef")]
		public static class Patch_DoThingDef_Safer
		{
			private static readonly MethodInfo DrawTextureMI = AccessTools.Method(
				typeof(GUI), nameof(GUI.DrawTexture), new[] { typeof(Rect), typeof(Texture) });

			private static readonly MethodInfo ClickerMI = AccessTools.Method(
				typeof(Patch_DoThingDef_Safer), nameof(Clicker));

			private static void Clicker(ThingDef thingDef, Rect rect)
			{
				if ((ResourceAlertKeyBindingDefOf.Deep_ResourceAlert_SetResource.KeyDownEvent
					 || ResourceAlertKeyBindingDefOf.Deep_ResourceAlert_SetResource.IsDownEvent)
					&& Widgets.ButtonInvisible(rect, true))
				{
					DebugLog.Message($"[ResourceAlert] ThingDef click: {thingDef.defName} rect={rect}");
					if (!Find.WindowStack.TryRemove(typeof(SetResourcesWindow), true))
					{
						DebugLog.Message($"[ResourceAlert] Opening SetResourcesWindow for def {thingDef.defName}");
						Find.WindowStack.Add(new SetResourcesWindow(thingDef));
					}
				}
			}

			[HarmonyTranspiler]
			[HarmonyPriority(Priority.VeryLow)]
			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
			{
				var code = new List<CodeInstruction>(instructions);

				LocalBuilder texLocal = generator.DeclareLocal(typeof(Texture));
				LocalBuilder rectLocal = generator.DeclareLocal(typeof(Rect));

				bool injected = false;
				for (int i = 0; i < code.Count; i++)
				{
					var ci = code[i];

					if (ci.Calls(DrawTextureMI))
					{
						// Stack before call: [..., rect, texture]
						yield return new CodeInstruction(OpCodes.Stloc_S, texLocal);
						yield return new CodeInstruction(OpCodes.Stloc_S, rectLocal);
						yield return new CodeInstruction(OpCodes.Ldarg_1);              // thingDef
						yield return new CodeInstruction(OpCodes.Ldloc_S, rectLocal);   // rect
						yield return new CodeInstruction(OpCodes.Call, ClickerMI);
						yield return new CodeInstruction(OpCodes.Ldloc_S, rectLocal);   // rect
						yield return new CodeInstruction(OpCodes.Ldloc_S, texLocal);    // texture

						injected = true;
					}

					yield return ci;
				}

				if (!injected)
					Log.Warning("[ResourceAlert] Patch_DoThingDef_Safer: DrawTexture call not found; no injection performed.");
			}
		}
	}
}
