using UnityEngine;
using Verse;
using System.Collections.Generic;
using RimWorld;
using Verse.Sound;
using System.Resources;

namespace ResourceAlert
{
	public class TrackedResourcesWindow : Window
	{
		private Dictionary<ThingDef, int> resourceDict;
		private Dictionary<ThingCategoryDef, int> categoryDict;

		public override Vector2 InitialSize => new Vector2(400f, 500f);

		public TrackedResourcesWindow()
		{
			doCloseX = true;
			closeOnClickedOutside = true;
			absorbInputAroundWindow = true;

			resourceDict = ResourceAlertManager.Resources;
			categoryDict = ResourceAlertManager.Categories;
		}


		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0, 0, inRect.width, 30f), "Tracked Resources");
			Text.Font = GameFont.Small;

			float curY = 40f;
			float iconSize = 24f;
			float spacing = 6f;
			float rowHeight = iconSize + spacing;
			float btnWidth = 50f;

			void DrawTracked<T>(Dictionary<T, int> dict, System.Action<T> onEdit, System.Action<T> onRemove) where T : Def
			{
				foreach (var kv in dict)
				{
					if (curY + rowHeight > inRect.height - 45f) break;

					T def = kv.Key;
					int value = kv.Value;
					Texture icon = def is ThingDef td ? td.uiIcon : (def as ThingCategoryDef)?.icon ?? BaseContent.BadTex;

					Rect iconRect = new Rect(10f, curY, iconSize, iconSize);
					GUI.DrawTexture(iconRect, icon);
					TooltipHandler.TipRegion(iconRect, def.LabelCap);

					Rect labelRect = new Rect(iconRect.xMax + 6f, curY + 3f, inRect.width - 150f, iconSize);
					Widgets.Label(labelRect, $"{def.LabelCap}: {value}");

					Rect editRect = new Rect(inRect.width - 90f, curY, btnWidth, iconSize);
					if (Widgets.ButtonText(editRect, "Edit"))
					{
						SoundDefOf.Click.PlayOneShot(SoundInfo.OnCamera());
						onEdit(def);
					}

					Rect removeRect = new Rect(inRect.width - 35f, curY, iconSize, iconSize);
					if (Widgets.ButtonImage(removeRect, TexButton.CloseXSmall))
					{
						SoundDefOf.Click.PlayOneShot(SoundInfo.OnCamera());
						onRemove(def);
						break; // To avoid modifying collection while enumerating
					}

					curY += rowHeight;
				}
			}

			if (Find.CurrentMap != null)
			{
				// Resources
				if (resourceDict.Count > 0)
				{
					Widgets.Label(new Rect(10f, curY, 200f, 24f), "Resources:");
					curY += 24f;
				}

				DrawTracked(
					resourceDict,
					def => Find.WindowStack.Add(new SetResourcesWindow((ThingDef)(object)def)),
					def => ResourceChecker.RemoveAlertableResource((ThingDef)(object)def)
				);


				// Categories
				if (categoryDict.Count > 0)
				{
					curY += 10f;
					Widgets.Label(new Rect(10f, curY, 200f, 24f), "Categories:");
					curY += 24f;
				}

				DrawTracked(
					categoryDict,
					def => Find.WindowStack.Add(new SetResourcesWindow((ThingCategoryDef)(object)def)),
					def => ResourceChecker.RemoveAlertableCategory((ThingCategoryDef)(object)def)
				);

				float btnW = 120f, btnH = 30f;
				var btnRect = new Rect((inRect.width - btnW) / 2, inRect.height - btnH - 10, btnW, btnH);
			}
			else
			{
				Widgets.Label(new Rect(10f, curY, 200f, 24f), "Load a map first.");
				curY += 24f;
			}
			// Close button (optional, but elegant)
			if (Widgets.ButtonText(new Rect(inRect.width - 80f, inRect.height - 35f, 70f, 28f), "Close"))
				Close();
		}

	}
}
