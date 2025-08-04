using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;
using System.Runtime;
using static RimWorld.ColonistBar;
using static Verse.HediffCompProperties_RandomizeSeverityPhases;
using Verse.Noise;
using System.Collections;
using System.Linq;
using System;

namespace ResourceAlert
{
	public class DefaultAlertSettings : ModSettings
	{
		// --- raw, string-keyed dictionaries for Scribe ---
		private Dictionary<string, int> rawResourceThresholds = new Dictionary<string, int>();
		private Dictionary<string, int> rawCategoryThresholds = new Dictionary<string, int>();

		// --- public view, exactly the same shape your UI already expects ---
		public IDictionary<ThingDef, int> defaultResourceThresholds { get; private set; }
		public IDictionary<ThingCategoryDef, int> defaultCategoryThresholds { get; private set; }

		public DefaultAlertSettings()
		{
			// wire up the adapters, pointing at the raw dicts
			defaultResourceThresholds =
			  new DefNameDictionary<ThingDef>(rawResourceThresholds, name => DefDatabase<ThingDef>.GetNamedSilentFail(name));
			defaultCategoryThresholds =
			  new DefNameDictionary<ThingCategoryDef>(rawCategoryThresholds, name => DefDatabase<ThingCategoryDef>.GetNamedSilentFail(name));
		}

		public override void ExposeData()
		{
			base.ExposeData();

			// persist only the raw, string-keyed versions
			Scribe_Collections.Look(ref rawResourceThresholds, "defaultResourceThresholds", LookMode.Value, LookMode.Value);
			Scribe_Collections.Look(ref rawCategoryThresholds, "defaultCategoryThresholds", LookMode.Value, LookMode.Value);

			DebugLog.Message($"loaded {rawResourceThresholds.Count} resource keys; {rawCategoryThresholds.Count} category keys.");

			DebugLog.Message("Raw keys: " + string.Join(", ", rawResourceThresholds.Keys));
			DebugLog.Message("Raw keys: " + string.Join(", ", rawCategoryThresholds.Keys));


			// 2) now *create* (or recreate) your adapters against the freshly‐loaded raw dict
			defaultResourceThresholds =
				new DefNameDictionary<ThingDef>(rawResourceThresholds, name => DefDatabase<ThingDef>.GetNamedSilentFail(name));
			defaultCategoryThresholds =
				new DefNameDictionary<ThingCategoryDef>(rawCategoryThresholds, name => DefDatabase<ThingCategoryDef>.GetNamedSilentFail(name));

			DebugLog.Message("[ExposeData] Adapters re-wired; you should now see Count()/GetEnumerator() logs when UI runs.");


		}
	}




	public class ResourceAlertMod : Mod
	{
		public static DefaultAlertSettings Settings;

		public ResourceAlertMod(ModContentPack content) : base(content)
		{
			// Load (or create) the settings; DefaultAlertSettings ctor wires up your adapters
			Settings = GetSettings<DefaultAlertSettings>();
		}

		public override string SettingsCategory()
		{
			return "Resource Alert";
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			var listing = new Listing_Standard();
			listing.Begin(inRect);

			// --- Section: list current default thresholds ---
			listing.Label("Default Tracked Resources:");
			listing.Label("These thresholds will be applied to every new map.");
			listing.Gap(6f);

			if (Settings.defaultResourceThresholds.Count == 0 &&
				Settings.defaultCategoryThresholds.Count == 0)
			{
				listing.Label("  (no defaults set)");
			}
			else
			{
				if (Settings.defaultResourceThresholds.Count > 0)
				{
					listing.Label("Resources:");
					DrawDefaultsWithIcons(listing, Settings.defaultResourceThresholds);
				}

				if (Settings.defaultCategoryThresholds.Count > 0)
				{
					listing.Label("Categories:");
					DrawDefaultsWithIcons(listing, Settings.defaultCategoryThresholds);
				}
			}

			listing.Gap(12f);

			// --- “Show Tracked Resources” button ---
			if (Find.CurrentMap != null)
			{
				if (listing.ButtonText("Show Tracked List for this map."))
				{
					SoundDefOf.Click.PlayOneShotOnCamera();
					Find.WindowStack.Add(new TrackedResourcesWindow());
				}
				listing.Gap(12f);
			}



			// --- Section: action buttons ---
			if (Find.CurrentMap != null)
			{
				if (listing.ButtonText("Make Current Map Settings As Default"))
				{
					// copy from the current map’s active settings
					Settings.defaultResourceThresholds.Clear();
					Settings.defaultCategoryThresholds.Clear();
					foreach (var kv in ResourceAlertManager.Resources)
						Settings.defaultResourceThresholds[kv.Key] = kv.Value;
					foreach (var kv in ResourceAlertManager.Categories)
						Settings.defaultCategoryThresholds[kv.Key] = kv.Value;

					LoadedModManager.GetMod<ResourceAlertMod>().WriteSettings();

					Messages.Message("Default thresholds updated from current map", MessageTypeDefOf.PositiveEvent);
				}
			}


			if (listing.ButtonText("Clear All Defaults"))
			{
				Settings.defaultResourceThresholds.Clear();
				Settings.defaultCategoryThresholds.Clear();
				Messages.Message("All default thresholds cleared", MessageTypeDefOf.PositiveEvent);
			}

			listing.End();

			// Writes out your settings XML
			base.DoSettingsWindowContents(inRect);
		}

