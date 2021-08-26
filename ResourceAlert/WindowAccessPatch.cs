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

namespace ResourceAlert
{
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
                    Log.Message("inside category. Thingdef: " + node.Label + "Rect: " + rect.ToString());
                    //Messages.Message("Deep_ResourceWarning_DebugMessage".Translate(), MessageTypeDefOf.NegativeEvent);
                    if (!Find.WindowStack.TryRemove(typeof(SetResourcesWindow), true))
                    {
                        // Find.WindowStack.Add(new SetResourcesWindow(node.));
                        // Messages.Message("Add category inside".Translate(), MessageTypeDefOf.NegativeEvent);
                    }
                }
            }
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
                if (!found)
                {
                    Log.Warning("Patch_DoCategory Instruction not found");
                }
                yield break;
            }
            private static MethodInfo m_Clicker = AccessTools.Method(typeof(ResourceRightClick.Patch_DoCategory), "Clicker", null, null);
            private static MethodInfo DrawTexture = AccessTools.Method(typeof(GUI), "DrawTexture", new Type[]
            {
                typeof(Rect),
                typeof(Texture)
            }, null);
        }

        [HarmonyPatch(typeof(Listing_ResourceReadout), "DoThingDef")]
        public static class Patch_DoThingDef
        {
            private static void Clicker(ThingDef thingDef, Rect rect)
            {
                if (Widgets.ButtonInvisible(rect, true))
                {
                    Log.Message("deep test inside. Thingdef: " + thingDef.defName + "Rect: " + rect.ToString());
                    //Messages.Message("Deep_ResourceWarning_DebugMessage".Translate(), MessageTypeDefOf.NegativeEvent);
                    if (!Find.WindowStack.TryRemove(typeof(SetResourcesWindow), true))
                    {
                        Log.Message("windowstack add: " + thingDef.defName + "Rect: " + rect.ToString());
                        Find.WindowStack.Add(new SetResourcesWindow(thingDef));
                    }
                }
            }

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
                //				IEnumerator<CodeInstruction> enumerator = null;
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
