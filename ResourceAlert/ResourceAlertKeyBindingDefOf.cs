using RimWorld;
using Verse;

namespace ResourceAlert
{
	[DefOf]
	public static class ResourceAlertKeyBindingDefOf
	{
		public static KeyBindingDef Deep_ResourceAlert_SetResource;
		static ResourceAlertKeyBindingDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(ResourceAlertKeyBindingDefOf));
		}
	}
}