		private const float IconSize = 24f;
		private const float IconTextPadding = 6f;
		private void DrawDefaultsWithIcons<T>(Listing_Standard listing, IDictionary<T, int> dict) where T : Def
		{
			foreach (var kv in dict)
			{
				var def = kv.Key;
				var value = kv.Value;
				// carve out a row exactly the height of our icon
				Rect rowRect = listing.GetRect(IconSize);

				// draw the icon
				Rect iconRect = new Rect(rowRect.x, rowRect.y, IconSize, IconSize);
				Texture2D tex = def is ThingDef td
					? td.uiIcon
					: (def as ThingCategoryDef)?.icon ?? BaseContent.BadTex;
				GUI.DrawTexture(iconRect, tex);
				TooltipHandler.TipRegion(iconRect, def.LabelCap);

				// draw the text next to it, vertically centered
				Rect labelRect = new Rect(
					iconRect.xMax + IconTextPadding,
					rowRect.y + (IconSize - Text.LineHeight) / 2f,
					rowRect.width - IconSize - IconTextPadding,
					Text.LineHeight
				);
				Widgets.Label(labelRect, $"{def.LabelCap}: {value}");
			}
			listing.Gap(IconTextPadding);
		}

	}
	public class DefNameDictionary<KDef> : IDictionary<KDef, int> where KDef : Def
	{
		private readonly Dictionary<string, int> _raw;
		private readonly Func<string, KDef> _resolver;

		public DefNameDictionary(Dictionary<string, int> raw, Func<string, KDef> resolver)
		{
			_raw = raw;
			_resolver = resolver;
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Constructor: raw count={_raw.Count}");
		}

		public int this[KDef key]
		{
			get
			{
				int value = _raw[key.defName];
				DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Getter: key={key.defName}, value={value}");
				return value;
			}
			set
			{
				_raw[key.defName] = value;
				DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Setter: key={key.defName}, value={value}");
			}
		}

		public ICollection<KDef> Keys
		{
			get
			{
				var keys = _raw.Keys.Select(name => _resolver(name)).Where(d => d != null).ToList();
				DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Keys accessed: count={keys.Count}");
				return keys;
			}
		}

		public ICollection<int> Values
		{
			get
			{
				var vals = _raw.Values.ToList();
				DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Values accessed: count={vals.Count}");
				return vals;
			}
		}

		public int Count
		{
			get
			{
				int cnt = _raw.Count;
				DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Count accessed: {cnt}");
				return cnt;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] IsReadOnly accessed: false");
				return false;
			}
		}

		public void Add(KDef key, int value)
		{
			_raw.Add(key.defName, value);
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Add(key={key.defName}, value={value})");
		}

		public bool ContainsKey(KDef key)
		{
			bool exists = _raw.ContainsKey(key.defName);
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] ContainsKey(key={key.defName}) => {exists}");
			return exists;
		}

		public bool Remove(KDef key)
		{
			bool removed = _raw.Remove(key.defName);
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Remove(key={key.defName}) => {removed}");
			return removed;
		}

		public bool TryGetValue(KDef key, out int v)
		{
			bool found = _raw.TryGetValue(key.defName, out v);
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] TryGetValue(key={key.defName}) => found={found}, value={v}");
			return found;
		}

		public void Clear()
		{
			_raw.Clear();
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Clear()");
		}

		public IEnumerator<KeyValuePair<KDef, int>> GetEnumerator()
		{
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] GetEnumerator() start");
			foreach (var kv in _raw)
			{
				var def = _resolver(kv.Key);
				if (def == null)
				{
					DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Resolver returned null for key={kv.Key}");
					continue;
				}
				DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Yielding (key={kv.Key}, value={kv.Value})");
				yield return new KeyValuePair<KDef, int>(def, kv.Value);
			}
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] GetEnumerator() end");
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(KeyValuePair<KDef, int> item)
		{
			Add(item.Key, item.Value);
		}

		public bool Contains(KeyValuePair<KDef, int> item)
		{
			bool contains = TryGetValue(item.Key, out var v) && v == item.Value;
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Contains(KeyValuePair(key={item.Key.defName}, value={item.Value})) => {contains}");
			return contains;
		}

		public void CopyTo(KeyValuePair<KDef, int>[] array, int arrayIndex)
		{
			foreach (var kv in this)
			{
				array[arrayIndex++] = kv;
				DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] CopyTo: writing (key={kv.Key.defName}, value={kv.Value}) at index={arrayIndex - 1}");
			}
		}

		public bool Remove(KeyValuePair<KDef, int> item)
		{
			bool removed = Contains(item) && Remove(item.Key);
			DebugLog.Message($"[DefNameDictionary<{typeof(KDef).Name}>] Remove(KeyValuePair(key={item.Key.defName}, value={item.Value})) => {removed}");
			return removed;
		}
	}

}

// a tiny IDictionary<KDef,int> that just wraps Dictionary<string,int> + a resolver Func
