using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ResourceWarning
{
	// Token: 0x02000022 RID: 34
	public static class ResourceRightClick
	{
		public static bool open = true;
		public static List<Def> PinnedDefCache = new List<Def>();
		private static string pintrans = Translator.Translate("PinnedRes");
		[HarmonyPatch(typeof(Listing_ResourceReadout), "DoCategory")]
		public static class Patch_DoCategory
		{
			private static void Clicker(TreeNode_ThingCategory node, Rect rect)
			{
				if (Input.GetKey(KeyCode.RightControl) && Widgets.ButtonInvisible(rect, true))
				{
					Log.Message("deep test inside");
					Messages.Message("Deep_ResourceWarning_DebugMessage".Translate(), MessageTypeDefOf.NegativeEvent);
				}
			}

			// Token: 0x060001E5 RID: 485 RVA: 0x00012B4A File Offset: 0x00010D4A
			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				bool found = false;
				foreach (CodeInstruction instruction in instructions)
				{
					if (!found && instruction.Calls(ResourceRightClick.Patch_DoCategory.DrawTexture))
					{
						found = true;
						yield return new CodeInstruction(OpCodes.Ldarg_1, null);
						yield return new CodeInstruction(OpCodes.Ldloc_2, null);
						yield return new CodeInstruction(OpCodes.Call, ResourceRightClick.Patch_DoCategory.m_Clicker);
						yield return instruction;
					}
					else
					{
						yield return instruction;
					}
				}
				IEnumerator<CodeInstruction> enumerator = null;
				if (!found)
				{
					Log.Warning("Patch_DoCategory Instruction not found");
				}
				yield break;
			}

			// Token: 0x040001C0 RID: 448
			private static MethodInfo m_Clicker = AccessTools.Method(typeof(ResourceRightClick.Patch_DoCategory), "Clicker", null, null);

			// Token: 0x040001C1 RID: 449
			private static MethodInfo DrawTexture = AccessTools.Method(typeof(GUI), "DrawTexture", new Type[]
			{
				typeof(Rect),
				typeof(Texture)
			}, null);
		}

		// Token: 0x02000063 RID: 99
		[HarmonyPatch(typeof(Listing_ResourceReadout), "DoThingDef")]
		public static class Patch_DoThingDef
		{
			// Token: 0x060001E7 RID: 487 RVA: 0x00012BC0 File Offset: 0x00010DC0
			private static void Clicker(ThingDef thingDef, Rect rect)
			{
				if (Input.GetKey(KeyCode.RightControl) && Widgets.ButtonInvisible(rect, true))
				{
					Messages.Message("Deep_ResourceWarning_DebugMessage".Translate(), MessageTypeDefOf.NegativeEvent);
				}
			}

			// Token: 0x060001E8 RID: 488 RVA: 0x00012C47 File Offset: 0x00010E47
			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				bool found = false;
				foreach (CodeInstruction codeInstruction in instructions)
				{
					if (codeInstruction.Calls(ResourceRightClick.Patch_DoThingDef.DrawTexture))
					{
						found = true;
						yield return codeInstruction;
						yield return new CodeInstruction(OpCodes.Ldarg_1, null);
						yield return new CodeInstruction(OpCodes.Ldloc_2, null);
						yield return new CodeInstruction(OpCodes.Call, ResourceRightClick.Patch_DoThingDef.m_Clicker);
					}
					else
					{
						yield return codeInstruction;
					}
				}
				IEnumerator<CodeInstruction> enumerator = null;
				if (!found)
				{
					Log.Warning("Patch_DoThingDef Instruction not found");
				}
				yield break;
			}

			// Token: 0x040001C2 RID: 450
			private static MethodInfo DrawTexture = AccessTools.Method(typeof(GUI), "DrawTexture", new Type[]
			{
				typeof(Rect),
				typeof(Texture)
			}, null);

			// Token: 0x040001C3 RID: 451
			private static MethodInfo m_Clicker = AccessTools.Method(typeof(ResourceRightClick.Patch_DoThingDef), "Clicker", null, null);
		}
	}
}
